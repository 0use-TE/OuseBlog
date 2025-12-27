### Avalonia中获取父对象数据上下文
#### 介绍
考虑这样一个场景，ItemsControl显示多个表单，需要提交，提交按钮是不是需要绑定ItemsControl共享的命令，但是每个Item当前的数据上下文是ItemsSource的每项类型，显然这个命令放在每项的数据上下文里面并不是一个好的选择，并且有可能还依赖外层的数据上下文，所以这时候就需要使用以下语法
#### 语法
1.```{Binding $parent[ItemsRepeater].((vm:MainViewModel)DataContext).MyCommand}```

2.```{Binding #ItemParent.((vm:MainViewModel)DataContext).MyCommand}```