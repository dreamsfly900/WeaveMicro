# WeaveMicro

#### 介绍
Weave微服务架构
主要目的，尽量简化和减少开发复杂度和难度，尽量双击可使用。
尽量不集成操作数据库等内容，由开发习惯自己选择。只负责最核心内容。
尽量简化调用方法和启动的方式方法。
#### 软件架构
注册分发中心，集成网关，认证中心，熔断机制，选举机制，架构实现了RPC相关功能。


#### 使用说明
1.注册中心

![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/184458_a18d83c0_598831.png "微信图片_20210115184007.png")

找到config.json文件 修改配置

```
{
  "port": 9001 //监听端口
}
```

2.  验证中心
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/184038_7eeb9ba6_598831.png "微信图片_20210115184007.png")

![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/184204_75b9d3ca_598831.png "微信图片_20210115184007.png")

找到Account.cs 方法替换成，自己的验证账号和密码的方法


3.  API网关

找到config.json文件 修改配置
```
{
  "Authentication": true,//是否开启验证
  "IdentityServer": "http://localhost:5000",//验证中心地址
  "Audience": "api1",
  "defaultScheme": "Bearer",
  "applicationUrl": "http://localhost:1221",//网关地址
  "Microcenter": "127.0.0.1:9001"//注册中心地址
}
```
4.编写自己的API方法

新建CLASS或者类库

```
  [Route("abcd")]//路由地址
    public class Class2: FunctionBase//FunctionBase可以继承，也可以不继承，方便以后扩展功能
    {
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]//指定方法为远程方法， 
       //FunAttribute { NONE,Get,POST } 包含三种请求类型，为方法写注释
        public String ff(mode md)
        {
          
         //   Console.WriteLine(md.name);
            return "Class2.ff的返回值";
        }
        [Authorize]//此处表明此方法需要权限验证
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]
        public String ff2([Param("用户名")] string name)//[Param("用户名")]  为每个参数写注释
        {

           /// Console.WriteLine(name);
            return "Class2.f2f22的返回值";
        }
    }
```

5.  RPC服务
找到config.json文件 修改配置


```
{
  "ServerIP": "127.0.0.1",//当前IP
  "Port": 10098,//监听端口
  "Microcenter": "127.0.0.1:9001"//服务中心地址
}
```


```
 RemoteService remoteService = new RemoteService("TEST");//初始化服务
            remoteService.Start();//启动服务
```


#### 其他说明

1.  每个RPC服务，和网关服务，会在目录下生成一个funconfig.json，
来表明当前注册的服务器和方法，今后可以扩展相关的监控或者其他内容

2.  注册中心断开后，会自动持久化，原本注册的内容，每次启动，检查远程服务是否开启。

3.  远程调用事件使用weaving-socket架构，远程响应时间0-15毫秒之间
4. API网关调用远程方法为单连接队列锁模式，每个远程RemoteService ，会开启一个连接。

5.多个RemoteService 拥有相同的方法和路由，使用选举方式分配调用。

6.当RemoteService 从注册中心断开，API网关会立刻更新相关方法进行调用熔断。

7.注册中心未来会更新加入全部已上线的，服务方法，以方便查阅开发。

8.无验证的调用
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/190658_08b771ed_598831.png "微信图片_20210115184007.png")

9.有验证的调用
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/190741_857a47d3_598831.png "微信图片_20210115190730.png")

#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
