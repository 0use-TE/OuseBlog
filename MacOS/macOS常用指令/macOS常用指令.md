### macOS 常用指令

在 Mac 终端或 SSH 会话中执行。`~` 是用户主目录，`.` 是当前目录，`..` 是上一级目录。

#### 1. 目录与文件

| 命令 | 用途 |
| --- | --- |
| `pwd` | 查看当前位置 |
| `ls -lah` | 查看文件详情，包含隐藏文件 |
| `cd ~/Developer` | 进入开发目录 |
| `cd ..` / `cd -` | 上一级 / 返回刚才的目录 |
| `mkdir -p ~/Developer` | 创建目录 |
| `touch notes.txt` | 文件不存在时创建空文件 |
| `cp a.txt b.txt` | 复制文件 |
| `cp -R src backup` | 复制文件夹 |
| `mv old.txt new.txt` | 移动或重命名 |
| `rm -i notes.txt` | 询问后删除文件，不进入废纸篓 |

路径包含空格时加引号，例如 `cd "$HOME/Developer/my project"`。

#### 2. 查看与查找

| 命令 | 用途 |
| --- | --- |
| `cat config.json` | 查看短文件 |
| `less server.log` | 分页查看，按 `q` 退出 |
| `tail -f server.log` | 持续查看新增日志，按 `Ctrl+C` 停止 |
| `find . -name "*.js"` | 查找当前目录及子目录中的 JS 文件 |
| `grep -n "TODO" main.js` | 查找文本并显示行号 |
| `nano notes.txt` | 编辑文件，`Ctrl+O` 保存，`Ctrl+X` 退出 |

#### 3. Mac 特有操作

| 命令 | 用途 |
| --- | --- |
| `open .` | 用 Finder 打开当前目录 |
| `open report.pdf` | 用默认应用打开文件 |
| `open -a "Safari"` | 启动指定应用 |
| `open https://example.com` | 用默认浏览器打开网页 |
| `pwd \| pbcopy` | 将当前路径复制到剪贴板 |
| `pbpaste` | 输出剪贴板文本 |
| `sw_vers` | 查看 macOS 版本 |
| `uname -m` | 查看当前进程架构，如 arm64、x86_64 |

SSH 中执行 `open` 不会把 Mac 窗口显示到客户端；剪贴板命令操作的是 Mac 端剪贴板，也可能受会话环境限制。

#### 4. 进程与磁盘

| 命令 | 用途 |
| --- | --- |
| `top` | 查看资源占用，按 `q` 退出 |
| `lsof -nP -iTCP:7897 -sTCP:LISTEN` | 查看端口监听，其他用户的进程可能需要 sudo |
| `kill 12345` | 请求结束指定 PID 的进程 |
| `df -h` | 查看磁盘空间 |
| `du -sh .` | 查看当前目录占用 |
| `whoami` | 查看当前用户名 |

#### 5. Homebrew

```sh
brew search git       # 搜索软件
brew install git      # 安装命令行工具
brew list             # 查看已安装软件
brew update           # 更新 brew 和软件目录
brew upgrade git      # 升级指定软件
brew uninstall git    # 卸载软件
```

桌面应用使用 `brew install --cask 软件名`。普通 brew 操作不要加 `sudo`。

#### 常用快捷键

- `Tab`：补全命令或路径。
- `↑` / `↓`：翻看历史命令。
- `Ctrl+R`：搜索历史命令。
- `Ctrl+C`：中断当前前台程序。
- `Ctrl+A` / `Ctrl+E`：移到命令行开头 / 结尾。

这里的 `Ctrl` 是 Control，不是 Command。不清楚命令用法时执行 `man 命令名`，按 `q` 退出。
