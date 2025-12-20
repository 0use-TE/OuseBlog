1. 按照规则创建js文件，比如./Components/Pages/Home.razor.js
2. js内必须设置为导出
3. 同级的Blazor中执行JSRuntime.InvokeAsync("import","./Comonents/Pages/Home.razor.js")路径要从项目根路径写起
4. 得到导出的模块，使用InvokeAsync...etc 即可