# Linux 解压缩指令



### 1. tar（最常用）

打包归档，不压缩：

```bash
tar -cvf archive.tar file1 file2    # 打包
tar -xvf archive.tar                 # 解包到当前目录
tar -xvf archive.tar -C /path/       # 解包到指定目录
```

打包 + 压缩（gz）：

```bash
tar -czvf archive.tar.gz file1 file2 # 打包压缩
tar -xzvf archive.tar.gz             # 解压
tar -xzvf archive.tar.gz -C /path/   # 解压到指定目录
```

常用选项说明：
- `-c` create 创建归档
- `-x` extract 解压
- `-v` verbose 显示过程
- `-f` file 指定文件名
- `-z` gzip 压缩
- `-C` directory 切换到指定目录

------

### 2. gzip

单独压缩文件（不打包）：

```bash
gzip filename          # 压缩，生成 filename.gz，原文件删除
gzip -k filename      # 压缩，保留原文件
gzip -d filename.gz   # 解压
gunzip filename.gz    # 解压，等价于 gzip -d
gzip -9 filename      # 最高压缩率
```

------

### 3. zip（跨平台常用）

```bash
zip archive.zip file1 file2       # 压缩多个文件
zip -r archive.zip directory/     # 递归压缩目录
zip -q -r archive.zip directory/ # 静默模式，不显示过程

unzip archive.zip                 # 解压到当前目录
unzip archive.zip -d /path/       # 解压到指定目录
unzip -l archive.zip              # 查看压缩包内容
unzip -q archive.zip              # 静默解压
```

需要安装：`sudo apt install zip unzip`

------

### 4. rar

```bash
rar a archive.rar file1 file2     # 压缩
rar x archive.rar                # 解压
rar x archive.rar /path/         # 解压到指定目录
unrar e archive.rar /path/       # 解压，不保留目录结构

rar l archive.rar                # 查看内容
```

需要安装：`sudo apt install rar unrar`

------

### 5. 7z（高压缩率）

```bash
7z a archive.7z file1 file2       # 压缩
7z x archive.7z                   # 解压
7z x archive.7z -o/path/          # 解压到指定目录（-o后无空格）

7z l archive.7z                    # 查看内容
7z t archive.7z                    # 测试完整性
```

需要安装：`sudo apt install p7zip-full`

------

### 6. 常见问题

- **tar.gz 和 tgz 是一样的**，只是扩展名习惯不同
- **.tar.bz2** 用 `tar -xjf archive.tar.bz2`
- **.tar.xz** 用 `tar -xJf archive.tar.xz`
- 解压时注意权限，可能需要 `sudo`
- 查看压缩包内容不解压：`tar -tzf archive.tar.gz`
