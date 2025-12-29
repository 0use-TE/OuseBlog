### Linux环境变量相关指令

#### 前言

首先我们要知道默认$PATH有哪些路径
一般有两个
/bin/local/bin 用于放用户安装的指令
/usr/bin 用于放系统指令
所以如果我们将指令或者是指令链接放在这两个路径下，就可以直接执行，不需要再export
Tip:默认指令所有用户均可见哦！

查看环境变量指令

```bash
echo $PATH
```

#### 添加环境变量

通过前言，我们可以知道添加环境变量有两个方法

1. 创建一个软链接到/usr/local/bin下面

   ```bash
   sudo ln -s 您的指令路径 目标路径
   ```

   示例:添加dotnet到/usr/local/bin下面

   ```bash
   sudo ln -s /home/ouse/.dotnet/dotnet /usr/local/bin/dotnet
   ```
2. 使用export
   仅限当前终端:

   ```bash
   export PATH="$PATH:$HOME/.dotnet"
   ```

   终端执行后，如果下次打开就找不到dotnet了，因此我们可以追加到$HOME/.bashrc最下面
   然后执行source $HOME/.bashrc确保当前终端起作用
   注：.bashrc是每次打开终端都会执行，.profile是只有第一次建立连接的终端才会执行!

#### 相关指令



| **需求**         | **指令**                                |
| ---------------- | --------------------------------------- |
| **查看所有变量** | `env`或`export`                         |
| **查看特定变量** | `echo $PATH`                            |
| **临时修改**     | `export MY_VAR="hello"`                 |
| **永久修改**     | 需要写入`~/.bashrc`或`/etc/environment` |
