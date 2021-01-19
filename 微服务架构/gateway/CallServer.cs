using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wRPCclient;

namespace gateway
{
    public class CallServer
    {
      static  ConcurrentDictionary<string, ClientChannelQueue> _serviceDic = new ConcurrentDictionary<string, ClientChannelQueue>();
        public static void heartbeat()
        {
          //  System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(keep),null);
        }
        //public static void keep(object obj)
        //{
        //    while (true)
        //    {
        //        foreach (String key in _serviceDic.Keys)
        //        {
        //            if (_serviceDic.ContainsKey(key))
        //            {
        //                ClientChannelQueue CCQ = _serviceDic[key];
        //                bool locked = false;
        //                CCQ._spinLock.Enter(ref locked);//获取锁
                         
        //                if (locked) //释放锁
        //                    CCQ._spinLock.Exit();
        //            }
        //        }
        //        System.Threading.Thread.Sleep(3000);
        //    }
            
        //}
        //public static string CallService(server ser, String rt, String rls, object[] objs)
        //{
        //    bool locked = false;
        //    wRPCclient.ClientChannel clientChannel = null;
        //    ClientChannelQueue CCQ=null;
        //    try
        //    {

        //        if (_serviceDic.ContainsKey(ser.IP + ":" + ser.Port))
        //        {
        //            CCQ = _serviceDic[ser.IP + ":" + ser.Port];
        //            clientChannel = CCQ.clientChannel;
        //        }
        //        else
        //        {
        //            clientChannel = new wRPCclient.ClientChannel(ser.IP, ser.Port);
        //            CCQ = new ClientChannelQueue();
        //            CCQ.clientChannel = clientChannel;
        //            _serviceDic.TryAdd(ser.IP + ":" + ser.Port, CCQ);

        //        }

        //        CCQ._spinLock.Enter(ref locked);//获取锁
        //        return clientChannel.Call<String>(rt, rls, objs);
        //    }
        //    catch (Exception e)
        //    {
        //        //if (!CCQ.clientChannel.connection())
        //        //{
                    
        //        //}
        //        _serviceDic.TryRemove(ser.IP + ":" + ser.Port, out CCQ);

        //        return JsonConvert.SerializeObject(new { code = 503, msg = e.Message });
        //    }
        //    finally
        //    {
        //        CCQ.clientChannel.Dispose();
        //        if (locked) //释放锁
        //            CCQ._spinLock.Exit();
        //    }
        //    //finally
        //    //{
        //    //    if (clientChannel != null)
        //    //        clientChannel.Dispose();
        //    //}
        //    return JsonConvert.SerializeObject(new { code = 503, msg = "服务器错误" });
        //}
        public static string CallService(server ser, String rt, String rls, object[] objs)
        {
            bool locked = false;
            wRPCclient.ClientChannel clientChannel = null;

            ClientChannelQueue CCQ = null;
           
            try
            {

                clientChannel = new wRPCclient.ClientChannel(ser.IP, ser.Port);
                CCQ = new ClientChannelQueue();
                CCQ.clientChannel = clientChannel;
                return clientChannel.Call<String>(rt, rls, objs);
            }
            catch (Exception e)
            {
                //if (!CCQ.clientChannel.connection())
                //{

                //}
                _serviceDic.TryRemove(ser.IP + ":" + ser.Port, out CCQ);

                return JsonConvert.SerializeObject(new { code = 503, msg = e.Message });
            }
            finally
            {
                CCQ.clientChannel.Dispose();
                
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
