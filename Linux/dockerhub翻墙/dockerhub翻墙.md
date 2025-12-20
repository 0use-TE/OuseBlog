# Docker 在 Linux 下翻墙拉取镜像指南



### 1. 问题描述



在 Linux（Ubuntu 等）上运行：

```
sudo docker run hello-world
```

报错：

```
Unable to find image 'hello-world:latest' locally
docker: Error response from daemon: Get "https://registry-1.docker.io/v2/": net/http: request canceled while waiting for connection (Client.Timeout exceeded while awaiting headers)
```

**原因**：Docker daemon 自身无法访问网络，即使你在终端翻墙了，Docker 并不会继承用户环境变量。

------

### 2. 原理



- **Docker 客户端**发起命令，但**镜像拉取是由 Docker daemon 处理**。
- Daemon 作为系统服务运行（systemd 管理），不会继承你的 shell 环境。
- 因此，需要专门为 daemon 设置 HTTP/HTTPS 代理。

------

### 3. 永久配置 Docker 代理（推荐）



1. 创建 systemd 覆盖目录：

```bash
sudo mkdir -p /etc/systemd/system/docker.service.d
```

1. 创建或编辑代理配置文件：

```bash
sudo nano /etc/systemd/system/docker.service.d/http-proxy.conf
```

内容示例（假设本地代理端口 7897）：

```bash
[Service]
Environment="HTTP_PROXY=http://localhost:7897"
Environment="HTTPS_PROXY=http://localhost:7897"
Environment="NO_PROXY=localhost,127.0.0.1"
```

1. 重载 systemd 并重启 Docker：

```bash
sudo systemctl daemon-reexec
sudo systemctl restart docker
```

1. 验证代理是否生效：

```bash
sudo systemctl show --property=Environment docker
```

------

### 4. 临时方法（一次性生效）



```bash
sudo HTTP_PROXY=http://localhost:7897 HTTPS_PROXY=http://localhost:7897 docker run hello-world
```

适合快速测试，不改系统配置。

------

### 5. 补充说明



- `NO_PROXY` 用于排除本地访问，避免代理干扰 localhost。
- Docker 代理配置可以与系统代理不同，必须针对 daemon 设置。
- 原理总结：**用户 shell 代理 ≠ Docker daemon 代理**。