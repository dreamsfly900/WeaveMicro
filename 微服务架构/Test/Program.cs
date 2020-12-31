using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using wRPC;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        { 
            ConcurrentDictionary<string, Assembly> keyValuePairs = new ConcurrentDictionary<string, Assembly>();
               String[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        
            foreach (String file in files) {
                try
                {

                    Assembly assembly = Assembly.LoadFrom(file);
                    Type[] ts = assembly.GetTypes();
                    foreach (Type tt in ts)
                    {
                        MethodInfo[] mis = tt.GetMethods();
                        foreach (MethodInfo mia in mis)
                        {
                           
                            InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mia, typeof(InstallFunAttribute));
                            if (myattribute != null)
                            {
                                if(!keyValuePairs.ContainsKey( mia.DeclaringType.FullName))
                                keyValuePairs.TryAdd(mia.DeclaringType.FullName, assembly);
                            }
                        }
                    }
                }
                catch
                { }
            }
            object obj = keyValuePairs["testdll2.Class1"].CreateInstance("testdll2.Class1");
            Type t = obj.GetType();
            MethodInfo mi = t.GetMethod("ff");
            if (mi != null)
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute != null)
                {

                }
            }
            object[] objs = new object[] { "aavvbb22" };
             
            mi.Invoke(obj, objs);
            Console.WriteLine("Hello World!");
        }
    }
    
}
namespace aabb {
  public  class A {

       public void bb(String aa)
        {
            Console.WriteLine(aa);
        }
    
    }

}
