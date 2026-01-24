### ssh上传公钥免密脚本

当我们完成ssh远程连接后，为了免密登录，可以将usr/.ssh下面的公钥拷贝到服务端，但是这样比较麻烦，为了简单期间，我们可以使用以下指令。

1. 对于使用openssh的linux系统，我们可以输入

   ```bash
   ssh-copy-id
   ```
2. 对于windows，可以打开powershell，输入以下指令

   ```
   cat $HOME\.ssh\id_rsa.pub | ssh ouse@192.168.137.80 "mkdir -p ~/.ssh && cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys"
   ```

然后我们便可以免密ssh登录了√
