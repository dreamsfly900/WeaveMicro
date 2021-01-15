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
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        int Port;
        public ServiceChannel(int port)
        {
            P2Server.resttime = 0;
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

                //Type tt = keyValuePairs[rpdata.Route.Replace('/', '.')];
                //Assembly ab = Assembly.GetAssembly(tt);
                //object obj = ab.CreateInstance(tt.FullName);
                object obj = keyValuePairs[rpdata.Route.Replace('/', '.')];
                Type t = obj.GetType();

                MethodInfo mi = t.GetMethod(rpdata.FunName);
                if (mi != null)
                {
                    InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                    if (myattribute != null)
                    {
                        object[] objs = rpdata.parameter;
                        //if (obj is FunctionBase && rpdata.Headers!=null)
                        //{


                        //    (obj as FunctionBase).Headers = new Dictionary<string, StringValues>() ;
                        //    foreach (String str in rpdata.Headers.Keys)
                        //    {
                        //        (obj as FunctionBase).Headers.Add(str, rpdata.Headers[str][0]);
                        //    }
                           
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
                        //DateTime dt2 = DateTime.Now;
                        //Console.WriteLine("service:" + (dt2 - P2Server.dt).TotalMilliseconds);


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

   
}
