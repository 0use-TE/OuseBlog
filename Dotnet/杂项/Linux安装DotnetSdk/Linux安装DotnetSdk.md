### Linux安装DotnetSdk

1. 可以前往官网直接下载二进制包([DotnetSdk](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))，也可以使用脚本,这里仅演示脚本自动安装
2. 使用curl或者wget下载自动安装脚本

   ```bash
   wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
   ```

   添加执行权限

   ```bash
   chmod +x ./dotnet-install.sh
   ```

   安装最新LTS版本

   ```bash
   ./dotnet-install.sh --version latest
   ```

   更多详情查看[DotnetSdk脚本安装](https://learn.microsoft.com/zh-cn/dotnet/core/install/linux-scripted-manual#scripted-install)
3. 添加dotnet到/usr/local/bin或者/usr/bin(不推荐,因为这个一般是放系统指令)
   我们可以创建一个软连接

   ```bash
   sudo ln -s ~/.dotnet/dotnet /usr/local/bin/dotnet
   ```

   也可以使用export PATH="\$PATH:\$HOME/.dotnet"
   直接在终端执行，将仅限本次
   这里建议追加在/bashrc里面，以每次打开终端都生效
4. 使用 dotnet --info查看是否添加成功
