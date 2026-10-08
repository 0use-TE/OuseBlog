# 3x-ui：自建 VPN 与控制面板

本文记录在一台 Linux 服务器上安装 Docker、使用 Docker Compose 部署 3x-ui，并通过 SSH 隧道安全访问 Web 管理面板。随后配置订阅服务，创建监听 `443` 端口的 VLESS 入站并添加客户端。

## 1. 安装 Docker

以 Debian / Ubuntu 为例，可以直接安装 Docker：

```bash
curl -fsSL https://get.docker.com | sh
```

安装完成后检查服务状态：

```bash
docker --version
docker compose version
systemctl status docker
```

设置 Docker 开机启动：

```bash
sudo systemctl enable --now docker
```

> 使用 Docker 部署 3x-ui 可以将面板、Xray 运行环境及相关依赖封装在容器中，避免直接修改宿主机环境，也方便后续升级、迁移和备份。

## 2. 创建 3x-ui 目录

创建独立目录保存 3x-ui 配置和数据库：

```bash
sudo mkdir -p /opt/3x-ui
cd /opt/3x-ui
```

创建 `compose.yml`：

```yaml
services:
  3x-ui:
    image: ghcr.io/mhsanaei/3x-ui:v3.7.0
    container_name: 3x-ui

    ports:
      - "0.0.0.0:443:443/tcp"
      - "0.0.0.0:2096:2096/tcp"
      - "127.0.0.1:2053:2053/tcp"

    volumes:
      - ./db:/etc/x-ui

    environment:
      XUI_PORT: "2053"
      XUI_ENABLE_FAIL2BAN: "false"
      XUI_LOG_LEVEL: "info"

    security_opt:
      - no-new-privileges:true

    restart: unless-stopped
```

启动：

```bash
docker compose up -d
```

查看运行状态：

```bash
docker compose ps
```

查看日志：

```bash
docker compose logs -f
```

> `443` 需要对公网开放，因为客户端需要通过该端口连接 VLESS 节点。

> `2096` 用于提供客户端订阅地址，因此使用订阅功能时需要允许客户端从公网访问该端口。

> `2053` 是 3x-ui 的管理面板端口，但这里仅绑定到 `127.0.0.1`，即服务器自身。这样公网无法直接访问管理后台，可以减少面板被扫描、爆破或利用漏洞攻击的风险。

## 3. 通过 SSH 访问 Web 管理面板

由于管理面板 `2053` 只监听服务器的 `127.0.0.1`，无法直接从公网打开，因此可以通过 SSH Local Forwarding 将它映射到自己的电脑。

在本地执行：

```bash
ssh -L 2053:127.0.0.1:2053 username@服务器IP
```

随后在本机浏览器打开：

```plaintext
http://127.0.0.1:2053
```

即可访问服务器上的 3x-ui 面板。

> `ssh -L` 会通过已经加密的 SSH 连接建立本地端口转发。本机访问 `127.0.0.1:2053` 时，请求实际上经过 SSH 隧道传输到服务器的 `127.0.0.1:2053`。

> 这种方式不需要把管理后台直接暴露到互联网，同时仍然可以像访问本地服务一样使用 Web 面板。

如果不需要在服务器上打开交互 Shell，也可以使用：

```bash
ssh -N -L 2053:127.0.0.1:2053 username@服务器IP
```

> `-N` 表示只建立 SSH 隧道而不执行远程命令，更适合单纯进行端口转发。

## 4. 修改订阅设置

登录 3x-ui 后进入面板的订阅相关设置，将订阅服务端口设置为：

```plaintext
2096
```

同时将订阅服务的监听域名设置为实际用于访问服务器的域名，例如：

```plaintext
example.com
```

然后保存并重启相关服务。

> 订阅地址最终需要提供给客户端使用，因此其中的域名必须能够被客户端解析并访问。如果生成的订阅地址仍然指向 `localhost`、`127.0.0.1` 或错误的内部地址，远程客户端将无法正常获取订阅。

> 使用域名而不是直接写服务器 IP，还可以将客户端配置与具体服务器 IP 解耦。以后服务器 IP 发生变化时，只需要修改 DNS 记录，而不必重新修改所有客户端。

## 5. 创建 VLESS 入站

在 3x-ui 中选择：

```plaintext
入站列表
→ 添加入站
```

协议选择：

```plaintext
VLESS
```

端口设置为：

```plaintext
443
```

然后根据实际需要配置传输方式、TLS、Reality 等参数。

保存后，可以在服务器检查端口：

```bash
ss -lntp | grep :443
```

也可以从其他设备测试：

```bash
nc -vz example.com 443
```

> VLESS 是 Xray 中常用的轻量代理协议。与早期 VMess 相比，VLESS 本身尽量减少协议层额外功能，把加密和身份认证等能力更多交给 TLS、Reality 等外层机制处理，因此结构相对简单，也比较适合作为现代 Xray 节点协议。

> 很多网络、防火墙、企业出口和云环境默认允许 HTTPS，也就是 TCP `443`。因此使用 `443` 通常具有较好的网络兼容性。从网络流量形态上看，如果 VLESS 配合 TLS 或 Reality 使用，选择 `443` 也更加符合正常 HTTPS 服务常见的端口使用习惯。

## 6. 添加客户端

创建 VLESS 入站以后，在该入站下面添加一个客户端。

通常至少需要生成：

```plaintext
UUID
```

例如：

```plaintext
550e8400-e29b-41d4-a716-446655440000
```

可以为客户端填写一个容易识别的备注，例如：

```plaintext
MacBook
iPhone
iPad
```

保存后，3x-ui 可以根据入站配置和客户端 UUID 生成对应的 VLESS 分享链接或者二维码。

> UUID 相当于 VLESS 客户端的身份标识。服务端会根据 UUID 判断连接是否属于允许访问的客户端，因此不同设备可以使用不同 UUID，便于单独管理、统计和撤销权限。

例如可以分别建立：

```plaintext
MacBook
iPhone
iPad
```

而不是所有设备共用同一个客户端。

> 为每台设备分配独立客户端后，如果某台设备的配置泄露，只需要删除对应客户端，而不需要更换整个节点的所有客户端配置。

## 7. 最终结构

最终的网络结构可以简单理解为：

```text
                         Internet
                            │
              ┌─────────────┴─────────────┐
              │                           │
          TCP 443                     TCP 2096
              │                           │
              ▼                           ▼
        VLESS / Xray                Subscription
              │
              │
        Docker: 3x-ui
              │
              │ 127.0.0.1:2053
              ▼
        3x-ui Web Panel
              ▲
              │
           SSH Tunnel
              │
              ▼
      Local 127.0.0.1:2053
```

其中真正需要暴露到公网的主要是：

```plaintext
443   VLESS 客户端连接
2096  客户端获取订阅
```

管理面板：

```plaintext
2053
```

只监听：

```plaintext
127.0.0.1
```

并通过：

```bash
ssh -N -L 2053:127.0.0.1:2053 username@服务器IP
```

进行访问。

> 这种结构把“数据入口”“订阅入口”和“管理入口”分开：VLESS 和订阅服务根据需求对公网开放，而权限最高的管理后台只允许通过 SSH 隧道访问，在保持使用方便的同时减少不必要的公网攻击面。