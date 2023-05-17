# WeaveMicro
- 支持.net core 2.x-7.x，正常使用 

- 支持linux系统，centos，radhat,麒麟，统信，鲲鹏，X86_64,等系统，经过测试互认证。

- 业务开发业务逻辑API类时， 搜索nuget包，wRPCService。执行业务逻辑，搜索WeaveRemoteService包

- RPM包下载

    [WeaveMicro-1-1.arm64](https://gitee.com/UDCS/weave-micro/blob/new/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/WeaveMicro-1-1.arm64.rpm)

    [WeaveMicro-1.0.0-1.aarch64.rpm](https://gitee.com/UDCS/weave-micro/blob/new/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/WeaveMicro-1.0.0-1.aarch64.rpm)

    [WeaveMicro-1.0.0-1.x86_64.rpm](https://gitee.com/UDCS/weave-micro/blob/new/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/WeaveMicro-1.0.0-1.x86_64.rpm)

# Weave微服务架构介绍

主要目的，尽量简化和减少开发复杂度和难度，微服务有分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点，也可以配合反向代理，应用在多种开发环境下，
使用.net core开发， 支持多CPU多系统，目前已做国产化适配认证。
#### 软件架构
注册分发中心，集成网关，认证中心，熔断机制，选举机制，架构实现了RPC相关功能。通信协议基于[weaving-socket](https://gitee.com/dotnetchina/weaving-socket)


# 使用说明

- 安装
   windows 直接运行发布后，从发布目录中拷贝。
   linux 系统：
     根据不通CPU选择不同的rpm包下载,

     使用    yum install WeaveMicro-*-*.*.rpm(WeaveMicro-1-1.arm64.rpm 或 WeaveMicro-1.0.0-1.aarch64.rpm 或 WeaveMicro-1.0.0-1.x86_64.rpm)进行安装。

     [WeaveMicro-1-1.arm64.rpm](https://gitee.com/UDCS/weave-micro/blob/new/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/WeaveMicro-1-1.arm64.rpm)

    [WeaveMicro-1.0.0-1.aarch64.rpm](https://gitee.com/UDCS/weave-micro/blob/new/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/WeaveMicro-1.0.0-1.aarch64.rpm)

    [WeaveMicro-1.0.0-1.x86_64.rpm](https://gitee.com/UDCS/weave-micro/blob/new/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/WeaveMicro-1.0.0-1.x86_64.rpm)
    
# 运行步骤：

#### 运行微服务服务中心
-  运行WeaveMicrocenter项目-》运行和服务中心-》WeaveMicrocenter.exe 
-  linux 系统 可以运行dotnet WeaveMicrocenter.dll 
-  安装 rpm 包后可以执行  `chmod  777 /microcenter/WeaveMicrocenter`  然后在执行 `/microcenter/WeaveMicrocenter`
 可以修改目录下config.json文件更改端口和API查询地址 
-  找到config.json文件 修改配置
-  注册中心启动后，可以通过http://127.0.0.1:5022/  查看和管理注册的网关和 API 内容，并可以通过页面进行API接口测试

 ```
                        {
                          "port": 9001,
                          "url": "http://*:5022"
                        }
 ```
 API文档使用

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

####  运行API网关

- 运行gateway项目-》API网关-》gateway.exe 
- linux 系统 可以运行dotnet gateway.dll 
- 安装 rpm 包后可以执行  `chmod  777 /gateway/gateway`  然后在执行 `/gateway/gateway`
可以修改目录下config.json文件更改端口和服务中心地址，认证中心注册地址，本地网关找到config.json文件 修改配置
```
{
  "Authentication": true,//开启登录认证
  "IdentityServer": "http://10.1.65.226",//登录认证地址
  "Audience": "ac-cloud",
  "defaultScheme": "Bearer",
  "applicationUrl": "http://*:5221",//网关请求地址 
   //网关请求地址支持IP写为*，但必须增加配置bindIP告知服务中心网关实际请求地址，用于在复杂环境下的部署
  "bindIP": "127.0.0.1",
  "Microcenter": "127.0.0.1:9002",//注册中心
  "filetype": ".jpg,.png,.doc,.txt",//指定可上传文件的后缀
  "httpspassword": "linezero",//https证书密码
  "Cookies": [ "Role", "UserId", "Name", "Phone" ],//开启认证后，token中所包含的指定信息，可明文带入远程服务中
  "Headers": [ "token" ]//http指定头部内容可以带入，远程服务中
}
```
####  运行认证验证中心（OAuth 2.0 框架）
 
   - 找到项目中IdentityServer 项目，ResourceOwnerPasswordValidator.cs 文件，ValidateAsync方法 根据方法示例替换成，自己的验证账号和密码的方法
   - 如果您有自己的项目支持OAuth 2.0 框架，请求可以部署发布您自己的认证服务，只需要在网关配置好相关配置。

```
 public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法

            string Loginname = context.UserName.Trim();//用户名
            string Password = context.Password.Trim();//密码
            string type = context.Request.Raw["prj"].Trim();//项目名称
            
            if (string.IsNullOrEmpty(type))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "The requested resource does not exist");
                return;
            }
            try
            {
                
                   //此处自己写判断
                        //验证成功
                        context.Result = new GrantValidationResult(
                             subject: context.UserName,
                             authenticationMethod: "custom",
                             claims: new Claim[] {
                                new Claim("UserId", "ceshi123"),
                                new Claim("Name","ceshi"),
                                new Claim("Phone", "135135"),
                                new Claim("Role","admin"),
                                 new Claim("Area","41")
                             });
                   
                
            }
            catch (Exception e)
            {
                //验证失败                
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential:" + e.Message);
            }

            
        }
```

##制作API服务
#### 编写自己的API服务，制作业务方法并运行
   - 项目中testdll 文件夹下为示例。
   - 新建一个应用控制台程序引用 WeaveRemoteService 包，可从nuget 搜索

   ```  static  void Main(string[] args)
        {
            RemoteService remoteService = new RemoteService("TEST");//TEST 为你注册的API名称
            remoteService.Start();
       }
```
    修改配置文件

```
{
  "ServerIP": "127.0.0.1",//本机需要绑定的IP
  "Port": 10098,//需要绑定的端口用于网关调用
  "Microcenter": "127.0.0.1:8001"//服务中心的IP和端口
}
```


   - 新建一个类库作为业务逻辑实现类，引用wRPCService包，可以从nuget搜索，应用控制台程序 引用这个新业务类库。
   
    - 新建CLASS继承 : FunctionBase 基类
    - 基类介绍，FunctionBase 包含 Cookies，和 Headers 两个属性，Headers为http请求头可以被带入的内容，Cookies 为开启认证后，登录的token中所包含的指定信息，可明文带入远程服务中。
    - 标识属性 Route ， [Route("abcd")]放在类上面，标识次类的API路由地址，
    - 标识属性 InstallFun ，  [InstallFun(FunAttribute.NONE, "此方法用于测试")] ，参数1：FunAttribute.NONE，标识此方法接收POST 传入的 json类型对象。参数2：用于API测试中的注释说明。FunAttribute { NONE,Get,POST ，file} 包含四种请求类型
    - 统一响应返回格式 IApiResult接口，（可选）如果需要统一API响应格式，可通过继承IApiResult接口，定义返回格式。系统会自动获取T的类型并展示到Swagger文档中。
    业务方法可以写成这样 `public async Task<IApiResult> Update(T_DisasterCase o)`
    - async，支持方法类型为异步响应async。

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

# 调用和验证
 
#### 通过网关调用服务

- 无验证的调用
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/190658_08b771ed_598831.png "微信图片_20210115184007.png")

- 有验证的调用
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/190741_857a47d3_598831.png "微信图片_20210115190730.png")

#### 不通过网关调用服务
- 新建一个应用控制台程序引用 wRPC 包，可从nuget 搜索


```
 wRPCclient.ClientChannel clientChannel = new wRPCclient.ClientChannel("127.0.0.1", 10098); //连接API服务的地址
 String retun = clientChannel.Call<String>("api/abcd", "ff2","asdasd");//"api/abcd" 路由地址，ff2，调用的方法名称，"asdasd" 传入的参数，有几个就写几个，多个参数，可以写多个如，Call<String>("路由名", "方法名","参数1"，"参数2");
  //Call<String> 为返回的值类型
```


# 其他说明

1.  每个RPC服务，和网关服务，会在目录下生成一个funconfig.json，
来表明当前注册的服务器和方法，今后可以扩展相关的监控或者其他内容

2.  注册中心断开后，会自动持久化，原本注册的内容，每次启动，检查远程服务是否开启。

3.  远程调用事件使用weaving-socket架构，远程响应时间0-1毫秒之间
4. API网关调用远程方法为单连接队列锁模式，每个远程RemoteService ，会开启一个连接。

5.多个RemoteService 拥有相同的方法和路由，使用选举方式分配调用。

6.当RemoteService 从注册中心断开，API网关会立刻更新相关方法进行调用熔断。

7.注册中心未来会更新加入全部已上线的，服务方法，以方便查阅开发。


8. 网关固定化内容，当网关config不配置服务中心时，可以自己编写funconfig.json内容，达到只使用网关启动请求服务。