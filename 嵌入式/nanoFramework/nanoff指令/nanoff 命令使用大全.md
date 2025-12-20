# nanoff 命令使用大全

## 概述

**nanoff** 是 .NET nanoFramework 的命令行工具，用于烧录固件、部署应用程序、管理设备配置等。主要用于 ESP32、STM32 等微控制器。需要安装 .NET SDK（6.0 或更高），通过 `dotnet tool install -g nanoff` 安装。

## 安装与更新

- 安装 nanoff

  ：

  ```bash
  dotnet tool install -g nanoff
  ```

- 更新 nanoff

  ：

  ```bash
  dotnet tool update -g nanoff
  ```

- 验证版本

  ：

  ```bash
  nanoff --version
  ```

  输出示例：

  ```
  nanoff version 2.5.126
  ```

**注意**：

- STM32 设备需避免安装路径含非 ASCII 字符（如中文），否则可能因 STM32 Cube Programmer  bug 导致错误。用 

  ```
  --tool-path
  ```

   指定路径：

  ```bash
  dotnet tool install nanoff --tool-path c:\nanoff-tool
  ```

## 常用命令与参数

以下是 nanoff 的核心命令和选项，分为烧录、管理和调试功能。

### 1. 固件烧录（Firmware Flashing）

烧录 nanoFramework 固件到设备（如 ESP32）。

- **更新到最新固件**：

  ```bash
  nanoff --platform esp32 --serialport COM3 --update
  ```

  - `--platform esp32`：指定平台（如 esp32、stm32）。
  - `--serialport COM3`：设备连接的 COM 端口（Windows 用 COM3，Linux/macOS 用 /dev/ttyUSB0）。
  - `--update`：下载并烧录最新固件。
  - 示例输出：`Using 'ESP32_PSRAM_REV0' based on device characteristics.`

- **指定目标固件**：

  ```bash
  nanoff --target ESP32_REV3 --serialport COM3 --update
  ```

  - `--target`：指定目标设备（如 `ESP32_REV3`、`ESP32_PSRAM_REV0`）。
  - 常见 ESP32 目标：
    - `ESP32_REV0`：ESP32 无 PSRAM。
    - `ESP32_PSRAM_REV0`：ESP32 带 PSRAM，通用。
    - `ESP32_S2_WROVER`：ESP32-S2 带 PSRAM。

- **烧录特定固件版本**：

  ```bash
  nanoff --target ESP32_REV3 --serialport COM3 --update --fwversion 1.8.0.545
  ```

  - `--fwversion`：指定固件版本（如 1.8.0.545）。

- **烧录本地固件文件**：

  ```bash
  nanoff --target ESP32_REV3 --serialport COM3 --deploy --image "C:\nanoCLR.bin" --address 0x10000
  ```

  - `--deploy`：烧录本地固件文件。
  - `--image`：固件文件路径（.bin 格式）。
  - `--address`：Flash 存储地址（ESP32 通常 0x10000）。

- **包含预览版固件**：

  ```bash
  nanoff --platform esp32 --serialport COM3 --update --preview
  ```

  - `--preview`：使用预览版固件（实验性功能）。

- **进入 Bootloader 模式**：

  - ESP32 烧录可能需手动进入 Bootloader：按住 **Boot 按钮**（GPIO 0），按一下 **EN 按钮**（复位），松开 EN 后再松开 Boot。
  - nanoff 通常通过 DTR/RTS 自动触发，但部分板子需手动操作，提示如：`Hold down the BOOT/FLASH button in ESP32 board`.

### 2. 设备管理

管理设备信息、端口和配置。

- **列出可用 COM 端口**：

  ```bash
  nanoff --listports
  ```

  - 输出示例：`COM3, COM5`
  - 插拔设备后再次运行，新增端口即为设备端口。

- **列出支持的目标**：

  ```bash
  nanoff --platform esp32 --listtargets
  ```

  - 输出支持的设备目标（如 `ESP32_REV0`, `ESP32_PSRAM_REV0`）。

  - 加 

    ```
    --preview
    ```

     显示预览版目标：

    ```bash
    nanoff --platform esp32 --listtargets --preview
    ```

- **查看设备详情**：

  ```bash
  nanoff --nanodevice --devicedetails --serialport COM3
  ```

  - 输出设备信息，如 CLR 版本、固件版本、Flash 大小等。

- **擦除 Flash**：

  ```bash
  nanoff --target ESP32_REV3 --serialport COM3 --masserase
  ```

  - `--masserase`：擦除整个 Flash，清除固件和数据。

### 3. 文件部署

将文件（如配置文件）部署到设备存储（支持 ESP32 等有存储的设备）。

- 部署文件

  ：

  ```bash
  nanoff --target ESP32_PSRAM_REV0 --serialport COM3 --update --filedeployment C:\deploy.json
  ```

  - `--filedeployment`：指定 JSON 文件路径，定义要部署的文件。

  - JSON 示例：

    ```json
    {
      "files": [
        {
          "source": "C:\\config.txt",
          "destination": "/config.txt"
        }
      ]
    }
    ```

### 4. 网络配置

配置 ESP32 的 Wi-Fi 等网络设置。

- 配置 Wi-Fi

  ：

  ```bash
  nanoff --target ESP32_PSRAM_REV0 --serialport COM3 --config --ssid MyWiFi --password MyPassword
  ```

  - `--config`：启用网络配置。
  - `--ssid` 和 `--password`：Wi-Fi 名称和密码。

### 5. 调试与日志

控制 nanoff 的输出和调试信息。

- **详细日志**：

  ```bash
  nanoff --platform esp32 --serialport COM3 --update --v diag
  ```

  - `--v diag`：启用诊断级别日志（verbose），用于排查问题。

- **静默模式**：

  ```bash
  nanoff --platform esp32 --serialport COM3 --update --v quiet
  ```

  - `--v quiet`：最小化输出，适合自动化脚本。

- **禁用目标校验**：

  ```bash
  nanoff --platform esp32 --serialport COM3 --update --nofitcheck
  ```

  - `--nofitcheck`：跳过目标设备匹配检查（当设备信息不明确时）。

- **禁用版本检查**：

  ```bash
  nanoff --platform esp32 --serialport COM3 --update --suppressnanoffversioncheck
  ```

  - `--suppressnanoffversioncheck`：禁用 nanoff 工具的版本检查。

### 6. 其他选项

- **清空缓存**：

  ```bash
  nanoff --clearcache
  ```

  - 清除固件缓存（位于用户目录的 `~/.nanoFramework/fw_cache`）。

- **禁用遥测**：

  - nanoff 默认发送匿名遥测数据（如命令参数、固件版本）。禁用方法：

    ```bash
    set NANOFRAMEWORK_TELEMETRY_OPTOUT=1
    ```

  - Windows 上用 `set`，Linux/macOS 用 `export`。

## 常见问题与解决

1. **E4000 错误**：

   - 原因：未正确进入 Bootloader 模式。

   - 解决：运行命令前，按住 Boot 按钮直到看到“Erasing”消息，或降低波特率：

     ```bash
     nanoff --platform esp32 --serialport COM3 --update --baud 115200
     ```

2. **端口不可用**：

   - 检查：用 `nanoff --listports` 确认端口，关闭占用程序（如 PuTTY）。
   - 确保 USB 线支持数据传输（非仅充电线）。

3. **固件版本不匹配**：

   - 错误示例：`mscorlib requires native v100.5.0.24, but target has v100.5.0.23`。
   - 解决：更新固件（`--update`）或指定版本（`--fwversion`），并更新 NuGet 包（如 `nanoFramework.CoreLibrary`）。

4. **设备未识别**：

   - 检查 USB 驱动（CP210x/CH340），从 [Silabs Drivers](https://www.silabs.com/developers/usb-to-uart-bridge-vcp-drivers) 或 [CH340 Drivers](http://www.wch.cn/downloads/CH341SER_ZIP.html) 下载。
   - 按 EN 按钮复位设备。

## 示例场景

1. **烧录最新 ESP32 固件**：

   ```bash
   nanoff --platform esp32 --serialport COM3 --update
   ```

2. **部署本地固件并配置 Wi-Fi**：

   ```bash
   nanoff --target ESP32_PSRAM_REV0 --serialport COM3 --deploy --image "C:\nanoCLR.bin" --address 0x10000 --config --ssid MyWiFi --password MyPassword
   ```

3. **调试固件烧录问题**：

   ```bash
   nanoff --platform esp32 --serialport COM3 --update --v diag --baud 115200
   ```

## 参考资源

- [nanoff 官方文档](https://docs.nanoframework.net/content/tools/nanoff.html)
- [nanoFramework 固件发布](https://github.com/nanoframework/nf-interpreter/releases)
- [ESP32 技术参考手册](https://www.espressif.com/sites/default/files/documentation/esp32_technical_reference_manual_en.pdf)
- [nanoff 源码](https://github.com/nanoframework/nanoFirmwareFlasher)