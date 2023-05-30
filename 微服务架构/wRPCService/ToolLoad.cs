using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using wRPC;

namespace wRPCService
{
    public class ToolLoad
    {
        public static Dictionary<string, object> Load(Dictionary<string, object> keyValuePairs)
        {

            String[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            foreach (String file in files)
            {
                try
                {

                    Assembly assembly = Assembly.LoadFrom(file);


                    Type[] ts = assembly.GetTypes();
                    foreach (Type tt in ts)
                    {
                        RouteAttribute RouteAttr = (RouteAttribute)Attribute.GetCustomAttribute(tt, typeof(RouteAttribute));
                        if (RouteAttr != null)
                        {
                            object obj = assembly.CreateInstance(tt.FullName);
                            MethodInfo[] mis = tt.GetMethods();
                            foreach (MethodInfo mia in mis)
                            {

                                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mia, typeof(InstallFunAttribute));
                                if (myattribute != null)
                                {

                                    if (RouteAttr != null)
                                    {
                                        if (!keyValuePairs.ContainsKey(RouteAttr.Route))
                                            keyValuePairs.Add(RouteAttr.Route, obj);
                                    }
                                    else
                                    {
                                        if (!keyValuePairs.ContainsKey(mia.DeclaringType.FullName))
                                            keyValuePairs.Add(mia.DeclaringType.FullName, obj);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                { Console.WriteLine(e.Message); }
            }
            return keyValuePairs;
        }

        public static service[] GetService()
        {
            List<service> listservice = new List<service>();
            String[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            foreach (String file in files)
            {
                try
                {

                    Assembly assembly = Assembly.LoadFrom(file);


                    Type[] ts = assembly.GetTypes();
                    foreach (Type tt in ts)
                    {
                        try
                        {
                            RouteAttribute RouteAttr = (RouteAttribute)Attribute.GetCustomAttribute(tt, typeof(RouteAttribute));
                            //  MethodInfo[] mis = tt.GetMethods();
                            if (RouteAttr != null)
                            {

                                object obj = assembly.CreateInstance(tt.FullName);
                                if (obj is FunctionBase)
                                {
                                    try
                                    {
                                        service[] services = (obj as FunctionBase).GetService();

                                        listservice.AddRange(services);
                                    }
                                    catch (Exception ee)
                                    {
                                        Console.WriteLine("GetService--eee-" + ee.Message);
                                    }

                                }
                                else
                                {
                                    try
                                    {
                                        service[] services = GetService(obj);
                                        listservice.AddRange(services);
                                    }
                                    catch (Exception ee)
                                    {
                                        Console.WriteLine("GetService--eee-" + ee.Message);
                                    }

                                }
                                //Console.WriteLine("aaa:end" );
                            }
                        }
                        catch (Exception ee2)
                        {
                            Console.WriteLine(tt.FullName+ "GetService22---" + ee2.Message);
                        }

                    }
                }
                catch(Exception e)
                { //Console.WriteLine(file+"---GetService---" +e.Message); 
                }
            }
            return listservice.ToArray();
        }
        static service[] GetService(object obj)
        {
            List<service> listservice = new List<service>();
            Type tt = obj.GetType();
            MethodInfo[] mis = tt.GetMethods();
            foreach (MethodInfo mi in mis)
            {

                if (mi != null)
                {


                    InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                    if (myattribute != null)
                    {
                        service serv = new service();
                        AuthorizeAttribute Authorizeattribute = (AuthorizeAttribute)Attribute.GetCustomAttribute(mi, typeof(AuthorizeAttribute));
                        if (Authorizeattribute != null)
                            serv.Authorize = true;
                        else
                            serv.Authorize = false;
                        RouteAttribute RouteAttr = (RouteAttribute)Attribute.GetCustomAttribute(tt, typeof(RouteAttribute));
                        if (RouteAttr != null)
                            serv.Route = RouteAttr.Route + "/" + mi.Name;
                        else
                            serv.Route = tt.FullName.Replace(".", @"/") + "/" + mi.Name;
                        serv.annotation = myattribute.Annotation;
                        serv.Method = myattribute.Type.ToString();
                        serv.ContentType = myattribute.ContentType;
                        ParameterInfo[] paramsInfo = mi.GetParameters();//得到指定方法的参数列表 
                        serv.parameter = new string[paramsInfo.Length];
                        serv.parameterexplain = new string[paramsInfo.Length];
                        for (int i = 0; i < paramsInfo.Length; i++)

                        {
                            ParamAttribute ParamAttr = (ParamAttribute)Attribute.GetCustomAttribute(paramsInfo[i], typeof(ParamAttribute));
                            Type tType = paramsInfo[i].ParameterType;
                            FieldInfo[] fis = tType.GetFields();
                            foreach (FieldInfo fi in fis)
                                if (fi.Name != "Empty")
                                    serv.parameterexplain[i] += fi.FieldType.Name + " " + fi.Name + ",";
                                else
                                    serv.parameterexplain[i] += fi.FieldType.Name + ",";


                            if (ParamAttr != null)
                            {

                                serv.parameterexplain[i] += "@" + ParamAttr.explain + "|";
                            }
                            else
                                serv.parameterexplain[i] += "@|";



                            //如果它是值类型,或者String   

                            serv.parameter[i] = paramsInfo[i].Name;

                        }

                        listservice.Add(serv);
                    }


                }
            }

            return listservice.ToArray();
        }
    }
}
