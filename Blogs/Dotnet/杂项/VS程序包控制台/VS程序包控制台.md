##                                       程序包控制台

### 功能

在 Visual Studio 的程序包管理器控制台中，可以使用 `NuGet`提供的命令来管理项目的依赖项，如果不喜欢命令行也可以使用Nuget包管理器来管理包，这提供了一个图形化管理页面。

### 打开方式

![image-20250328131755937](assets/image-20250328131755937.png)

### 包控制台选项

![image-20250328131954642](assets/image-20250328131954642.png)

程序包源：可以是Nuget包（微软维护的包源），也可以是自己的包源（可以是网络上的，也可以是本地的，想要使用自己打的包，就需要添加自己打的包的地址，比如C://Program/MyNugetPackage/Markdown.nuget）

默认项目：设置默认包安装到的项目，也可以命令中指定项目名称

### **基础命令**

1. **Install-Package**
    安装指定的 NuGet 包到项目中。

   ```powershell
   Install-Package <包名> -Version <版本号>
   ```

   示例：安装最新版本的 `Newtonsoft.Json` 包：

   ```powershell
   Install-Package Newtonsoft.Json
   ```

2. **Uninstall-Package**
    从项目中移除已安装的 NuGet 包。

   ```powershell
   Uninstall-Package <包名>
   ```

   示例：

   ```powershell
   Uninstall-Package Newtonsoft.Json
   ```

3. **Update-Package**
    更新项目中已安装的包到最新版本（或指定版本）。

   ```powershell
   Update-Package <包名> -Version <版本号>
   ```

   示例：更新所有包：

   ```powershell
   Update-Package
   ```

4. **Get-Package**
    列出项目中已安装的 NuGet 包。

   ```powershell
   Get-Package
   ```

------

### **包源管理命令**

1. **Add-PackageSource**
    添加新的 NuGet 包源。

   ```powershell
   Register-PackageSource -Name <源名> -Location <包源URL> -ProviderName NuGet
   ```

2. **Get-PackageSource**
    查看当前配置的包源。

   ```powershell
   Get-PackageSource
   ```

3. **Remove-PackageSource**
    删除已配置的包源。

   ```powershell
   Unregister-PackageSource -Name <源名>
   ```

------

### **搜索和查找命令**

1. **Find-Package**
    查找指定的包（默认会查找当前配置的源）。

   ```powershell
   Find-Package <包名> -Source <源名>
   ```

   示例：查找所有包含 "Json" 的包：

   ```powershell
   Find-Package Json
   ```

2. **Search-Package**
    搜索包，支持筛选版本、关键字等。

   ```powershell
   Search-Package <关键字>
   ```

------

### **示例工作流**

1. **安装包：**

   ```powershell
   Install-Package EntityFramework
   ```

2. **更新某个包到指定版本：**

   ```powershell
   Update-Package EntityFramework -Version 6.4.4
   ```

3. **查看当前已安装的包：**

   ```powershell
   Get-Package
   ```

4. **添加新的包源：**

   ```powershell
   Register-PackageSource -Name "MySource" -Location "https://my.nuget.org/v3/index.json" -ProviderName NuGet
   ```

------

### **参数常用选项**

- `-Version`：指定包的版本。
- `-ProjectName`：指定操作的项目。
- `-Source`：指定使用的包源。
- `-Prerelease`：允许使用预发布版本。

这些命令可以帮助你在 Visual Studio 的程序包管理器控制台中轻松管理项目的 NuGet 依赖项！