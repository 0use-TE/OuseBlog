### 1. 准备你的可执行文件

假设你已经使用 `dotnet publish` 命令将 .NET 应用发布为自包含版本，并且可执行文件位于 `/opt/AISchool/publish/AISchool`。

首先，确保该文件是可执行的：

```
chmod +x /opt/AISchool/publish/AISchool
```

### 2. 创建 systemd 服务文件

接下来，需要为你的 .NET 应用创建一个 systemd 服务文件，以便它能够作为系统服务自动启动。

在 `/etc/systemd/system/` 目录下创建一个新的服务文件，例如 `aischool.service`：

```
sudo nano /etc/systemd/system/aischool.service
```

将以下内容写入服务文件：

```
[Unit]
Description=AISchool .NET Service
After=network.target

[Service]
WorkingDirectory=/opt/AISchool/publish
ExecStart=/opt/AISchool/publish/AISchool
Restart=always
RestartSec=5
SyslogIdentifier=aischool
User=ftadmin
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
# Environment=ASPNETCORE_ENVIRONMENT=Production   # 如果是 ASP.NET Core Web 应用

[Install]
WantedBy=multi-user.target
```

#### 参数说明：

- **WorkingDirectory**：设置程序的工作目录，通常是可执行文件所在的目录。
- **ExecStart**：定义服务的启动命令，这里是指向你发布的 .NET 自包含应用的路径。
- **Restart**：设置服务崩溃后自动重启。`always` 表示无论如何都会重启。
- **RestartSec**：指定服务重启之前的等待时间，这里设置为 5 秒。
- **SyslogIdentifier**：日志中服务的标识符，可以帮助区分日志。
- **User**：指定运行服务的用户。为安全起见，不要使用 `root` 用户，最好是 `ftadmin` 这样的非特权用户。
- **Environment**：你可以设置一些环境变量。例如，`DOTNET_PRINT_TELEMETRY_MESSAGE=false` 用来禁用 .NET 的远程分析消息。
- **WantedBy**：指定服务在哪些运行级别下自动启动。`multi-user.target` 通常表示多用户模式，也就是普通的系统运行模式。

### 3. 重新加载 systemd 配置

创建完服务文件后，使用以下命令重新加载 systemd 配置，使新添加的服务文件生效：

```
sudo systemctl daemon-reload
```

### 4. 启动服务

现在，你可以启动服务了：

```
sudo systemctl start aischool.service
```

### 5. 设置服务开机自启

如果希望服务在系统启动时自动启动，可以执行以下命令：

```
sudo systemctl enable aischool.service
```

### 6. 检查服务状态

要查看服务的态，可以使用以下命令：

```
sudo systemctl status aischool.service
```

它将显示服务的当前状态，例如是否正在运行、上次运行时的日志信息等。

#### 示例输出：

```
● aischool.service - AISchool .NET Service
   Loaded: loaded (/etc/systemd/system/aischool.service; enabled; vendor preset: disabled)
   Active: active (running) since Tue 2025-11-04 14:27:42 CST; 3s ago
 Main PID: 123375 (AISchool)
    Tasks: 25
   Memory: 66.1M
   CGroup: /system.slice/aischool.service
           └─123375 /opt/AISchool/publish/AISchool
```

### 7. 查看服务日志

你可以通过 `journalctl` 命令查看服务的日志输出：

```
sudo journalctl -u aischool.service -f
```

这将实时显示服务的日志输出，就像 `tail -f` 一样，帮助你查看应用的运行状态。

### 8. 停止服务

如果你需要停止服务，可以使用：

```
sudo systemctl stop aischool.service
```

### 9. 查看所有服务状态

你可以查看所有正在运行的服务状态：

```
sudo systemctl list-units --type=service
```