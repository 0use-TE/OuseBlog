### BlazorServer集成Api和RazorPage
#### 集成WebApi
1. 如果您使用的是miniApi，那就直接使用app就可以了
1. 控制器，您需要添加两处:   
添加服务 ```builder.Services.AddControllers();```  
添加中间件 ```app.MapControllers();```
#### 集成RazorPage
您需要修改两处:  
添加服务```builder.Services.AddRazorPages();```  
添加中间件```app.MapRazorPages();```

#### 结语
希望能这篇博客能帮助到您!