### Docker配置代理

#### 前言

docker pull等指令由守护进程执行，因此不会走代理，有三种方案。

1. 配置国内镜像源
2. 配置守护进程代理
3. 设置tun模式

这里仅介绍第二种。

#### 步骤

先确保linux可以访问您的代理，我的方案是，windows开代理，打开局域网转发，然后ssh和linux的7897端口建立一个隧道。
然后便可以执行以下操作。

1. 创建配置目录

   ```bash
   sudo mkdir -p /etc/systemd/system/docker.service.d
   ```
2. 创建并编辑代理文件

   ```bahs
   sudo nano /etc/systemd/system/docker.service.d/http-proxy.conf
   ```
3. 粘贴以下内容

   ```ini
   [Service]
   Environment="HTTP_PROXY=http://127.0.0.1:7897"
   Environment="HTTPS_PROXY=http://127.0.0.1:7897"
   Environment="NO_PROXY=localhost,127.0.0.1,docker-registry.somecorporation.com"
   ```
4. 刷新配置并重启docker

   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart docker
   ```
5. 验证

   执行以下命令查看 Docker 的信息：

   ```bash
   docker info | grep Proxy
   ```

   如果你看到类似下面的输出，说明配置成功了：

   ```bash
   HTTP Proxy: http://127.0.0.1:7897
   HTTPS Proxy: http://127.0.0.1:7897
   ```

#### 结束

然后便可以使用Docker√(希望光明网早点毁灭Boom!)
