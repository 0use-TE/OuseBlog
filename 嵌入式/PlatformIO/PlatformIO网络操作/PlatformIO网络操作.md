### PlatformIO网络操作

#### 前言

目标：以ESP32为例，同时支持STA+AP

#### 代码

```cpp
WiFi.mode(WIFI_AP_STA);//配置为AP和STA双模式
WiFi.softAP(ssid,password);//软路由模式开启一个wifi，默认ip为192.168.4.1
WiFi.begin("your_ssid", "your_password");
```

#### 有用的代码

1. STA连接成功，打印Ip

```cpp
Serial.print("STA 模式 IP 地址: http://");
Serial.println(WiFi.localIP());
```

2. 打印软路由的Ip,固定为192.168.4.1

```cpp
Serial.print("AP 模式 IP 地址: http://");
Serial.println(WiFi.softAPIP());
```

3. 连接等待和状态存储

```cpp
//连接等待和状态存储
int attempts = 0;//尝试的次数
while (WiFi.status() != WL_CONNECTED && attempts < 4) // 尝试 2 秒
{
	delay(500);
	Serial.print(".");
	attempts++;
}
```

#### 结语

爽飞了!
