### EFCore代码自动迁移

**在program.cs下面添加**

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // 自动应用所有挂起的迁移，如果数据库不存在则创建
    dbContext.Database.Migrate();
}
```
