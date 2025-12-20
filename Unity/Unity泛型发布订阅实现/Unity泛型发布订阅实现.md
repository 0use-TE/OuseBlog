##                                                                                Unity泛型发布订阅实现

### 简介

发布订阅就不多说了，软件开发中经常使用的一种设计模式，比如Ros里面的通讯系统，游戏开发中等等
他其实就是对观察者模式的一种封装，俺感觉可以替代观察者模式了
使用发布订阅能大大降低耦合性，比单例强不知道多少，unity中应该尽可能少并且不使用单例模式，因为单例模式太容易造成耦合了，一个人开发还好，几个人开发很容易出问题
俺最新在开发一个新游戏，于是便写了一个发布订阅模块，起名叫做事件中心，他超级好用，泛型＋强类型，下面你看使用就Ok了

### 代码

先直接放代码，使用Type替代传统的字符串类型的键，性能也不会降低多少！

```csharp

public class EventCenter
{
    /// <summary>
    /// 发送者和参数
    /// </summary>
    private static Dictionary<Type, List<Action<object, EventArgs>>> EventSystem = new Dictionary<Type, List<Action<object, EventArgs>>>();
    /// <summary>
    /// 订阅
    /// </summary>
    /// <typeparam name="T">继承EvetnArgs，表示要传递的参数</typeparam>
    /// <param name="handler"></param>
    /// <param name="filter"></param>
    public static void Subscribe<T>(Action<object, T> handler, params Func<T, bool>[] filter) where T : EventArgs
    {
        Type type = typeof(T);
        if (!EventSystem.ContainsKey(type))
            EventSystem[type] = new List<Action<object, EventArgs>>();
        // 包装 handler 和 filter
        Action<object, EventArgs> wrappedHandler = (sender, args) =>
        {
            T typedArgs = (T)args;
            foreach (var f in filter)
            {
                if (!f(typedArgs))
                {
                    return; // 只要有一个不满足，就不执行 handler
                }
            }
            handler(sender, typedArgs);
        };

        EventSystem[type].Add(wrappedHandler);
    }
    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    /// <param name="filter"></param>
    public static void Unsubscribe<T>(Action<object, T> handler, params Func<T, bool>[] filter) where T : EventArgs
    {
        Type type = typeof(T);
        if (EventSystem.ContainsKey(type))
        {
            var list = EventSystem[type];
            Action<object, EventArgs> wrappedHandler = (sender, args) =>
            {
                T typedArgs = (T)args;
                foreach (var f in filter)
                {
                    if (!f(typedArgs))
                    {
                        return;
                    }
                }
            };
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var existingHandler = list[i];
                if (existingHandler.Target == handler.Target && existingHandler.Method == handler.Method)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
            if (list.Count == 0)
            {
                EventSystem.Remove(type);
            }
        }
    }
    public static void Publish<T>(object sender, T eventData) where T : EventArgs
    {
        Type type = typeof(T);
        if (EventSystem.ContainsKey(type))
        {
            foreach (var handler in EventSystem[type])
            {
                handler?.Invoke(sender, eventData);
            }
        }
    }
}

```

### 使用示例

有以下开发中的场景，怪物死了，游戏管理器里面的数据需要更新，比如KillCnt(杀敌数)，然后UI要根据游戏管理器里面的数据进行更新
我们使用上面的事件中心

1. 创建需要传递的事件类，两个