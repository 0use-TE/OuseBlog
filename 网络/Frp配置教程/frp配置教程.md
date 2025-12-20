先从GitHub（https://github.com/fatedier/frp）或者其它网站下载frp客户端或者服务端 然后服务端运行在服务器，配置好后，客户端先配置frpc.toml

```toml
serverAddr = "192.168.0.1" #服务器Ip
serverPort = 80 #服务器Ip
auth.method = "token" #验证方法，一般是token
auth.token = "www" #验证密钥

[[proxies]]
name = "rdp" #服务名
type = "tcp" #服务类型 tcp/udp/http 对于https需要配置证书
localIP = "127.0.0.1" #本机ip
localPort = 3389 #要穿透的端口
remotePort = 20001  #远程服务器使用哪个端口代理自己
```

