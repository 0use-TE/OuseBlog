##                                Unity泛型持久化系统实现

### 代码

```csharp
/// <summary >
/// 数据持久化管理器 使用示例:<see cref="TestPersistence"/>
/// </summary>
public static class DataPersistenceManager
{
    private static string BasePath = Application.persistentDataPath;
    /// <summary>
    /// 加载单一数据（基于 Type）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T Load<T>() where T : class
    {
        Type key = typeof(T);
        string filePath = GetFilePath(key);
        return LoadFromFile<T>(filePath);
    }
    /// <summary>
    /// 加载关卡数据（基于 Type 和 ID）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T Load<T>( int index) where T : class
    {
        Type key = typeof(T);
        string filePath = GetFilePath(key, index);
        return LoadFromFile<T>(filePath);
    }

    /// <summary>
    /// 保存单一数据（基于 Type）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Save<T>( T value) where T : class
    {
        Type key = typeof(T);
        string filePath = GetFilePath(key);
        SaveToFile(filePath, value);
    }

    /// <summary>
    /// 保存同一个类型的多份数据（基于 Type 和 ID）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public static void Save<T>(int index, T value) where T : class
    {
        var key=typeof(T);
        string filePath = GetFilePath(key, index);
        SaveToFile(filePath, value);
    }

    /// <summary>
    /// 内部加载方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static T LoadFromFile<T>(string filePath) where T : class
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                T data = JsonConvert.DeserializeObject<T>(json);
                if (data != null)
                {
                    Debug.Log($"加载成功: {filePath}");
                    return data;
                }
                Debug.LogWarning($"反序列化返回 null，使用默认值: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"加载失败: {filePath}, 错误: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"文件不存在: {filePath}");
        }
        var value=Activator.CreateInstance<T>();
        Save(value);
        return value;
    }

    /// <summary>
    /// 内部保存方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="value"></param>
    private static void SaveToFile<T>(string filePath, T value) where T : class
    {
        try
        {
            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Debug.Log($"保存成功: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存失败: {filePath}, 错误: {e.Message}");
        }
    }
    /// <summary>
    /// 生成单一文件路径
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string GetFilePath(Type key)
    {
        return Path.Combine(BasePath, $"{key.Name}.json");
    }
    /// <summary>
    /// 生成带 ID 的文件路径（用于关卡，或者同一个类型的多个配置）
    /// </summary>
    private static string GetFilePath(Type key, int index)
    {
        return Path.Combine(BasePath, $"{key.Name}_{index}.json");
    }
}
```

