### 前言

Caddy是一个使用go实现的高并发web服务器，相对于Nginx，配置简单，更加易用。

### 教程

1. 新建并进入caddy文件夹

   ```bash
   sudo mkdir /opt/caddy
   cd /opt/caddy 
   ```
2. 创建docker-compose.yml文件

   ```bash
   sudo nano docker-compose.yml
   ```

   写入

   ```yml
   services:
     caddy:
       image: caddy:latest
       restart: unless-stopped
       # 端口映射：宿主机端口:容器端口
       ports:
         - "80:80"
         - "443:443"
       volumes:
         - ./Caddyfile:/etc/caddy/Caddyfile
         - ./site:/srv
         - ./data:/data
         - ./config:/config
   ```
3. 创建并配置Caddyfile文件

   ```bash
   sudo nano Caddyfile
   ```

   ```toml
   :80 {
       root * /srv/test

       # 开启自动压缩（支持 zstd 和 gzip）
       encode zstd gzip

       file_server {
           # 开启此项后，Caddy 会优先寻找目录下存在的 .br 或 .gz 文件发送
           precompressed gzip
       }

       try_files {path} /index.html
   }
   ```
4. 托管一个静态web

   ```bash
   sudo mkdir site/test
   ```

   将index.html放在这里
5. 启动docker

   ```bash
   sudo docker compose up -D
   ```

### 结语

通过以上操作，便可以使用caddy完成各种web服务了

### 参考配置

#### 1. 基础反向代理 (Reverse Proxy)

这是 Caddy 最出圈的功能：**自动申请 SSL 证书**并转发流量。

```toml
example.com {
    # 将流量转发到后端的 Node.js/Python/Go 服务
    reverse_proxy localhost:3000
}
```

#### 2. 静态网站 + 强缓存 + 安全头

如果你在部署 Vue/React 项目，除了你写的 `try_files`，通常还会加上缓存控制和安全防护。

www.example.com {
    root * /var/www/my-app
    encode zstd gzip
    file_server
    
    # 路由重定向到 index.html (SPA 必备)
    try_files {path} /index.html

    # 设置安全响应头
    header {
        # 开启 HSTS
        Strict-Transport-Security "max-age=31536000;"
        # 防止被 iframe 嵌套
        X-Frame-Options "DENY"
        # 简单的缓存策略：静态资源缓存 1 年
        @static {
            file
            path *.ico *.css *.js *.gif *.jpg *.jpeg *.png *.svg *.woff
        }
        header @static Cache-Control max-age=31536000
    }
}
