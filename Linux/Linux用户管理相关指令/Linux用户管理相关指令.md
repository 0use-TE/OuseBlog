### Linux用户管理相关指令

#### 1. 创建用户的指令

Linux 中有两个指令可以创建用户，建议使用 **`adduser`**，因为它更智能且包含交互式设置。

* **推荐方式（交互式）：**
  **Bash**

  ```
  sudo adduser username
  ```

  *这会自动创建家目录（/home/username）、设置密码、让你填写用户信息。*
* **基础方式（仅创建账号）：**
  **Bash**

  ```
  sudo useradd username
  ```

  *注意：`useradd` 默认不创建家目录，也不设密码，通常需要配合 `-m` 参数。*

#### 2. 分配到 sudo 组（赋予管理员权限）

在 Ubuntu/Debian 系中，管理员组通常叫 `sudo`；在 CentOS/RHEL 系中，通常叫 `wheel`。

* **指令：**
  **Bash**

  ```
  sudo usermod -aG sudo username
  ```

  *参数解释：`-a` 是 append（追加），`-G` 是 Group（组）。**必须带 -a**，否则用户会退出之前的其他组。*

#### 3. 如何查询用户所属组

你可以通过以下几种方式确认用户是否已经成功进入了 sudo 组：

* **方式一：使用 `groups` 指令**
  **Bash**

  ```
  groups username
  ```

  *输出示例：`username : username sudo`（说明已在 sudo 组中）。*
* **方式二：使用 `id` 指令（最详细）**
  **Bash**

  ```
  id username
  ```

#### 4. 修改密码

使用 `passwd` 指令可以修改密码。

* **修改自己的密码：**
  **Bash**

  ```
  passwd
  ```
* **管理员修改他人密码：**
  **Bash**

  ```
  sudo passwd username
  ```

#### 总结一个完整流程：

如果你要为一个新同事开通权限，流程通常是这样的：

1. `sudo adduser ouse` （创建用户并设置初始密码）
2. `sudo usermod -aG sudo ouse` （给他管理员权限）
3. `id ouse` （确认一下组对不对）
4. 通知同事登录后用 `passwd` 改成他自己的密码。

#### 💡 小贴士

**权限生效时间**：用户加入 `sudo` 组后，**必须重新登录（注销再登）**，权限才会生效。如果你当前正以该用户身份登录，直接运行 `sudo` 可能会报错，重连一下 SSH 即可。
