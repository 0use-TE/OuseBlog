### PlatformIO模块化开发

#### 前言

为了模块化，逻辑清晰，更容易单元测试，可以采用以下项目结构开发，这里以通过WebSocket控制车为例讲解。

#### 文件结构

```ini
-ProjectName
--.pio //项目依赖和配置文件,不用关心
--.vscode //VSCode配置文件，不用关心
--.include //这个文件放头文件，一般不用
--lib
---Settings//这里放所有引脚定义和配置,文档
----Settings.hpp
----Settings.md
---MotorModule//电机控制模块
----MotorModule.hpp//电机控制模块
...还有Wifi连接和WebSocket模块，不再赘述
--src
---main.cpp//引用所有的模块，按照顺序初始化和执行
--test //这里放单元测试，使用的是Unity测试框架(不是游戏引擎)
```

#### 具体

所有模块使用类进行封装，因为在main.cpp中是使用的全局变量，因此各模块内部尽可能使用栈！

1. Setting.hpp

```cpp
#pragma once
// AI-Thinker 标准引脚
#define PWDN_GPIO_NUM 32
#define RESET_GPIO_NUM -1
#define XCLK_GPIO_NUM 0
#define SIOD_GPIO_NUM 26
#define SIOC_GPIO_NUM 27
#define Y9_GPIO_NUM 35
#define Y8_GPIO_NUM 34
#define Y7_GPIO_NUM 39
#define Y6_GPIO_NUM 36
#define Y5_GPIO_NUM 21
#define Y4_GPIO_NUM 19
#define Y3_GPIO_NUM 18
#define Y2_GPIO_NUM 5
#define VSYNC_GPIO_NUM 25
#define HREF_GPIO_NUM 23
#define PCLK_GPIO_NUM 22 
```

2. WifiModule

   ```cpp
   #pragma once
   #include <Arduino.h>
   #include <WiFi.h>
   #include "Settings.hpp"
   #include "ConfigModule.hpp"
   
   class WifiModule
   {
   private:
       // 存储 ConfigModule 的引用
       ConfigModule &config;
   
   public:
       // 构造函数，接受 ConfigModule 引用作为依赖
       WifiModule(ConfigModule &cfg) : config(cfg) {}
   
       void init()
       {
           // 1. 尝试从 NVS 加载配置
           String savedSSID = config.getSavedSSID();
           String savedPass = config.getSavedPassword();
   
           // 始终开启 AP 模式作为后备/控制
           WiFi.mode(WIFI_AP_STA);
           WiFi.softAP("RoverCar", "12345678");
   
           if (savedSSID.length() > 0)
           {
               // 使用保存的配置进行连接
               Serial.printf("尝试连接已保存的网络: %s\n", savedSSID.c_str());
               WiFi.begin(savedSSID.c_str(), savedPass.c_str());
           }
   
           // 2. 连接等待和状态存储
           int attempts = 0;
           while (WiFi.status() != WL_CONNECTED && attempts < 4) // 尝试 2 秒
           {
               delay(500);
               Serial.print(".");
               attempts++;
           }
   
           Serial.println();
   
           if (WiFi.status() == WL_CONNECTED)
           {
               config.setConnectionStatus("Connected! IP: " + WiFi.localIP().toString());
               Serial.print("STA 模式 IP 地址: http://");
               Serial.println(WiFi.localIP());
           }
           else
           {
               config.setConnectionStatus("Connection Failed. Check credentials.");
               Serial.println("WiFi 连接失败。");
               Serial.print("AP 模式 IP 地址: http://");
               Serial.println(WiFi.softAPIP());
           }
       }
   };
   ```

3. main.cpp

```c++
#include "WifiModule.hpp"
#include "WebModule.hpp"
#include "CameraModule.hpp"
#include "ConfigModule.hpp"
// 实例化模块
ConfigModule configModule;
WifiModule wifiModule(configModule); // 注入 configModule
WebModule webModule(configModule);   // 注入 configModule
CameraModule camera;

void setup()
{
    Serial.begin(115200);
    // 启动顺序：
    wifiModule.init(); // 包含加载 NVS 配置和连接逻辑
    camera.init();     // 启动摄像头
    webModule.init();  // 启动 Web 服务器和 API
}

void loop()
{
    webModule.loop();
}
```

#### 结语

按照这种方式，您就拥有了模块化，单元测试友好的代码架构！