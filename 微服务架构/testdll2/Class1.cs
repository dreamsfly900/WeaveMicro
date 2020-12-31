using System;
using wRPC;

namespace testdll2
{
    public class Class1
    {
       [ InstallFun(FunAttribute.ALL,"此方法用于测试")]
        public void ff(string str)
        { 

            Console.WriteLine(str);
        }
    }
}
