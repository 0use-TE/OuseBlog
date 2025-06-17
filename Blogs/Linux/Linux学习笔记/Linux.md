##                                                                                                          Linux指令



### 1. 基本文件与目录操作

- **`pwd`**：显示当前工作目录的路径
  - 示例：`pwd`

- **`ls`**：列出当前目录中的文件和目录
  - `ls -l`：以列表形式显示文件详细信息
  - `ls -a`：显示包括隐藏文件在内的所有文件
  - 示例：`ls -l /home/user`

- **`cd`**：切换目录
  - `cd /path/to/directory`：进入指定目录
  - `cd ..`：返回到上一级目录
  - `cd ~`：返回到用户的主目录

- **`mkdir`**：创建新目录
  - 示例：`mkdir new_folder`

- **`rmdir`**：删除空目录
  - 示例：`rmdir empty_folder`

- **`rm`**：删除文件或目录
  - `rm file.txt`：删除文件
  - `rm -r directory`：递归删除目录及其内容
  - `rm -f file.txt`：强制删除文件，不提示

### 2. 文件查看与编辑

- **`cat`**：显示文件内容
  - 示例：`cat file.txt`

- **`less`**：逐页查看文件内容，使用上下箭头滚动
  - 示例：`less longfile.txt`

- **`head`**：查看文件的前几行
  - 示例：`head -n 5 file.txt`（显示前 5 行）

- **`tail`**：查看文件的最后几行
  - 示例：`tail -n 5 file.txt`（显示最后 5 行）
  - `tail -f file.txt`：动态查看文件内容（常用于日志）

- **`nano`** 和 **`vim`**：文件编辑器
  - `nano file.txt`：使用 Nano 编辑文件
  - `vim file.txt`：使用 Vim 编辑文件

### 3. 文件权限管理

- **`chmod`**：改变文件或目录权限
  - `chmod 755 file.txt`：将文件权限设为所有者可读写执行，其他用户只读和执行
  - 权限的三部分：`r`（读），`w`（写），`x`（执行）

- **`chown`**：改变文件或目录的所有者
  - 示例：`chown user:group file.txt`

### 4. 系统信息查看

- **`uname`**：显示系统信息
  - `uname -a`：显示详细的系统和内核信息

- **`df`**：查看磁盘空间使用情况
  - `df -h`：以人类可读的格式显示磁盘空间

- **`free`**：查看内存使用情况
  - `free -h`：以人类可读的格式显示内存使用

- **`top`** 和 **`htop`**：实时监视系统性能和进程
  - `top`：显示系统实时运行状态
  - `htop`：类似 `top`，但显示更直观（需要安装）

### 5. 进程管理

- **`ps`**：查看当前运行的进程
  - `ps aux`：显示所有进程

- **`kill`**：终止进程
  - 示例：`kill 1234`（终止进程 ID 为 1234 的进程）
  - `kill -9 1234`：强制终止进程

- **`pkill`**：根据进程名终止进程
  - 示例：`pkill -f process_name`

- **`jobs`**：查看当前终端的后台任务
- **`bg`** 和 **`fg`**：管理后台和前台任务
  - `bg %1`：将任务 1 放到后台运行
  - `fg %1`：将任务 1 放到前台运行

### 6. 网络管理

- **`ifconfig`**：显示或配置网络接口信息（推荐使用 `ip` 命令）
- **`ip addr`**：查看 IP 地址信息
  - 示例：`ip addr show`

- **`ping`**：测试网络连通性
  - 示例：`ping www.example.com`

- **`netstat`**：查看网络状态
  - `netstat -tuln`：列出所有监听的端口

- **`traceroute`**：显示数据包到目标的路由
  - 示例：`traceroute www.example.com`

### 7. 文件压缩与解压缩

- **`tar`**：打包和解压文件
  - `tar -cvf archive.tar file1 file2`：打包文件为 tar 格式
  - `tar -xvf archive.tar`：解压 tar 包
  - `tar -czvf archive.tar.gz file1 file2`：创建 tar.gz 压缩包
  - `tar -xzvf archive.tar.gz`：解压 tar.gz 压缩包

### 8. 用户管理

- **`useradd`** 和 **`usermod`**：添加和修改用户
  - `useradd username`：创建新用户
  - `usermod -aG groupname username`：将用户添加到组

- **`passwd`**：修改用户密码
  - 示例：`passwd username`

- **`userdel`**：删除用户
  - 示例：`userdel username`

### 9. 软件包管理（Debian 系统）

- **`apt`**：用于 Debian 系统的包管理
  - `sudo apt update`：更新软件包列表
  - `sudo apt upgrade`：升级所有安装的软件包
  - `sudo apt install package_name`：安装新软件包
  - `sudo apt remove package_name`：卸载软件包

### 10. 磁盘管理

- **`fdisk`**：磁盘分区工具
  - 示例：`sudo fdisk -l` 列出所有分区

- **`mount`** 和 **`umount`**：挂载与卸载文件系统
  - `mount /dev/sdb1 /mnt/usb`：挂载设备到指定目录
  - `umount /mnt/usb`：卸载设备

### 11. 搜索与查找命令

- **`find`**：在文件系统中搜索文件或目录
  - 示例：`find / -name "file.txt"` 查找名为 file.txt 的文件

- **`grep`**：搜索文件中的内容
  - 示例：`grep "search_term" file.txt` 在文件中查找关键字

### 12. 别名与历史命令

- **`history`**：查看命令历史记录
  - 示例：`history`

- **`alias`**：创建命令别名
  - 示例：`alias ll='ls -alF'` 创建 `ll` 别名

### 13. 文件传输

- **`scp`**：在本地和远程主机之间传输文件
  - 示例：`scp local_file user@remote_host:/path/to/remote_file`

- **`rsync`**：同步文件和目录
  - 示例：`rsync -avh /source/ /destination/`

### 14. 系统服务管理

- **`systemctl`**：管理系统服务
  - `systemctl start service_name`：启动服务
  - `systemctl stop service_name`：停止服务
  - `systemctl enable service_name`：设置服务开机启动
  - `systemctl disable service_name`：禁用服务开机启动

### 15. 权限提升

- **`sudo`**：以超级用户权限运行命令
  - 示例：`sudo command`

- **`su`**：切换用户身份
  - 示例：`su - username`

# 我的常用

## Lsof

`lsof`（List Open Files）命令是一个非常强大的工具，用于列出系统中打开的文件，包括网络连接、设备文件、普通文件等。因为在 Linux/Unix 系统中，几乎所有的东西都是文件，`lsof` 可以用来监控系统资源、排查问题、管理系统进程等。

### 基本用法

- **显示所有打开的文件：**
  ```bash
  lsof
  ```

- **显示某个用户的所有打开文件：**
  ```bash
  lsof -u username
  ```

- **查看某个特定文件被哪些进程打开：**
  ```bash
  lsof /path/to/file
  ```

### 网络相关

- **查看使用某个端口的进程：**
  ```bash
  lsof -i :80
  ```
  以上命令会显示使用端口 80（通常是 HTTP 端口）的进程。

- **列出所有的网络连接：**
  ```bash
  lsof -i
  ```

- **查看 TCP 连接：**
  ```bash
  lsof -i tcp
  ```

- **查看 UDP 连接：**
  ```bash
  lsof -i udp
  ```

### 与进程相关

- **查找特定进程 ID（PID）打开的文件：**
  ```bash
  lsof -p 1234
  ```
  这会列出进程 ID 为 1234 的所有打开文件。

- **查看某个命令（程序）打开的所有文件：**
  ```bash
  lsof -c nginx
  ```
  上述命令会显示所有由 `nginx` 进程打开的文件。

### 其他常用选项

- **查看所有打开的端口：**
  ```bash
  lsof -i -P -n
  ```
  - `-P`：显示端口号而不是服务名称。
  - `-n`：不解析 IP 地址，直接显示数字形式，加快速度。

- **查看指定文件类型：**
  ```bash
  lsof -u root -a -d txt
  ```
  上述命令会显示用户 `root` 打开的所有类型为文本（`txt`）的文件。

- **杀掉占用某个文件的进程：**
  如果某个文件被进程占用且无法删除，可以使用以下命令查看并结束该进程：
  ```bash
  lsof /path/to/file
  kill -9 <PID>
  ```

### `lsof` 的实际用途

1. **排查端口被占用的问题：**
   当某个端口被占用时，使用 `lsof -i :port_number` 可以快速找到对应的进程，并杀掉或重启服务。

2. **监控用户文件使用情况：**
   系统管理员可以使用 `lsof -u username` 监控特定用户正在使用的文件，以便于了解用户在系统上的活动。

3. **文件系统维护：**
   在卸载文件系统之前，可以使用 `lsof` 确认是否有进程在使用该文件系统，避免因为强制卸载导致的数据损坏。

4. **排查设备问题：**
   通过查看设备文件是否被进程使用，可以定位某些设备问题，例如：音频设备、网络设备等。

### 实例

- **查找与远程主机的所有连接：**
  ```bash
  lsof -i @remotehost
  ```

- **查找所有处于 LISTEN 状态的网络端口：**
  ```bash
  lsof -i -sTCP:LISTEN
  ```

- **查看某个进程使用的共享库：**
  
  ```bash
  lsof -p <PID> | grep DEL
  ```

### 常见组合

- **结合 `grep` 使用：**
  结合 `grep` 可以更高效地筛选结果，例如查找所有包含 `mysql` 的进程：
  ```bash
  lsof | grep mysql
  ```

- **使用 `watch` 实时查看变化：**
  使用 `watch` 命令每隔一定时间刷新显示，可以实时监控系统资源：
  ```bash
  watch -n 2 'lsof -i'
  ```

### 注意事项
- **权限问题：** 某些情况下需要使用 `sudo` 来执行 `lsof` 命令，以便查看所有用户的进程信息。
- **性能影响：** 在大型系统或具有大量文件打开的系统中，运行 `lsof` 可能会占用较多的系统资源。

## nohup

`nohup`（No Hang Up）是一个用于在后台运行命令的 Linux/Unix 工具，即使用户注销或关闭终端，命令也会继续运行。通常与 `&` 符号结合使用，将命令放在后台执行，并将输出重定向到文件。

### 基本用法

- **基本格式：**
  ```bash
  nohup command &
  ```
  这样可以使 `command` 在后台运行，即使用户退出登录，它也不会被终止。默认情况下，输出会被重定向到 `nohup.out` 文件。

### 详细使用示例

1. **后台运行程序：**
   ```bash
   nohup your_program &
   ```
   在这里，`your_program` 会在后台运行，并且不会因为会话断开而停止。

2. **指定输出文件：**
   如果你不想将输出写到默认的 `nohup.out` 文件，可以指定一个输出文件：
   ```bash
   nohup your_program > output.log 2>&1 &
   ```
   - `> output.log`：将标准输出重定向到 `output.log` 文件。
   - `2>&1`：将标准错误输出也重定向到标准输出，即 `output.log` 文件中。

3. **结合 shell 管道使用：**
   ```bash
   nohup command | tee output.log &
   ```
   使用 `tee` 命令可以将输出同时显示到终端和文件中。

### 常见应用场景

1. **长时间运行的任务：**
   当你运行一个需要很长时间才能完成的任务（如数据备份、大型编译或数据分析等），使用 `nohup` 可以保证即使断开连接，任务也会继续执行。

2. **避免挂断信号：**
   `nohup` 通过忽略挂断信号（SIGHUP），使程序继续在后台运行，即使终端关闭或用户注销。

3. **与服务器进程结合：**
   当运行服务器进程时，使用 `nohup` 可以确保服务器在后台稳定运行，不受用户会话影响。

### 其他常用选项和技巧

- **查看运行中的后台任务：**
  在执行带有 `&` 符号的命令后，可以使用以下命令查看所有后台任务：
  ```bash
  jobs -l
  ```

- **将后台任务移到前台：**
  使用以下命令将任务移回到前台：
  ```bash
  fg %1
  ```
  这里，`%1` 是任务编号，可以从 `jobs` 命令的输出中找到。

- **杀死后台运行的任务：**
  查找到任务的进程 ID（PID）后，可以使用以下命令终止它：
  ```bash
  kill -9 <PID>
  ```

### 注意事项

- **输出文件权限：**
  如果你发现 `nohup` 无法写入 `nohup.out` 文件，可能是因为权限问题，请确保当前目录对用户具有写权限。

- **命令行登录工具：**
  如果你使用的是远程登录工具（如 `ssh`），在执行长时间运行的命令时，使用 `nohup` 是一种很好的实践。

- **结合 `disown` 使用：**
  也可以将命令通过 `&` 放到后台执行，再用 `disown` 命令使其与终端分离，例如：
  ```bash
  command &
  disown
  ```
  这样，即使没有用 `nohup`，进程也不会受到会话关闭的影响。

### 实用例子

- **运行一个 HTTP 服务器：**
  ```bash
  nohup python3 -m http.server 8000 &
  ```
  这将在后台启动一个 HTTP 服务器，即使你退出终端，它也会继续运行。

- **数据备份任务：**
  ```bash
  nohup tar -czf backup.tar.gz /path/to/directory > backup.log 2>&1 &
  ```
  在后台进行数据备份，并将输出重定向到 `backup.log` 文件中。

`nohup` 是一个非常有用的命令，适合那些需要保持进程在后台持续运行的任务，可以避免因为会话中断而导致任务中止的风险。

## shell脚本

要生成一个 shell 脚本，通常我们需要编写一个文本文件，并将它的权限设置为可执行。这里是一个创建和使用 shell 脚本的简单过程。

### 1. **创建一个 Shell 脚本文件**

你可以在终端中使用 `touch` 命令创建一个新的文件，也可以使用文本编辑器（例如 `nano`、`vim` 或 `gedit`）来创建和编辑脚本。

#### 使用命令行创建文件：

```bash
touch myscript.sh
```

#### 使用编辑器创建文件（以 `nano` 为例）：

```bash
nano myscript.sh
```

### 2. **编写 Shell 脚本**

打开文件后，你可以编写你的脚本内容。脚本的第一行通常是 `shebang`，它指定了脚本的解释器。常用的解释器包括 `/bin/bash`、`/bin/sh` 等。

例如，创建一个简单的脚本，打印 "Hello, World!"：

```bash
#!/bin/bash
# 这是一个简单的 shell 脚本

echo "Hello, World!"
```

### 3. **保存并退出**

如果你使用 `nano` 编辑器：

- 编写脚本后，按 `CTRL + X` 来退出。
- 按 `Y` 来确认保存。
- 输入文件名（如果是新文件，直接按回车键即可）。

### 4. **给脚本赋予可执行权限**

脚本创建后，默认是不可执行的，你需要使用 `chmod` 命令来改变权限：

```bash
chmod +x myscript.sh
```

这样你就能执行这个脚本了。

### 5. **运行 Shell 脚本**

赋予可执行权限后，你可以直接运行脚本。你可以在终端中输入：

```bash
./myscript.sh
```

如果你把脚本放在系统的某个路径中，比如 `/usr/local/bin`，你可以直接通过脚本名来运行它。

### 6. **更复杂的脚本示例**

你可以创建更复杂的脚本，下面是一个示例脚本，它接受命令行参数并执行不同的操作：

```bash
#!/bin/bash

# 简单的命令行参数示例

if [ "$1" == "start" ]; then
    echo "Starting the process..."
elif [ "$1" == "stop" ]; then
    echo "Stopping the process..."
else
    echo "Usage: $0 {start|stop}"
fi
```

在这个脚本中，`$1` 是传入的第一个参数。你可以这样运行脚本：

```bash
./myscript.sh start
```

输出：

```
Starting the process...
```

### 7. **调试 Shell 脚本**

如果你遇到问题，可以加上 `-x` 来调试脚本，它会显示执行的每一行命令：

```bash
#!/bin/bash -x
```

或者在命令行执行时使用：

```bash
bash -x myscript.sh
```

### 总结

- 使用 `touch` 或编辑器创建 `.sh` 文件。
- 文件开头加上 `#!/bin/bash`。
- 编写脚本逻辑。
- 使用 `chmod +x` 为脚本赋予可执行权限。
- 使用 `./myscript.sh` 来运行脚本。