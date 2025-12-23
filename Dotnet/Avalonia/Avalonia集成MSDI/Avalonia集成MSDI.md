### Avalonia集成MSDI
Hi，你好，如果您有Asp.Net Core的开发经验,相比对MSDI非常属性，我也是，Avalonia刚入门就开始寻找能否将MSDI集成进Avalonia里面，历经转折，终于研究下出了以下方案。
### 集成教程
1. 安装prism.avalonia,它将DryIoc容器成功集成进了Avalonia,同时提供其他非常好用的工具，比如RegionManager,IModule...
2. 安装包DryIoc.Microsoft.DependencyInjection,添加以下代码
```csharp
  protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var serviceColllection = new ServiceCollection();
            serviceColllection.AddSingleton<INotificationService, NotificationService>();
            //Logging
            serviceColllection.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            //Pupulate ServiceCollection To DryIoc
            containerRegistry.GetContainer().Populate(serviceColllection);

            // Register you Services, Views, Dialogs, etc.
        }
```
3. 执行dotnet run
### 结束
具体您可以查看github地址 [PrismAvaloniaWithMSDI](https://github.com/0use-TE/PrismAvaloniaWithMSDITempalte)