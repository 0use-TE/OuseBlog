# .NET 项目模板制作指南

本文档说明如何使用 dotnet 模板机制创建你自己的项目模板。

## 概述

这是一个 **.NET dotnet new 模板**，用于通过 `dotnet new` 命令快速生成项目结构。它支持多平台（Desktop、WebAssembly、Android 等）。

## 项目结构

```
YourTemplateRepo/
├── README.md                                 # 模板说明文档（显示在 NuGet 页面）
├── LICENSE.txt                               # 许可证文件
│
├── YourTemplate/                             # 模板包项目
│   ├── YourTemplate.csproj                  # NuGet 包定义
│   └── Program.cs                           # 最小占位程序（SDK 风格 csproj 必须）
│
└── TemplateContent/                         # 模板内容（打包到 NuGet 中）
    ├── .template.config/
    │   └── template.json                   # 模板配置
    │
    ├── MyMainProject/                       # 主项目/共享项目
    ├── MyMainProject.Desktop/               # 平台特定项目
    ├── MyMainProject.Wasm/                  # WebAssembly 项目
    └── MyMainProject.Android/               # Android 项目
```

## 逐步创建模板

### 第一步：创建模板包项目

创建一个文件夹，里面放一个设置 `PackageType: Template` 的 `.csproj` 文件：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <IncludeBuildOutput>false</IncludeBuildOutput>
    <PackageType>Template</PackageType>
    <Version>1.0.0</Version>
    <Authors>你的名字</Authors>
  </PropertyGroup>
</Project>
```

**关键属性：**
- `IncludeBuildOutput=false` — 告诉 SDK 不要生成 DLL
- `PackageType=Template` — 将此项目标记为模板包

### 第二步：创建模板内容

所有在 `TemplateContent/` 下的文件都会在模板实例化时出现。将其组织成正常的 .NET 解决方案结构：

```
TemplateContent/
├── .template.config/
│   └── template.json          # 必须：模板元数据
│
├── MyMainProject/             # 主共享项目
├── MyMainProject.Desktop/     # 平台特定项目
└── MyMainProject.Wasm/       # 另一个平台特定项目
```

### 第三步：配置 template.json

创建 `.template.config/template.json` 来定义模板行为：

```json
{
  "$schema": "http://json.schemastore.org/template",
  "author": "作者名",
  "name": "模板名称",
  "identity": "唯一标识符",
  "shortName": "短名称",
  "sourceName": "项目根文件夹名称",
  "tags": {
    "language": "C#",
    "type": "project"
  },
  "specialCustomOperations": {
    "**/*.cs": {
      "operations": [
        {
          "type": "conditional",
          "configuration": {
            "tokenPrefix": "//-template-"
          }
        }
      ]
    }
  }
}
```

**关键字段说明：**

| 字段 | 用途 |
|------|------|
| `sourceName` | 实例化时会被替换为用户提供的项目名称的文件夹名 |
| `shortName` | 使用 `dotnet new 短名称` 创建项目时用的名称 |
| `identity` | 模板的唯一标识符 |
| `specialCustomOperations` | 启用 `//-template-` 前缀的条件编译 |

### 第四步：配置模板包项目

在模板包的 `.csproj` 中，引入内容目录和元数据文件：

```xml
<ItemGroup>
  <Content Include="..\TemplateContent\**\*" Pack="true" PackagePath="content\"
           Exclude="..\TemplateContent\.vs\**;**\bin\**\*;**\obj\**\*" />
</ItemGroup>

<ItemGroup>
  <None Include="..\LICENSE.txt">
    <Pack>True</Pack>
    <PackagePath>\</PackagePath>
  </None>
  <None Include="..\README.md">
    <Pack>True</Pack>
    <PackagePath>\</PackagePath>
  </None>
</ItemGroup>
```

### 第五步：使用条件编译（可选）

在 C# 文件中使用 `//-template-` 前缀添加仅模板需要的代码块：

```csharp
//-template- #if DEBUG
void Debug方法() { }
//-template- #endif

void 正常方法() { }
```

实例化模板时，这些代码会**保留**（但前缀行会被**移除**）。这样可以包含模板特定的构建配置。

### 第六步：打包

```bash
dotnet pack YourTemplate/YourTemplate.csproj
```

这会生成一个 `.nupkg` 文件。可以本地安装：

```bash
dotnet new install ./YourTemplate/bin/Debug/YourTemplate.1.0.0.nupkg
```

或发布到 NuGet 供他人使用。

## sourceName 替换机制

`template.json` 中的 `sourceName`（如 "MyMainProject"）会自动替换为用户提供的新项目名称。这意味着：

- 包含 `MyMainProject` 的文件夹会被重命名
- .csproj 文件中的 `AssemblyName`、`RootNamespace` 会被重命名
- 命名空间引用会被更新

**注意：** 确保文件夹名称与 `sourceName` 完全一致（区分大小写），避免替换失败。

## 多平台项目注意事项

### 跨平台项目模板

如果模板包含多平台项目（如 Desktop、WASM、Android），需要：

1. 各平台项目引用主共享项目
2. 主项目的第一个 TFM 为 `net10.0`（或最低版本）
3. 平台特定项目使用对应的 TFM（如 `net10.0-browser`、`net10.0-android`）

### Avalonia 项目特有事项

- 使用 `bin\$(Configuration)\$(TargetFramework)` 模式区分平台输出
- `Avalonia.Diagnostics` 仅在 Debug 配置下包含
- XAML 文件使用 `DependentUpon` 关联代码隐藏类

## 常见问题

1. **安装后模板不显示** — 运行 `dotnet new --debug:reinit` 刷新模板缓存
2. **sourceName 替换不生效** — 检查文件夹名称是否与 `sourceName` 完全一致（区分大小写）
3. **条件编译块未被移除** — 确认 `specialCustomOperations` 对目标文件类型配置正确
4. **打包后内容为空** — 检查 `.csproj` 中的 `Include` 路径是否正确，排除项是否过于宽泛
