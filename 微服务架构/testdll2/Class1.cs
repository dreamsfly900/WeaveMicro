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
    [Route("api/abcd")]
    public class Class2: FunctionBase
    {
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
    }
}
