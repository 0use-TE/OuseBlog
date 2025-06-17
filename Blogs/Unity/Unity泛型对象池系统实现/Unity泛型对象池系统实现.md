## Unity泛型对象池系统实现

### 代码

#### IPoolable

```csharp
/// <summary>
/// 所有需要放入对象池的对象都要继承这个接口！！！
/// 里面有两个回调
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 从对象池中取出会执行
    /// </summary>
    void OnSpawn();
    /// <summary>
    /// 放回对象池会执行
    /// </summary>
    void OnDespawn();
}
```

#### ObjectPoolManager

```csharp
/// <summary>
/// 全局静态对象池管理类，泛型配合SO，性能比起字符串略低一点，但是大大提高了开发效率
/// </summary>
public static class ObjectPoolManager
{
    /// <summary>
    /// 存储所有对象池，键是 Type，值是对应的对象池，取出的时候会 as 成 值是预制体内部对象池类
    /// 因为内部对象池是泛型的，所以这里只能使用object
    /// 作用：用于管理特定类型的对象池，比如T是Enemy，那么object里维护的就是Enemy的对象池
    /// </summary>    
    private static Dictionary<Type, object> pools = new Dictionary<Type, object>();
    /// <summary>
    /// 存储预制体，键是 Type
    /// 这是从PoolPrefabsSO中得到的，通过反射动态获取对用的组件类型
    /// 比如，我在创建了PoolPrefab(在PoolPrefabSO类里面定义了)，字符串传入的是Enemy，那么这个键就是typeof(Enemy)值就是Enemy
    /// </summary>
    private static Dictionary<Type, Component> prefabs = new Dictionary<Type, Component>();
    /// <summary>
    /// 每种对象对用的池子的最大数，100绝对够用了！我们每波也就几十个怪
    /// </summary>
    private static int maxPoolSize = 100;
    /// <summary>
    /// 生成的对象挂在这个对象下
    /// </summary>
    private static Transform poolParent;

    /// <summary>
    /// 初始化对象池,由游戏管理器调用初始化，所有的对象池里的对象都会挂载到GameManager对象下面
    /// </summary>
    /// <param name="poolPrefabsSO"></param>
    /// <param name="poolParent"></param>
    /// <param name="maxPoolSize"></param>
    public static void Initialize(PoolPrefabsSO poolPrefabsSO, Transform poolParent, int maxPoolSize = 100)
    {
        ObjectPoolManager.poolParent = poolParent;
        ObjectPoolManager.maxPoolSize = maxPoolSize;
        LoadPrefabsFromSO(poolPrefabsSO);
    }

    /// <summary>
    /// 从 ScriptableObject 加载预制体，从GameManager传入
    /// </summary>
    /// <param name="poolPrefabsSO"></param>
    private static void LoadPrefabsFromSO(PoolPrefabsSO poolPrefabsSO)
    {
        foreach (var poolPrefab in poolPrefabsSO.Prefabs)
        {
            if (poolPrefab== null || string.IsNullOrEmpty(poolPrefab.componentName))
            {
                Utilities.DebugError("PoolManager", "对象池组件名不能为null");
                continue;
            }
            // 通过 componentName 获取 Type
            Type componentType = Type.GetType(poolPrefab.componentName);
            if (componentType == null)
                componentType = Assembly.GetExecutingAssembly().GetType(poolPrefab.componentName);

            if (componentType == null)
            {
                Debug.LogError($"错误 无法找到类型: {poolPrefab.componentName}");
                continue;
            }

            if (!typeof(IPoolable).IsAssignableFrom(componentType))
            {
                Debug.LogError($"类型 {componentType.Name} 没有实现 IPoolable 接口!");
                continue;
            }
            // 从预制体上获取组件
            Component component = poolPrefab.prefab.GetComponent(componentType);
            if (component == null)
            {
                Debug.LogError($"预制体 {poolPrefab.prefab.name} 没有组件 {componentType.Name}!");
                continue;
            }
            // 注册到预制体字典（使用 Type 作为键）
            prefabs[componentType] = component;

            // 初始化对象池
            if (!pools.ContainsKey(componentType))
            {
                // 使用反射创建泛型对象池
                Type poolType = typeof(InternalPool<>).MakeGenericType(componentType);
                object poolInstance = Activator.CreateInstance(poolType);
                pools[componentType] = poolInstance;
                // 预加载对象
                MethodInfo preloadMethod = poolType.GetMethod("Preload");
                preloadMethod.Invoke(poolInstance, new object[] { 20 });
            }
        }
    }

    // 获取对象
    public static T Get<T>(Vector3 position, Quaternion rotation) where T : Component, IPoolable
    {
        Type type = typeof(T);
        if (!pools.ContainsKey(type))
        {
            Debug.LogError($"池中没有类型 {type.Name} !");
            return null;
        }
        var pool = pools[type] as InternalPool<T>;
        return pool.Get(position, rotation);
    }

    // 归还对象
    public static void Return<T>(T item) where T : Component, IPoolable
    {
        Type type = item.GetType();
        if (!pools.ContainsKey(type))
        {
            Debug.LogError($"池中没有类型 {type.Name} !");
            return;
        }
        var pool = pools[type] as InternalPool<T>;
        pool.Return(item);
    }
    
    // 内部对象池类（每个类型一个池）
    private class InternalPool<T> where T : Component, IPoolable
    {
        private Queue<T> pool = new Queue<T>();
        // 预加载方法：在初始化时创建一定数量的对象并放入 pool
        public void Preload(int count)
        {
            Type type = typeof(T);
            if (!prefabs.ContainsKey(type))
            {
                Debug.LogError($"预制体类型 {type.Name} 找不到! Cannot 无法执行初始池子.");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                T item = UnityEngine.Object.Instantiate(prefabs[type], poolParent) as T;
                item.gameObject.SetActive(false); // 初始时禁用对象
                pool.Enqueue(item);
            }
        }
        public T Get(Vector3 position, Quaternion rotation)
        {
            T item = null;

            if (pool.Count > 0)
                item = pool.Dequeue();

            if (item == null)
            {
                Type type = typeof(T);
                if (!prefabs.ContainsKey(type))
                {
                    Debug.LogError($"预制体{type.Name} 找不到!");
                    return null;
                }

                item = UnityEngine.Object.Instantiate(prefabs[type], position, rotation, poolParent) as T;
            }
            else
            {
                item.transform.position = position;
                item.transform.rotation = rotation;
                item.gameObject.SetActive(true);
            }

            item.OnSpawn();
            return item;
        }

        public void Return(T item)
        {
            if (pool.Count >= maxPoolSize)
                UnityEngine.Object.Destroy(item.gameObject);
            else
            {
                item.OnDespawn();
                item.gameObject.SetActive(false);
                pool.Enqueue(item);
            }
        }
    }
}
```

