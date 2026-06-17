# Avalonia 停靠布局持久化 — 通用实现提示词

> 复制下方「Agent 提示词」整段到新项目对话中，按需替换 `{{占位符}}` 即可。  
> 参考实现：`XTower`、`GOZAReframe`。

---

## Agent 提示词（直接复制）

```
请为 Avalonia + Crystal.Avalonia + GOZA.Dock 项目实现「停靠布局持久化」通用方案。严格遵循以下约定，不要写 View CodeBehind 业务逻辑。

## 技术栈

- Avalonia 12 + Crystal.Avalonia 2.x（ViewModelLocator / ILifecycleAware）
- GOZA.Dock（DockShell / DockRegion / DockSplitter）
- CommunityToolkit.Mvvm
- System.Reactive（ObserveProperty + Throttle，避免 GridSplitter 拖动频繁保存）
- System.Text.Json 源码生成器（AOT 友好）
- Serilog + Serilog.Sinks.File + Serilog.Formatting.Compact（JSONL 日志）
- DI：Microsoft.Extensions.DependencyInjection

## 存储路径（跨平台）

- 根目录：`Environment.SpecialFolder.LocalApplicationData / {{AppName}} / LocalData`
- 布局文件：`layout.jsonl`（单行 JSON，符合 JSONL）
- 日志文件：`log-.jsonl`（CompactJsonFormatter，RollingInterval.Day）

## 持久化接口（注册到 DI）

```csharp
public interface IDataPersistence
{
    void Save<T>(string fileName, T data, JsonTypeInfo<T> typeInfo) where T : class;
    T? Load<T>(string fileName, JsonTypeInfo<T> typeInfo) where T : class;
    string GetAppStoragePath(string fileName);
    string GetAppStorageBasePath();
}
```

- 实现类 `DataPersistence`：`.jsonl` 读写首行非空 JSON；异常记日志不崩溃
- **不要**在 View CodeBehind 里 Save/Load

## 布局数据模型（MainViewStorageModel）

放在 MainViewModel 同文件或 Persistence 文件夹，配 partial JsonSerializerContext：

```csharp
[JsonSerializable(typeof(MainViewStorageModel))]
[JsonSerializable(typeof(Dictionary<string, int>))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class MainViewJsonContext : JsonSerializerContext;

internal sealed class MainViewStorageModel
{
    // 网格尺寸
    public double Left { get; set; } = 250;           // 左栏像素宽
    public double Right { get; set; } = 280;            // 右栏像素宽
    public double CenterTopBottomProportion { get; set; } = 5.0 / 3.0; // 中间上下 Star 比例

    // 各 DockRegion 内 Tab 顺序（按 Id）
    public List<string> LeftItems { get; set; } = [];
    public List<string> RightItems { get; set; } = [];
    public List<string> CenterTopItems { get; set; } = [];
    public List<string> CenterBottomItems { get; set; } = [];

    // 各区域当前选中 Tab
    public string? LeftSelectedTabId { get; set; }
    public string? RightSelectedTabId { get; set; }
    public string? CenterTopSelectedTabId { get; set; }
    public string? CenterBottomSelectedTabId { get; set; }

    // 主题 + Tab 最后所在区域（解决关闭后菜单重开回到默认位）
    public bool IsDarkMode { get; set; }
    public Dictionary<string, int> TabPreferredRegions { get; set; } = new(StringComparer.Ordinal);
}
```

## Dock Tab 约定

每个面板 ViewModel 继承 `DockViewModel : IDockTabItem`：

- `Id`：稳定唯一字符串（持久化键）
- `Header`：Tab 标题
- `DockPosition`：**仅作首次默认区域**，不是运行时唯一位置
- `IsClosable => true`（可关闭 Tab）

DI 注册：

```csharp
services.AddDockTabItem<ProjectView, ProjectViewModel>(); // 内部 AddMvvmSingleton + IEnumerable<DockViewModel>
services.AddSingleton<IDataPersistence, DataPersistence>();
services.AddMvvmSingleton<MainView, MainViewModel>();
```

## MainViewModel 职责（核心）

实现 `ILifecycleAware`，**所有**布局逻辑在 VM：

### 1. 网格属性（XAML TwoWay 绑定）

```xml
<ColumnDefinition Width="{Binding LeftGridLength, Mode=TwoWay}" />
<RowDefinition Height="{Binding CenterTopGridLength, Mode=TwoWay}" />
```

VM 暴露 `GridLength` 属性：LeftGridLength、RightGridLength、CenterColumnWidth、LeftSplitterColumnWidth、RightSplitterColumnWidth、CenterTopGridLength、CenterBottomGridLength、CenterHSplitterRowHeight。

**禁止**绑定 string 到 ColumnDefinitions（Avalonia 编译绑定不支持）。

### 2. 生命周期

| 阶段 | 做什么 |
|------|--------|
| **构造函数** | `_suspendAutoSave = true` → Load → **恢复 Tab 顺序** → LoadPreferredRegions → 订阅各 Region CollectionChanged → `_suspendAutoSave = false` |
| **OnLoadedAsync** | 恢复网格尺寸 + 主题；`ObserveProperty(...).Throttle(500ms).Subscribe(Save)`（只订阅一次） |
| **OnUnloaded** | `Save()` |

> Tab 顺序必须在**构造函数**恢复（GOZAReframe 经验：避免 OnLoaded 清空已激活视图触发 ILifecycleAware.OnUnloaded）。

### 3. Tab 区域记忆（关键）

维护运行时字典：

```csharp
private readonly Dictionary<string, DockPosition> _preferredRegions = new(StringComparer.Ordinal);
```

- Tab 被 **Add 到某 Region**（含跨区拖动）：`CollectionChanged` → 更新 `_preferredRegions[id] = region`
- Tab **关闭**：不删 `_preferredRegions` 记录
- Tab **菜单重新打开**：`AddToRegion(item, GetPreferredRegion(item))`，**不用** `item.DockPosition`
- `Save()` 时把 `_preferredRegions` 写入 `TabPreferredRegions` 持久化

```csharp
private DockPosition GetPreferredRegion(DockViewModel item) =>
    _preferredRegions.TryGetValue(item.Id, out var region) ? region : item.DockPosition;
```

### 4. 菜单 Toggle 面板

```csharp
private void Toggle(DockViewModel item)
{
    if (!Items.Contains(item)) { Focus(item); return; }
    if (IsSelected(item)) Close(item);
    else Focus(item);
}

private void Focus(DockViewModel item)
{
    if (!Items.Contains(item))
        AddToRegion(item, GetPreferredRegion(item));
    SelectItem(item);
}
```

### 5. 自动保存

- `_suspendAutoSave`：恢复布局期间禁止 Save
- Tab 增删 / 选中变化：直接 `Save()`
- GridLength 变化：Rx `ObserveProperty` + `Throttle(500ms)` → `Save()`

### 6. ReactiveExtensions（可复制）

```csharp
public static IObservable<EventPattern<PropertyChangedEventArgs>> ObserveProperty<TObj>(
    this TObj source, params string[] propertyNames)
    where TObj : INotifyPropertyChanged =>
    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            h => source.PropertyChanged += h, h => source.PropertyChanged -= h)
        .Where(x => propertyNames.Contains(x.EventArgs.PropertyName));
```

### 7. 主题切换

```csharp
[RelayCommand]
private void SwitchTheme()
{
    IsDarkMode = Application.Current?.ActualThemeVariant != ThemeVariant.Dark;
    Application.Current!.RequestedThemeVariant = IsDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
    Save();
}
```

Menu 绑定 `SwitchThemeCommand`；`IsDarkMode` 写入布局文件，OnLoadedAsync 恢复。

## MainView.axaml 结构（无 CodeBehind 逻辑）

```xml
<UserControl ViewModelLocator.AutoWireViewModel="True" x:DataType="vm:MainViewModel">
  <DockPanel>
    <Menu><!-- 视图菜单 ToggleXxxCommand + SwitchThemeCommand --></Menu>
    <DockShell>
      <Grid>
        <Grid.ColumnDefinitions><!-- GridLength TwoWay --></Grid.ColumnDefinitions>
        <DockRegion ItemsSource="{Binding LeftItems}" SelectedItem="{Binding LeftSelectedItem}" />
        <DockSplitter />
        <Grid><!-- 中间上下 DockRegion + RowDefinitions TwoWay --></Grid>
        <DockSplitter />
        <DockRegion ItemsSource="{Binding RightItems}" SelectedItem="{Binding RightSelectedItem}" />
      </Grid>
    </DockShell>
  </DockPanel>
</UserControl>
```

View / Window 的 `.axaml.cs` 仅 `InitializeComponent()`。

## App.axaml.cs 注册

```csharp
services.AddSingleton<IDataPersistence, DataPersistence>();
services.AddLogging(o => o.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: Day)
    .CreateLogger()));
```

## 禁止事项

- ❌ View CodeBehind 里 Attach Grid / Save / Load
- ❌ LayoutService 依赖 View 控件引用（Grid 应用层绑定 VM 即可）
- ❌ 重开 Tab 时用 `DockPosition` 代替 `PreferredRegion`
- ❌ 用反射 JSON 序列化（AOT 项目）
- ❌ Grid 尺寸用 LayoutUpdated 无限触发 Save（用 Rx Throttle）

## 占位符（实现前替换）

| 占位符 | 示例 |
|--------|------|
| `{{AppName}}` | XTower |
| `{{LayoutStorageName}}` | layout.jsonl |
| `{{DockRegions}}` | Left / CenterTop / CenterBottom / Right |
| `{{DockItems}}` | ProjectViewModel, MonsterViewModel, ... |

按以上方案实现，参考 GOZAReframe MainViewModel 与 XTower 当前代码结构。
```

---

## 文件清单（新项目快速对照）

| 文件 | 职责 |
|------|------|
| `Persistence/IDataPersistence.cs` | 持久化接口 |
| `Persistence/DataPersistence.cs` | LocalApplicationData 读写 |
| `Extensions/ReactiveExtensions.cs` | ObserveProperty |
| `ViewModels/DockViewModel.cs` | IDockTabItem + AddDockTabItem 扩展 |
| `ViewModels/MainViewModel.cs` | ILifecycleAware + 布局全部逻辑 |
| `Views/MainView.axaml` | GridLength 绑定 + DockRegion |
| `Views/MainView.axaml.cs` | 仅 InitializeComponent |
| `App.axaml.cs` | DI 注册 + Serilog |

## NuGet 包

```xml
<PackageReference Include="Crystal.Avalonia" />
<PackageReference Include="GOZA.Dock" />
<PackageReference Include="CommunityToolkit.Mvvm" />
<PackageReference Include="System.Reactive" />
<PackageReference Include="Serilog.Extensions.Logging" />
<PackageReference Include="Serilog.Sinks.File" />
<PackageReference Include="Serilog.Formatting.Compact" />
```

---

## 设计要点（为什么这样设计）

1. **Tab 顺序在构造函数恢复**：Singleton VM + 已激活子 View 时，OnLoaded 再 Clear 会误触发子 VM 的 OnUnloaded。
2. **PreferredRegion 与 DockPosition 分离**：DockPosition 是注册默认位；拖动/关闭后重开靠 `_preferredRegions`。
3. **GridLength TwoWay**：Splitter 拖动自动回写 VM，无需 CodeBehind 读 Grid。
4. **Rx Throttle**：PropertyChanged 在拖动时会连发，500ms 节流足够且 OnUnloaded 仍立即 Save。
5. **jsonl + JsonSerializerContext**：AOT/Trim 安全，单行布局文件便于人工查看与覆盖写入。
