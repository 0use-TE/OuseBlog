### 解决BlazorServer无HttpContext问题
#### 前言
为解决BlazorServer使用SignaIR而无传统Http请求导致的各种问题，我思考并想到了以下方案。

#### 解决方案
1. 首先需要WebApi,您可以BlazorServer集成WebApi或者RazorPages,也可以是任何后端技术。  
2. 然后您可以JS互操作，使用C#执行js发起请求,对于简单的请求也可以使用NavigationManager.NavigationTo("Your beckend api")。这里推荐使用NavigationManager,因为这样可以省些js互操作，当然有缺点:比如注册，您可能需要使用查询字符串或者是路由参数将账号密码传递到后端，由于是导航，这些内容将会显示在浏览器上。解决方案很多，比如导航前先根据账号密码生成一个哈希，然后url传递的是这个哈希，然后api解密哈希获取账号密码。
常见误区:您的互操作C#代码仍是服务端执行，然后通过SignaIR把要执行的js上下文发送给客户端，浏览器执行。

#### 集成RaozrPage的另一种玩法
如果您的项目集成了RazorPage，对于登录，注册界面，您完全可以使用RazorPages实现，缺点是需要学习razorpgage，而且RazorPages无法使用您Blazor的组件或者主题库，这回造成样式不一致性。
#### 结语
希望这篇博客能帮助到您，如果您对BlazorServer集成RazorPage和WebApi感兴趣，可以在我的博客区查找 BlazorServer集成Api和RazorPage