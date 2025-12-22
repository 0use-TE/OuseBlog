using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

// 1. 获取当前路径
var root = Directory.GetCurrentDirectory();
var excludeKeywords = new[] { ".github", "bin", "obj", ".vscode", "App.cs" };

// 2. 扫描并构建对象
var posts = Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
    .Where(file => !excludeKeywords.Any(k => file.Contains(k)))
    .Select(file => {
        var fileInfo = new FileInfo(file);
        return new {
            title = Path.GetFileNameWithoutExtension(file),
            path = Path.GetRelativePath(root, file).Replace("\\", "/"),
            category = fileInfo.Directory?.Name ?? "未分类",
            date = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd")
        };
    })
    .OrderByDescending(p => p.date)
    .ToList();

// 3. 解决报错的关键：配置序列化选项
var options = new JsonSerializerOptions { 
    WriteIndented = true, 
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    // 【关键修复】手动指定反射解析器，解决 InvalidOperationException
    TypeInfoResolver = new DefaultJsonTypeInfoResolver() 
};

// 4. 生成文件
try {
    var jsonString = JsonSerializer.Serialize(posts, options);
    File.WriteAllText("index.json", jsonString);
    Console.WriteLine($"成功! 已索引 {posts.Count} 篇文章。");
}
catch (Exception ex) {
    Console.WriteLine($"序列化失败: {ex.Message}");
}