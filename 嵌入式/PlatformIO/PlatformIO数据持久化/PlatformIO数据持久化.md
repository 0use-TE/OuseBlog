### PlatformIO数据持久化

#### 前言

在嵌入式开发中，日志，用户配置等均需要数据持久化，分为本地和云端，云端就是通过无线协议上传到服务器存储，这里只介绍本地。
一般有两个方案，挂载一个文件系统或者使用硬件提供的NVS
对于日志记录等大容量数据，建议挂载一个LittleFS，您可以在我的博客查看如何使用PlatformIO为芯片挂载文件系统
因此这里只介绍NVS

#### NVS

**NVS** 全称是 **Non-Volatile Storage**（非易失性存储)，类似于注册表，专门用于保存简单的键值对，需要硬件支持。
确保硬件支持后即可使用以下Api

```cpp
Preferences preferences; //在栈区实例化一个Preferences
const char *NVS_NAMESPACE = "wifi_config";//Preferences通过不同的字符串区分不同的空间，从而实现一个key可以根据namespace获取到不同的value

Preferences为了高效独写，使用了batch设计模式，begin开始后，执行的各种操作都不会立即执行，只有end时候，会执行中间所有的操作
//读取示例
preferences.begin(NVS_NAMESPACE, true); // 只读模式
String ssid = preferences.getString("ssid", "");
preferences.end();
//写入示例
preferences.begin(NVS_NAMESPACE, true); // 只读模式
preferences.putString("ssid", ssid);
preferences.putString("pass", password);
preferences.end();
```

### 结语

#### NVS是真方便!