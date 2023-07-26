
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using wRPC;
using static wRPC.FunctionBase;

namespace wRPCService
{
    public class ServiceChannel
    {
      public  Weave.Server.WeaveP2Server P2Server = new Weave.Server.WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        int Port;
      
        public ServiceChannel(int port)
        {
           // MM_BeginPeriod(1);
            P2Server.resttime = 10;
            P2Server.weaveReceiveBitEvent += P2Server_weaveReceiveBitEvent;
              Port = port;
         
           
            
        }

        private async void P2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
                 
                String rdata = GZIP.GZipDecompress(data);
                Rpcdata<Object[]> rpdata = Newtonsoft.Json.JsonConvert.DeserializeObject<Rpcdata<Object[]>>(rdata);
                if (!keyValuePairs.ContainsKey(rpdata.Route))

                {
                    P2Server.Send(soc, 0x05, GZIP.GZipCompress( "server Route error"));
                    return;
                }

                //Type tt = keyValuePairs[rpdata.Route.Replace('/', '.')];
                //Assembly ab = Assembly.GetAssembly(tt);
               // object obj = ab.CreateInstance(tt.FullName);
                object obj = keyValuePairs[rpdata.Route];
                Type t = obj.GetType();
                obj = t.Assembly.CreateInstance(t.FullName);
                MethodInfo mi = t.GetMethod(rpdata.FunName);
                if (mi != null)
                {
                    InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                    if (myattribute != null)
                    {
                        object[] objs = rpdata.parameter;
                        if (obj is FunctionBase)
                        {
                            (obj as FunctionBase).P2Server = P2Server;
                            (obj as FunctionBase).soc = soc;
                        }
                        if (obj is FunctionBase && rpdata.Headers != null)
                        {


                            (obj as FunctionBase).Headers = rpdata.Headers;
                             

                        }
                        if (obj is FunctionBase && rpdata.Cookies != null)
                        {


                             
                            (obj as FunctionBase).Cookies = rpdata.Cookies;

                        }
                        if (obj is FunctionBase && rpdata.Filedata != null)
                        {



                            (obj as FunctionBase).Filedata = rpdata.Filedata;

                        }
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
                        AsyncStateMachineAttribute Asyncmyattribute = (AsyncStateMachineAttribute)Attribute.GetCustomAttribute(mi, typeof(AsyncStateMachineAttribute));
                        object rpcdata;
                        if (Asyncmyattribute != null)
                        {
                            var task = (mi.Invoke(obj, objs) as Task);
                            //  task.Wait();
                            var resultProperty = task.GetType().GetProperty("Result");

                            rpcdata = resultProperty.GetValue(task);
                            //tassk.Wait();
                            //rpcdata = tassk.Result;
                        }
                        else
                        {
                           
                            rpcdata = mi.Invoke(obj, objs);
                          
                          
                        }
                        if (rpcdata != null)
                        {
                            String tmpdata = Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata);
                           
                            int sendlen = 1024 * 1024;
                            if (tmpdata.Length > sendlen)
                            {
                                int lern = (tmpdata.Length / sendlen);
                                int lerna = (tmpdata.Length % sendlen) > 0 ? 1 : 0;
                                for (int sa = 0; sa < lern + lerna; sa++)
                                {
                                    int sylen = sendlen;
                                    int sylen2 = tmpdata.Length - sa * sendlen;
                                    if (sylen2 < sylen)
                                        sylen = sylen2;
                                    byte[] outdata = GZIP.GZipCompress(tmpdata.Substring(sa * sendlen, sylen));

                                    P2Server.Send(soc, 0x11, outdata);
                                }
                                P2Server.Send(soc, 0x10, GZIP.GZipCompress("成功"));
                            }else
                            P2Server.Send(soc, 0x01, GZIP.GZipCompress(tmpdata));
                        }else
                            P2Server.Send(soc, 0x10, GZIP.GZipCompress("成功"));

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
