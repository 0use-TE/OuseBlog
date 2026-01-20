<<<<<<< HEAD
### PlatformIOGPIO
=======
## PlatformIOGPIO

### 介绍

GPIO在嵌入式中有多重要就不多做介绍了，这里直接介绍Api

### 配置

输入和输出均使用 **pinMode** 函数
签名如下

```c
void pinMode(uint8_t pin,uint8_t mode);
//pin为引脚，看硬件图
//mode为模式，一般为INPUT(输入)和OUTPUT(输出)  tip:这里分别是高阻态输入和推挽输出,其它模式一般不用
//执行后便配置好了引脚模式
```

### 读取和设置

设置高低电平

```c
 void digitalWrite(uint8_t pin,uint8_t val);
//pin为要设置的引脚，val写0是低电平，1是高电平，可以使用宏定义LOW和HIGH
//例：digitalWrite(18,HIGH)
```

读取引脚值

```c
uint8_t digitalRead(uint8_t pin);
//读取pin的值，0为低电平，1为高电平
//判断可以使用宏定义，LOW和HIGN
if(digitalRead(18)==HIGH)
    Serial.println("18号引脚是高电平!");
```

#### 结语

由此可见，Arduino为了提供了非常便捷的函数！
>>>>>>> ca33e45845129ace8786cb1fc9fb5ee622890b36
