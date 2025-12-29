### Dotnet项目指定sdk

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
