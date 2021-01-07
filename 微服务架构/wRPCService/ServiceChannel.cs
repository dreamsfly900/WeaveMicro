using System;
using System.Collections.Concurrent;
using System.Reflection;
using wRPC;

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
            String rdata=GZIP.GZipDecompress(data);
            Rpcdata<Object[]>  rpdata=Newtonsoft.Json.JsonConvert.DeserializeObject<Rpcdata<Object[]>>(rdata);
            if (!keyValuePairs.ContainsKey(rpdata.Route.Replace('/', '.')))
            
            {
                P2Server.Send(soc, 0x05, "server Route error");
                return;
            }
               
            Type tt = keyValuePairs[rpdata.Route.Replace('/', '.')];
            Assembly ab=Assembly.GetAssembly(tt);
            object obj = ab.CreateInstance(tt.FullName);
            //Type t = obj.GetType();
          
             MethodInfo mi = tt.GetMethod(rpdata.FunName);
            if (mi != null)
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute != null)
                {
                    object[] objs = rpdata.parameter;
                    if (obj is FunctionBase)
                    {
                        (obj as FunctionBase).HttpContext = rpdata.HttpContext;
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

                    object rpcdata = mi.Invoke(obj, objs);
                    byte[] outdata = GZIP.GZipCompress(Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata));
                    P2Server.Send(soc, 0x01, outdata);

                }
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
    }
}
