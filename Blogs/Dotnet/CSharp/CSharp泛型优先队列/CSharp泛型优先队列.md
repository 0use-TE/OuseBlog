##  优先队列

### 代码

C#的BCL(基础类库)没有提供泛型的优先队列，以下是我实现的一个泛型优先队列

```csharp
using System;
using System.Collections.Generic;
public class PriorityQueue<T>
{
    private List<T> items; // 存储队列元素
    private readonly IComparer<T> comparer;

    // 构造方法：传入比较器和可选的初始数据集合
    public PriorityQueue(IComparer<T> comparer, IEnumerable<T> initialData = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
        this.items = new List<T>();

        // 如果有初始数据，则添加并堆化
        if (initialData != null)
        {
            items.AddRange(initialData);
            Heapify();
        }
    }

    // 获取队列中元素的数量
    public int Count => items.Count;

    // 入队操作，添加元素并上浮以保持堆的性质
    public void Enqueue(T item)
    {
        items.Add(item);
        SiftUp(items.Count - 1);
    }

    // 出队操作，取出优先级最高的元素
    public T Dequeue()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        T highestPriorityItem = items[0];
        items[0] = items[^1]; // 将最后一个元素移到根节点
        items.RemoveAt(items.Count - 1);
        SiftDown(0);
        return highestPriorityItem;
    }

    // 查看优先级最高的元素（不出队）
    public T Peek()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        return items[0];
    }

    // 堆化方法，将初始数据集合转化为有效的堆结构
    private void Heapify()
    {
        for (int i = (items.Count - 2) / 2; i >= 0; i--)
        {
            SiftDown(i);
        }
    }

    // 上浮操作：当元素加入队列时，逐层向上调整
    private void SiftUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparer.Compare(items[index], items[parentIndex]) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    // 下沉操作：当元素出队时，逐层向下调整
    private void SiftDown(int index)
    {
        int lastIndex = items.Count - 1;
        while (index < lastIndex)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;

            if (leftChildIndex <= lastIndex && comparer.Compare(items[leftChildIndex], items[smallestIndex]) < 0)
                smallestIndex = leftChildIndex;

            if (rightChildIndex <= lastIndex && comparer.Compare(items[rightChildIndex], items[smallestIndex]) < 0)
                smallestIndex = rightChildIndex;

            if (smallestIndex == index)
                break;

            Swap(index, smallestIndex);
            index = smallestIndex;
        }
    }

    // 辅助方法：交换两个元素的位置
    private void Swap(int i, int j)
    {
        T temp = items[i];
        items[i] = items[j];
        items[j] = temp;
    }
}
```

### 示例