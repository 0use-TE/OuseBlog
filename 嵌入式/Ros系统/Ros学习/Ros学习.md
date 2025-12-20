#                                                         编程基础

## 建立项目

工作空间：相当于是一工程，不同的项目对应不同的工程
在 Ubuntu 中创建一个 ROS 工作空间（通常称为 `catkin` 工作空间）是学习和开发 ROS 项目的第一步。以下是创建 ROS 工作空间的步骤：

### 步骤 1: 打开终端
首先，打开你的终端。你可以按下快捷键 `Ctrl + Alt + T` 来启动终端。

### 步骤 2: 创建 ROS 工作空间目录
在终端中输入以下命令来创建工作空间目录：

```bash
mkdir -p ~/catkin_ws/src
```

解释：
- `mkdir` 是创建目录的命令。
- `-p` 表示在必要时创建所有父目录。
- `~/catkin_ws/src` 是工作空间的目录结构，其中 `~` 代表当前用户的主目录。

执行完命令后，你将创建一个名为 `catkin_ws` 的工作空间，并在其中创建一个名为 `src` 的子目录。

### 步骤 3: 初始化工作空间
接下来，需要初始化工作空间。在终端中输入以下命令：

```bash
cd ~/catkin_ws/
catkin_make
```

解释：
- `cd ~/catkin_ws/` 将终端目录切换到工作空间。
- `catkin_make` 是一个用于构建 ROS 工作空间的工具，它会生成一组必要的文件和文件夹，确保工作空间的结构正确。

成功执行后，`catkin_make` 会在 `catkin_ws` 目录中创建 `build` 和 `devel` 两个目录。

### 步骤 4: 设置环境
为了让你的终端自动识别新的 ROS 工作空间，你需要在每次打开新终端时，设置工作空间环境。可以通过下面的命令加载工作空间：

```bash
source ~/catkin_ws/devel/setup.bash
```

为了自动执行此操作，可以将上面的命令添加到你的 `~/.bashrc` 文件中，这样每次打开终端时都会自动加载工作空间：

```bash
echo "source ~/catkin_ws/devel/setup.bash" >> ~/.bashrc
source ~/.bashrc
```

### 总结
到目前为止，你已经成功创建了一个 ROS 工作空间，并且配置了环境以便于使用。接下来，你可以开始在 `src` 目录下创建 ROS 包，进行开发工作。

## 自动加载环境变量

在 ROS 中，我们通过 `catkin_make` 构建项目时会生成一些文件，这些文件包含了各种环境变量和路径信息，让 ROS 知道在哪些地方查找包、库和其他资源。为了让终端能够识别这些新生成的文件和路径，我们需要使用 `source` 命令来加载它们。这里是对每个步骤的解释：

### 为什么要使用 `source ~/catkin_ws/devel/setup.bash`

1. **加载环境变量**：当你在终端中执行 `source ~/catkin_ws/devel/setup.bash` 时，系统会加载 ROS 工作空间生成的环境变量。这样，终端就能知道你当前的 ROS 包在哪里、如何运行 ROS 节点、如何找到依赖的库等等。

2. **每次新终端都要加载**：如果不手动加载环境变量，终端会找不到你在工作空间中创建的 ROS 包。这是因为环境变量只对当前终端有效，关闭终端或打开一个新终端时，这些设置就会丢失。

### 自动加载环境变量

为了避免每次打开新终端都要手动输入 `source ~/catkin_ws/devel/setup.bash`，我们可以将这条命令添加到终端的配置文件 `~/.bashrc` 中。这个文件会在每次打开新终端时自动执行所有包含的命令。这样做有以下好处：

1. **自动化**：只需要设置一次，之后每次打开新终端时都会自动加载 ROS 环境。
2. **简化工作流程**：不用每次都手动输入 `source` 命令，节省时间和精力。

### 解释代码

1. **命令**：
   ```bash
   echo "source ~/catkin_ws/devel/setup.bash" >> ~/.bashrc
   ```
   这行命令的意思是将 `source ~/catkin_ws/devel/setup.bash` 这条命令追加到 `~/.bashrc` 文件的末尾。也就是说，它会把你设置的环境变量加载命令写入到终端的启动脚本中。

2. **执行**：
   ```bash
   source ~/.bashrc
   ```
   这条命令立即执行 `.bashrc` 文件中的所有命令，以便当前终端会话中立即生效，而无需关闭再打开。

### 为什么必须这样做

1. **工作空间与 ROS 环境关联**：`catkin_make` 只是构建了工作空间，它不会自动告诉你的终端使用哪个工作空间。如果没有加载工作空间的环境配置，你的终端将无法识别新的 ROS 包和节点。

2. **保持环境一致性**：每次启动新终端时，都需要确保环境变量正确设置，以便你能够无缝地使用和开发 ROS 应用程序。

### 总结

即使你已经用 `catkin_make` 创建了工作空间，也需要通过 `source ~/catkin_ws/devel/setup.bash` 来加载工作空间的环境变量。为了避免每次都手动执行这条命令，将它添加到 `~/.bashrc` 中可以使每次打开终端时自动加载。这样，你就可以直接在终端中使用 ROS，而不需要每次手动配置环境变量。

## 四种通讯模式

在 ROS（Robot Operating System）中，通信方式主要用于不同节点之间的数据交换。以下是四种主要的通信方式，并配有 C++ 代码示例。

### 1. 发布/订阅（Publish/Subscribe）

**定义**：通过主题（Topic）进行的异步通信。一个节点发布消息，其他节点可以订阅这个主题以接收消息。

**示例**：

- **发布者节点**：

```cpp
#include "ros/ros.h"
#include "std_msgs/String.h"

int main(int argc, char **argv) {
    ros::init(argc, argv, "talker");
    ros::NodeHandle n;
    ros::Publisher pub = n.advertise<std_msgs::String>("chatter", 1000);
    
    ros::Rate loop_rate(10); // 10Hz

    while (ros::ok()) {
        std_msgs::String msg;
        msg.data = "Hello, ROS!";
        pub.publish(msg);
        ros::spinOnce();
        loop_rate.sleep();
    }
    return 0;
}
```

- **订阅者节点**：

```cpp
#include "ros/ros.h"
#include "std_msgs/String.h"

void chatterCallback(const std_msgs::String::ConstPtr& msg) {
    ROS_INFO("Received: [%s]", msg->data.c_str());
}

int main(int argc, char **argv) {
    ros::init(argc, argv, "listener");
    ros::NodeHandle n;
    ros::Subscriber sub = n.subscribe("chatter", 1000, chatterCallback);
    
    ros::spin(); // Keep the node running
    return 0;
}
```

### 2. 服务/客户端（Service/Client）

**定义**：服务是一种同步的通信方式，节点通过调用服务进行请求-响应式的交互。

**示例**：

- **服务端节点**：

```cpp
#include "ros/ros.h"
#include "my_package/MyService.h" // 自定义服务消息

bool handleService(my_package::MyService::Request &req, my_package::MyService::Response &res) {
    res.result = req.a + req.b; // 例如返回两个数的和
    return true;
}

int main(int argc, char **argv) {
    ros::init(argc, argv, "service_server");
    ros::NodeHandle n;
    ros::ServiceServer service = n.advertiseService("add_two_numbers", handleService);
    
    ROS_INFO("Ready to add two numbers.");
    ros::spin();
    return 0;
}
```

- **客户端节点**：

```cpp
#include "ros/ros.h"
#include "my_package/MyService.h" // 自定义服务消息

int main(int argc, char **argv) {
    ros::init(argc, argv, "service_client");
    ros::NodeHandle n;
    ros::ServiceClient client = n.serviceClient<my_package::MyService>("add_two_numbers");
    
    my_package::MyService srv;
    srv.request.a = 5;
    srv.request.b = 3;
    
    if (client.call(srv)) {
        ROS_INFO("Sum: %d", srv.response.result);
    } else {
        ROS_ERROR("Failed to call service");
    }
    return 0;
}
```

### 3. 动态参数（Dynamic Reconfigure）

**定义**：允许在运行时动态修改节点的参数。

**示例**：

- **节点**：

```cpp
#include "ros/ros.h"
#include "dynamic_reconfigure/server.h"
#include "my_package/MyConfig.h" // 自动生成的配置文件

void callback(my_package::MyConfig &config, uint32_t level) {
    ROS_INFO("Reconfigure Request: %d", config.param);
}

int main(int argc, char **argv) {
    ros::init(argc, argv, "dynamic_reconfigure_node");
    dynamic_reconfigure::Server<my_package::MyConfig> server;
    dynamic_reconfigure::Server<my_package::MyConfig>::CallbackType f;
    f = boost::bind(&callback, _1, _2);
    server.setCallback(f);
    
    ros::spin();
    return 0;
}
```

### 4. 动作库（Action Library）

**定义**：用于处理需要持续时间的任务，如移动机器人到某个位置。客户端可以发送目标，并在执行过程中获取进度。

**示例**：

- **动作服务器节点**：

```cpp
#include "ros/ros.h"
#include "actionlib/server/simple_action_server.h"
#include "my_package/MyAction.h" // 自定义动作消息

void execute(const my_package::MyGoalConstPtr &goal, actionlib::SimpleActionServer<my_package::MyAction> *as) {
    // 执行动作
    for (int i = 0; i < 10; ++i) {
        if (as->isPreemptRequested()) {
            as->setPreempted();
            return;
        }
        ros::Duration(1).sleep(); // 模拟工作
    }
    my_package::MyResult result;
    as->setSucceeded(result);
}

int main(int argc, char **argv) {
    ros::init(argc, argv, "action_server");
    ros::NodeHandle n;
    actionlib::SimpleActionServer<my_package::MyAction> server(n, "my_action", boost::bind(&execute, _1, &server), false);
    server.start();
    ros::spin();
    return 0;
}
```

- **动作客户端节点**：

```cpp
#include "ros/ros.h"
#include "actionlib/client/simple_action_client.h"
#include "my_package/MyAction.h" // 自定义动作消息

int main(int argc, char **argv) {
    ros::init(argc, argv, "action_client");
    actionlib::SimpleActionClient<my_package::MyAction> client("my_action", true);
    
    client.waitForServer(); // 等待服务器启动

    my_package::MyGoal goal; // 创建目标
    client.sendGoal(goal); // 发送目标
    client.waitForResult(); // 等待结果

    return 0;
}
```

### 总结

以上四种方式是 ROS 中节点之间进行通信的基本方法。每种方式都有其适用场景，选择合适的通信方式可以有效地实现节点间的数据交换和协作。你可以根据需要选择使用发布/订阅、服务/客户端、动态参数或动作库来实现不同的功能。

## Ros文件目录

Ros文件目录及介绍在 ROS（Robot Operating System）中，工作空间的标准格式通常遵循以下结构。一个典型的 ROS 工作空间的目录结构如下：

```
catkin_ws/
├── src/
│   ├── CMakeLists.txt
│   └── package_name/
│       ├── CMakeLists.txt
│       ├── package.xml
│       └── src/
│           ├── node1.cpp
│           └── node2.cpp
├── build/
├── devel/
└── install/
```

### 目录结构详解

1. **catkin_ws/**:
   - 这是你的工作空间的根目录。你可以使用任何你喜欢的名字，但通常使用 `catkin_ws`（工作空间的缩写）。

2. **src/**:
   - 源代码目录，所有的 ROS 包都应该放在这里。每个 ROS 包通常有自己的子目录，子目录名称与包名称相同。

3. **CMakeLists.txt**:
   - 在 `src/` 目录下的 `CMakeLists.txt` 文件是构建工作空间的配置文件。ROS 使用 CMake 构建系统，所以这个文件定义了如何编译和链接工作空间中的所有包。

4. **package_name/**:
   - 每个 ROS 包应该有自己的目录。目录名称通常与包名相同，包含包的所有源代码和其他文件。

5. **package.xml**:
   - 每个 ROS 包必须包含一个 `package.xml` 文件。这个文件包含包的元数据，如名称、版本、维护者和依赖项等。

6. **src/**:
   - 在包的目录下，通常有一个 `src/` 子目录，存放该包的源代码文件，如节点（node）的实现文件（例如 `.cpp` 或 `.py`）。

7. **build/**:
   - 这个目录在你运行 `catkin_make` 时自动生成，存储编译生成的文件。

8. **devel/**:
   - 这个目录也在你运行 `catkin_make` 时自动生成，存放开发时需要的文件，如环境配置文件。

9. **install/**:
   - 这个目录在你运行 `catkin_make install` 时生成，存放安装后的文件，通常包括所有编译的二进制文件和其他资源。

### 创建工作空间的命令

以下是创建标准 ROS 工作空间的基本步骤：

1. **创建工作空间目录**：
   ```bash
   mkdir -p ~/catkin_ws/src
   ```

2. **进入工作空间**：
   ```bash
   cd ~/catkin_ws/
   ```

3. **初始化工作空间**：
   ```bash
   catkin_make
   ```

4. **设置环境**：
   ```bash
   source devel/setup.bash
   ```

5. **可选：添加到 `.bashrc`**：
   如果希望每次打开终端时都自动加载环境变量，可以将以下命令添加到 `~/.bashrc` 中：
   ```bash
   echo "source ~/catkin_ws/devel/setup.bash" >> ~/.bashrc
   source ~/.bashrc
   ```

### 总结

工作空间是 ROS 开发的基础，了解其标准格式和目录结构可以帮助你更好地管理和组织你的 ROS 项目。

## Ros常用Shell指令

在 ROS（Robot Operating System）中，有一些常用的 Shell 命令，帮助你管理工作空间、包和节点。下面是一些常用的 ROS Shell 命令：

### 1. 工作空间相关命令

- **创建工作空间**:
  ```bash
  mkdir -p ~/catkin_ws/src
  cd ~/catkin_ws
  catkin_make
  ```

- **编译工作空间**:
  ```bash
  catkin_make
  ```

- **清理工作空间**:
  ```bash
  catkin_make clean
  ```

- **源工作空间**:
  ```bash
  source devel/setup.bash
  ```

### 2. 包相关命令

- **创建 ROS 包**:
  ```bash
  cd ~/catkin_ws/src
  catkin_create_pkg package_name std_msgs rospy roscpp
  ```

- **编译特定包**:
  ```bash
  cd ~/catkin_ws
  catkin_make --pkg package_name
  ```

- **列出工作空间中的所有包**:
  ```bash
  rospack list
  ```

- **检查包的依赖关系**:
  ```bash
  rospack depends package_name
  ```

### 3. 节点相关命令

- **运行节点**:
  ```bash
  rosrun package_name node_name
  ```

- **列出当前运行的节点**:
  ```bash
  rosnode list
  ```

- **检查节点状态**:
  ```bash
  rosnode info node_name
  ```

- **杀死节点**:
  ```bash
  rosnode kill node_name
  ```

### 4. 话题相关命令

- **列出所有话题**:
  ```bash
  rostopic list
  ```

- **查看话题的消息类型**:
  ```bash
  rostopic type topic_name
  ```

- **查看话题的消息内容**:
  ```bash
  rostopic echo topic_name
  ```

- **发布消息到话题**:
  ```bash
  rostopic pub /topic_name std_msgs/String "data: 'Hello, ROS!'"
  ```

### 5. 服务相关命令

- **列出所有服务**:
  ```bash
  rosservice list
  ```

- **查看服务的类型**:
  ```bash
  rosservice type service_name
  ```

- **调用服务**:
  ```bash
  rosservice call service_name "args"
  ```

### 6. TF 和时钟相关命令

- **查看 TF 变换**:
  ```bash
  rosrun tf view_frames
  ```

- **查看 TF 变换**:
  ```bash
  rosrun tf tf_monitor
  ```

### 7. 其他常用命令

- **启动 ROS Master**:
  ```bash
  roscore
  ```

- **查看 ROS 版本**:
  ```bash
  rosversion -d
  ```

- **查看环境变量**:
  ```bash
  printenv | grep ROS
  ```

## 头文件和命名空间相关

在 ROS 中，头文件的使用和命名空间是很重要的部分，特别是在 C++ 中。以下是一些常用的 ROS 头文件和如何使用命名空间的相关信息。

### 一、常用 ROS 头文件

1. **ros/ros.h**: 这是大多数 ROS C++ 程序的基础头文件，提供了与 ROS 通信的基本功能。
   ```cpp
   #include <ros/ros.h>
   ```

2. **std_msgs/String.h**: 这是标准消息类型之一，表示字符串消息。
   ```cpp
   #include <std_msgs/String.h>
   ```

3. **geometry_msgs/Twist.h**: 用于表示机器人速度（线速度和角速度）的消息类型。
   ```cpp
   #include <geometry_msgs/Twist.h>
   ```

4. **sensor_msgs/Image.h**: 用于处理图像数据的消息类型。
   ```cpp
   #include <sensor_msgs/Image.h>
   ```

5. **nav_msgs/Odometry.h**: 用于表示机器人里程计信息的消息类型。
   ```cpp
   #include <nav_msgs/Odometry.h>
   ```

6. **actionlib/server/simple_action_server.h**: 用于实现简单动作服务器。
   ```cpp
   #include <actionlib/server/simple_action_server.h>
   ```

7. **actionlib/client/simple_action_client.h**: 用于实现简单动作客户端。
   ```cpp
   #include <actionlib/client/simple_action_client.h>
   ```

### 二、使用命名空间

在 C++ 中，使用命名空间可以减少在代码中重复写 `ros::` 的需要。可以通过以下两种方式实现：

1. **使用 `using` 声明**：在文件顶部添加 `using` 声明，这样在整个文件中就可以直接使用类型，而不需要每次都加 `ros::`。

   ```cpp
   #include <ros/ros.h>
   using namespace ros;

   int main(int argc, char **argv) {
       init(argc, argv, "my_node");
       NodeHandle n;

       // 不需要加 ros:: 前缀
       Publisher pub = n.advertise<std_msgs::String>("topic", 1000);
       return 0;
   }
   ```

2. **局部使用**：在需要的地方使用 `using` 声明，而不影响整个文件的命名空间。

   ```cpp
   #include <ros/ros.h>
   #include <std_msgs/String.h>
   
   int main(int argc, char **argv) {
       ros::init(argc, argv, "my_node");
       ros::NodeHandle n;
   
       using std_msgs::String;
   
       Publisher pub = n.advertise<String>("topic", 1000);
       return 0;
   }
   ```

### 三、头文件的规律

1. **功能模块化**：ROS 的头文件通常按功能模块划分，例如消息、服务、动作等。相关的头文件会在特定的目录中，例如 `std_msgs`、`geometry_msgs`、`sensor_msgs` 等。

2. **命名约定**：头文件一般遵循 `package_name/MessageType.h` 的格式。例如，`std_msgs` 包中的消息类型 `String` 的头文件为 `std_msgs/String.h`。

3. **包含顺序**：通常，头文件的包含顺序是先包含 ROS 的基础头文件 (`ros/ros.h`)，再包含其他模块的头文件。这样可以确保所有基础功能都能正常工作。

4. **条件编译**：在某些情况下，使用条件编译来避免重复包含头文件，例如使用 `#ifndef` 和 `#define` 指令。

### 四、总结

- 常用的 ROS 头文件为你的程序提供了消息、服务和其他功能的支持。
- 可以通过 `using namespace` 或 `using` 声明减少代码中的 `ros::` 前缀。
- 头文件的组织和命名遵循特定的规则，通常按功能模块划分，确保易于查找和使用。

如果你还有其他具体的问题或者想要进一步了解的内容，请随时问我！

## Ros基础头文件常用函数

在 ROS 的 C++ 编程中，`ros/ros.h` 是最常用的基础头文件之一，它提供了与 ROS 通信的基本功能。以下是一些常用的函数及其功能：

### 1. ros::init()

- **用途**: 初始化 ROS 节点。
- **用法**:
  ```cpp
  ros::init(argc, argv, "node_name");
  ```
- **参数**:
  - `argc`: 命令行参数的数量。
  - `argv`: 命令行参数的数组。
  - `"node_name"`: 节点的名称。

### 2. ros::NodeHandle

- **用途**: 提供与 ROS 的主节点的接口，用于发布、订阅和服务的管理。
- **用法**:
  ```cpp
  ros::NodeHandle nh;
  ```

### 3. ros::Publisher

- **用途**: 创建一个发布者对象，用于向特定主题发布消息。
- **用法**:
  ```cpp
  ros::Publisher pub = nh.advertise<message_type>("topic_name", queue_size);
  ```
- **参数**:
  - `message_type`: 消息类型，例如 `std_msgs::String`。
  - `"topic_name"`: 发布的主题名称。
  - `queue_size`: 发布消息的队列大小。

### 4. ros::Subscriber

- **用途**: 创建一个订阅者对象，用于接收特定主题的消息。
- **用法**:
  ```cpp
  ros::Subscriber sub = nh.subscribe("topic_name", queue_size, callback_function);
  ```
- **参数**:
  - `"topic_name"`: 订阅的主题名称。
  - `queue_size`: 订阅消息的队列大小。
  - `callback_function`: 当接收到消息时调用的回调函数。

### 5. ros::spin()

- **用途**: 进入循环，等待回调函数被调用。
- **用法**:
  
  ```cpp
  ros::spin();
  ```
- **说明**: 该函数会阻塞当前线程，直到节点被关闭，通常在 `main` 函数的最后调用。

### 6. ros::Rate

- **用途**: 控制循环频率。
- **用法**:
  ```cpp
  ros::Rate loop_rate(frequency);
  while (ros::ok()) {
      // 执行代码
      loop_rate.sleep();  // 睡眠以保持频率
  }
  ```
- **参数**:
  - `frequency`: 循环的频率（以 Hz 为单位）。

### 7. ros::ok()

- **用途**: 检查 ROS 节点是否仍然在运行。
- **用法**:
  
  ```cpp
  if (ros::ok()) {
      // 执行代码
  }
  ```
- **说明**: 返回 `true` 表示节点仍在运行，`false` 表示节点已经关闭或出现错误。

### 8. ros::Time

- **用途**: 获取 ROS 时间（包括系统时间和模拟时间）。
- **用法**:
  
  ```cpp
  ros::Time current_time = ros::Time::now();
  ```
- **说明**: `ros::Time::now()` 返回当前的时间戳。

### 9. ros::Duration

- **用途**: 表示持续时间，通常用于时间计算和延时。
- **用法**:
  
  ```cpp
  ros::Duration d(2.0);  // 持续 2 秒
  d.sleep();  // 暂停 2 秒
  ```

### 10. ros::param

- **用途**: 用于访问参数服务器中的参数。
- **用法**:
  ```cpp
  nh.getParam("param_name", variable);  // 获取参数
  nh.setParam("param_name", value);      // 设置参数
  ```
- **示例**:
  ```cpp
  int param_value;
  nh.getParam("my_param", param_value);
  nh.setParam("my_param", 10);
  ```

### 总结

这些函数和类提供了 ROS C++ 编程的基础功能，可以帮助你实现节点的基本通信、参数管理、时间处理等功能。理解这些函数的用法对于编写高效的 ROS 节点至关重要。如果你对某个特定函数或使用方式有更多问题，请随时问我！

## Main函数的参数介绍

在 ROS 中，`main` 函数的参数通常用于初始化和设置节点的环境。具体来说，`argc` 和 `argv` 参数具有以下意义：

### 参数解析

1. **`int argc`**:
   - `argc` 是 "argument count" 的缩写，它表示命令行参数的数量。在 C/C++ 中，当你从命令行启动程序时，可以传递多个参数，这个参数就表示这些参数的个数。包括程序名称本身在内，`argc` 的值至少为 1。

2. **`char **argv`**:
   - `argv` 是 "argument vector" 的缩写，它是一个指向字符指针数组的指针。每个元素都是一个 C 风格的字符串（以 null 结尾），表示从命令行传递给程序的参数。
   - 例如，假设你在命令行中运行程序时输入了 `./talker arg1 arg2`，那么 `argc` 的值将是 3，而 `argv` 将是一个数组：
     - `argv[0]` 指向 `"./talker"`
     - `argv[1]` 指向 `"arg1"`
     - `argv[2]` 指向 `"arg2"`

### 在 ROS 中的用途

在 ROS 中，使用这两个参数的主要目的是为了将命令行参数传递给 `ros::init()` 函数，从而允许 ROS 初始化过程使用这些参数。这样做的好处包括：

- **参数传递**: 你可以通过命令行传递参数，这样节点可以根据这些参数来调整其行为。
- **支持多节点**: `ros::init()` 函数使用 `argc` 和 `argv` 参数来确保每个节点都能正确初始化，避免命名冲突。

### 示例代码

下面是一个简单的 ROS 节点示例，展示了如何使用这些参数：

```cpp
#include <ros/ros.h>
#include <std_msgs/String.h>

int main(int argc, char **argv) {
    // 初始化 ROS 节点，"talker" 是节点的名称
    ros::init(argc, argv, "talker");

    // 创建 NodeHandle 对象
    ros::NodeHandle n;

    // 创建发布者，发布到 "chatter" 主题
    ros::Publisher pub = n.advertise<std_msgs::String>("chatter", 1000);

    // 创建消息
    std_msgs::String msg;
    msg.data = "Hello, ROS!";

    // 设置循环频率
    ros::Rate loop_rate(10);  // 10 Hz

    while (ros::ok()) {
        // 发布消息
        pub.publish(msg);
        ROS_INFO("Published: [%s]", msg.data.c_str());

        // 处理回调函数
        ros::spinOnce();  // 处理任何传入的消息
        loop_rate.sleep();  // 睡眠以保持频率
    }

    return 0;
}
```

### 总结

- `argc` 和 `argv` 是标准 C/C++ 的参数，用于获取命令行参数的数量和内容。
- 在 ROS 中，它们被用于初始化节点，并使节点能够使用命令行参数进行配置。
- `ros::init(argc, argv, "node_name")` 这一行代码确保节点的正确初始化。

如果你有更多问题或需要进一步的解释，请告诉我！

## NodeHandle类

`ros::NodeHandle` 是 ROS C++ 中用于与 ROS 系统进行交互的核心类之一，它提供了一系列功能，用于管理节点的发布、订阅、服务和参数等。以下是 `NodeHandle` 的一些常用函数及其用途：

### 1. 发布与订阅

- **`advertise()`**
  - **用途**: 创建一个发布者，用于发布特定类型的消息。
  - **用法**:
    ```cpp
    ros::Publisher pub = nh.advertise<message_type>("topic_name", queue_size);
    ```

- **`subscribe()`**
  - **用途**: 创建一个订阅者，用于接收特定类型的消息。
  - **用法**:
    ```cpp
    ros::Subscriber sub = nh.subscribe("topic_name", queue_size, callback_function);
    ```

### 2. 服务

- **`advertiseService()`**
  - **用途**: 注册一个服务，以便其他节点可以请求该服务。
  - **用法**:
    ```cpp
    ros::ServiceServer service = nh.advertiseService("service_name", service_callback);
    ```

- **`serviceClient()`**
  - **用途**: 创建一个服务客户端，用于请求特定服务。
  - **用法**:
    ```cpp
    ros::ServiceClient client = nh.serviceClient<service_type>("service_name");
    ```

### 3. 参数

- **`getParam()`**
  - **用途**: 从参数服务器获取参数值。
  - **用法**:
    ```cpp
    nh.getParam("param_name", param_value);
    ```

- **`setParam()`**
  - **用途**: 向参数服务器设置参数值。
  - **用法**:
    ```cpp
    nh.setParam("param_name", param_value);
    ```

- **`hasParam()`**
  - **用途**: 检查参数服务器上是否存在指定参数。
  - **用法**:
    ```cpp
    bool exists = nh.hasParam("param_name");
    ```

### 4. 其他功能

- **`resolveName()`**
  - **用途**: 解析节点名，将相对名称转换为绝对名称。
  - **用法**:
    ```cpp
    std::string absolute_name = nh.resolveName("relative_name");
    ```

- **`getNamespace()`**
  - **用途**: 获取当前节点的命名空间。
  - **用法**:
    ```cpp
    std::string ns = nh.getNamespace();
    ```

- **`getLoggers()`**
  - **用途**: 获取日志相关的信息。
  - **用法**:
    ```cpp
    std::vector<ros::Logger> loggers = nh.getLoggers();
    ```

### 示例

以下是一个使用 `ros::NodeHandle` 的示例代码，展示了如何创建发布者、订阅者和服务：

```cpp
#include <ros/ros.h>
#include <std_msgs/String.h>
#include <std_srvs/SetBool.h>

// 服务回调函数
bool serviceCallback(std_srvs::SetBool::Request &req, std_srvs::SetBool::Response &res) {
    res.success = true;
    res.message = "Service called!";
    return true;
}

// 消息回调函数
void messageCallback(const std_msgs::String::ConstPtr &msg) {
    ROS_INFO("Received: [%s]", msg->data.c_str());
}

int main(int argc, char **argv) {
    ros::init(argc, argv, "example_node");
    ros::NodeHandle nh;

    // 创建发布者
    ros::Publisher pub = nh.advertise<std_msgs::String>("example_topic", 1000);

    // 创建订阅者
    ros::Subscriber sub = nh.subscribe("example_topic", 1000, messageCallback);

    // 创建服务
    ros::ServiceServer service = nh.advertiseService("example_service", serviceCallback);

    ros::Rate loop_rate(10);  // 控制循环频率

    while (ros::ok()) {
        // 发布消息
        std_msgs::String msg;
        msg.data = "Hello, ROS!";
        pub.publish(msg);

        ros::spinOnce();  // 处理回调
        loop_rate.sleep();  // 睡眠以保持频率
    }

    return 0;
}
```

### 总结

`ros::NodeHandle` 提供了丰富的功能，方便我们在 ROS 中进行节点间的通信、参数管理和服务调用。你可以根据需要选择合适的函数进行使用。如果你还有其他问题，或想了解更详细的内容，请随时问我！

## stringstream

`std::stringstream` 是 C++ 标准库中的一个类，属于 `<sstream>` 头文件，它提供了一种方便的方法来构造和操作字符串。你可以使用 `std::stringstream` 作为输入输出流来处理字符串，就像使用标准输入输出流（如 `std::cin` 和 `std::cout`）一样。

### 主要用途

1. **字符串拼接**：可以将多个字符串或数据拼接成一个字符串。
2. **类型转换**：可以将数字转换为字符串，或将字符串解析为数字。

### 基本用法

以下是一些 `std::stringstream` 的基本用法示例：

#### 1. 创建和使用 `std::stringstream`

```cpp
#include <iostream>
#include <sstream>  // 需要包含此头文件
#include <string>

int main() {
    // 创建一个 stringstream 对象
    std::stringstream ss;

    // 向 stringstream 中写入数据
    ss << "Hello, " << "ROS! " << 2024;

    // 从 stringstream 中读取数据
    std::string result = ss.str();  // 获取最终拼接的字符串

    std::cout << result << std::endl;  // 输出: Hello, ROS! 2024

    return 0;
}
```

#### 2. 类型转换

`std::stringstream` 也可以用于将字符串转换为数字，以及将数字转换为字符串。

```cpp
#include <iostream>
#include <sstream>
#include <string>

int main() {
    std::string number_str = "42";
    int number;

    // 将字符串转换为整数
    std::stringstream(number_str) >> number;
    std::cout << "The number is: " << number << std::endl;  // 输出: The number is: 42

    // 将整数转换为字符串
    std::stringstream ss;
    ss << number;
    std::string converted_str = ss.str();
    std::cout << "The converted string is: " << converted_str << std::endl;  // 输出: The converted string is: 42

    return 0;
}
```

### 3. 清空 `std::stringstream`

如果你需要重用 `std::stringstream` 对象，可以使用 `str()` 函数来清空内容。

```cpp
#include <iostream>
#include <sstream>
#include <string>

int main() {
    std::stringstream ss;
    ss << "First message";
    
    std::cout << ss.str() << std::endl;  // 输出: First message

    // 清空内容
    ss.str("");  // 或者使用 ss.clear(); 先清空再重用

    ss << "Second message";
    std::cout << ss.str() << std::endl;  // 输出: Second message

    return 0;
}
```

### 总结

- `std::stringstream` 是一个强大的工具，用于字符串的拼接和类型转换。
- 可以使用 `<<` 操作符向 `std::stringstream` 写入数据，使用 `str()` 函数获取最终的字符串。
- 通过 `>>` 操作符可以方便地将字符串转换为其他数据类型。

如果你对 `std::stringstream` 或其他 C++ 功能有任何疑问，请随时问我！

## 配置节点的配置文件

要在 ROS 中配置一个新的包并确保它可以正常编译和运行，你需要关注两个主要文件：`CMakeLists.txt` 和 `package.xml`。以下是这两个文件的详细配置步骤以及关键要素。

### 1. `CMakeLists.txt`

`CMakeLists.txt` 是一个构建系统文件，用于定义包的构建信息。以下是你需要配置的内容：

```cmake
cmake_minimum_required(VERSION 3.0.2)
project(talker)  # 替换为你的包名

# 找到catkin的相关组件
find_package(catkin REQUIRED COMPONENTS
  roscpp        # ROS C++客户端库
  std_msgs      # 标准消息类型
)

# Declare a catkin package
catkin_package(
#  INCLUDE_DIRS include
#  LIBRARIES talker
#  CATKIN_DEPENDS roscpp std_msgs
#  DEPENDS system_lib
)

# 包含目录
include_directories(
  ${catkin_INCLUDE_DIRS}  # 包含catkin库的目录
)

# 添加可执行文件
add_executable(talker src/talker.cpp)  # 替换为你的源文件路径
# 链接库
target_link_libraries(talker ${catkin_LIBRARIES})  # 链接到catkin库
```

#### 关键部分解释：
- `find_package(catkin REQUIRED COMPONENTS ...)`: 查找 ROS 的核心库和其他必要的组件。
- `catkin_package(...)`: 声明这是一个 catkin 包。
- `include_directories(...)`: 包含需要的头文件目录。
- `add_executable(...)`: 创建一个可执行文件，指定源文件。
- `target_link_libraries(...)`: 链接到 catkin 库，确保可执行文件能够使用 ROS 提供的功能。

### 2. `package.xml`

`package.xml` 用于描述包的元数据，包括名称、版本、依赖关系等。以下是示例配置：

```xml
<package format="2">
  <name>talker</name>  <!-- 包名 -->
  <version>0.0.0</version>
  <description>The talker ROS package</description>

  <maintainer email="your_email@example.com">Your Name</maintainer>  <!-- 维护者信息 -->

  <license>BSD</license>  <!-- 许可证 -->

  <buildtool_depend>catkin</buildtool_depend>  <!-- 构建工具依赖 -->

  <depend>roscpp</depend>  <!-- 运行时依赖 -->
  <depend>std_msgs</depend>  <!-- 运行时依赖 -->

  <build_depend>roscpp</build_depend>  <!-- 构建时依赖 -->
  <build_depend>std_msgs</build_depend>  <!-- 构建时依赖 -->

  <export>
  </export>
</package>
```

#### 关键部分解释：
- `<name>`: 包的名称。
- `<version>`: 包的版本。
- `<description>`: 对包的简要描述。
- `<maintainer>`: 维护者信息。
- `<license>`: 包的许可证。
- `<depend>`: 运行时依赖。
- `<build_depend>`: 构建时依赖。

### 3. 创建包后步骤

1. **创建包**:
   使用以下命令创建新包（确保在工作空间的 `src` 目录下）：
   ```bash
   catkin_create_pkg talker roscpp std_msgs
   ```

2. **创建源文件**:
   在 `talker/src` 目录中创建 `talker.cpp` 文件，并将你的 ROS 节点代码放入其中。

3. **编译包**:
   在工作空间的根目录下（通常是 `catkin_ws`）运行以下命令编译包：
   ```bash
   catkin_make
   ```

4. **运行节点**:
   确保 ROS 核心已启动，然后在另一个终端窗口中运行：
   ```bash
   rosrun talker talker
   ```

5. **查看发布的消息**:
   使用 `rostopic` 查看你发布的消息：
   ```bash
   rostopic echo chatter
   ```

### 4. 注意事项

- **路径**: 确保 `talker.cpp` 文件的路径与 `CMakeLists.txt` 中的路径相匹配。
- **依赖**: 确保所有必要的依赖都在 `package.xml` 和 `CMakeLists.txt` 中声明。
- **编译**: 每次修改 `CMakeLists.txt` 或添加新文件后，都需要重新运行 `catkin_make`。

### 总结

确保在 `CMakeLists.txt` 和 `package.xml` 中正确配置依赖项、源文件路径以及其他信息，能够帮助你的 ROS 包正常工作。如果你有其他问题或需要更具体的帮助，请随时问我！

## 配置文件样例

你提供的 `CMakeLists.txt` 文件配置了一些关键内容，确保你的 ROS 包可以正确构建和链接。下面是对每个部分的详细解释：

### 1. `cmake_minimum_required(VERSION 3.0.2)`

- **用途**: 指定 CMake 所需的最低版本。
- **解释**: 这行代码要求 CMake 的版本至少为 3.0.2，以便支持此项目中的功能和特性。

### 2. `project(topic)`

- **用途**: 定义项目的名称。
- **解释**: 这里项目的名称为 `topic`，这个名称可以在编译时和安装时使用。

### 3. `find_package(catkin REQUIRED roscpp std_msgs)`

- **用途**: 查找和配置 ROS 的依赖库。
- **解释**: 
  - `catkin`: 这是 ROS 的构建系统。
  - `REQUIRED`: 表示这些依赖是必需的，如果没有找到，会导致配置失败。
  - `roscpp` 和 `std_msgs`: 指定你的项目依赖于这两个 ROS 包，其中 `roscpp` 是用于 C++ 的 ROS 客户端库，`std_msgs` 是用于标准消息类型的包。

### 4. `include_directories(include ${catkin_INCLUDE_DIRS})`

- **用途**: 指定头文件的搜索路径。
- **解释**: 
  - `include`: 这是你自己的头文件目录，通常你会在这里放置自己的头文件。
  - `${catkin_INCLUDE_DIRS}`: 这是 ROS 包的头文件目录，包含了在 `find_package` 中找到的所有包的头文件路径。

### 5. `add_executable(talker src/talker.cpp)`

- **用途**: 定义可执行文件。
- **解释**: 
  - `talker`: 这是生成的可执行文件的名称。
  - `src/talker.cpp`: 这是包含 `talker` 节点代码的源文件的路径。

### 6. `target_link_libraries(talker ${catkin_LIBRARIES})`

- **用途**: 链接库到可执行文件。
- **解释**: 
  - `talker`: 之前定义的可执行文件名称。
  - `${catkin_LIBRARIES}`: 链接到 ROS 中找到的库，确保 `talker` 可执行文件能够访问 ROS 的功能和标准消息类型。

### 总结

总体来说，这个 `CMakeLists.txt` 配置了项目的基本信息、必要的依赖、头文件搜索路径、可执行文件的定义以及库的链接。这些都是构建 ROS 包的基础，确保你的程序可以正常编译和运行。

如果你还有其他问题，或者需要进一步的帮助，请随时告诉我！

```tex
cmake_minimum_required(VERSION 3.0.2)
project(topic)

find_package(catkin REQUIRED COMPONENTS
  roscpp
  std_msgs
)

catkin_package()

include_directories(
  include
  ${catkin_INCLUDE_DIRS}
)

add_executable(talker src/talker.cpp)

target_link_libraries(talker ${catkin_LIBRARIES})

# 安装 talker 节点到适当的目录
install(TARGETS talker
  RUNTIME DESTINATION ${CATKIN_PACKAGE_BIN_DESTINATION}
)
```

## Launch配置文件

在使用 `roslaunch` 启动 ROS 节点时，你可以在 `.launch` 文件中定义多个参数和设置。这些参数通常包括节点的包名、节点的类型、节点的名称以及一些特定于节点的参数。以下是一些常见的参数和设置：

### 1. 节点参数
每个节点都需要指定以下参数：

- **pkg**：指定节点所在的包的名称。
- **type**：节点的可执行文件名称（通常是编译后的可执行文件）。
- **name**：节点的名称，通常用于识别节点。

```xml
<node pkg="package_name" type="executable_name" name="node_name" />
```

### 2. 输出类型
你可以指定节点的输出方式：

- **output**：可以设置为 `screen`（输出到终端）或 `log`（输出到日志文件）。

```xml
<node pkg="package_name" type="executable_name" name="node_name" output="screen" />
```

### 3. 传递参数
可以在 `.launch` 文件中传递参数给节点：

```xml
<param name="param_name" value="param_value" />
```

例如：

```xml
<node pkg="my_package" type="my_node" name="my_node_name">
    <param name="robot_speed" value="1.0" />
</node>
```

### 4. 组和命名空间
你可以将节点放入命名空间中：

```xml
<group ns="my_namespace">
    <node pkg="package_name" type="executable_name" name="node_name" />
</group>
```

### 5. 依赖性
可以指定在启动节点之前需要启动的节点：

```xml
<node pkg="package_name" type="executable_name" name="node_name" required="true" />
```

### 6. 例子
以下是一个完整的 `.launch` 文件示例，展示了如何使用这些参数：

```xml
<launch>
    <param name="robot_speed" value="1.0" />

    <node pkg="my_package" type="my_node" name="my_node_name" output="screen">
        <param name="robot_mode" value="automatic" />
    </node>

    <group ns="sensor_group">
        <node pkg="sensor_package" type="sensor_node" name="sensor1" />
        <node pkg="sensor_package" type="sensor_node" name="sensor2" />
    </group>
</launch>
```

### 7. 启动命令
启动 `.launch` 文件时，命令格式如下：

```bash
roslaunch package_name launch_file_name.launch
```

确保你已经通过 `catkin_make` 构建了相关的包，并且运行 `source devel/setup.bash` 设置了环境。

### 小结
通过这些参数和设置，你可以灵活地控制 ROS 节点的启动和行为。根据你的应用需求，可以在 `.launch` 文件中自由组合这些参数。如果你有特定的参数需求或使用场景，欢迎随时问我！

## 发送自定义消息

在 ROS 中，发送自定义消息需要几个步骤。以下是一个完整的流程，涵盖创建自定义消息类型、配置包、编写节点以发送自定义消息的步骤。

### 步骤 1：创建自定义消息

1. **创建消息目录**：
   在你的 ROS 包中，创建一个名为 `msg` 的目录。如果你的包名是 `my_package`，目录结构应该如下：
   ```
   my_package/
   ├── msg/
   │   └── MyCustomMessage.msg
   ├── src/
   │   └── my_node.cpp
   ├── CMakeLists.txt
   └── package.xml
   ```

2. **定义消息内容**：
   在 `MyCustomMessage.msg` 文件中定义你的消息格式。举个例子：
   
   ```plaintext
   string text
   int32 number
   ```

### 步骤 2：修改 `CMakeLists.txt`

在 `CMakeLists.txt` 文件中，添加以下内容以生成消息：

1. **查找依赖**：
   找到 `find_package(catkin REQUIRED COMPONENTS ...)` 行，并确保包含 `message_generation`：
   ```cmake
   find_package(catkin REQUIRED COMPONENTS
     roscpp
     std_msgs
     message_generation
   )
   ```

2. **添加消息**：
   在 `add_message_files` 部分添加你的消息文件：
   ```cmake
   add_message_files(
     FILES
     MyCustomMessage.msg
   )
   ```

3. **生成消息**：
   在 `generate_messages` 部分添加：
   ```cmake
   generate_messages(DEPENDENCIES std_msgs)
   ```

4. **确保在 `catkin_package` 中包含消息生成的库**：
   ```cmake
   catkin_package(
     CATKIN_DEPENDS message_runtime std_msgs
   )
   ```

### 步骤 3：修改 `package.xml`

确保你的 `package.xml` 文件中包含了以下依赖：
```xml
<build_depend>message_generation</build_depend>
<exec_depend>message_runtime</exec_depend>
```

### 步骤 4：构建包

在你的 ROS 工作区中，运行以下命令以构建包：
```bash
cd ~/catkin_ws
catkin_make
```

### 步骤 5：编写发送节点

在你的 C++ 源文件（例如 `my_node.cpp`）中，添加以下代码以发送自定义消息：

```cpp
#include "ros/ros.h"
#include "my_package/MyCustomMessage.h" // 替换为你的包名和消息名

int main(int argc, char **argv) {
    ros::init(argc, argv, "custom_message_talker");
    ros::NodeHandle n;

    ros::Publisher custom_pub = n.advertise<my_package::MyCustomMessage>("custom_topic", 1000);
    
    ros::Rate loop_rate(10);

    while (ros::ok()) {
        my_package::MyCustomMessage msg;
        msg.text = "Hello, this is a custom message!";
        msg.number = 42;

        ROS_INFO("Sending: %s, %d", msg.text.c_str(), msg.number);
        custom_pub.publish(msg);

        ros::spinOnce();
        loop_rate.sleep();
    }

    return 0;
}
```

### 步骤 6：运行节点

在终端中，启动 ROS 核心：
```bash
roscore
```

然后在另一个终端中运行你的节点：
```bash
rosrun my_package my_node
```

### 步骤 7：查看消息

你可以使用 `rostopic` 查看发送的消息：
```bash
rostopic echo /custom_topic
```

### 总结

以上就是在 ROS 中发送自定义消息的完整过程。你可以根据需求调整消息的内容和格式。如果你还有其他问题或需要进一步的帮助，请随时告诉我！

## 消息C++代码的实现

在 ROS 中添加自定义消息需要几个步骤。下面是详细的步骤，帮助你在 ROS 中创建和使用自定义消息：

### 1. 创建消息文件

首先，你需要在你的 ROS 包中创建一个 `msg` 目录，用于存放自定义消息文件。

假设你的包名是 `topic`，请执行以下操作：

```bash
cd ~/catkin_Learn/src/topic
mkdir msg
```

在 `msg` 目录中，创建一个消息文件，例如 `MyMessage.msg`：

```plaintext
# MyMessage.msg
string text
int32 number
```

### 2. 修改 CMakeLists.txt 文件

在 `CMakeLists.txt` 文件中，你需要添加对自定义消息的支持。打开 `CMakeLists.txt` 文件并进行以下修改：

1. 添加 `message_generation` 依赖：

   ```cmake
   find_package(catkin REQUIRED COMPONENTS
     std_msgs
     message_generation  # 添加这一行
   )
   ```

2. 在 `catkin_package` 中添加 `message_runtime`：

   ```cmake
   catkin_package(
     CATKIN_DEPENDS std_msgs message_runtime  # 添加 message_runtime
   )
   ```

3. 添加消息文件：

   ```cmake
   add_message_files(
     FILES
     MyMessage.msg  # 添加你的消息文件名
   )
   ```

4. 在文件底部，添加生成消息的指令：

   ```cmake
   generate_messages(
     DEPENDENCIES
     std_msgs  # 添加依赖的消息
   )
   ```

### 3. 修改 package.xml 文件

确保 `package.xml` 中有 `message_generation` 和 `message_runtime` 的依赖项：

```xml
<build_depend>message_generation</build_depend>
<exec_depend>message_runtime</exec_depend>
```

### 4. 编译包

返回到工作空间的根目录，运行以下命令以编译你的包：

```bash
cd ~/catkin_Learn
catkin_make
```

### 5. 使用自定义消息

编译完成后，你就可以在你的节点中使用自定义消息了。下面是一个使用自定义消息的示例代码：

#### 发布自定义消息

```cpp
#include "ros/ros.h"
#include "topic/MyMessage.h"  // 包含自定义消息头文件

int main(int argc, char **argv) {
    ros::init(argc, argv, "my_message_publisher");
    ros::NodeHandle n;
    ros::Publisher pub = n.advertise<topic::MyMessage>("my_topic", 1000);
    
    ros::Rate loop_rate(10);
    while (ros::ok()) {
        topic::MyMessage msg;  // 创建自定义消息实例
        msg.text = "Hello, ROS!";
        msg.number = 42;
        
        pub.publish(msg);  // 发布消息
        ros::spinOnce();
        loop_rate.sleep();
    }
    return 0;
}
```

#### 订阅自定义消息

```cpp
#include "ros/ros.h"
#include "topic/MyMessage.h"  // 包含自定义消息头文件

void messageCallback(const topic::MyMessage::ConstPtr& msg) {
    ROS_INFO("Received: %s, %d", msg->text.c_str(), msg->number);
}

int main(int argc, char **argv) {
    ros::init(argc, argv, "my_message_subscriber");
    ros::NodeHandle n;
    ros::Subscriber sub = n.subscribe("my_topic", 1000, messageCallback);
    
    ros::spin();
    return 0;
}
```



## 服务C++代码的实现

在 ROS 中，服务（Service）是一种请求-响应的通信机制，可以用来在节点之间进行同步通信。以下是如何在 C++ 中创建和使用 ROS 服务的详细步骤。

### 步骤 1：创建服务消息类型

1. **定义服务消息**：
   首先，创建一个名为 `srv` 的目录，并定义服务消息文件。假设你的包名是 `my_package`，你可以在 `srv` 目录中创建一个服务文件，例如 `AddTwoInts.srv`。

   ```plaintext
   my_package/
   ├── srv/
   │   └── AddTwoInts.srv
   ├── src/
   │   └── my_service_node.cpp
   ├── CMakeLists.txt
   └── package.xml
   ```

2. **内容**：
   在 `AddTwoInts.srv` 文件中定义请求和响应的格式：
   ```plaintext
   int64 a
   int64 b
   ---
   int64 sum
   ```

   这里 `---` 分隔请求和响应部分。

### 步骤 2：修改 `CMakeLists.txt`

在 `CMakeLists.txt` 文件中，添加以下内容以生成服务消息：

1. **查找依赖**：
   确保 `find_package` 行中包含 `message_generation`：
   ```cmake
   find_package(catkin REQUIRED COMPONENTS
     roscpp
     std_msgs
     message_generation
   )
   ```

2. **添加服务文件**：
   在 `add_service_files` 部分添加你的服务文件：
   ```cmake
   add_service_files(
     FILES
     AddTwoInts.srv
   )
   ```

3. **生成消息**：
   在 `generate_messages` 部分添加：
   ```cmake
   generate_messages(DEPENDENCIES std_msgs)
   ```

4. **确保在 `catkin_package` 中包含消息生成的库**：
   ```cmake
   catkin_package(
     CATKIN_DEPENDS message_runtime std_msgs
   )
   ```

### 步骤 3：修改 `package.xml`

确保你的 `package.xml` 文件中包含以下依赖：
```xml
<build_depend>message_generation</build_depend>
<exec_depend>message_runtime</exec_depend>
```

### 步骤 4：构建包

在你的 ROS 工作区中，运行以下命令以构建包：
```bash
cd ~/catkin_ws
catkin_make
```

### 步骤 5：编写服务节点

在你的 C++ 源文件（例如 `my_service_node.cpp`）中，添加以下代码以实现服务端逻辑：

```cpp
#include "ros/ros.h"
#include "my_package/AddTwoInts.srv" // 替换为你的包名和服务名

// 服务回调函数
bool add(my_package::AddTwoInts::Request &req, my_package::AddTwoInts::Response &res) {
    res.sum = req.a + req.b; // 计算和
    ROS_INFO("Request: a=%ld, b=%ld", req.a, req.b);
    ROS_INFO("Sending back response: [%ld]", res.sum);
    return true; // 服务成功
}
int main(int argc, char **argv) {
    ros::init(argc, argv, "add_two_ints_server"); // 初始化节点
    ros::NodeHandle n;

    // 创建服务
    ros::ServiceServer service = n.advertiseService("add_two_ints", add);

    ROS_INFO("Ready to add two ints.");
    ros::spin(); // 进入循环等待请求
    return 0;
}
```

### 步骤 6：编写客户端节点

创建另一个源文件（例如 `my_client_node.cpp`），添加以下代码以实现客户端逻辑：

```cpp
#include "ros/ros.h"
#include "my_package/AddTwoInts.srv" // 替换为你的包名和服务名

int main(int argc, char **argv) {
    ros::init(argc, argv, "add_two_ints_client"); // 初始化节点
    ros::NodeHandle n;

    ros::ServiceClient client = n.serviceClient<my_package::AddTwoInts>("add_two_ints"); // 创建客户端

    my_package::AddTwoInts srv; // 创建服务请求

    srv.request.a = 5; // 设置请求参数
    srv.request.b = 3;

    if (client.call(srv)) { // 发送请求
        ROS_INFO("Sum: %ld", srv.response.sum); // 显示响应结果
    } else {
        ROS_ERROR("Failed to call service add_two_ints");
    }
    return 0;
}
```

### 步骤 7：运行节点

在终端中，启动 ROS 核心：
```bash
roscore
```

然后在另一个终端中运行服务端节点：
```bash
rosrun my_package my_service_node
```

在第三个终端中运行客户端节点：
```bash
rosrun my_package my_client_node
```

### 

