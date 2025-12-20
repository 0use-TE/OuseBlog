```csharp
  var folderPath = Path.Combine(AppContext.BaseDirectory, "WebViewAssets", "browser");
            Debug.WriteLine("文件路径："+folderPath);
            _webServer =new WebServer(o =>{
                o.WithUrlPrefix("http://localhost:8888/");
            }).WithStaticFolder("/",folderPath,true);
			_webServer.Start();
```

