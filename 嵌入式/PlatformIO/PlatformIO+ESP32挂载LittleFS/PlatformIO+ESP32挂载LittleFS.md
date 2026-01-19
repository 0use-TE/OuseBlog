### **PlatformIO+ESP32挂载LittleFS**

#### 分步教程

1. platformio.ini添加如下配置

```
board_build.filesystem = littlefs
```

1. 当前目录创建一个名为data的文件夹，pio run --target uploadfs 指令默认会把data文件夹下的所有文件打包为镜像传给下位机
2. 在data文件夹添加测试文件，比如index.html
3. 指令或者图形化上传文件系统，图形化教程:点击侧边pio图标，点击 您的开发板名称(比如esp32dev)/Platform/Upload Filesystem Image
4. 写测试代码，如下

```
#include <Arduino.h>
#include "LittleFS.h"
 
void setup() {
  Serial.begin(115200);
  
  if(!LittleFS.begin(true)){
    Serial.println("An Error has occurred while mounting LittleFS");
    return;
  }
  
  File file = LittleFS.open("/index.html");
  if(!file){
    Serial.println("Failed to open file for reading");
    return;
  }
  
  Serial.println("File Content:");
  while(file.available()){
    Serial.write(file.read());
  }
  file.close();
}
 
void loop() {

}
```

### 常用api

1. 初始化与格式化

```
LittleFS.begin()
bool LittleFS.begin(bool formatIfFailed = false);
```

初始化文件系统，挂载。如果挂载失败，是否格式化。

```
LittleFS.format()
bool LittleFS.format();
```

格式化文件系统，清除所有数据。

1. 文件操作

```
LittleFS.open()
File LittleFS.open(const char *path, const char *mode);
```

打开文件，支持读取、写入、追加等模式。

```
File.read()
int File.read();
```

读取文件内容，一个字节。

```
File.print() / File.println()
size_t File.print(const String &str);
```

向文件写入字符串。

```
File.close()
void File.close();
```

关闭文件。

1. 文件系统管理

```c++
LittleFS.exists()
bool LittleFS.exists(const char *path);
```

检查文件或目录是否存在。

```c++
LittleFS.remove()
bool LittleFS.remove(const char *path);
```

删除文件。

```
LittleFS.rename()
bool LittleFS.rename(const char *oldPath, const char *newPath);
```

重命名或移动文件。

1. 文件系统信息

```c++
LittleFS.totalBytes()
size_t LittleFS.totalBytes();
```

获取文件系统总大小。c

```c
LittleFS.usedBytes()
size_t LittleFS.usedBytes();
```

获取已用空间。

```c
LittleFS.freeBytes()
size_t LittleFS.freeBytes();
```