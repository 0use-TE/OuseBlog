### 发布BlazorWasm到GithubPages

#### 前言

这是一个演示如何将 **Blazor WebAssembly (WASM)** 顺畅地发布到 **GitHub Pages** 的教程，这里还提供了一个示例仓库(有延时效果哦!)[PublishBlazorWASMToGithubPageDemo](https://github.com/0use-TE/PublishBlazorWASMToGithubPageDemo)

#### 教程

特别感谢 [jsakamoto 的工具包](https://github.com/jsakamoto/PublishSPAforGitHubPages.Build)。 它解决了 Blazor-WASM 在发布到 GitHub Pages 时常见的许多问题（如路径引用和 404 错误）。 如果你想了解这个工具包的详细效果，请前往该仓库查看更多信息！（**强烈推荐！**）

**操作步骤：**

1. **安装 NuGet 包：**

   ```bash
   dotnet add package jsakamoto.PublishSPAforGitHubPages.Build
   ```
2. **在 `.csproj` 文件中添加以下属性：**

   ```xm
   <PropertyGroup>
     <GHPages>true</GHPages>
   </PropertyGroup>
   ```
3. **在本地测试成功后，添加 GitHub Action：** 在你的仓库中创建 Action 脚本。请注意根据你的项目结构替换 `Publish` 步骤中的项目名称。

   ```yaml
   name: 部署到 GitHub Pages

   on:
     push:
       branches: [ master ] # 只有当 master 分支有代码提交时才触发

   # 设置权限：允许 Action 向仓库写入内容（部署到 gh-pages 分支需要此权限）
   permissions:
     contents: write

   jobs:
     deploy-to-github-pages:
       runs-on: ubuntu-latest
       steps:
         # 1. 检出代码
         - name: Checkout
           uses: actions/checkout@v4

         # 2. 安装 .NET SDK
         - name: Setup .NET
           uses: actions/setup-dotnet@v4
           with:
             dotnet-version: '10.0.x'

         # 3. 发布项目
         - name: Publish 
           # 请确保路径与你的项目名一致
           run: dotnet publish PublishBlazorWASMToGithubPageDemo -c Release -o release

         # 4. 部署到 GitHub Pages 分支
         - name: Deploy
           uses: JamesIves/github-pages-deploy-action@v4
           with:
             folder: release/wwwroot # 这是发布后的静态文件所在目录
             branch: gh-pages        # 部署到的目标分支
   ```
4. **在仓库设置中开启 GitHub Pages：** 进入仓库的 **Settings > Pages** 界面，将 Source 设置为 `gh-pages` 分支。
5. **验证结果：** 访问你的 GitHub Pages 演示网站，并按 `F12` 打开开发者工具检查是否有报错。

#### 结语

感谢你的阅读！如果你遇到任何问题，欢迎随时告诉我。
