# WeaveMicro
支持.net core 2.x-5.x，正常使用 ，业务逻辑类 搜索nuget包，wRPCService。执行业务逻辑，搜索WeaveRemoteService包
#### 介绍
Weave微服务架构
主要目的，尽量简化和减少开发复杂度和难度，微服务有分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点，也可以配合反向代理，应用在多种开发环境下，
使用.net core开发， 支持多CPU多系统，目前已做国产化适配认证。
#### 软件架构
注册分发中心，集成网关，认证中心，熔断机制，选举机制，架构实现了RPC相关功能。


#### 使用说明
1.注册中心

![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/184458_a18d83c0_598831.png "微信图片_20210115184007.png")

找到config.json文件 修改配置

```
{
  "port": 9001,
  "url": "http://*:5022"
}
```
注册中心启动后，可以通过http://127.0.0.1:5022/apiHtml/server.html  查看和管理注册的网关和 API 内容，并可以通过页面进行API接口测试
![输入图片说明](%E5%BE%AE%E4%BF%A1%E5%9B%BE%E7%89%87_20220823145117.png)
也可通过http://127.0.0.1:5022 直接查看和测试API

2.  验证中心

![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/184038_7eeb9ba6_598831.png "微信图片_20210115184007.png")


![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/184204_75b9d3ca_598831.png "微信图片_20210115184007.png")

找到Account.cs 方法替换成，自己的验证账号和密码的方法


3.  API网关

![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/192918_a576c018_598831.png "微信图片_20210115192900.png")

找到config.json文件 修改配置
```
{
  "Authentication": true,//开启登录认证
  "IdentityServer": "http://10.1.65.226",//登录认证地址
  "Audience": "ac-cloud",
  "defaultScheme": "Bearer",
  "applicationUrl": "http://*:5221",//网关请求地址
  "bindIP": "127.0.0.1",
  "Microcenter": "127.0.0.1:9002",//注册中心
  "filetype": ".jpg,.png,.doc,.txt",//指定可上传文件的后缀
  "httpspassword": "linezero",//https证书密码
  "Cookies": [ "Role", "UserId", "Name", "Phone" ],//开启认证后，token中所包含的指定信息，可明文带入远程服务中
  "Headers": [ "token" ]//http指定头部内容可以带入，远程服务中
}
```
网关请求地址支持IP写为*，但必须增加配置bindIP告知服务中心网关实际请求地址，用于在复杂环境下的部署
4.编写自己的API方法

新建CLASS或者类库

```
  [Route("abcd")]//路由地址
    public class Class2: FunctionBase//FunctionBase可以继承，也可以不继承，方便以后扩展功能
    {
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]//指定方法为远程方法， 
       //FunAttribute { NONE,Get,POST ，file} 包含四种请求类型，为方法写注释
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
[InstallFun(FunAttribute.file, "此方法用于测试")]
        public String ff()//接收文件并保存
        {
           object obj=  this.Cookies;            object obj2 = this.Headers;


            //   Console.WriteLine(md.name);
            System.IO.FileStream streamWriter = new System.IO.FileStream(this.Filedata.filename,System.IO.FileMode.Create);
            streamWriter.Write(this.Filedata.data,0, this.Filedata.data.Length);
            streamWriter.Close();
            return this.Filedata.filename;
        }
```
（可选）如果需要统一API响应格式，可通过继承IApiResult接口，定义返回格式。系统会自动获取T的类型并展示到Swagger文档中
```
    /// <summary>
    /// Api返回结果
    /// </summary>
    public class ApiResult<T> : IApiResult
    {
        /// <summary>
        /// 状态码：200成功500异常0操作失败
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 异常数据
        /// </summary>
        public dynamic errData { get; set; }
    }
    //API签名可以这样写（wRPCService v1.0.8 以上版本）
    public async Task<IApiResult> Update(T_DisasterCase o)...
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

6. API文档使用

找到config.json文件 修改配置
```
{
  "port": 9001,
  "url": "http://*:5022",
  "SwaggerPath": "swagger"
}
```
可使用SwaggerPath配置文档查看地址，默认为根目录（无此配置或设置为空字符串同等效果），如不启用API文档，修改配置如下
```
{
  "port": 9001,
  "url": "http://*:5022",
  "SwaggerPath": null
}
```


#### 其他说明

1.  每个RPC服务，和网关服务，会在目录下生成一个funconfig.json，
来表明当前注册的服务器和方法，今后可以扩展相关的监控或者其他内容

2.  注册中心断开后，会自动持久化，原本注册的内容，每次启动，检查远程服务是否开启。

3.  远程调用事件使用weaving-socket架构，远程响应时间0-1毫秒之间
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
