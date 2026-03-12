### Avalonia集成DialogHost

#### 前言

DialogHostAvalonia 是一个用于 Avalonia UI 的对话框托管控件，类似于 WPF 的 DialogHost。它可以让我们方便地在任意位置弹出对话框，并且支持主题切换。

#### 1. 安装 NuGet 包

在项目目录下执行：

```bash
dotnet add package DialogHost.Avalonia
```

或者通过 NuGet 包管理器搜索 `DialogHost.Avalonia` 安装。

#### 2. App.axaml 配置

在应用入口文件中引入 DialogHost 的命名空间和样式：

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:dialoghostavalonia="using:DialogHostAvalonia"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="WasmProject.App"
             RequestedThemeVariant="Light">

    <Application.Styles>
        <dialoghostavalonia:DialogHostStyles></dialoghostavalonia:DialogHostStyles>
        <!-- 其他样式... -->
    </Application.Styles>
</Application>
```

#### 3. 在 View 中使用 DialogHost

在需要弹出对话框的 View 中，使用 `DialogHost` 控件包裹内容：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:dialoghostavalonia="using:DialogHostAvalonia"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="WasmProject.Views.MainView">

    <dialoghostavalonia:DialogHost Identifier="MainContent"
                                   CloseOnClickAway="True">

        <!-- 这里的 Content 就是要显示的主要内容 -->
        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
            <Button Content="弹出" Command="{Binding PopupCommand}" />
            <Button Command="{Binding SwitchThemeCommand}">切换主题</Button>
        </StackPanel>

    </dialoghostavalonia:DialogHost>
</UserControl>
```

关键属性说明：
- `Identifier`: 唯一标识符，用于 `DialogHost.Show()` 方法调用
- `CloseOnClickAway`: 点击遮罩区域是否关闭对话框，默认 `True`

#### 4. 在 ViewModel 中调用对话框

```csharp
using Prism.Commands;
using DialogHostAvalonia;

public class MainViewModel
{
    public AsyncDelegateCommand PopupCommand { get; set; }

    public MainViewModel()
    {
        PopupCommand = new AsyncDelegateCommand(async () =>
        {
            // 创建对话框内容
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = "弹出对话框" });
            stackPanel.Children.Add(new Button
            {
                Content = "关闭",
                Command = new DelegateCommand(() => DialogHost.Close("MainContent"))
            });

            // 显示对话框并等待结果
            var result = await DialogHost.Show(stackPanel, "MainContent");
        });
    }
}
```

#### 5. 主题适配

DialogHost 支持主题切换，可以根据当前主题动态设置背景色：

```xml
<UserControl.Resources>
    <ResourceDictionary>
        <ResourceDictionary.ThemeDictionaries>
            <ResourceDictionary x:Key='Light'>
                <SolidColorBrush x:Key='BackgroundBrush'>White</SolidColorBrush>
            </ResourceDictionary>
            <ResourceDictionary x:Key='Dark'>
                <SolidColorBrush x:Key='BackgroundBrush'>Black</SolidColorBrush>
            </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
    </ResourceDictionary>
</UserControl.Resources>

<dialoghostavalonia:DialogHost>
    <dialoghostavalonia:DialogHost.Styles>
        <Style Selector="dialoghostavalonia|DialogHost">
            <Setter Property="Background" Value="{DynamicResource BackgroundBrush}"></Setter>
        </Style>
    </dialoghostavalonia:DialogHost.Styles>
    <!-- 内容... -->
</dialoghostavalonia:DialogHost>
```

#### 6. 更多用法

- **带参数传递**: `DialogHost.Show(content, "Identifier", (sender, args) => { /* 关闭后的回调 */ })`
- **关闭对话框**: `DialogHost.Close("Identifier")` 或 `DialogHost.Close("Identifier", result)`
- **获取返回值**: `Show` 方法返回 `Task<object>`，可以通过 `result` 获取用户传递的数据

#### 总结

DialogHostAvalonia 提供了简洁的 API 来实现对话框功能，支持：
- 任意位置弹出
- 主题适配
- 异步等待结果
- 点击遮罩关闭

非常适合用于确认对话框、表单弹窗等场景。
