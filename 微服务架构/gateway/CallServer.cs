using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wRPCclient;
using wRPC;
using static wRPCclient.ClientChannel;

namespace gateway
{
    public class CallServer
    {
      static  ConcurrentDictionary<string, ClientChannelQueue> _serviceDic = new ConcurrentDictionary<string, ClientChannelQueue>();
        public static void heartbeat()
        {
          
          //  System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(keep),null);
        }
         
        public static string CallService(server ser, String rt, String rls, object[] objs,
            Dictionary<string, String> Headers, Dictionary<string, String> keysCookies, ClientChannel.recdata rec=null, wRPCclient.filedata fd =null)
        {
            bool locked = false;
            wRPCclient.ClientChannel clientChannel = null;
            
            ClientChannelQueue CCQ = null;
           
            try
            {
                
                clientChannel = new wRPCclient.ClientChannel(ser.IP, ser.Port);
                CCQ = new ClientChannelQueue();
                CCQ.clientChannel = clientChannel;
                clientChannel.Headers = Headers;
                clientChannel.Cookies = keysCookies;
               
                if (fd != null) {
                    clientChannel.Filedata = fd;
                  
                }
                clientChannel.recs = rec;
                if (rec != null)
                {
                    clientChannel.Call<object>(rt, rls, objs);
                    return "";
                }
                else
                {
                    var data = clientChannel.Call<object>(rt, rls, objs);
                    return Newtonsoft.Json.JsonConvert.SerializeObject(data);
                }
            }
            catch (Exception e)
            {
                //if (!CCQ.clientChannel.connection())
                //{

                //}
               // _serviceDic.TryRemove(ser.IP + ":" + ser.Port, out CCQ);

                return JsonConvert.SerializeObject(new { code = 503, msg = "服务器错误" });
            }
            finally
            {
                if(clientChannel!=null)
                clientChannel.Dispose();
                
            }
            
            return JsonConvert.SerializeObject(new { code = 503, msg = "服务器错误" });
        }
    }
    public class ClientChannelQueue
    {
        public ClientChannel clientChannel;
        public  SpinLock _spinLock = new SpinLock();
    }
}
