# WeaveMicro
河南创志威科技有限公司
- 支持.net core 2.x-7.x，正常使用 

- 支持linux系统，centos，radhat,麒麟，统信，鲲鹏，X86_64,等系统，经过测试互认证。[查看证书](%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/%E5%BE%AE%E4%BF%A1%E5%9B%BE%E7%89%87_20230524143454.png)

- 业务开发业务逻辑API类时， 搜索nuget包，wRPCService。执行业务逻辑，搜索WeaveRemoteService包

- WINX64 运行包
[weave_win_1.0.0-1.X64.zip](https://gitee.com/UDCS/weave-micro/releases)
- RPM包下载

    [WeaveMicro-1-1.arm64](https://gitee.com/UDCS/weave-micro/releases)

    [WeaveMicro-1.0.0-1.aarch64.rpm](https://gitee.com/UDCS/weave-micro/releases)

    [WeaveMicro-1.0.0-1.x86_64.rpm](https://gitee.com/UDCS/weave-micro/releases)
- 登录认证中心RPM包下载

     [登录认证中心RPM包发行与使用说明](https://gitee.com/UDCS/weave-micro/releases/tag/1.0.1.a)


# Weave微服务架构介绍

主要目的，尽量简化和减少开发复杂度和难度，微服务由4部分组成。
- 包括以下内容：
- 1.分发网关 2.服务中心 3.认证中心 4.服务API 
- 具有多负载分布式特点，也可以配合反向代理，应用在多种开发环境下，
- 流数据支持，回复响应类型支持。
使用.net core开发， 支持多CPU多系统，目前已做国产化适配认证。
#### 软件架构
注册分发中心，集成网关，认证中心，熔断机制，选举机制，架构实现了RPC相关功能。通信协议基于[weaving-socket](https://gitee.com/dotnetchina/weaving-socket)
![输入图片说明](%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%9E%B6%E6%9E%84/a.png)
# 安装-下载使用说明

- 安装
   windows 直接运行发布后，从发布目录中拷贝。
   linux 系统：
     根据不通CPU选择不同的rpm包下载,

     使用    yum install WeaveMicro-*-*.*.rpm(WeaveMicro-1-1.arm64.rpm 或 WeaveMicro-1.0.0-1.aarch64.rpm 或 WeaveMicro-1.0.0-1.x86_64.rpm)进行安装。

     [包下载地址](https://gitee.com/UDCS/weave-micro/releases)
 
- 设置服务随linux后台启动运行教程 [linux服务设置教程](https://gitee.com/UDCS/weave-micro/releases/download/1.0.0/%E8%AE%BE%E7%BD%AE%E7%A8%8B%E5%BA%8Flinux%E9%9A%8F%E6%9C%8D%E5%8A%A1%E5%90%AF%E5%8A%A8%E6%95%99%E7%A8%8B.zip)
    
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
##### 运行微服务服务监控页面
- 根据您配置的http://*:5022，地址访问http://127.0.0.1/apiHtml/server.html 来查看，加入中心的网关和服务情况和数量。
####  运行API网关

- 运行gateway项目-》API网关-》gateway.exe 
- linux 系统 可以运行dotnet gateway.dll 
- 安装 rpm 包后可以执行  `chmod  777 /gateway/gateway`  然后在执行 `/gateway/gateway`
可以修改目录下config.json文件更改端口和服务中心地址，认证中心注册地址，本地网关找到config.json文件 修改配置
- 支持添加头部以及禁用HTTP谓词，以便支持网监要求
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
  "Headers": [ "token" ],//http指定头部内容可以带入，远程服务中
   //设置不允许的HTTP谓词，如TRACE
  "HttpDisableMethods": [ "Trace", "Debug" ],
   //添加额外的头部
  "AddHeaders": {
    "Referrer-Policy": "no-referrer",
    "X-Content-Type-Options": "nosniff",
    "X-Download-Options": "noopen",
    "X-Permitted-Cross-Domain-Policies": "master-only",
    "X-Frame-Options": "sameorigin",
    "X-XSS-Protection": "1; mode=block"
  },
}
```
####  运行认证验证中心（OAuth 2.0 框架）
 下载对应的打包发布程序您的服务器上
 [登录认证中心RPM包发行与使用说明](https://gitee.com/UDCS/weave-micro/releases/tag/1.0.1.a)
#### 认证中心的认证编写
之前版本认证中心需要重新编译内容，自己提供实现，目前可通过接口继承进行实现。简化编写方法
新建2.0类库 ，nuget 搜索 WeaveVerify，继承 IdentityBase，实现内部方法

```
  public class Verifyabc : IdentityBase
  {
      public override string PrjName { get; set; } = "abc";//项目名称

      public override Verifymode attestation(string Loginname, string Password)
      {
          Verifymode vm = new Verifymode();
          if (true)
          { //认证成功赋值内容
              vm.Verify = true;
              vm.Claims = new Claim[] {
                  new Claim("UserId", "123"),
                  new Claim("Name", "admin"),
                  new Claim("GivenName", "sdfq") } ;
             
          }
          else
          {
//认证失败提示错误内容
              vm.Verify = false;
              vm.ERRMessage = "XXX错误~！";
          }
          return vm;
      }
  }
```
测试提交参数
![测试提交参数截图](https://foruda.gitee.com/images/1697712264366894372/f6b34e4a_598831.png "屏幕截图")
# 制作API服务

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
## API-GET方法

```
 [InstallFun(FunAttribute.Get, "此方法用于测试")]//指定方法为远程方法， 
       //FunAttribute { NONE,Get,POST ，file} 包含四种请求类型，为方法写注释
        public String ff(String md)
        {
          
         //   Console.WriteLine(md);
            return "Class2.ff的返回值";
        }

```
## API-post方法

```
 [InstallFun(FunAttribute.POST, "此方法用于测试")]//指定方法为远程方法， 
       //FunAttribute { NONE,Get,POST ，file} 包含四种请求类型，为方法写注释
        public String ff(String md,int a)
        {
          
         //   Console.WriteLine(md);
            return "Class2.ff的返回值";
        }

```
## API-json 实体对象请求方法

```
 [InstallFun(FunAttribute.NONE, "此方法用于测试")]//指定方法为远程方法， 
       //FunAttribute { NONE,Get,POST ，file} 包含四种请求类型，为方法写注释
        public String ff(mode md)//只能传入一个对象
        {
          
         //   Console.WriteLine(md.name);
            return "Class2.ff的返回值";
        }

```
## API-文件上传方法

```
[InstallFun(FunAttribute.file, "此方法用于测试")]
        public String ff(string name)//接收文件并保存
        {
           object obj=  this.Cookies;            object obj2 = this.Headers; 
            System.IO.FileStream streamWriter = new System.IO.FileStream(this.Filedata.filename,System.IO.FileMode.Create);
            streamWriter.Write(this.Filedata.data,0, this.Filedata.data.Length);
            streamWriter.Close();
            return this.Filedata.filename;
        }

```
## API- 异步返回调用
```
   public async Task<String> ff()
        {
           object obj=  this.Cookies;            object obj2 = this.Headers;


            
             await Task.Delay(1000);
            return await Task.Run(() => { ; return "aaaaaa"; } ) ;
        }
```
## API-流式写入返回（字符串）
流式返回可以在数据量较大的时候，数据快速响应到页面
```
 [InstallFun(FunAttribute.Get, "测试流传输")]
        public void ff22()
        {
            int i = 0;
            DateTime dt = DateTime.Now;
            while (true)
            { i++;
                this.PushStream("Weave微服务架构,是.net core下开发的由分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点");
                if (i > 20000)
                  break;
             }
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("ff22:"+(dt2-dt).TotalMilliseconds);

        }
```
## 使用统一的回复数据规则-并简化使用方法
```
 [Route("api/test")]
 public class testclass : ApiBase<object>//继承ApiBase<object>基类
 {
     public async Task<ApiResult<String>> test()
     {
         return await TRY(async () =>
         {
// 写自己的方法内容
             return "";
         });
     }
     public async Task<IApiResult> testapi()
     {
         return await TRY(async () =>
         {
// 写自己的方法内容
             return "";
         });
     }
 }
```

## API-流式写入返回（二进制）

```
     [InstallFun(FunAttribute.Get, "测试流传输", "application/octet-stream")]
        public void ffs22()
        {
            DateTime dt = DateTime.Now;
            int i = 0;
            while (true)
            {
                i++;
                this.PushStream(System.Text.UTF8Encoding.UTF8.GetBytes("Weave微服务架构,是.net core下开发的由分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点"));
                if (i > 100000)
                    break;
            }
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("ffs22:" + (dt2 - dt).TotalMilliseconds);
            /// Console.WriteLine(name);

        }

```

## API-获取HTTP header信息
首先需要在gateway网关中修改配置文件，
>  
  "Headers": [ "token" ]//添加需要获取到的HTTP header
```
[InstallFun(FunAttribute.Get, "测试")]
        public string ff22()
        { 
          var value = this.Headers["对应KEY"];
            return "";
        }
```
## API-获取OAuth2.0  认证信息中包含信息
认证中心需要返回对应的资料内容如下：

```
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
```

然后需要在gateway网关中修改配置文件，
>  "Cookies": [ "Areacode", "Role", "UserId", "Name", "Phone" ]

```
[InstallFun(FunAttribute.Get, "测试")]
        public string ff22()
        { 
          var value = this.Cookies["对应KEY"];
            return "";
        }
```
# 调用和验证

 
#### 通过网关HTTP/HTTPS Restful调用服务

- 无验证的调用
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/190658_08b771ed_598831.png "微信图片_20210115184007.png")

- 有验证的调用
![输入图片说明](https://images.gitee.com/uploads/images/2021/0115/190741_857a47d3_598831.png "微信图片_20210115190730.png")

#### RPC-远程方法调用-不通过网关调用服务
- 新建一个应用控制台程序引用 wRPC 包，可从nuget 搜索


```
 wRPCclient.ClientChannel clientChannel = new wRPCclient.ClientChannel("127.0.0.1", 10098); //连接API服务的地址
 String retun = clientChannel.Call<String>("api/abcd", "ff2","asdasd");//"api/abcd" 路由地址，ff2，调用的方法名称，"asdasd" 传入的参数，有几个就写几个，多个参数，可以写多个如，Call<String>("路由名", "方法名","参数1"，"参数2");
  //Call<String> 为返回的值类型
```

# 其他说明

- 1.  每个RPC服务，和网关服务，会在目录下生成一个funconfig.json，
来表明当前注册的服务器和方法，今后可以扩展相关的监控或者其他内容
- 2.  注册中心断开后，会自动持久化，原本注册的内容，每次启动，检查远程服务是否开启。
- 3.  远程调用事件使用weaving-socket架构，远程响应时间0-1毫秒之间
- 4. API网关调用远程方法为单连接队列锁模式，每个远程RemoteService ，会开启一个连接。
- 5.多个RemoteService 拥有相同的方法和路由，使用选举方式分配调用。
- 6.当RemoteService 从注册中心断开，API网关会立刻更新相关方法进行调用熔断。
- 7.注册中心未来会更新加入全部已上线的，服务方法，以方便查阅开发。
- 8. 网关固定化内容，当网关config不配置服务中心时，可以自己编写funconfig.json内容，达到只使用网关启动请求服务。
- 9. 每一次通过网关调用API服务时，服务中心都会收到对应的日志，保存在目录下。![接收请求日志](%E5%BE%AE%E4%BF%A1%E5%9B%BE%E7%89%87_20230517211922.png)
 