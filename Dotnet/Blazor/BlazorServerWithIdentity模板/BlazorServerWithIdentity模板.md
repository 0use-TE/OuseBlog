### BlazorServerWithIdentity模板
#### 前言
总所周知，Blazor有四种渲染模式，其中Server模式，服务端渲染，是最适合处理后端交互的渲染模式，但缺点是所有的C#代码均是在服务器端执行，服务端维护虚拟dom并通过SignaIR与客户端通讯，因此产生了一个问题，那就是除了第一次建立连接是常规的Http请求，后续根本无法通过C#发起Http请求。  
盲区:使用HttpClient不就发生了Http请求吗?错误的X，Server交互性，您的所有C#代码都是由服务端执行的，所以这里变成了服务器执行HttpClient。  
那如何解决Server渲染模式的这个问题呢?
我的个人网站有篇博客叫做 解决BlazorServer无HttpContext问题 会探讨解决方案。  
所以我就选择集成了WebApi而非js或者razor pages。  
#### 正文
BlazorServer集成Identity，就意味着要实现:
1. 业务逻辑：登录,登出,创建，注册...
1. 样板代码：集成EFCore,连接数据库,WebApi...  
每次都要做重复工作，因此我便开发了一个模板，以方便您快速开始BlazorServer之旅。 
点击链接前往Github查看源码或者获取更多详情。
[BlazorServerWithIdentityTemplate](https://github.com/0use-TE/BlazorServerWithIdentityTemplate0)
#### 特点
1. 🛠 Web API 支持
2. 🔐 Identity 认证系统
3. 🗄 EF Core（使用 SQLite）
4. 🎨 UI 风格使用 MudBlazor
5. 🔑 基础登录和登出功能

#### 结语
希望这个模板能帮助到您,如果对您有帮助，不妨```Star++；```