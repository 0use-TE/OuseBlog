using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

// 1. 获取当前路径
var root = Directory.GetCurrentDirectory();
var excludeKeywords = new[] { ".github", "bin", "obj", ".vscode", "App.cs", "index.json", "temp_data" };

// 2. 扫描并构建对象
var posts = Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
    .Where(file => !excludeKeywords.Any(k => file.Contains(k)))
    .Select(file => {
        var fileInfo = new FileInfo(file);
        var fileName = Path.GetFileNameWithoutExtension(file);
        var relativePath = Path.GetRelativePath(root, file).Replace("\\", "/");
        
        // --- 核心逻辑：只取最顶级目录 ---
        // 按照 / 分割相对路径
        var parts = relativePath.Split('/');
        string categoryDisplay;

        // 如果路径包含文件夹 (例如: Dotnet/Blazor/test.md -> parts[0] 是 Dotnet)
        if (parts.Length > 2)
        {
            categoryDisplay = parts[parts.Count()-3];
        }
        else
        {
            // 如果文件直接在根目录下
            categoryDisplay = "未分类";
        }
        // ------------------------------

        return new {
            title = fileName,
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