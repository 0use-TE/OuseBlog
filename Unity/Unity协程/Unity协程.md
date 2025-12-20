##                                              Unity协程

#### 1. 什么是协程

- 协程是一种特殊的函数，可以在多个帧内分步骤执行，而不阻塞主线程。
- 适用于需要延迟执行、等待时间、分步操作的场景，例如动画、计时器、等待用户输入等。

#### 2. 协程的基本原理

- 协程通过返回 `IEnumerator` 来实现。

- 使用 `yield return` 语句可以暂停执行并返回控制权给 Unity，直到下一次继续执行。

  细节(看不懂可以看完下面再回来查看)：基于迭代器模式，不是多线程！开启的协程还是在主线程里面运行的，假设Update一秒循环60次，我只需要返回60次null就实现了延时一秒，执行其他操作后，如果还需要延迟，则可以再次yield(迭代)对用的次数，这里就有个问题了，我怎么知道一秒需要迭代多少次，在Unity里面一个一个叫做TIme的类有一个叫做deltaTime的属性，这代表每两次循环的间隔时间，所以我们就可以算出来需要迭代多少次了，如下:

  ```csharp
  IEnumerator MyCoroutine()
  {
      int delay=1;//需要延时的时间，这里是秒
      var cnt= delay*1/Time.deltaTime;;
      for(int i=0;i<cnt;i++)
      yield return  null;//delay除以每帧间隔就可以得到需要迭代的次数
  }
  ```

  但是Unity为我们封装了上面的实现

  ```cs
  yield return new WaitForSeconds(2); // 等待2秒，内部以及帮我们实现好了迭代次数的计算
  ```

  

#### 3. 协程的使用方法

##### 3.1 启动协程

使用 `StartCoroutine` 方法启动协程：

```csharp
StartCoroutine(MyCoroutine());
```

##### 3.2 定义协程

协程需要返回 `IEnumerator`，并可以使用 `yield` 语句进行暂停：

```csharp
IEnumerator MyCoroutine()
{
    Debug.Log("协程开始");
    yield return new WaitForSeconds(2); // 等待2秒
    Debug.Log("协程继续");
}
```

#### 4. 常用的 `yield` 类

##### 4.1 `WaitForSeconds`

- 用于在协程中等待指定的时间（以秒为单位）。

```csharp
yield return new WaitForSeconds(2); // 等待2秒
```

##### 4.2 `WaitForEndOfFrame`

- 等待到当前帧的最后一刻后再继续执行。

```csharp
yield return new WaitForEndOfFrame(); // 等待到当前帧结束
```

##### 4.3 `WaitForFixedUpdate`

- 等待到下一个固定更新（Fixed Update）循环后再继续执行。

```csharp
yield return new WaitForFixedUpdate(); // 等待下一个固定更新
```

##### 4.4 `null`

- 使用 `yield return null` 暂停当前协程，直到下一帧继续执行。

```csharp
yield return null; // 等待到下一帧
```

#### 5. 协程的优势

- **简化异步编程**：通过 `yield` 语句简化了异步操作，避免了复杂的回调函数。
- **控制流畅性**：在不阻塞主线程的情况下，允许在多个帧内分步执行逻辑。
- **可读性**：使用协程使得时间延迟和异步逻辑更加直观。

#### 6. 注意事项

- **游戏暂停**：当游戏暂停时，`WaitForSeconds` 和其他等待类的计时器会停止。
- **协程的管理**：可以通过 `StopCoroutine()` 停止协程，或者使用 `StopAllCoroutines()` 停止所有协程。
- **性能**：过多的协程可能会影响性能，尤其是在需要频繁创建和销毁的情况下。
- **调试**：在调试协程时，可能会遇到难以追踪的状态问题，注意使用日志输出。

#### 7. 示例代码

下面是两个简单的协程示例，展示了使用 `WaitForSeconds` 实现延迟：

```csharp
using UnityEngine;
using System.Collections;

public class CoroutineExample : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndPrint());
    }

    IEnumerator WaitAndPrint()
    {
        Debug.Log("等待开始");
        yield return new WaitForSeconds(2); // 等待2秒
        Debug.Log("等待结束，继续执行");
    }
}
```

```csharp
using UnityEngine;
using System.Collections;

public class CoroutineExample : MonoBehaviour
{
    private bool canAttack;

    void Update()
    {
        if(Input.GetKeyDown())
       StartCoroutine(WaitAndPrint());
    }
    //技能的计时器
    IEnumerator WaitAndPrint()
    {
        canAttack=false;
        Debug.Log("等待开始");
        yield return new WaitForSeconds(2); // 等待2秒
        Debug.Log("等待结束，继续执行");
        canAttack=true;
    }
}
```

#### 结语

协程是多线程的一个很好的替代品，他简单安全，能帮我实现异步逻辑，是一个非常好的工具。