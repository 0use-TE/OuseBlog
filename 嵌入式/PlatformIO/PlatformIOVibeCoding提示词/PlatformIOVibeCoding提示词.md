# PlatformIO Demo 项目开发规范

## 项目结构

```
src/
├── main.cpp              # 入口文件
├── config.h              # 配置文件
lib/                      # 功能模块目录（每个模块独立）
│   ├── <module_name>/    # 模块目录
│   │   ├── <module_name>.h   # 头文件
│   │   ├── <module_name>.cpp # 源文件（简单模块可用 .hpp 包含实现）
│   │   ├── docs/
│   │   │   └── <module_name>.md  # 模块文档
│   │   └── test/         # 单元测试
│   │       ├── test_<module_name>.cpp
│   │       └── CMakeLists.txt
include/                  # 公共头文件
test/                     # 整体测试
platformio.ini            # PlatformIO配置
README.md                 # 项目说明
```

## 开发要求

### 1. 模块化开发

每个模块必须包含：
- **头文件（.h）**：声明公共接口、宏定义、数据结构
- **源文件（.cpp）**：实现功能（简单模块可直接用 .hpp 包含实现）
- **单元测试（test/）**：使用 Unity 测试框架
- **文档（docs/）**：说明 API 用法、注意事项、示例代码

### 2. 内存管理

- **优先使用栈分配**：`Type var;` 而不是 `Type* var = new Type();`
- 动态分配仅在以下情况使用：
  - 对象生命周期不确定
  - 数组大小运行时确定
  - 需要运行时多态

### 3. 日志模块

- 提供日志级别：`ERROR`, `WARN`, `INFO`, `DEBUG`, `VERBOSE`
- 输出格式：`[LEVEL] <message>`
- **默认使用 Serial 输出**，无需文件系统
- **可选持久化配置**（生产环境查问题用）：
  ```cpp
  #define LOG_PERSIST_ENABLE   true/false  // 是否启用持久化
  #define LOG_PERSIST_DIR      "/logs"     // 日志目录
  #define LOG_PERSIST_MAX_DAYS 7           // 保留天数
  #define LOG_PERSIST_MAX_SIZE 65536       // 单文件最大字节（默认64KB）
  #define LOG_PERSIST_LEVEL    INFO        // 持久化日志级别
  ```
- 启用持久化时自动检测并挂载文件系统

### 4. 单元测试

- 使用 Unity 测试框架（PlatformIO 内置）
- 测试模块的公共接口
- 测试边界条件和异常情况
- 每个模块独立测试

---

## 用户输入

请提供以下信息：

| 项目 | 说明 | 示例 |
|------|------|------|
| **目标芯片** | 使用的 MCU 型号 | ESP32、ESP8266、STM32F4、RP2040 |
| **附加要求** | 特殊需求或依赖 | LittleFS、日志持久化、特定引脚、使用某库等 |
| **实现功能** | 项目要实现的功能 | 温湿度监测、Web服务器控制、LED灯控制等 |
| **日志持久化**（选填） | | |
| - 启用持久化 | 是否保存日志到文件系统 | true/false（默认 false） |
| - 保留天数 | 日志文件保留时间 | 7（默认7天） |
| - 单文件大小 | 单个日志文件最大字节 | 65536（默认64KB） |

---

## 输出要求

1. **platformio.ini**：完整的 PlatformIO 配置
2. **config.h**：所有可配置参数（包含日志持久化配置）
3. **main.cpp**：模块初始化和主循环示例
4. **各模块文件**：头文件、源文件
5. **单元测试**：每个模块的 test_<module_name>.cpp
6. **模块文档**：docs/<module_name>.md
7. **README.md**：项目说明

## 代码风格

- 使用英文命名（变量、函数、类）
- 常量使用大写加下划线：`MAX_BUFFER_SIZE`
- 函数采用小驼峰：`readSensorData()`
- 类采用大驼峰：`TemperatureSensor`
- 头文件使用 `#pragma once` 防止重复包含
- 关键逻辑添加注释
