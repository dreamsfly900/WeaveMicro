using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using wRPC;
using wRPC.DataLogic;

namespace testdll2
{
    public class mode
    {
        public string name;
        public int age;
    }
    public class modeA
    {
        public string name;
        public int age;
    }
    public class Class1
    {
        [InstallFun(FunAttribute.Get, "此方法用于测试")]
        public void ff(string str)
        {

            Console.WriteLine(str);
        }
    }
    [Route("big/api/page")]
    public class PageController
    {
        [Authorize]
        [InstallFun(FunAttribute.NONE, "插入页面配置信息")]
        public int InsertConfig(T_Page model)
        {
            //model.UserId = "2";
            return 1;
        }
    }
    [Route("hydrops/camera")]//路由地址
    public class cameraController : FunctionBase
    { 
        [Authorize]
        [InstallFun(FunAttribute.Get)]
        public bool upfile(String path, String file, byte[] data)
        {
            try
            {
                return true;
                //using (MemoryStream ms = new MemoryStream(data))
                //{
                //    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                //    if (!Directory.Exists(path))
                //        Directory.CreateDirectory(path);
                //    img.Save(path + file);
                //    return true;
                //}
            }
            catch { }
            return false;

        }
    }
    [Route("api/test")]
    public class testclass : ApiBase<object>
    {
        public async Task<ApiResult<String>> test()
        {
            return await TRY(async () =>
            {
                return "";
            });
        }
        public async Task<IApiResult> testapi()
        {
            return await TRY(async () =>
            {
                return "";
            });
        }
    }
    [Route("api/abcd")]
    public class Class2 : FunctionBase
    {
        
        /// <summary>
        /// 此方法用于测试XML注释（需要生成XML文档）
        /// </summary>
        /// <returns></returns>
        [InstallFun(FunAttribute.file, "此方法用于测试")]
        public async Task<String> ff()
        {
           object obj=  this.Cookies;            object obj2 = this.Headers;
          

             
                await Task.Delay(1000);
            return await Task.Run(() => { ; return "aaaaaa"; } ) ;
        }
        [Authorize]
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]
        public String    ff2([Param("用户名")] string name)
        {
           
           /// Console.WriteLine(name);
            return "Class2.f2f22的返回值";
        }
        [InstallFun(FunAttribute.Get, "测试流传输")]
        public List<String> ff23()
        {
            int i = 0;
            DateTime dt = DateTime.Now;
            List<String> listt = new List<string>();
            while (true)
            {
                i++;
                listt.Add("Weave微服务架构,是.net core下开发的由分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点");
                if (i > 20000)
                    break;
            }
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("ff22:" + (dt2 - dt).TotalMilliseconds);
            listt.Add("111111111112222222222222222224444444444444f分段传输");
            return listt;
        }
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
    }
}
