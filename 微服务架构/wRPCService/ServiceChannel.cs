using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using wRPC;
using static wRPC.FunctionBase;

namespace wRPCService
{
    public class ServiceChannel
    {
        Weave.Server.WeaveP2Server P2Server = new Weave.Server.WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        ConcurrentDictionary<string, Type> keyValuePairs = new ConcurrentDictionary<string, Type>();
        int Port;
        public ServiceChannel(int port)
        {
            P2Server.weaveReceiveBitEvent += P2Server_weaveReceiveBitEvent;
              Port = port;
         
           
            
        }

        private void P2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
                String rdata = GZIP.GZipDecompress(data);
                Rpcdata<Object[]> rpdata = Newtonsoft.Json.JsonConvert.DeserializeObject<Rpcdata<Object[]>>(rdata);
                if (!keyValuePairs.ContainsKey(rpdata.Route.Replace('/', '.')))

                {
                    P2Server.Send(soc, 0x05, GZIP.GZipCompress( "server Route error"));
                    return;
                }

                Type tt = keyValuePairs[rpdata.Route.Replace('/', '.')];
                Assembly ab = Assembly.GetAssembly(tt);
                object obj = ab.CreateInstance(tt.FullName);
                //Type t = obj.GetType();

                MethodInfo mi = tt.GetMethod(rpdata.FunName);
                if (mi != null)
                {
                    InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                    if (myattribute != null)
                    {
                        object[] objs = rpdata.parameter;
                        if (obj is FunctionBase && rpdata.Headers!=null)
                        {


                            (obj as FunctionBase).Headers = new Dictionary<string, StringValues>() ;
                            foreach (String str in rpdata.Headers.Keys)
                            {
                                (obj as FunctionBase).Headers.Add(str, rpdata.Headers[str][0]);
                            }
                           
                        }
                        //if (myattribute.Type != FunAttribute.RPC)
                        //{
                        //    objs = new object[rpdata.parameter.Length + 1];
                        //    int i = 0;
                        //    foreach (object oo in rpdata.parameter)
                        //    {
                        //        objs[i] = oo;
                        //        i++;
                        //    }
                        //    objs[objs.Length - 1] = rpdata.HttpContext;

                        //}
                        ParameterInfo[] paramsInfo = mi.GetParameters();//得到指定方法的参数列表 
                        if (paramsInfo.Length != objs.Length)
                        { P2Server.Send(soc, 0x02, GZIP.GZipCompress("参数不正确"));return; }
                            for (int i = 0; i < objs.Length; i++)

                        {
                         
                            Type tType = paramsInfo[i].ParameterType;

                            //如果它是值类型,或者String   

                            if (tType.Equals(typeof(string)) || (!tType.IsInterface && !tType.IsClass))

                            {

                                //改变参数类型   

                                objs[i] = Convert.ChangeType(objs[i], tType);

                            }

                            else if (tType.IsClass)//如果是类,将它的json字符串转换成对象   

                            {

                                objs[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(objs[i].ToString(), tType);

                            }

                        }
                        object rpcdata = mi.Invoke(obj, objs);
                        byte[] outdata = GZIP.GZipCompress(Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata));
                        P2Server.Send(soc, 0x01, outdata);

                    }
                }

            }
            catch (Exception e)
            {
                P2Server.Send(soc, 0x02, GZIP.GZipCompress(e.Message));
            }
        }

        public void Start()
        {
            keyValuePairs = ToolLoad.Load(keyValuePairs);

            P2Server.Start(Port);
        }
    }

   public class ToolLoad
    {
        public static ConcurrentDictionary<string, Type> Load(ConcurrentDictionary<string, Type> keyValuePairs)
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
                        MethodInfo[] mis = tt.GetMethods();
                        foreach (MethodInfo mia in mis)
                        {

                            InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mia, typeof(InstallFunAttribute));
                            if (myattribute != null)
                            {
                                //object obj = assembly.CreateInstance(tt.FullName);
                                //if (obj is FunctionBase)
                                //{
                                //    service[] services = (obj as FunctionBase).GetService();
                                //}
                                //else
                                //{
                                //    service[] services= GetService(obj);
                                //}
                                if (RouteAttr != null)
                                {
                                    if (!keyValuePairs.ContainsKey(RouteAttr.Route))
                                        keyValuePairs.TryAdd(RouteAttr.Route, tt);
                                }
                                else
                                {
                                    if (!keyValuePairs.ContainsKey(mia.DeclaringType.FullName))
                                        keyValuePairs.TryAdd(mia.DeclaringType.FullName, tt);
                                }
                            }
                        }
                    }
                }
                catch
                { }
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
                        RouteAttribute RouteAttr = (RouteAttribute)Attribute.GetCustomAttribute(tt, typeof(RouteAttribute));
                      //  MethodInfo[] mis = tt.GetMethods();
                      
                                object obj = assembly.CreateInstance(tt.FullName);
                                if (obj is FunctionBase)
                                {
                                    service[] services = (obj as FunctionBase).GetService();
                                    listservice.AddRange(services);
                                }
                                else
                                {
                                    service[] services = GetService(obj);
                                    listservice.AddRange(services);
                                }
                           
                    }
                }
                catch
                { }
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
                        RouteAttribute RouteAttr = (RouteAttribute)Attribute.GetCustomAttribute(tt, typeof(RouteAttribute));
                        if (RouteAttr != null)
                            serv.Route = RouteAttr.Route;
                        else
                            serv.Route = tt.FullName.Replace(".", @"/");
                        serv.annotation = myattribute.Annotation;
                        serv.Method = mi.Name;
                        ParameterInfo[] paramsInfo = mi.GetParameters();//得到指定方法的参数列表 
                        serv.parameter = new string[paramsInfo.Length];
                        for (int i = 0; i < paramsInfo.Length; i++)

                        {

                            Type tType = paramsInfo[i].ParameterType;

                            //如果它是值类型,或者String   

                            serv.parameter[i] = tType.Name;

                        }

                        listservice.Add(serv);
                    }


                }
            }

            return listservice.ToArray();
        }
    }
}
