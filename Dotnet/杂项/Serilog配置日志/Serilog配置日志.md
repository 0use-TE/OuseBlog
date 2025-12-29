### Serilog配置日志

```csharp
  //Logger
      // 配置日志
      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()                     // 最低日志等级
          .WriteTo.File(
              "Logs/log-.txt",                     // 日志文件路径
              rollingInterval: RollingInterval.Day, // 每天一个日志文件
              retainedFileCountLimit: 7           // 保留最近7天
          )
          .CreateLogger();
```

后续添加输出目标，只需要安装对应的包
然后WriteTo.xxx()即可
