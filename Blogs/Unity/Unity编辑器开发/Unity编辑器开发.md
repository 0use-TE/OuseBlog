##                                      Unity编辑器开发

### 前言

自从学Dotnet以来，俺好久没开发游戏了，前段时间，俺当上了游戏开发部部长，学校今年也对社团提出了新要求，说了一大堆，就是要拿出成果，不要僵尸社团，不然直接没了，大一的现在编程基础不够，没法学习Unity，得从编程学起，不知道要到什么时候，估计得要暑假，所以俺得先做点东西了，于是我就准备和大三的学长一起先开发一款塔防游戏，具体内容等有进展了我会放出来。聊到塔防游戏(类保卫萝卜)，比较麻烦的一点就是路径搜索算法了，因为路径会存在交叉点，常规的算法，比如DFS，BFS，A*等不能直接工作，所以我和Touken一起想出了一种解决方案，会单独写一篇，这个方案用到了Unity的编辑器开发，我之前虽然学过，但是不够系统，因此需要系统学下，固有了本篇文章。

### 介绍

Unity编辑器开发就是对现有编辑器功能做拓展，不过多介绍了，上干货

**他是如何工作的**？通过继承Unity提供的Editor类(所有编辑器开发相关的东西都要放在Editor文件夹，这是一个约定，Unity打包时候不会打包该文件夹，该文件夹可以位于子目录里面，可以有多个，工作方式类似分布类)，然后编译，编译后，Unity读取(基于反射)生成的dll，Unity会说，奥，你是我的孩子，我要把你加载到我的引擎里面，然后你写的工具就被Unity加载进去了，此时，Unity编辑器既运行自己的代码，也跑你拓展的代码。

**可以拓展哪些？**编辑器可以对多个窗口进行拓展，也可以直接创建自己的窗口，代码风格和思维有点像web，用html那种玩意创建UI，然后unity监听开发者事件，传给你写的C#代码(js捕捉用户操作)，C#根据事件类型执行回调，比如根据你按的是鼠标左键还是右键Debug.Log不同的信息
不同的功能会继承不同Unity提供给开发者的类，下面都会一一实现。

### AI解析部分

学习一个系统，我们应该先搞懂它的整体工作方式，不然直接学内容，效率很低的，而且是死记硬背，并不是真正掌握使用！所以我先让Ai来给我讲懂Unity编辑器体系，如果你看不懂，那你就应该要多打打基础，学习C#和Unity了，我个人是看懂了，而且感觉Grok讲的非常棒（个别API有错误，不影响学习架构），我学会了体系的运转，最下面会有一个运转图

![image-20250331215016640](assets/image-20250331215016640.png)

#### Unity编辑器扩展体系概览

- **体系结构**：
  - 基于`UnityEditor`命名空间，运行在编辑器环境中，与游戏运行时分离。
  - 通过反射机制加载扩展（如`[CustomEditor]`、`[MenuItem]`）。
- **核心模块**：
  - Inspector（检视面板）、Scene视图、菜单项、独立窗口、资产处理。
- **运转机制**：
  - 事件驱动，编辑器主循环调度，结合序列化和渲染管道。

#### 继承关系与类图
- **核心基类**：
  - `UnityEngine.Object` → `ScriptableObject` → `Editor` / `EditorWindow`。
  - `MonoBehaviour`是被操作的目标。
- **类图**：
  ```
  [UnityEngine.Object]
      ├── [ScriptableObject]
      │   ├── [Editor]
      │   └── [EditorWindow]
      ├── [MonoBehaviour]
      └── [GameObject]
  ```
- **辅助类**：`Handles`、`Gizmos`（静态绘制）、`SerializedObject`（数据管理）。

#### 核心类与功能
- **`Editor`**：
  - 自定义Inspector（`OnInspectorGUI`）和Scene视图（`OnSceneGUI`）。
  - 属性：`target`（单个组件）、`targets`（多选组件）。
- **`EditorWindow`**：
  - 独立窗口，`OnGUI`绘制UI。
  - 属性：`position`、`titleContent`。
- **`Handles` / `Gizmos`**：
  - 绘制Scene视图辅助工具。
- **`SerializedObject`**：
  - 管理序列化数据，配合`Editor`使用。
- **`MenuItem`**：
  - 添加菜单项，静态方法。

#### 编辑器扩展的不同类型
- **Inspector扩展**：
  - 目标：组件/资产的Inspector。
  - 类：`Editor`，方法：`OnInspectorGUI`。
- **Scene视图扩展**：
  - 目标：场景交互。
  - 类：`Editor`，方法：`OnSceneGUI`。
- **资产扩展**：
  - Inspector：`Editor`。
  - 导入：`AssetPostprocessor`。
- **菜单项扩展**：
  - 类：无，属性：`[MenuItem]`。
- **独立窗口扩展**：
  - 类：`EditorWindow`。

#### UI绘制原理
- **IMGUI（即时模式）**：
  - 每帧重绘，无状态保留，依赖代码逻辑。
- **核心类**：
  - `GUILayout`：自动布局。
  - `EditorGUILayout`：编辑器专用，集成序列化。
  - `GUI`：手动布局。
- **绘制方法**：
  - `Label`、`Button`、`IntField`、`PropertyField`等。

#### 渲染时机
- **Inspector（`OnInspectorGUI`）**：
  - 按需渲染：选中对象、属性修改、窗口调整触发。
  - 非每帧刷新，Unity优化避免冗余调用。
- **EditorWindow（`OnGUI`）**：
  - 接近每帧渲染，只要窗口可见。
  - 最小化或遮挡时暂停。
- **Scene视图（`OnSceneGUI`）**：
  - 接近每帧，选中对象且视图可见时触发。
- **机制**：
  - 编辑器主循环调度，调用绘制方法，渲染到GUI管道。

#### 数据保存与序列化
- **IMGUI无状态**：
  - 输入值（如10）不自动保存，需绑定存储。
- **Inspector（`Editor`）**：
  - 保存：修改`target`的序列化字段，Unity自动保存。
  - 原因：`target`是真实组件，由序列化系统维护。
- **EditorWindow**：
  - 不保存：临时数据，除非手动存储（静态变量、ScriptableObject）。
  - 原因：不绑定序列化对象。
- **其他类型**：
  - `AssetPostprocessor`：保存资产属性。
  - `MenuItem`：间接影响序列化数据。

#### 序列化系统的作用
- **管理对象**：
  - `MonoBehaviour`、`ScriptableObject`、资产的序列化字段。
- **保存流程**：
  - 修改字段 → 标记脏数据 → 保存到磁盘（场景/资产文件）。
- **Editor依赖序列化**：
  - 操作`target`，修改序列化字段，自动持久化。
- **EditorWindow例外**：
  - 独立对象，不受序列化管理，需手动持久化。

#### 综合对比
| **类型**   | **类**               | **序列化管理** | **保存**         | **渲染时机**   |
| ---------- | -------------------- | -------------- | ---------------- | -------------- |
| Inspector  | `Editor`             | 是             | 是（序列化字段） | 按需渲染       |
| Scene视图  | `Editor`             | 是             | 是（序列化字段） | 接近每帧       |
| 编辑器窗口 | `EditorWindow`       | 否             | 否（需手动）     | 每帧（可见时） |
| 菜单项     | `[MenuItem]`         | 否（间接）     | 是（若改序列化） | 无渲染         |
| 资产导入   | `AssetPostprocessor` | 是             | 是（资产属性）   | 导入时执行     |

---

#### 总结要点
- **体系运转**：
  - 编辑器通过事件循环驱动扩展，`Editor`操作序列化组件，`EditorWindow`独立运行。
- **UI绘制**：
  - IMGUI即时模式，`GUILayout`/`EditorGUILayout`自动布局，渲染由主循环调度。
- **渲染时机**：
  - Inspector按需，窗口/Scene视图接近每帧，仅可见区域渲染。
- **数据保存**：
  - 序列化系统管理`Editor`的目标组件，`EditorWindow`需手动处理。

不懂的地方请查资料或者问AI，这对编辑器开发真的很重要！！！理解了本质之后，接下来的学习就会变得非常简单，而且不仅限于Unity，好多框架也都使用IMGUI这种渲染模式。

### 绘制工具

我们先来学习Unity为我们提供的绘制工具，这样接下来才能更好的学习其他部分，不然你知道如何让Unity执行自己的代码也做不了什么，你需要绘制UI，实现你想拓展的功能

下面是AI的讲解，我让他结合了Web前端一起讲解，这样才好理解！知道了三种渲染工具的区别和底层设计，才能学会如何混用，因为实际使用中，确实是需要一起使用的！

下面是AI整理后我各种提问后的版本，比如AI先给我了一份，不全也有不懂的，接着问，把所有搞懂后再让他整理，所以好多地方看不懂是正常的，请自己搜索。

---

#### 绘制工具整体概述
- **背景**：
  - `GUI`、`GUILayout`、`EditorGUILayout`是Unity IMGUI系统的核心绘制工具，基于即时模式（Immediate Mode），每次调用生成UI。
- **目标**：
  - 提供从手动控制到编辑器集成的不同层次UI绘制能力。
- **运行模式**：
  - 支持基于事件（按需渲染，如Inspector）和每帧执行（如`EditorWindow`、`MonoBehaviour`）。

---

#### `GUI` 的设计与使用
- **设计理念**：
  - **精确控制**：手动指定`Rect`，类似CSS绝对定位。
  - **低层次抽象**：直接映射底层GUI渲染，灵活但需手动管理。
- **核心方法**：
  - `GUI.Label(Rect, string)`：显示标签。
  - `GUI.Button(Rect, string)`：绘制按钮，返回`bool`。
  - `GUI.TextField(Rect, string)`：文本输入框。
  - `GUI.BeginGroup(Rect)` / `EndGroup()`：分组绘制。
- **适用场景**：
  - 固定布局、运行时调试UI。
- **示例**：
  ```csharp
  void OnGUI()
  {
      GUI.Label(new Rect(10, 10, 100, 20), "标签");
      if (GUI.Button(new Rect(10, 40, 100, 30), "按钮")) Debug.Log("点击");
  }
  ```

---

#### `GUILayout` 的设计与使用
- **设计理念**：
  - **自动布局**：按顺序排列，类似CSS Flexbox。
  - **便利性**：无需手动指定位置，Unity自动计算。
- **核心方法**：
  - `GUILayout.Label(string)`：显示标签。
  - `GUILayout.Button(string)`：绘制按钮，返回`bool`。
  - `GUILayout.TextField(string)`：文本输入框。
  - `GUILayout.BeginHorizontal()` / `EndHorizontal()`：水平布局。
  - `GUILayout.BeginVertical()` / `EndVertical()`：垂直布局。
- **适用场景**：
  - 动态布局、快速原型。
- **示例**：
  ```csharp
  void OnGUI()
  {
      GUILayout.Label("标签");
      if (GUILayout.Button("按钮")) Debug.Log("点击");
      GUILayout.BeginHorizontal();
      GUILayout.Label("左"); GUILayout.Label("右");
      GUILayout.EndHorizontal();
  }
  ```

---

#### `EditorGUILayout` 的设计与使用
- **设计理念**：
  - **编辑器集成**：专为编辑器设计，与序列化系统深度结合。
  - **一致性**：模仿Inspector风格，简化工具开发。
- **核心方法**：
  - `EditorGUILayout.LabelField(string)`：编辑器风格标签。
  - `EditorGUILayout.IntField(string, int)`：整数输入框。
  - `EditorGUILayout.PropertyField(SerializedProperty, bool)`：绘制序列化字段。
  - `EditorGUILayout.ObjectField(string, Object, Type)`：对象引用字段。
  - `EditorGUILayout.Foldout(bool, string)`：折叠菜单。
- **适用场景**：
  - Inspector、编辑器窗口。
- **示例**：
  ```csharp
  public override void OnInspectorGUI()
  {
      serializedObject.Update();
      EditorGUILayout.PropertyField(serializedObject.FindProperty("number"));
      serializedObject.ApplyModifiedProperties();
  }
  ```

---

#### 三种工具的设计对比
- **特性对比**：
  - `GUI`：手动布局，低级控制，无序列化支持。
  - `GUILayout`：自动布局，中级抽象，跨运行时/编辑器。
  - `EditorGUILayout`：编辑器专用，高级抽象，支持序列化。
- **类比**：
  - `GUI` → CSS绝对定位。
  - `GUILayout` → Flexbox。
  - `EditorGUILayout` → Bootstrap表单组件。
- **实现层级**：
  - `GUI` → 底层渲染。
  - `GUILayout` → 封装`GUI`+布局。
  - `EditorGUILayout` → 封装`GUILayout`+编辑器功能。

---

#### 序列化支持详解
- **概念**：
  - 序列化是将组件字段保存到磁盘并恢复的过程。
- **`SerializedObject`**：
  - 包装组件的序列化数据，通过`serializedObject`获取。
- **`SerializedProperty`**：
  - 表示具体字段，用`FindProperty("字段名")`查找。
- **`PropertyField`**：
  - 自动绘制序列化字段UI，简化编辑。
- **简化之处**：
  - 一行代码处理字段显示和修改，支持复杂类型（如数组）。
- **示例**：
  ```csharp
  serializedObject.Update();
  EditorGUILayout.PropertyField(serializedObject.FindProperty("numbers"), true);
  serializedObject.ApplyModifiedProperties();
  ```

---

#### 强类型优化（`nameof`）
- **问题**：
  - `FindProperty("number")`用字符串麻烦，易错。
- **解决方案**：
  - 用`nameof(MyComponent.number)`实现强类型。
- **优化版**：
  ```csharp
  private SerializedProperty numberProp;
  void OnEnable()
  {
      numberProp = serializedObject.FindProperty(nameof(MyComponent.number));
  }
  public override void OnInspectorGUI()
  {
      serializedObject.Update();
      EditorGUILayout.PropertyField(numberProp);
      serializedObject.ApplyModifiedProperties();
  }
  ```
- **优点**：
  - 编译时检查，重命名安全，简洁优雅。

---

#### 运行模式适应性
- **基于事件（如Inspector）**：
  - 三者均可运行，按需渲染。
- **每帧执行（如`OnGUI`）**：
  - `GUI`和`GUILayout`：运行时+编辑器均可。
  - `EditorGUILayout`：仅编辑器（如`EditorWindow`）。
- **示例**：
  ```csharp
  public override void OnInspectorGUI() // 按需
  {
      GUI.Label(new Rect(10, 10, 100, 20), "GUI");
      GUILayout.Label("GUILayout");
      EditorGUILayout.LabelField("EditorGUILayout");
  }
  void OnGUI() // 每帧
  {
      GUI.Label(new Rect(10, 10, 100, 20), "GUI");
      GUILayout.Label("GUILayout");
      EditorGUILayout.LabelField("EditorGUILayout"); // 仅编辑器
  }
  ```

---

#### 实用程序集：`GUIUtility`
- **对应**：`GUI`。
- **作用**：底层GUI支持。
- **核心方法**：
  - `ScreenToGUIPoint`：屏幕坐标转GUI坐标。
  - `GetControlID`：生成控件ID。
  - `hotControl`：获取活跃控件。
- **示例**：
  ```csharp
  void OnGUI()
  {
      Vector2 guiPos = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);
      GUI.Label(new Rect(guiPos.x, guiPos.y, 100, 20), "鼠标");
  }
  ```

---

#### 实用程序集：`GUILayoutUtility`
- **对应**：`GUILayout`。
- **作用**：布局计算。
- **核心方法**：
  - `GetRect(float width, float height)`：获取布局矩形。
  - `GetLastRect()`：获取上个控件矩形。
- **示例**：
  ```csharp
  void OnGUI()
  {
      GUILayout.Label("标签");
      Rect lastRect = GUILayoutUtility.GetLastRect();
      GUI.Box(lastRect, "框");
  }
  ```

---

#### 实用程序集：`EditorUtility`
- **对应**：`EditorGUILayout`。
- **作用**：编辑器功能支持。
- **核心方法**：
  - `SetDirty(Object)`：标记脏数据。
  - `DisplayDialog(string, string, string)`：显示对话框。
  - `OpenFilePanel(string, string, string)`：文件选择。
- **示例**：
  ```csharp
  public override void OnInspectorGUI()
  {
      if (GUILayout.Button("提示"))
          EditorUtility.DisplayDialog("提示", "保存?", "是");
  }
  ```

---

#### 综合总结
- **绘制工具**：
  - `GUI`：精确，手动，低级。
  - `GUILayout`：自动，便利，中级。
  - `EditorGUILayout`：编辑器，序列化，高级。
- **实用程序集**：
  - `GUIUtility`：底层控制。
  - `GUILayoutUtility`：布局辅助。
  - `EditorUtility`：编辑器增强。
- **适应性**：
  - 三者支持按需和每帧渲染（`EditorGUILayout`限编辑器）。

### 拓展Inspector—Editor

当下搞懂了运行方式和绘制工具，接下来的内容真的是超级简单，我们先一起来拓展检视窗口，检视窗口是展示一个对象（包括资产和场景窗口）的信息的窗口，对于场景里面的对象，展示的就是这个对象身上组件的信息，可以是unity提供的也可以是我们自己的，对于资产，则是资产的信息，比如对于图片，就是分辨率，压缩格式等等

对于检视拓展，有两种类型，第一种是对整个组件拓展，第二种是对单个属性

##### 我们先来看整个组件拓展的使用流程

请按照以下顺序尝试，看看函数重载，多思考，而不是直接拷贝！

1. ```csharp
   /// <summary>
   /// 需要拓展的组件，可以是Unity的，也可以是我们自己写的
   /// </summary>
   
   public class MyComponent : MonoBehaviour
   {
       public int myInt = 10;
       public float myFloat = 5.0f;
       public string myString = "Hello";
       public bool myBool = false;
   }
   ```

2. ```csharp
   [CustomEditor(typeof(MyComponent))]//关联的组件
   public class InspectorEditor : Editor
   {
       public override void OnInspectorGUI()
       {
           MyComponent myComponent = (MyComponent)target;//这个target是Editor对象的属性，不用担心为null和类型安全问题，因为它不可能是null而且类型一定是MyComponent，因
                                                         //为这个检视拓展能够执行的核心条件就是用户点击到了该组件(您添加了特性！)，只有匹配才会执行这个回调
          //这种Begin,End的设计模式是状态模式+建造模式+组合模式
           GUILayout.BeginArea(new Rect(0,0,100,100));//相对于初始定位(点击的位置或者是window的左上角开始)定义一个区域，相当于是div
           myComponent.myString=GUILayout.TextField(myComponent.myString, new GUIStyle());
           GUILayout.EndArea();
   
           //GUILayout没有提供int等类型的输入框，所以下面使用EditorGUILayout
           EditorGUILayout.BeginVertical();
           myComponent.myInt=EditorGUILayout.IntSlider("My Int",myComponent.myInt, 0, 100);
           myComponent.myFloat= EditorGUILayout.FloatField("My Float",myComponent.myFloat);
           GUILayout.EndVertical();
           //绘制按钮
           if(GUILayout.Button("重置数值"))
           {
               myComponent.myInt = 50;
               myComponent.myFloat= 66.6f;
               myComponent.myString = "Ouse";
           }
       }
   }
   
   ```

3. 然后你就能看见下面的样式
   ![image-20250331231740307](assets/image-20250331231740307.png)

4. 您会发现原本应该由Unity自动渲染的内容，现在都没有渲染了，全部变成了你的逻辑，比如myBool变量，如果您想绘制原有的，您需要加上

   ```csharp
           DrawDefaultInspector();
   ```

   如下:

   ```csharp
    public override void OnInspectorGUI()
    {
        MyComponent myComponent = (MyComponent)target;//这个target是Editor对象的属性，不用担心为null和类型安全问题，因为它不可能是null而且类型一定是MyComponent，因
                                                      //为这个检视拓展能够执行的核心条件就是用户点击到了该组件(您添加了特性！)，只有匹配才会执行这个回调
       //这种Begin,End的设计模式是状态模式+建造模式+组合模式
        GUILayout.BeginArea(new Rect(0,0,100,100));//相对于初始定位(点击的位置或者是window的左上角开始)定义一个区域，相当于是div
        myComponent.myString=GUILayout.TextField(myComponent.myString, new GUIStyle());
        GUILayout.EndArea();
   
        //GUILayout没有提供int等类型的输入框，所以下面使用EditorGUILayout
        EditorGUILayout.BeginVertical();
        myComponent.myInt=EditorGUILayout.IntSlider("My Int",myComponent.myInt, 0, 100);
        myComponent.myFloat= EditorGUILayout.FloatField("My Float",myComponent.myFloat);
        GUILayout.EndVertical();
        //绘制按钮
        if(GUILayout.Button("重置数值"))
        {
            myComponent.myInt = 50;
            myComponent.myFloat= 66.6f;
            myComponent.myString = "Ouse";
        }
        // 绘制默认 Inspector（包含所有字段）
        DrawDefaultInspector();
    }
   ```

   ![image-20250331232351071](assets/image-20250331232351071.png)

   是不是很Amazing！
   
   ##### 下面我们来尝试对单个属性
   
   假设我们想要创建一个名为PositiveValueAttribute的自定义属性，用于确保某个字段的值始终为正数，并在Inspector中提供额外的视觉反馈（例如，如果值是负数，显示警告）。我们将为这个属性创建一个PropertyDrawer，实现以下功能：
   
   1. 如果值是负数，字段背景变成红色并显示警告。
   2. 提供一个按钮，手动将负值重置为0。

使用流程：

1. 首先自定义一个特性,拓展需要的属性，这些属性会添加到应用该特性的字段的元数据里面

   ```c#
   public class PositiveValueAttribute : PropertyAttribute
   {
       // 可以扩展属性，比如添加一个提示信息字段
       public string warningMessage = "Value must be positive!";
   }
   ```

2. 将渲染逻辑绑定到这个特性上

   ```csharp
   [CustomPropertyDrawer(typeof(PositiveValueAttribute))]
   public class PositiveValueDrawer : PropertyDrawer
   {
       public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
       {
           // 获取关联的PositiveValueAttribute
           PositiveValueAttribute attr = attribute as PositiveValueAttribute;
           // 开始属性绘制
           EditorGUI.BeginProperty(position, label, property);
           // 根据属性类型处理
           if (property.propertyType == SerializedPropertyType.Float)
           {
               float value = property.floatValue;
               DrawField(position, property, label, attr, value, () => property.floatValue = 0f);
           }
           else if (property.propertyType == SerializedPropertyType.Integer)
           {
               int value = property.intValue;
               DrawField(position, property, label, attr, value, () => property.intValue = 0);
           }
           else
           {
               EditorGUI.LabelField(position, label.text, "Use PositiveValue with int or float.");
           }
   
           EditorGUI.EndProperty();
       }
   
       private void DrawField<T>(Rect position, SerializedProperty property, GUIContent label,
                               PositiveValueAttribute attr, T value, System.Action resetAction)
                               where T : System.IComparable<T>
       {
           // 分割区域：左侧为字段，右侧为重置按钮
           Rect fieldRect = new Rect(position.x, position.y, position.width - 60, position.height);
           Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 50, position.height);
   
           // 检查值是否为负数
           bool isNegative = value.CompareTo(default(T)) < 0;
   
           // 如果是负数，设置红色背景
           if (isNegative)
           {
               GUIStyle style = new GUIStyle(GUI.skin.box) { normal = { background = MakeTex(2, 2, Color.red) } };
               GUI.Box(position, GUIContent.none, style);
               label.tooltip = attr.warningMessage; // 添加警告提示
           }
           // 绘制字段
           EditorGUI.BeginChangeCheck();
           if (property.propertyType == SerializedPropertyType.Float)
               property.floatValue = EditorGUI.FloatField(fieldRect, label, property.floatValue);
           else if (property.propertyType == SerializedPropertyType.Integer)
               property.intValue = EditorGUI.IntField(fieldRect, label, property.intValue);
   
           if (EditorGUI.EndChangeCheck())
               property.serializedObject.ApplyModifiedProperties();
           // 绘制重置按钮
           if (isNegative && GUI.Button(buttonRect, "Reset"))
           {
               resetAction();
               property.serializedObject.ApplyModifiedProperties();
           }
       }
   
       // 创建纯色纹理的辅助方法
       private Texture2D MakeTex(int width, int height, Color color)
       {
           Color[] pix = new Color[width * height];
           for (int i = 0; i < pix.Length; i++)
               pix[i] = color;
           Texture2D result = new Texture2D(width, height);
           result.SetPixels(pix);
           result.Apply();
           return result;
       }
   }
   ```

   

3. 将特性应用到你需要应用的字段上面

   ```csharp
   public class AttributeTest : MonoBehaviour
   {
       [PositiveValue]
       public float health = 100f;
   
       [PositiveValue]
       public int ammo = 10;
   }
   ```

   

4. 结果

   ![image-20250401141903585](assets/image-20250401141903585.png)

### 拓展Menu—[MenuItem]

AI讲的非常棒，直接放AI讲的

#### 2.1 什么是菜单扩展？

菜单扩展是通过在Unity编辑器的菜单栏中添加自定义选项，来执行特定的功能或脚本逻辑。它非常适合：
- 快速创建对象或资源。
- 执行批量操作。
- 打开自定义工具窗口。

在Unity中，菜单扩展主要通过`[MenuItem]`特性实现。

#### 2.2 实现方式

- **工具**：使用C#静态方法，结合`[MenuItem]`特性。
- **位置**：代码可以放在任何脚本中，不需要专门放在`Editor`文件夹（但如果是编辑器专用的功能，建议放在`Editor`文件夹）。
- **触发**：用户点击菜单项时，Unity调用对应的静态方法。

---

#### 2.3 基础示例：添加简单菜单项

#### 示例代码
以下是一个简单的例子，在“GameObject”菜单下添加一个选项，用于创建带有默认设置的Cube：

```csharp
using UnityEditor;
using UnityEngine;

public class MenuExample
{
    [MenuItem("GameObject/Create Custom Cube")]
    public static void CreateCustomCube()
    {
        // 创建一个新的Cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "CustomCube";
        cube.transform.position = Vector3.zero;

        // 注册Undo操作
        Undo.RegisterCreatedObjectUndo(cube, "Create Custom Cube");
    }
}
```

#### 运行结果
1. 在Unity编辑器顶部菜单栏，点击“GameObject”。
2. 你会看到一个新选项“Create Custom Cube”。
3. 点击后，场景中会生成一个名为“CustomCube”的立方体，位于坐标(0, 0, 0)。

#### 代码解析
- **`[MenuItem("GameObject/Create Custom Cube")]`**：
  - 定义菜单路径，`GameObject`是现有菜单，`Create Custom Cube`是新项。
  - 路径用`/`分隔，可以嵌套多级（例如`"Tools/MyTool/SubTool"`）。
- **静态方法**：
  - `CreateCustomCube`必须是`static`，因为菜单项不依赖实例。
- **`Undo.RegisterCreatedObjectUndo`**：
  - 记录操作到Undo栈，用户可以通过Ctrl+Z撤销。

---

#### 2.4 进阶功能

#### 2.4.1 菜单项验证（Validation）
你可以通过添加验证方法，控制菜单项是否启用。例如，只有在场景中选中物体时才启用菜单。

```csharp
using UnityEditor;
using UnityEngine;

public class MenuExample
{
    [MenuItem("Tools/Reset Selected Position")]
    public static void ResetPosition()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Reset Position");
            obj.transform.position = Vector3.zero;
        }
    }

    [MenuItem("Tools/Reset Selected Position", true)]
    public static bool ValidateResetPosition()
    {
        return Selection.gameObjects.Length > 0; // 只有选中物体时启用
    }
}
```

- **`ValidateResetPosition`**：
  - 方法名与主方法一致，但第二个参数为`true`，表示这是验证方法。
  - 返回`true`时菜单项启用，返回`false`时变灰。
- **`Selection.gameObjects`**：
  - 获取当前选中的GameObject数组。

#### 2.4.2 快捷键
你可以在菜单项中绑定快捷键，提升操作效率。

```csharp
[MenuItem("Tools/Reset Selected Position %r")] // % 表示 Ctrl/Cmd，r 表示 R 键
public static void ResetPosition()
{
    foreach (GameObject obj in Selection.gameObjects)
    {
        Undo.RecordObject(obj.transform, "Reset Position");
        obj.transform.position = Vector3.zero;
    }
}
```

- **快捷键语法**：
  - `%`：Ctrl (Windows) 或 Cmd (Mac)。
  - `#`：Shift。
  - `&`：Alt。
  - 后接字母或数字，例如`%r`表示Ctrl+R。

#### 2.4.3 菜单优先级（Priority）
可以通过设置优先级调整菜单项的顺序或分组。

```csharp
using UnityEditor;
using UnityEngine;

public class MenuExample
{
    [MenuItem("Tools/My Tools/Create Cube", false, 10)]
    public static void CreateCube() { /* ... */ }

    [MenuItem("Tools/My Tools/Delete Selected", false, 11)]
    public static void DeleteSelected() { /* ... */ }

    [MenuItem("Tools/My Tools/Reset All", false, 20)]
    public static void ResetAll() { /* ... */ }
}
```

- **`false, 10`**：
  - 第二个参数表示不是验证方法。
  - 第三个参数是优先级，数值越小越靠前。
  - 优先级相差10以上会自动添加分隔线。

---

#### 2.5 实战示例：批量操作工具

#### 需求
创建一个菜单项，批量为选中的GameObject添加指定组件（例如`Rigidbody`）。

#### 代码
```csharp
using UnityEditor;
using UnityEngine;

public class BatchTool
{
    [MenuItem("Tools/Batch/Add Rigidbody")]
    public static void AddRigidbodyToSelected()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (!obj.GetComponent<Rigidbody>())
            {
                Undo.AddComponent<Rigidbody>(obj, "Add Rigidbody");
            }
        }
    }

    [MenuItem("Tools/Batch/Add Rigidbody", true)]
    public static bool ValidateAddRigidbody()
    {
        return Selection.gameObjects.Length > 0;
    }
}
```

#### 效果
- 选中多个GameObject后，点击“Tools/Batch/Add Rigidbody”。
- 所有未拥有`Rigidbody`的物体都会添加该组件，并支持Undo。

---

#### 2.6 底层原理

#### 2.6.1 [MenuItem] 如何工作？
- **反射扫描**：
  - 在编辑器启动或脚本编译时，Unity扫描所有程序集，查找带有`[MenuItem]`特性的静态方法。
  - 将菜单路径和方法注册到C++层的菜单系统中。
- **菜单构建**：
  - Unity的菜单系统（基于C++）根据注册信息动态生成菜单项。
  - 验证方法（如`ValidateResetPosition`）会在每次菜单刷新时调用。
- **方法调用**：
  - 用户点击菜单项时，Unity通过Mono运行时调用对应的C#静态方法。

#### 2.6.2 伪代码（C++层）
```cpp
// Unity C++ 伪代码
class MenuSystem {
    struct MenuEntry {
        String path;
        ManagedMethod* method;
        ManagedMethod* validateMethod;
        int priority;
    };
    Vector<MenuEntry> menuEntries;

    void InitializeMenus() {
        for (each Assembly in ManagedAssemblies) {
            for (each Type in Assembly) {
                for (each Method in Type) {
                    if (Method.HasAttribute("MenuItem")) {
                        MenuItemAttr attr = Method.GetAttribute("MenuItem");
                        menuEntries.Add({ attr.path, Method, nullptr, attr.priority });
                    }
                }
            }
        }
    }

    void OnMenuClick(String path) {
        MenuEntry* entry = FindEntry(path);
        if (entry && (!entry->validateMethod || entry->validateMethod->Invoke())) {
            entry->method->Invoke();
        }
    }
};
```

---

#### 2.7 注意事项
1. **静态方法限制**：
   - 菜单项方法必须是`static`，无法直接访问实例数据。
   - 需要通过`Selection`或场景对象间接操作。
2. **Undo支持**：
   - 手动注册Undo操作（如`Undo.RegisterCreatedObjectUndo`），否则更改不可撤销。
3. **性能**：
   - 避免在验证方法中执行复杂计算，因为它每帧都可能被调用。

---

#### 2.8 总结

菜单扩展通过`[MenuItem]`特性实现，简单却强大，适用于快速工具开发。你可以：
- 添加自定义创建选项。
- 执行批量操作。
- 绑定快捷键提升效率。

### 拓展window— EditorWindow

与AI深入交流后，得到了下面的一份内容

#### 一、自定义窗口的基础

##### 1.1 什么是自定义窗口？
- **定义**：`EditorWindow`是Unity编辑器中的独立浮动或停靠窗口，用于实现特定功能（如关卡编辑、资源管理）。
- **特点**：
  - 基于IMGUI（即时模式GUI）绘制。
  - 必须放在`Editor`文件夹下。
  - 通过菜单项或其他代码打开。

##### 1.2 基础示例
以下是一个简单的窗口，展示按钮和文本输入：

```csharp
using UnityEditor;
using UnityEngine;

public class SimpleEditorWindow : EditorWindow
{
    private string inputText = "Hello, Unity!";

    [MenuItem("Tools/My Custom Window")]
    public static void ShowWindow()
    {
        GetWindow<SimpleEditorWindow>("My Window");
    }

    private void OnGUI()
    {
        inputText = EditorGUILayout.TextField("Input:", inputText);
        if (GUILayout.Button("Print"))
        {
            Debug.Log(inputText);
        }
    }
}
```

- **运行**：
  - 点击“Tools/My Custom Window”，打开一个标题为“My Window”的窗口。
  - 输入文本并点击按钮，控制台输出内容。

---

#### 二、核心API

`EditorWindow`提供了丰富的API，分为以下几类：

##### 2.1 创建与管理
- **`GetWindow<T>(string title)`**：
  - 创建或获取窗口实例，设置标题。
  - 示例：`GetWindow<MyWindow>("My Window");`
- **`Close()`**：
  - 关闭窗口。
- **`position`**：
  - 获取/设置窗口位置和大小（`Rect`）。
  - 示例：`position = new Rect(100, 100, 300, 200);`

##### 2.2 生命周期
- **`OnEnable()`**：窗口启用时调用，初始化资源。
- **`OnDisable()`**：窗口禁用时调用，清理资源。
- **`OnGUI()`**：每帧调用，绘制UI。
- **`OnDestroy()`**：窗口销毁时调用。

##### 2.3 绘制与交互
- **`EditorGUILayout`**：自动布局控件。
  - `TextField`、`Slider`、`Button` 等。
- **`EditorGUI`**：手动布局控件，需指定`Rect`。
- **`GUI`**：基础IMGUI控件。
- **`Repaint()`**：强制刷新窗口UI。

##### 2.4 其他实用API
- **`titleContent`**：设置标题和图标（`GUIContent`）。
  - 示例：`titleContent = new GUIContent("Tool", EditorGUIUtility.FindTexture("UnityLogo"));`
- **`minSize` / `maxSize`**：设置窗口尺寸限制。
- **`Show()`**：显示窗口。

---

#### 三、渲染逻辑与 Repaint

##### 3.1 渲染逻辑
- **IMGUI 的每帧渲染**：
  
  - `OnGUI`每帧调用，生成绘制命令。
  - C++层主循环驱动，调用C#的`OnGUI`，最终由原生UI系统渲染。
- **底层伪代码**：
  
  ```cpp
  void EditorMainLoop() {
      while (running) {
          ProcessInputEvents();
          UpdateEditorState();
          DrawAllWindows();
          RenderFrame();
      }
  }
  void DrawAllWindows() {
      for (EditorWindow* win : activeWindows) {
          if (win->IsVisible() && (win->NeedsRepaint() || HasEvents())) {
              win->OnGUI();
              win->ClearRepaintFlag();
          }
      }
  }
  ```

##### 3.2 为什么需要 Repaint？
- **误解**：`OnGUI`每帧调用，UI应该自动更新。
- **真相**：
  
  - 逻辑上每帧执行，但实际渲染是**事件驱动**的。
  - 数据在`OnGUI`外更新（如`EditorApplication.update`），不会立即触发屏幕刷新。
- **Repaint 的作用**：
  
  - 设置`NeedsRepaint`标志，强制当前帧结束后重绘。
  - 示例：
    ```csharp
    private void UpdateCount()
    {
        objectCount = GameObject.FindObjectsOfType<GameObject>().Length;
        Repaint(); // 确保UI实时更新
    }
    ```

##### 3.3 何时需要手动刷新？
- **需要 Repaint**：
  - 数据在`OnGUI`外修改（例如`EditorApplication.update`）。
  - 要求实时显示变化。
- **无需 Repaint**：
  - 数据在`OnGUI`内修改（如用户输入），Unity自动处理事件。

---

#### 四、保存机制

##### 4.1 窗口数据的保存问题
- **窗口变量**：`EditorWindow`的字段（如`private int counter`）是临时的，关闭窗口后丢失。
- **解决**：将数据绑定到持久化对象（如`GameObject`或`ScriptableObject`），并标记为“脏”。

##### 4.2 EditorUtility.SetDirty
- **作用**：标记`UnityEngine.Object`为“脏”，通知序列化系统需要保存。
- **底层**：
  
  - 设置C++层的`isDirty`标志。
  - 伪代码：
    ```cpp
    void SetDirty(Object* obj) {
        obj->SetDirtyFlag(true);
        MarkSceneDirty(obj->GetScene());
    }
    ```

##### 4.3 示例：绑定到场景对象
```csharp
public class SceneSaveWindow : EditorWindow
{
    private GameObject targetObject;
    private float scale = 1f;

    [MenuItem("Tools/Scene Save Window")]
    public static void ShowWindow()
    {
        GetWindow<SceneSaveWindow>("Scene Save");
    }

    private void OnGUI()
    {
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target", targetObject, typeof(GameObject), true);
        if (targetObject != null)
        {
            scale = EditorGUILayout.Slider("Scale", scale, 0.1f, 5f);
            if (GUILayout.Button("Apply"))
            {
                Undo.RecordObject(targetObject.transform, "Change Scale");
                targetObject.transform.localScale = Vector3.one * scale;
                EditorUtility.SetDirty(targetObject); // 标记为脏
            }
        }
    }
}
```
- **效果**：修改场景中物体的缩放，保存到场景文件。

##### 4.4 示例：动态创建并保存 ScriptableObject
```csharp
public class DynamicSOWindow : EditorWindow
{
    private DynamicData data;
    private string assetPath = "Assets/DynamicData.asset";

    [MenuItem("Tools/Dynamic SO Window")]
    public static void ShowWindow()
    {
        GetWindow<DynamicSOWindow>("Dynamic SO");
    }

    private void OnEnable()
    {
        data = AssetDatabase.LoadAssetAtPath<DynamicData>(assetPath);
    }

    private void OnGUI()
    {
        if (data == null)
        {
            if (GUILayout.Button("Create New Data"))
            {
                data = ScriptableObject.CreateInstance<DynamicData>();
                AssetDatabase.CreateAsset(data, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = data;
            }
        }
        else
        {
            Undo.RecordObject(data, "Edit Dynamic Data");
            data.name = EditorGUILayout.TextField("Name", data.name);
            data.value = EditorGUILayout.IntField("Value", data.value);
            data.color = EditorGUILayout.ColorField("Color", data.color);
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
        }
    }
}

public class DynamicData : ScriptableObject
{
    public string name = "Default Name";
    public int value = 42;
    public Color color = Color.white;
}
```
- **效果**：
  - 动态创建`DynamicData`资产，保存到`Assets/DynamicData.asset`。
  - 编辑字段并持久化，下次打开窗口加载已有数据。

---

#### 五、脏系统的细节

##### 5.1 默认是否应用脏？
- **Inspector 修改**：
  - 通过`SerializedProperty`修改，Unity自动标记为“脏”。
  - 示例：`public int a;`在Inspector中改值，自动保存。
- **代码修改**：
  - 直接修改内存（如`a = 10`），不自动标记。
  - 需要`SetDirty`或`ApplyModifiedProperties`。

##### 5.2 不标记的后果
- **仅内存修改**：
  - 数据在C#层更新，Inspector显示新值。
  - 但不持久化，下次打开Unity恢复原始值。
- **实验**：
  ```csharp
  test.a = 42; // 无 SetDirty
  ```
  - 关闭Unity后，`a`丢失。

##### 5.3 完整保存流程
- **推荐写法**：
  ```csharp
  Undo.RecordObject(test, "Change A");
  test.a = 42;
  EditorUtility.SetDirty(test);
  ```
- **SerializedObject 替代**：
  ```csharp
  SerializedObject so = new SerializedObject(test);
  so.FindProperty("a").intValue = 42;
  so.ApplyModifiedProperties();
  ```

---

#### 六、常见问题与解答

##### 6.1 为什么 OnGUI 每帧调用还需要 Repaint？
- **原因**：渲染是事件驱动的，数据在`OnGUI`外更新不触发刷新。
- **解决**：`Repaint()`强制重绘。

##### 6.2 修改不标记，序列化系统起作用吗？
- **部分起作用**：`Undo.RecordObject`同步局部修改，但不保证整个对象保存。
- **完整起作用**：需要`SetDirty`。

---

#### 七、总结

- **功能**：`EditorWindow`创建独立工具窗口，支持动态UI和交互。
- **API**：涵盖创建、生命周期、绘制和保存。
- **渲染**：IMGUI每帧调用`OnGUI`，但刷新需事件或`Repaint`。
- **保存**：绑定场景对象或`ScriptableObject`，用`SetDirty`和`AssetDatabase`持久化。
- **脏系统**：代码修改需手动标记，Inspector修改自动标记。

### 编辑器开发常用Api

#### 一、获取当前操作和输入状态

##### 1.1 监听鼠标和键盘输入
- **`Event.current`**：
  - **功能**：获取当前帧的输入事件（鼠标、键盘等），在 `OnGUI` 中使用。
  - **属性**：
    - `type`：事件类型（如 `EventType.MouseDown`、`EventType.KeyDown`）。
    - `mousePosition`：鼠标位置（屏幕坐标）。
    - `keyCode`：按下的键盘键。
    - `modifiers`：修饰键（如 Ctrl、Shift）。
  - **示例**：
    ```csharp
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0) // 左键点击
        {
            Debug.Log("Left mouse clicked at: " + e.mousePosition);
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
        {
            Debug.Log("Space key pressed!");
        }
    }
    ```

##### 1.2 判断操作类型
- **`EventType`**：
  - **常用值**：
    - `MouseDown` / `MouseUp`：鼠标按下/松开。
    - `KeyDown` / `KeyUp`：键盘按下/松开。
    - `ScrollWheel`：鼠标滚轮。
    - `Repaint`：窗口重绘。
  - **示例**：
    ```csharp
    private void OnGUI()
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown:
                Debug.Log("Mouse down detected");
                break;
            case EventType.KeyDown:
                Debug.Log("Key down: " + e.keyCode);
                break;
        }
    }
    ```

##### 1.3 修饰键状态
- **`Event.modifiers`**：
  - **功能**：检测 Ctrl、Shift、Alt 等修饰键。
  - **示例**：
    ```csharp
    if (Event.current.type == EventType.KeyDown && Event.current.control)
    {
        Debug.Log("Ctrl + " + Event.current.keyCode + " pressed");
    }
    ```

---

#### 二、获取当前选中的对象

##### 2.1 获取选中的 GameObject
- **`Selection`**：
  - **功能**：提供对当前选中对象的访问。
  - **常用属性/方法**：
    - `Selection.activeGameObject`：当前活动 GameObject。
    - `Selection.gameObjects`：所有选中的 GameObject 数组。
    - `Selection.objects`：所有选中的对象（包括资产）。
  - **示例**：
    ```csharp
    private void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            EditorGUILayout.LabelField("Selected:", Selection.activeGameObject.name);
        }
        else
        {
            EditorGUILayout.LabelField("No GameObject selected");
        }
    }
    ```

##### 2.2 监听选择变化
- **`Selection.selectionChanged`**：
  - **功能**：当选择发生变化时触发的事件。
  - **示例**：
    ```csharp
    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }
    
    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }
    
    private void OnSelectionChanged()
    {
        Debug.Log("Selection changed to: " + (Selection.activeObject?.name ?? "None"));
    }
    ```

---

#### 三、获取编辑器上下文和状态

##### 3.1 当前场景信息
- **`SceneView`**：
  - **功能**：访问当前场景视图。
  - **`SceneView.lastActiveSceneView`**：获取最后一个活动 Scene View。
  - **示例**：
    ```csharp
    private void OnGUI()
    {
        if (GUILayout.Button("Focus Scene View"))
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
    }
    ```

##### 3.2 编辑器模式
- **`EditorApplication.isPlaying`**：
  - **功能**：判断当前是否在 Play Mode。
  - **示例**：
    ```csharp
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Play Mode:", EditorApplication.isPlaying.ToString());
    }
    ```

##### 3.3 项目资产
- **`AssetDatabase`**：
  - **功能**：管理项目中的资产。
  - **常用方法**：
    - `AssetDatabase.LoadAssetAtPath<T>(string path)`：加载资产。
    - `AssetDatabase.CreateAsset(Object asset, string path)`：创建资产。
  - **示例**：
    ```csharp
    private void CreateAsset()
    {
        ScriptableObject so = ScriptableObject.CreateInstance<ScriptableObject>();
        AssetDatabase.CreateAsset(so, "Assets/NewAsset.asset");
    }
    ```

---

#### 四、Undo 系统相关 API

在编辑器开发中，Undo 是操作的核心支持，常用 API 包括：
- **`Undo.RecordObject(Object target, string name)`**：
  - 记录对象状态。
- **`Undo.RegisterCreatedObjectUndo(Object objectToUndo, string name)`**：
  - 记录创建操作。
- **`Undo.DestroyObjectImmediate(Object objectToUndo)`**：
  - 删除并支持撤销。

---

#### 五、综合示例：监听操作并处理对象

以下是一个综合示例，展示如何监听用户操作、获取选中对象并支持 Undo：

```csharp
using UnityEditor;
using UnityEngine;

public class OperationMonitorWindow : EditorWindow
{
    private Vector2 scrollPos;

    [MenuItem("Tools/Operation Monitor")]
    public static void ShowWindow()
    {
        GetWindow<OperationMonitorWindow>("Operation Monitor");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += Repaint; // 选择变化时刷新
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // 显示当前输入状态
        EditorGUILayout.LabelField("Mouse Position:", e.mousePosition.ToString());
        EditorGUILayout.LabelField("Event Type:", e.type.ToString());
        EditorGUILayout.LabelField("Key Pressed:", e.keyCode.ToString());
        EditorGUILayout.LabelField("Modifiers:", e.modifiers.ToString());

        // 显示选中对象
        if (Selection.activeGameObject != null)
        {
            EditorGUILayout.LabelField("Selected Object:", Selection.activeGameObject.name);
            if (GUILayout.Button("Scale Up"))
            {
                Undo.RecordObject(Selection.activeGameObject.transform, "Scale Up");
                Selection.activeGameObject.transform.localScale *= 1.1f;
                EditorUtility.SetDirty(Selection.activeGameObject);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No object selected");
        }

        // 处理鼠标和键盘输入
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Debug.Log("Left click at: " + e.mousePosition);
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R && e.control)
        {
            Debug.Log("Ctrl+R pressed: Resetting scale");
            if (Selection.activeGameObject != null)
            {
                Undo.RecordObject(Selection.activeGameObject.transform, "Reset Scale");
                Selection.activeGameObject.transform.localScale = Vector3.one;
                EditorUtility.SetDirty(Selection.activeGameObject);
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
```

- **输入监听**：
  - 使用 `Event.current` 实时显示鼠标位置、事件类型和按键。
  - 检测左键点击和 Ctrl+R 组合键。
- **对象操作**：
  - 显示当前选中的 GameObject。
  - “Scale Up” 按钮放大物体，支持 Undo。
  - Ctrl+R 重置缩放。
- **状态刷新**：
  - `Selection.selectionChanged` 触发窗口重绘。

---

#### 六、底层原理与注意事项

##### 6.1 事件系统
- **`Event.current`**：
  - 由 C++ 层主循环捕获输入，传递到 C#。
  - 在 `OnGUI` 中每帧更新，反映当前帧的状态。

##### 6.2 选择机制
- **`Selection`**：
  - Unity 在 C++ 层维护一个全局选择状态，C# 通过桥接访问。
  - 变化时触发事件，异步通知开发者。

##### 6.3 注意事项
- **事件使用**：
  - `Event.current` 只能在 `OnGUI` 中访问，需注意上下文。
- **性能**：
  - 频繁调用 `FindObjectOfType` 或 `AssetDatabase` 操作可能影响性能，建议缓存。
- **Undo 搭配**：
  - 修改对象时始终使用 Undo，确保用户体验。

---

#### 七、总结

在编辑器开发中，常用 API 帮助我们获取用户操作和环境状态：
- **输入**：`Event.current` 监听鼠标、键盘。
- **选择**：`Selection` 获取选中对象。
- **上下文**：`SceneView`、`EditorApplication` 提供编辑器状态。
- **Undo**：支持操作回滚。



### 拓展Scene

#### 一、Scene View 扩展基础

##### 1.1 什么是 Scene View 扩展？
- **定义**：
  - Scene View 扩展是在 Unity 编辑器的 **Scene View**（场景视图）中添加自定义绘制和交互逻辑的功能。
  - 它是编辑器开发的重要部分，允许你在场景中直接可视化和操作内容。
- **用途**：
  - 绘制辅助图形（如网格、路径）。
  - 添加交互工具（如拖动控制点、点击创建物体）。
  - 增强场景编辑体验。

##### 1.2 与其他绘制方式的对比
- **Handles**：
  - 可交互的绘制工具，用于 Scene View 扩展。
  - 示例：拖动位置、旋转控制点。
- **Gizmos**：
  - 不可交互，仅用于可视化辅助。
  - 在 `OnDrawGizmos` 中使用，适用于运行时和编辑时。
- **区别**：
  - `Handles` 是编辑器专用的交互式工具，`Gizmos` 是静态显示。

---

#### 二、核心 API

Scene View 扩展依赖以下核心 API：

##### 2.1 Scene View 事件
- **`SceneView.duringSceneGui`**：
  - **功能**：全局事件，在 Scene View 的 GUI 绘制阶段触发。
  - **参数**：`SceneView` 对象，提供上下文（如相机位置）。
  - **订阅方式**：
    ```csharp
    SceneView.duringSceneGui += OnSceneGUI;
    ```
- **`OnSceneGUI()`**：
  - **功能**：绑定到特定组件的 Scene View 绘制方法，在 `CustomEditor` 中实现。

##### 2.2 绘制工具（Handles）
- **`Handles.DrawLine(Vector3 p1, Vector3 p2)`**：
  - 绘制 3D 线段。
- **`Handles.Label(Vector3 position, string text)`**：
  - 在指定位置显示文本。
- **`Handles.PositionHandle(Vector3 position, Quaternion rotation)`**：
  - 绘制可拖动的位置控制点，返回新位置。
- **`Handles.FreeMoveHandle`**：
  - 自由移动手柄，带自定义样式。
- **`Handles.color`**：
  - 设置绘制颜色。

##### 2.3 交互工具
- **`HandleUtility`**：
  - **`GUIPointToWorldRay(Vector2 guiPoint)`**：将 2D 鼠标位置转为 3D 射线。
  - **`PickGameObject(Vector2 position)`**：拾取鼠标下的物体。
- **`Tools.current`**：
  - 获取当前工具（`Tool.Move`、`Tool.Rotate` 等）。

##### 2.4 输入事件
- **`Event.current`**：
  - 监听鼠标、键盘输入。
  - 示例：检测点击、按键。

---

#### 三、实现方式

Scene View 扩展有两种主要实现方式：

##### 3.1 全局方式：独立脚本
通过订阅 `SceneView.duringSceneGui` 在全局绘制。

##### 示例 1：绘制网格
```csharp
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneGridDrawer
{
    static SceneGridDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.gray;
        float size = 10f;
        for (float x = -size; x <= size; x++)
        {
            Handles.DrawLine(new Vector3(x, 0, -size), new Vector3(x, 0, size));
        }
        for (float z = -size; z <= size; z++)
        {
            Handles.DrawLine(new Vector3(-size, 0, z), new Vector3(size, 0, z));
        }

        // 2D GUI 提示
        Handles.BeginGUI();
        GUI.Label(new Rect(10, 10, 200, 20), "Grid Size: " + size);
        Handles.EndGUI();
    }
}
```
- **效果**：
  - Scene View 显示一个 20x20 的灰色网格，带提示文本。
- **解析**：
  - `[InitializeOnLoad]`：编辑器启动时注册。
  - `Handles.BeginGUI` / `EndGUI`：绘制 2D UI。

##### 3.2 组件绑定方式：CustomEditor
通过 `OnSceneGUI` 为特定组件添加交互。

##### 示例 2：交互式控制点
```csharp
using UnityEngine;

public class ScenePoint : MonoBehaviour
{
    public Vector3 offset = Vector3.up;
}

// Editor 脚本
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenePoint))]
public class ScenePointEditor : Editor
{
    private void OnSceneGUI()
    {
        ScenePoint point = (ScenePoint)target;
        Vector3 worldPos = point.transform.position + point.offset;

        // 绘制连接线
        Handles.color = Color.green;
        Handles.DrawLine(point.transform.position, worldPos);

        // 可拖动控制点
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point, "Move Offset");
            point.offset = newPos - point.transform.position;
            EditorUtility.SetDirty(point);
        }

        // 标签
        Handles.Label(worldPos, "Offset: " + point.offset);
    }
}
```
- **效果**：
  - 选中 `ScenePoint` 组件的物体时，显示一个可拖动的控制点，连接到物体中心。
  - 支持 Undo。
- **解析**：
  - `Handles.PositionHandle`：内置位置控制点。
  - `EditorGUI.BeginChangeCheck`：检测拖动变化。

---

#### 四、进阶示例

##### 示例 3：点击创建物体
在 Scene View 中点击创建立方体：

```csharp
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneObjectCreator
{
    static SceneObjectCreator()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && e.control) // Ctrl + 左键
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = hit.point + Vector3.up * 0.5f;
                Undo.RegisterCreatedObjectUndo(cube, "Create Cube");
                e.Use(); // 消费事件
            }
        }

        Handles.BeginGUI();
        GUI.Label(new Rect(10, 10, 200, 20), "Ctrl + Left Click to create cube");
        Handles.EndGUI();
    }
}
```
- **效果**：
  - 按 Ctrl + 左键点击场景，创建立方体，支持 Undo。
- **解析**：
  - `HandleUtility.GUIPointToWorldRay`：计算点击位置。
  - `e.Use()`：防止事件传递给其他工具。

---

#### 五、底层原理

##### 5.1 Scene View 渲染流程
- **主循环**：
  - Scene View 由 C++ 层驱动，每帧渲染场景内容和附加的 GUI。
- **`duringSceneGui`**：
  - 在 GUI 阶段调用，C# 通过委托插入逻辑。
- **伪代码**：
  ```cpp
  void SceneView::Render() {
      RenderSceneGeometry(); // 绘制场景
      RenderHandles();       // 绘制 Handles
      for (Delegate* d : duringSceneGui) {
          d->Invoke(this);   // 调用 OnSceneGUI
      }
      RenderGUI();           // 绘制 2D GUI
  }
  ```

##### 5.2 Handles 的实现
- **绘制**：
  - 使用 OpenGL/DirectX 在 3D 空间渲染。
  - C# 层封装为 `Handles` API。
- **交互**：
  - `HandleUtility` 处理鼠标事件，映射到 3D 坐标。

---

#### 六、常见问题与解答

##### 6.1 如何避免冲突？
- **检查工具状态**：
  
  ```csharp
  if (Tools.current != Tool.None) return; // 只在无工具时生效
  ```

##### 6.2 如何优化性能？
- **缓存计算**：
  - 避免每帧重复计算（如射线检测）。
- **限制绘制范围**：
  - 只在需要时绘制，例如：
    ```csharp
    if (sceneView.camera.WorldToScreenPoint(worldPos).z > 0) { /* 绘制 */ }
    ```

##### 6.3 与 Gizmos 的区别？
- **Handles**：编辑器交互，动态控制。
- **Gizmos**：静态显示，无交互。

---

#### 七、总结

- **功能**：
  - Scene View 扩展通过绘制和交互增强场景编辑。
- **实现**：
  - 全局（`duringSceneGui`）或组件绑定（`OnSceneGUI`）。
- **核心 API**：
  - `Handles`：交互式绘制。
  - `HandleUtility`：鼠标映射。
  - `Event.current`：输入监听。
- **原理**：
  - C++ 渲染管线驱动，C# 插入逻辑。

### 拓展层级

#### 一、Hierarchy 扩展基础

##### 1.1 什么是 Hierarchy 扩展？
- **定义**：
  - Hierarchy 扩展是指在 Unity 编辑器的 **Hierarchy 窗口** 中自定义物体的显示和交互逻辑。
  - Hierarchy 窗口是显示场景中所有 GameObject 的层级视图，扩展它可以增强开发者的管理和识别能力。
- **用途**：
  - 为特定物体添加视觉标记（如图标、颜色）。
  - 自定义右键菜单。
  - 显示额外信息（如组件状态）。

##### 1.2 与其他扩展的区别
- **EditorWindow**：独立工具窗口。
- **Scene View 扩展**：场景视图绘制。
- **Hierarchy 扩展**：专注于层级窗口的显示和交互。

---

#### 二、核心 API

Hierarchy 扩展主要依赖以下 API：

##### 2.1 Hierarchy 绘制事件
- **`EditorApplication.hierarchyWindowItemOnGUI`**：
  
  - **功能**：在 Hierarchy 窗口绘制每个物体时触发。
  - **参数**：
    
    - `int instanceID`：物体的实例 ID。
    - `Rect selectionRect`：物体在 Hierarchy 中的绘制区域。
  - **订阅方式**：
    
    ```csharp
    EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    ```

##### 2.2 获取物体信息
- **`EditorUtility.InstanceIDToObject(int instanceID)`**：
  - 将实例 ID 转为对应的 `UnityEngine.Object`（如 `GameObject`）。
- **`GameObject.GetComponent<T>()`**：
  - 获取物体的组件。

##### 2.3 绘制工具
- **`GUI` / `EditorGUI`**：
  - 在 2D 空间绘制图标、文本等。
  - 示例：`GUI.Label`、`GUI.DrawTexture`。
- **`EditorGUIUtility`**：
  - 提供编辑器工具函数，如加载图标。
  - 示例：`EditorGUIUtility.ObjectContent`。

##### 2.4 交互控制
- **`Event.current`**：
  - 监听鼠标、键盘输入。
  - 示例：检测右键点击。
- **`GenericMenu`**：
  - 创建右键菜单。
  - 示例：`GenericMenu.AddItem`。

---

#### 三、实现方式

Hierarchy 扩展通常通过全局事件订阅实现，适用于所有场景中的物体。

##### 3.1 示例 1：为特定物体添加颜色标记
为带有特定组件的物体添加背景颜色：

```csharp
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyColorMarker
{
    static HierarchyColorMarker()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        // 检查特定组件（例如 Rigidbody）
        if (go.GetComponent<Rigidbody>() != null)
        {
            Rect bgRect = new Rect(selectionRect.x - 2, selectionRect.y, selectionRect.width + 4, selectionRect.height);
            EditorGUI.DrawRect(bgRect, new Color(0.2f, 0.6f, 0.2f, 0.3f)); // 绿色背景
        }
    }
}
```
- **效果**：
  - Hierarchy 中带有 `Rigidbody` 的物体显示绿色背景。
- **解析**：
  - `EditorGUI.DrawRect`：绘制背景矩形。
  - `selectionRect`：物体的绘制区域。

##### 3.2 示例 2：添加图标和右键菜单
为物体添加图标，并提供自定义右键菜单：

```csharp
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyIconAndMenu
{
    private static Texture2D icon;

    static HierarchyIconAndMenu()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        icon = EditorGUIUtility.FindTexture("Prefab Icon"); // 使用内置图标
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        // 添加图标（例如标记带有 Transform 的物体）
        Rect iconRect = new Rect(selectionRect.x - 18, selectionRect.y, 16, 16);
        if (go.GetComponent<Transform>() != null) // 示例条件
        {
            GUI.DrawTexture(iconRect, icon);
        }

        // 右键菜单
        Event e = Event.current;
        if (selectionRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 1)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Tag as Special"), false, () => TagAsSpecial(go));
            menu.AddItem(new GUIContent("Reset Position"), false, () => ResetPosition(go));
            menu.ShowAsContext();
            e.Use();
        }
    }

    private static void TagAsSpecial(GameObject go)
    {
        Debug.Log($"{go.name} tagged as special!");
    }

    private static void ResetPosition(GameObject go)
    {
        Undo.RecordObject(go.transform, "Reset Position");
        go.transform.position = Vector3.zero;
        EditorUtility.SetDirty(go);
    }
}
```
- **效果**：
  - Hierarchy 中每个物体左侧显示一个图标（这里用 Prefab 图标）。
  - 右键点击物体，弹出菜单，提供“Tag as Special”和“Reset Position”选项。
- **解析**：
  - `GUI.DrawTexture`：绘制图标。
  - `GenericMenu`：创建右键菜单。
  - `e.Use()`：消费事件，避免干扰默认行为。

---

#### 四、进阶示例

##### 示例 3：显示组件状态和交互
为带有特定组件的物体显示状态，并支持交互：

```csharp
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyComponentStatus
{
    static HierarchyComponentStatus()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null) return;

        Rect toggleRect = new Rect(selectionRect.xMax - 40, selectionRect.y, 16, 16);
        Rect labelRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 20, 16);

        // 检查并显示 Rigidbody 状态
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            bool isKinematic = EditorGUI.Toggle(toggleRect, rb.isKinematic);
            if (isKinematic != rb.isKinematic)
            {
                Undo.RecordObject(rb, "Toggle Kinematic");
                rb.isKinematic = isKinematic;
                EditorUtility.SetDirty(go);
            }
            GUI.Label(labelRect, "RB");
        }
    }
}
```
- **效果**：
  - Hierarchy 中带有 `Rigidbody` 的物体右侧显示一个勾选框（控制 `isKinematic`）和“RB”标签。
  - 支持 Undo。
- **解析**：
  - `EditorGUI.Toggle`：绘制交互式勾选框。
  - `selectionRect.xMax`：定位到右侧。

---

#### 五、底层原理

##### 5.1 Hierarchy 渲染流程
- **主循环**：
  - Hierarchy 窗口由 C++ 层渲染，每个物体调用绘制逻辑。
- **`hierarchyWindowItemOnGUI`**：
  - 在绘制每个物体时触发，C# 通过委托插入自定义逻辑。
- **伪代码**：
  ```cpp
  void HierarchyWindow::Draw() {
      for (GameObject* go : sceneObjects) {
          Rect rect = GetItemRect(go);
          DrawDefaultItem(go, rect);
          for (Delegate* d : hierarchyItemDelegates) {
              d->Invoke(go->GetInstanceID(), rect); // 调用 OnHierarchyGUI
          }
      }
  }
  ```

##### 5.2 绘制与交互
- **GUI 系统**：
  - 使用 IMGUI 在 2D 空间绘制，基于 Hierarchy 的坐标系。
- **事件处理**：
  - `Event.current` 捕获鼠标输入，`GenericMenu` 处理菜单。

---

#### 六、常见问题与解答

##### 6.1 如何避免覆盖默认绘制？
- **方法**：
  - 只在特定区域绘制（如左侧或右侧），不干扰 `selectionRect` 的文本。
  - 示例：`iconRect.x = selectionRect.x - 18`。

##### 6.2 如何优化性能？
- **缓存**：
  - 缓存 `EditorUtility.InstanceIDToObject` 的结果，避免每帧转换。
  - 示例：
    ```csharp
    static Dictionary<int, GameObject> cache = new Dictionary<int, GameObject>();
    GameObject go = cache.TryGetValue(instanceID, out var cached) ? cached : EditorUtility.InstanceIDToObject(instanceID) as GameObject;
    ```

##### 6.3 如何处理多选？
- **检查选中状态**：
  
  ```csharp
  if (Selection.Contains(instanceID)) { /* 选中时的逻辑 */ }
  ```

---

#### 七、总结

- **功能**：
  - Hierarchy 扩展增强层级窗口的显示和交互。
- **核心 API**：
  - `EditorApplication.hierarchyWindowItemOnGUI`：绘制事件。
  - `GUI` / `EditorGUI`：绘制工具。
  - `GenericMenu`：右键菜单。
- **实现**：
  - 通过全局订阅事件，适用于所有物体。
- **原理**：
  - C++ 渲染驱动，C# 插入自定义绘制

