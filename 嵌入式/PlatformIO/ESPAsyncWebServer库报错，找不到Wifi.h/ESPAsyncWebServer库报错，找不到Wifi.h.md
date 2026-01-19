### **PlatformIO中，ESPAsyncWebServer库报错，找不到Wifi.h**

错误1：fatal error: WiFiServer.h: No such file or directory → 来自 Arduino 自带 WebServer 库 错误2：fatal error: WiFi.h: No such file or directory → ESPAsyncWebServer 编译时找不到 WiFi.h

```ini
; PlatformIO Project Configuration File
;
;   Build options: build flags, source filter
;   Upload options: custom upload port, speed and extra flags
;   Library options: dependencies, extra library storages
;   Advanced options: extra scripting
;
; Please visit documentation for the other options and examples
; https://docs.platformio.org/page/projectconf.html

[env:esp32cam]
platform = espressif32
board = esp32cam
framework = arduino

lib_deps= 
 esp32async/ESPAsyncWebServer @ ^3.9.2 ; 依赖库 lib_deps = esp32async/ESPAsyncWebServer

lib_ignore = WebServer ; 永久屏蔽 Arduino 自带的老旧同步 WebServer（解决 WiFiServer.h 找不到）

monitor_speed = 115200
```

