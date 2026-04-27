## 🚀 NAS 自动化访问与提权完全手册

### 第一部分：配置 SSH 密钥免密登录

1. **生成密钥：** 在本地电脑（Windows 或 Mac/Linux）运行：`ssh-keygen -t ed25519`。
2. **上传公钥：**
   * **Windows (PowerShell):**
     **PowerShell**

     ```
     cat $HOME\.ssh\id_ed25519.pub | ssh ouse@192.168.137.80 "mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys"
     ```
   * **Linux/macOS:**`ssh-copy-id ouse@192.168.137.80`

### 第二部分：禁止密码登录（使用 nano 编辑器）

1. **打开配置：**
   **Bash**

   ```
   sudo nano /etc/ssh/sshd_config
   ```
2. **查找并修改以下三项（使用 `Ctrl+W` 快速搜索）：**

   * `PubkeyAuthentication yes`：**开启公钥认证**。这是免密登录的通行证。
   * `PasswordAuthentication no`：**关闭密码认证**。防止别人通过猜测密码暴力破解你的 NAS。
   * `ChallengeResponseAuthentication no`：**关闭挑战应答**。补上最后一道缝隙，确保系统不会以任何形式向你索要密码。
3. **保存退出：** 按 `Ctrl+O` -> `Enter` 保存，按 `Ctrl+X` 退出。
4. **生效：**`sudo synoservicectl --restart sshd`

### 第三部分：配置 sudo 免密（使用 visudo）

`visudo` 会在你保存退出时自动校验语法。如果你写错了，它会提示你，防止因为一个分号或空格的错误导致你再也无法使用 `sudo`。

1. **启动命令：**
   **Bash**

   ```
   sudo visudo
   ```
2. **在 vi 界面中操作（关键动作）：**

   * **移动**：使用方向键移动到文件的**最底部**。
   * **插入**：按下键盘上的 **`i`** 键（左下角出现 `-- INSERT --`）。
   * **添加配置**：输入以下内容（把 `ouse` 换成你的用户名）：
     **Plaintext**

     ```
     ouse ALL=(ALL) NOPASSWD: ALL
     ```
   * **退出输入**：按下 **`Esc`** 键（`-- INSERT --` 消失）。
   * **保存退出**：直接输入 **`:wq`** 然后按回车。

### 📖 核心配置项含义详解

为了方便你理解这些配置的关联性，可以参考下表：


| **指令**                   | **编辑工具** | **核心作用**                         |
| -------------------------- | ------------ | ------------------------------------ |
| **PubkeyAuthentication**   | `nano`       | 允许使用“钥匙”开门                 |
| **PasswordAuthentication** | `nano`       | 拆掉“密码”锁，防止暴力破解         |
| **ChallengeResponse**      | `nano`       | 拒绝任何形式的交互式密码询问         |
| **NOPASSWD**               | `visudo`     | 赋予特定用户执行`sudo`时免检票的权利 |

### ✅ 最终成果

现在你可以直接从本地终端或 VS Code 连入 NAS：

1. **登录时**：不需要输入密码。
2. **执行 `sudo` 时**：不需要输入密码。
3. **VS Code Docker 插件**：权限报错彻底消失，秒开容器列表。
