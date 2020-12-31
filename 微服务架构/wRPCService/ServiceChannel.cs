using System;
using System.Collections.Concurrent;
using System.Reflection;
using wRPC;

namespace wRPCService
{
    public class ServiceChannel
    {
        Weave.Server.WeaveP2Server P2Server = new Weave.Server.WeaveP2Server(Weave.Base.WeaveDataTypeEnum.Bytes);
        ConcurrentDictionary<string, Assembly> keyValuePairs = new ConcurrentDictionary<string, Assembly>();
        int Port;
        public ServiceChannel(int port)
        {
            P2Server.weaveReceiveBitEvent += P2Server_weaveReceiveBitEvent;
              Port = port;
            keyValuePairs=ToolLoad.Load(keyValuePairs);
        }

        private void P2Server_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            String rdata=GZIP.GZipDecompress(data);
            Rpcdata<Object[]>  rpdata=Newtonsoft.Json.JsonConvert.DeserializeObject<Rpcdata<Object[]>>(rdata);
            object obj = keyValuePairs[rpdata.Route.Replace('/', '.')].CreateInstance(rpdata.Route.Replace('/','.'));
            Type t = obj.GetType();
            MethodInfo mi = t.GetMethod(rpdata.FunName);
            if (mi != null)
            {
                InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));
                if (myattribute != null)
                {

                }
            }
            object[] objs = rpdata.parameter;
            if (rpdata.type != FunAttribute.RPC)
            { 
               objs = new object[rpdata.parameter.Length + 1];
                int i = 0;
                foreach (object oo in rpdata.parameter)
                {
                    objs[i] = oo;
                    i++;
                }
                objs[objs.Length - 1] = rpdata.HttpContext;

            }
            object rpcdata = mi.Invoke(obj, objs);
            byte[] outdata = GZIP.GZipCompress(Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata));
            P2Server.Send(soc, 0x01, outdata);
        }

        public void Start()
        {
           
            P2Server.Start(Port);
        }
    }

   public class ToolLoad
    {
        public static ConcurrentDictionary<string, Assembly> Load(ConcurrentDictionary<string, Assembly> keyValuePairs)
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
                        MethodInfo[] mis = tt.GetMethods();
                        foreach (MethodInfo mia in mis)
                        {

                            InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mia, typeof(InstallFunAttribute));
                            if (myattribute != null)
                            {
                                if (!keyValuePairs.ContainsKey(mia.DeclaringType.FullName))
                                    keyValuePairs.TryAdd(mia.DeclaringType.FullName, assembly);
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
