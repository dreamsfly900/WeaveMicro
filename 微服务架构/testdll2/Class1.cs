using System;
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
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]
        public void ff(string str)
        {

            Console.WriteLine(str);
        }
    }
    [Route("abcd")]
    public class Class2: FunctionBase
    {
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]
        public String ff(mode md)
        {
           object obj=  this.Cookies;
         //   Console.WriteLine(md.name);
            return "Class2.ff的返回值";
        }
        [Authorize]
        [InstallFun(FunAttribute.NONE, "此方法用于测试")]
        public String ff2([Param("用户名")] string name)
        {

           /// Console.WriteLine(name);
            return "Class2.f2f22的返回值";
        }
    }
}
