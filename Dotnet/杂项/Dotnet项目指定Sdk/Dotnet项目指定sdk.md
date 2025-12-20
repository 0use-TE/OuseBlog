🧱 一、项目级指定（最常用）

在项目根目录的 global.json 文件中指定 SDK 版本。

作用： 限定该目录及子目录的 dotnet 命令使用特定 SDK 版本。

示例：

```json
{ 
   "sdk": {
      "version": "8.0.302", 
   		"rollForward": "latestFeature" 
  } 
}
```

version: 指定确切的 SDK 版本。

rollForward: 控制当该版本不存在时的向前兼容策略（例如 latestFeature、major、disable）。

📍 优先级最高：同目录下执行 dotnet build、dotnet run，会优先使用此版本。