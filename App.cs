using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

// 1. 获取当前路径
var root = Directory.GetCurrentDirectory();
// 增加 index.json 和 temp_data 的过滤，防止自循环
var excludeKeywords = new[] { ".github", "bin", "obj", ".vscode", "App.cs", "index.json", "temp_data" };

// 2. 扫描并构建对象
var posts = Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
    .Where(file => !excludeKeywords.Any(k => file.Contains(k)))
    .Select(file => {
        var fileInfo = new FileInfo(file);
        // 统一路径格式
        var relativePath = Path.GetRelativePath(root, file).Replace("\\", "/");
        
        // --- 核心逻辑：有多少级取多少级 ---
        var parts = relativePath.Split('/');
        // 排除掉最后的文件名部分，取前面的所有文件夹名
        // 例如: Dotnet/Blazor/Test.md -> ["Dotnet", "Blazor"]
        var categories = parts.Take(parts.Length - 1).ToList();
        
        // 如果文件直接在根目录，分类设为"未分类"
        string categoryDisplay = categories.Any() ? string.Join(", ", categories) : "未分类";
        // ---------------------------------

        return new {
            title = Path.GetFileNameWithoutExtension(file),
            path = relativePath,
            category = categoryDisplay,
            date = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd")
        };
    })
    .OrderByDescending(p => p.date)
    .ToList();

// 3. 配置序列化选项
var options = new JsonSerializerOptions { 
    WriteIndented = true, 
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
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
    Environment.Exit(1);
}