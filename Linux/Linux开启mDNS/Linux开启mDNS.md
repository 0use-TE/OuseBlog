# Linux 开启 mDNS 教程

## 什么是 mDNS？

mDNS（Multicast DNS）是一种局域网内的零配置服务发现协议，允许设备通过 `.local` 域名相互访问，而无需配置 DNS 服务器。

例如，开发板的 hostname 是 `nexus`，你可以通过 `nexus.local` 直接访问它。

## 步骤

### 1. 检查 avahi-daemon 状态

```bash
systemctl status avahi-daemon
```

如果没有安装或未启动，状态会显示 `inactive`。

### 2. 安装 avahi-daemon

```bash
sudo apt update
sudo apt install avahi-daemon
```

### 3. 启动并设置开机自启

```bash
sudo systemctl enable avahi-daemon
sudo systemctl start avahi-daemon
```

### 4. 验证状态

```bash
systemctl status avahi-daemon
```

确认显示 `active (running)` 即可。

## 使用方法

设备启动后，可以通过以下方式访问：

```
nexus.local
```

只要在同一局域网内，其他设备就可以通过 `.local` 域名解析到对应的 IP 地址。

## 常见问题

- **无法解析**：确保设备在同一局域网，且防火墙未阻止 mDNS（UDP 端口 5353）
- **hostname 查看**：`hostname` 命令可查看当前设备的主机名
