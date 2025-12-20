```xml
<Content Include="WebViewAssets\**\*.*">
     <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
 </Content>
```

| Never	不复制到输出目录（默认值）                    |
| ------------------------------------------------------ |
| Always	每次编译都复制文件                           |
| PreserveNewest	仅当源文件比输出目录文件“新”时才复制 |