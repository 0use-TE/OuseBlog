### macOS SSH 公钥登录与禁用密码认证

示例：从其他电脑连接 Mac `192.168.1.100`，用户名 `bean`。请替换为实际地址和用户名。

#### 1. 开启远程登录

Mac 上打开 **系统设置 → 通用 → 共享 → 远程登录**，允许 `bean` 访问。

先从客户端连接，首次连接核对主机指纹，登录后保留会话：

```sh
ssh bean@192.168.1.100
```

#### 2. 客户端生成密钥

```sh
ssh-keygen -t ed25519
```

按提示选择保存位置、设置私钥口令。以下按默认路径演示；已有密钥可以复用，不要覆盖。

- `id_ed25519`：私钥，留在客户端。
- `id_ed25519.pub`：公钥，上传到 Mac。

#### 3. 客户端上传公钥

**Windows PowerShell：**

```powershell
Get-Content "$HOME\.ssh\id_ed25519.pub" | ssh -T bean@192.168.1.100 "umask 077; mkdir -p ~/.ssh && chmod 700 ~/.ssh && printf '\n' >> ~/.ssh/authorized_keys && cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys"
```

**macOS / Linux：**

```sh
ssh -T bean@192.168.1.100 'umask 077; mkdir -p ~/.ssh && chmod 700 ~/.ssh && printf "\n" >> ~/.ssh/authorized_keys && cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys' < ~/.ssh/id_ed25519.pub
```

这一步输入 bean 的账号密码。命令会追加公钥，保留原有授权。

#### 4. 先测试公钥登录

客户端另开终端执行，原会话不要关闭：

```sh
ssh -o ControlPath=none -o PreferredAuthentications=publickey -i ~/.ssh/id_ed25519 bean@192.168.1.100
```

PowerShell 中将 `~/.ssh/id_ed25519` 替换为 `"$HOME\.ssh\id_ed25519"`。

**成功后才能继续。** 提示 `Enter passphrase for key` 要的是私钥口令，不是 Mac 账号密码；可以用 SSH agent 缓存。

#### 5. Mac 上禁用密码认证

在 Mac 的管理员会话中备份并编辑：

```sh
sudo cp -p /etc/ssh/sshd_config "/etc/ssh/sshd_config.backup.$(date +%Y%m%d-%H%M%S)"
sudo nano /etc/ssh/sshd_config
```

将以下配置放在文件最前面的全局区域，位于 `Include`、`Match` 和其他同名配置之前：

```text
PubkeyAuthentication yes
PasswordAuthentication no
KbdInteractiveAuthentication no
AuthenticationMethods publickey
```

按 `Ctrl+O`、回车保存，`Ctrl+X` 退出。OpenSSH 多数设置取先读到的值，不能靠末尾追加覆盖。

检查语法与实际配置：

```sh
sudo /usr/sbin/sshd -t
sudo /usr/sbin/sshd -T | grep -E '^(pubkeyauthentication|passwordauthentication|kbdinteractiveauthentication|authenticationmethods) '
```

预期值依次为 `yes`、`no`、`no`、`publickey`，输出顺序可能不同。若有 `Match` 块，需另外核对对应用户和来源的设置。

macOS 自带“远程登录”通常会对新连接读取配置，无需执行 `systemctl`。保留原会话，再重复第 4 步，确认仍能用公钥登录。

#### 6. 确认密码登录被拒绝

客户端执行：

```sh
ssh -o ControlPath=none -o PubkeyAuthentication=no -o PreferredAuthentications=password,keyboard-interactive bean@192.168.1.100
```

应无法登录，通常提示 `Permission denied (publickey)`。只有公钥测试成功、密码测试被拒绝后，才关闭原会话。

配置出错时，在原管理员会话中用第 5 步的备份恢复主配置，执行 `sudo /usr/sbin/sshd -t` 后重新测试。

SSH 公钥登录不会取消 sudo 的密码要求。

参考：[Apple 远程登录说明](https://support.apple.com/guide/mac-help/allow-a-remote-computer-to-access-your-mac-mchlp1066/mac)、[OpenSSH 配置手册](https://man.openbsd.org/sshd_config)。
