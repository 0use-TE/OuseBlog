**PlatformIO中，ESPAsyncWebServer库报错，找不到Wifi.h**

错误1：fatal error: WiFiServer.h: No such file or directory → 来自 Arduino 自带 WebServer 库 错误2：fatal error: WiFi.h: No such file or directory → ESPAsyncWebServer 编译时找不到 WiFi.h

; 依赖库 lib_deps = esp32async/ESPAsyncWebServer

; 永久屏蔽 Arduino 自带的老旧同步 WebServer（解决 WiFiServer.h 找不到） lib_ignore = WebServer