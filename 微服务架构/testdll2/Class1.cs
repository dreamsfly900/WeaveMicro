using Model;
using System;
using System.Threading.Tasks;
using wRPC;

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
    [Route("api/abcd")]
    public class Class2: FunctionBase
    {
        /// <summary>
        /// 此方法用于测试XML注释（需要生成XML文档）
        /// </summary>
        /// <returns></returns>
        [InstallFun(FunAttribute.file, "此方法用于测试")]
        public async Task<String> ff()
        {
           object obj=  this.Cookies;            object obj2 = this.Headers;


            //   Console.WriteLine(md.name);
            //System.IO.FileStream streamWriter = new System.IO.FileStream(this.Filedata.filename,System.IO.FileMode.Create);
            //streamWriter.Write(this.Filedata.data,0, this.Filedata.data.Length);
            //streamWriter.Close();
            //this.Filedata.filename
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
        public void ff22()
        {
            int i = 0;
            while (true)
            { i++;
                this.PushStream("Weave微服务架构,是.net core下开发的由分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点");
                if (i > 1000)
                    return;
             }
            /// Console.WriteLine(name);
           
        }
        [InstallFun(FunAttribute.Get, "测试流传输", "application/octet-stream")]
        public void ffs22()
        {
            int i = 0;
            while (true)
            {
                i++;
                this.PushStream(System.Text.UTF8Encoding.UTF8.GetBytes("Weave微服务架构,是.net core下开发的由分发网关，服务中心，认证中心，服务API 组成，具有多负载分布式特点"));
                if (i > 1000)
                    return;
            }
            /// Console.WriteLine(name);

        }
    }
}
