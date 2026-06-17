# Windows 远程桌面（RDP）开启原理笔记

## 1. RDP是什么

RDP（Remote Desktop Protocol）是 Windows 的远程桌面协议。

- 客户端：`mstsc.exe`
- 服务器组件：TermService
- 默认端口：TCP 3389

```
RDP Client
    │
    │ TCP 3389
    ▼
Windows RDP Server (TermService)
```

## 2. 开启远程桌面

打开运行：`Win + R`

输入：`SystemPropertiesRemote`

勾选：允许远程连接到此计算机

## 3. 系统内部发生的变化

开启远程桌面时，Windows会执行三个步骤。

### 3.1 修改注册表

路径：`HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server`

字段：`fDenyTSConnections`

- `1` = 禁止RDP
- `0` = 允许RDP

开启远程桌面后：`fDenyTSConnections = 0`

等价命令：
```bash
reg add HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server /v fDenyTSConnections /t REG_DWORD /d 0 /f
```

### 3.2 开启防火墙规则

路径：`HKLM\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules`

添加规则：允许 TCP 3389 入站

等价命令：
```bash
netsh advfirewall firewall add rule name="Remote Desktop" dir=in action=allow protocol=tcp localport=3389
```

### 3.3 启动服务

服务名称：TermService（Remote Desktop Services）

必须保持运行状态。

等价命令：
```bash
sc start TermService
```

## 4. 远程连接

客户端运行：`mstsc.exe`

输入目标IP即可连接。
