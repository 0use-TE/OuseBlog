### PlatformIOWebModule

#### 前言

本篇将介绍esp32async/ESPAsyncWebServer包的使用
添加依赖，这个包同时包含WebServer和WebSocket模块

```ini
lib_deps=
 esp32async/ESPAsyncWebServer @ ^3.9.2
lib_ignore = WebServer
```

#### WebServer

实例化一个Server

```cpp
AsyncWebServer server{80};//列表初始化里面传入端口号
```

添加一个api
注意：第一个参数为路由，第二个为HttpAction，例如HTTP_GET，HTTP_POST
第三个是一个lamda回调，匹配成功将触发
request集成了操作请求体和一些特殊函数(比如request->send()表示返回响应体)

```cpp
server.on("/", HTTP_GET, [&](AsyncWebServerRequest *request)
                  { request->send(200, "text/html", HTML); });
```

```cpp
request->send(200, "text/html", HTML);//第一个参数是状态码，第二个是响应体类型，第三个是响应体
```

```cpp
// 保存配置 /save (接收 POST 数据) 然后重启设备
server.on("/save", HTTP_POST, [&](AsyncWebServerRequest *request)
          {
              if (request->hasParam("ssid", true) && request->hasParam("pass", true)) {
                  String ssid = request->getParam("ssid", true)->value();
                  String pass = request->getParam("pass", true)->value();

                  config.saveWifiConfig(ssid, pass); // 保存配置

                  request->send(200, "text/plain", "配置已保存。设备将在 5 秒内重启并尝试连接新网络。");

                  delay(100);
                  ESP.restart(); // 重启
              } else {
                  request->send(400, "text/plain", "参数缺失。");
              } });
```

启动

```cpp
server.begin();
```

#### WebClient

#### WebSocketServer

实例化一个WebSocket服务端

```cpp
AsyncWebSocket wsCtrl{"/ctrl"};
```

将WebScoket绑定到WebServer上，复用端口，内部自动处理Http Upgrate到WebSocket

```cpp
server.addHandler(&wsCtrl);
```

##### 接受回调

WebSocket的回调函数，用于处理客户端连接、消息、断开等事件

```cpp
wsCtrl.onEvent([&](AsyncWebSocket *server, AsyncWebSocketClient *client,
                 AwsEventType type, void *arg, uint8_t *data, size_t len)
{
    if (type == WS_EVT_CONNECT) {
        // 客户端连接成功
        Serial.printf("WebSocket客户端连接: clientId=%u\n", client->id());
    }
    else if (type == WS_EVT_DISCONNECT) {
        // 客户端断开连接
        Serial.printf("WebSocket客户端断开: clientId=%u\n", client->id());
    }
    else if (type == WS_EVT_DATA) {
        // 收到客户端消息
        AwsFrameInfo *info = (AwsFrameInfo *)arg;
        if (info->final && info->index == 0 && info->len == len) {
            // 消息完整且在一帧内
            String message = "";
            for (size_t i = 0; i < len; i++) {
                message += (char)data[i];
            }
            Serial.printf("收到客户端[%u]消息: %s\n", client->id(), message.c_str());
        }
    }
    else if (type == WS_EVT_PONG) {
        // 收到PONG帧（心跳响应）
        Serial.printf("收到客户端[%u] PONG\n", client->id());
    }
});
```

##### 发送到指定客户端

```cpp
// 发送文本消息给指定客户端
client->text("Hello Client");

// 发送二进制数据给指定客户端
uint8_t buf[] = {0x01, 0x02, 0x03};
client->binary(buf, sizeof(buf));

// 发送JSON格式消息
StaticJsonDocument<200> doc;
doc["status"] = "ok";
doc["value"] = 123;
String json;
serializeJson(doc, json);
client->text(json);
```

##### 广播（发送给所有客户端）

```cpp
// 广播文本消息给所有客户端
wsCtrl.textAll("Broadcast message");

// 广播JSON消息
StaticJsonDocument<200> doc;
doc["type"] = "broadcast";
doc["message"] = "Hello everyone!";
String json;
serializeJson(doc, json);
wsCtrl.textAll(json);

// 广播二进制数据
uint8_t data[] = {0xAA, 0xBB, 0xCC};
wsCtrl.binaryAll(data, sizeof(data));

// 获取当前客户端数量
Serial.printf("当前客户端数量: %u\n", wsCtrl.count());
```

##### 清理无效客户端

```cpp
// 清理断开的客户端，释放资源
wsCtrl.cleanupClients();
```

#### WebSocketClient

使用ESPAsyncWebSockets库也可以实现WebSocket客户端功能，需要添加依赖：

```ini
lib_deps=
    links2004/WebSockets @ ^2.4.1
```

##### 实例化客户端

```cpp
#include <WebSocketsClient.h>

WebSocketsClient webSocket;
```

##### 连接到服务端

```cpp
// 连接到WebSocket服务端
webSocket.begin("192.168.1.100", 8080, "/ws");

// 设置连接成功的回调
webSocket.onEvent([](WStype_t type, uint8_t *payload, size_t length) {
    switch(type) {
        case WStype_DISCONNECTED:
            Serial.println("与服务器断开连接");
            break;
        case WStype_CONNECTED:
            Serial.println("已连接到服务器");
            // 连接成功后可以发送认证消息
            webSocket.sendTXT("auth_token_xxx");
            break;
        case WStype_TEXT:
            Serial.printf("收到服务器消息: %s\n", payload);
            // 处理文本消息
            break;
        case WStype_BIN:
            Serial.printf("收到服务器二进制数据，长度: %zu\n", length);
            // 处理二进制数据
            break;
        case WStype_PING:
            // 收到心跳请求
            Serial.println("收到服务器PING");
            break;
        case WStype_PONG:
            // 收到心跳响应
            Serial.println("收到服务器PONG");
            break;
    }
});

// 设置心跳间隔（默认2000ms）
webSocket.setReconnectInterval(5000);

// 启用Heartbeat（自动发送PING保持连接）
webSocket.enableHeartbeat(1000, 3000, 2);
```

##### 发送消息到服务端

```cpp
// 发送文本消息
webSocket.sendTXT("Hello Server");

// 发送JSON消息
StaticJsonDocument<200> doc;
doc["command"] = "get_status";
String json;
serializeJson(doc, json);
webSocket.sendTXT(json);

// 发送二进制数据
uint8_t buffer[] = {0x01, 0x02, 0x03, 0x04};
webSocket.sendBIN(buffer, sizeof(buffer));

// 发送大数据（自动分包）
String largeData = "这是很长的数据..."; // 超过WebSocket帧限制时会自动分包
webSocket.sendTXT(largeData);
```

##### 断线重连

```cpp
// 手动断开连接
webSocket.disconnect();

// 重新连接
webSocket.reconnect();
```

#### HTTPClient

ESP32可以使用HTTPClient库进行HTTP请求，需要包含头文件：

```cpp
#include <HTTPClient.h>
```

##### GET请求

```cpp
HTTPClient http;

// 发起GET请求
http.begin("https://api.example.com/data");

int httpCode = http.GET();

if (httpCode == HTTP_CODE_OK) {
    String payload = http.getString();
    Serial.println(payload);
} else {
    Serial.printf("GET请求失败，HTTP Code: %d\n", httpCode);
}

http.end();
```

##### POST请求

```cpp
HTTPClient http;

// 发送JSON数据
http.begin("https://api.example.com/submit");
http.addHeader("Content-Type", "application/json");

String jsonData = "{\"name\":\"test\",\"value\":123}";
int httpCode = http.POST(jsonData);

if (httpCode > 0) {
    String response = http.getString();
    Serial.printf("POST响应: %d - %s\n", httpCode, response.c_str());
} else {
    Serial.printf("POST请求错误: %s\n", http.errorToString(httpCode).c_str());
}

http.end();
```

##### POST表单数据

```cpp
HTTPClient http;

http.begin("https://api.example.com/form");
http.addHeader("Content-Type", "application/x-www-form-urlencoded");

String postData = "ssid=mywifi&password=12345678";
int httpCode = http.POST(postData);

http.end();
```

##### 带Bearer Token认证

```cpp
HTTPClient http;

http.begin("https://api.example.com/protected");
http.addHeader("Authorization", "Bearer your_token_here");

int httpCode = http.GET();

if (httpCode == HTTP_CODE_OK) {
    String payload = http.getString();
    Serial.println(payload);
} else if (httpCode == HTTP_CODE_UNAUTHORIZED) {
    Serial.println("认证失败，请检查Token");
}

http.end();
```

##### 下载文件

```cpp
HTTPClient http;

http.begin("https://example.com/firmware.bin");
int httpCode = http.GET();

if (httpCode == HTTP_CODE_OK) {
    File file = SPIFFS.open("/firmware.bin", "w");
    if (file) {
        http.writeToStream(&file);
        file.close();
        Serial.println("文件下载完成");
    }
}

http.end();
```

##### 处理重定向

```cpp
http.setFollowRedirects(HTTPC_FORCE_FOLLOW_REDIRECTS);
http.begin("http://example.com");
```

#### 结语
恭喜您成功学会了网络相关Api！