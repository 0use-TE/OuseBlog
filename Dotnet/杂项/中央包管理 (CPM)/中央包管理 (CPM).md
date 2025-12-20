#  中央包管理 (CPM) 简明笔记

## 1️⃣ 概念

- **Central Package Management (CPM)**：统一管理解决方案所有 NuGet 包的版本号。
- 规则：
  1. `.csproj` 中 `<PackageReference>` **不能写版本号**。
  2. 所有版本号写在 `Directory.Packages.props` 中。
  3. 启用 `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`。

------

## 2️⃣ 文件结构

```xml
Solution/
├─ Directory.Build.props          ← 启用 CPM
├─ Directory.Packages.props       ← 定义包版本
├─ MainApp/
│   └─ MainApp.csproj
└─ PrismProject/
    └─ PrismProject.csproj
```

------

## 3️⃣ 核心配置

### 3.1 `Directory.Build.props`

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
</Project>
```

### 3.2 `Directory.Packages.props`

```xml
<Project>
  <ItemGroup>
    <PackageVersion Include="Avalonia.Desktop" Version="11.3.6" />
    <PackageVersion Include="Avalonia.Diagnostics" Version="11.3.6" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.21" />
  </ItemGroup>
</Project>
```

### 3.3 `.csproj`

```xml
<ItemGroup>
  <PackageReference Include="Avalonia.Desktop" />
  <PackageReference Include="Avalonia.Diagnostics" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
</ItemGroup>
```

> ⚠️ 注意：名字必须完全一致，版本号只写在 `Directory.Packages.props`。

------

## 4️⃣ 检查方法

```bash
dotnet restore -v diag
```

- 输出应显示：`Using central package management`
- 如果报“未定义 PackageVersion”，说明 `Directory.Packages.props` 缺少对应包。