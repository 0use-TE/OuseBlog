### macOS 创建账号与管理员配置

以下命令在 Mac 终端或 SSH 会话中执行，需要已有管理员权限。示例用户名为 `bean`。

#### 1. 创建用户

```sh
sudo sysadminctl -addUser bean -fullName "Bean" -shell /bin/zsh -password -
```

按提示输入密码：`sudo` 要的是当前管理员密码，`User password:` 要的是新用户密码。输入时不显示字符是正常的。

需要直接创建管理员，在命令末尾加 `-admin`；用户已存在则不要重复创建。

#### 2. 将已有用户设为管理员

```sh
sudo dseditgroup -o edit -a bean -t user admin
```

检查结果，输出包含 `yes` 即成功，之后让该用户重新登录：

```sh
dseditgroup -o checkmember -m bean admin
```

#### 3. 切换用户

```sh
su - bean
whoami
```

输入 bean 的密码，`whoami` 应输出 `bean`。执行 `exit` 返回原会话。

#### 4. 密码不对时重设

在原管理员会话执行，按提示设置并确认 bean 的新密码：

```sh
sudo passwd bean
```

创建时的 FDE 警告涉及 FileVault 磁盘解锁权限，不等于账号创建失败，也不能直接解释密码错误。

#### 5. 配置 sudo 免密

使用 visudo 编辑，保存时会检查语法：

```sh
sudo EDITOR=nano visudo
```

在文件末尾添加，`bean` 替换为需要免密的用户名：

```sudoers
bean ALL=(ALL) NOPASSWD: ALL
```

按 `Ctrl+O`、回车保存，`Ctrl+X` 退出；如果提示语法错误，继续编辑修正。

切换到 bean 账号后，清除缓存并测试：

```sh
sudo -k
sudo -n whoami
```

输出 `root` 即成功。恢复密码验证时，用 visudo 删除刚添加的那一行。

**这会允许该用户无需密码执行任意管理员命令，与 SSH 公钥免密登录是两回事。**

需要 SSH 登录时，在 **系统设置 → 通用 → 共享 → 远程登录** 中允许该用户访问。

下一篇：[SSH 公钥登录与禁用密码认证](../macOSSSH公钥登录与禁用密码认证/macOSSSH公钥登录与禁用密码认证.md)。
