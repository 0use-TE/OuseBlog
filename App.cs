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
        var fileName = Path.GetFileNameWithoutExtension(file);
        var relativePath = Path.GetRelativePath(root, file).Replace("\\", "/");
        
        // 1. 获取所有层级的文件夹名
        var parts = relativePath.Split('/');
        var folderLevels = parts.Take(parts.Length - 1).ToList();

        // 2. 核心逻辑：如果最后一级文件夹名等于文件名，就把它删掉
        if (folderLevels.Count > 0 && folderLevels.Last().Equals(fileName, StringComparison.OrdinalIgnoreCase))
        {
            folderLevels.RemoveAt(folderLevels.Count - 1);
        }
        
        // 3. 组合分类字符串
        string categoryDisplay = folderLevels.Any() ? string.Join(", ", folderLevels) : "未分类";

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