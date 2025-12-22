using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;

// 1. 获取当前执行路径（GitHub Action 会在仓库根目录运行）
var root = Directory.GetCurrentDirectory();

// 2. 扫描所有 Markdown 文件
// 排除掉不相关的目录，避免把 README 或者 Action 脚本也抓进去
var excludeKeywords = new[] { ".github", "bin", "obj", ".vscode" };

var posts = Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
    .Where(file => !excludeKeywords.Any(k => file.Contains(k)))
    .Select(file => {
        var fileInfo = new FileInfo(file);
        // 获取相对路径并统一使用正斜杠 /
        var relativePath = Path.GetRelativePath(root, file).Replace("\\", "/");
        // 获取直接父级的文件夹名
        var parentDir = fileInfo.Directory?.Name ?? "未分类";

        return new {
            title = Path.GetFileNameWithoutExtension(file),
            path = relativePath,
            category = parentDir,
            date = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd")
        };
    })
    .OrderByDescending(p => p.date) // 最新的排在前面
    .ToList();

// 3. 配置 JSON 写入选项（处理中文不转义）
var options = new JsonSerializerOptions { 
    WriteIndented = true, 
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
};

// 4. 生成文件
var jsonString = JsonSerializer.Serialize(posts, options);
File.WriteAllText("index.json", jsonString);

Console.WriteLine($"------------------------------------------");
Console.WriteLine($"成功生成索引！共计: {posts.Count} 篇文章");
Console.WriteLine($"文件已保存在: {Path.Combine(root, "index.json")}");
Console.WriteLine($"------------------------------------------");