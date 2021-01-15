using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Weave.Client;
using wRPC;

namespace wRPCclient
{


    public class ClientChannel : IDisposable
    {
        String IP;
        int Port;
        Weave.Client.TcpSynClient tcpSynClient;
        public IHeaderDictionary Headers { get; set; }
        bool isline = false;
        public ClientChannel(String _IP, int port)
        {
            IP = _IP;
            Port = port;
            tcpSynClient = new TcpSynClient(Weave.Client.DataType.bytes, IP, Port);
            isline = tcpSynClient.Start();
            if(!isline)
                throw new Exception("无法连接服务器");
        }
        public bool connection()
        {
            return isline=tcpSynClient.Start();
        }
        public bool IsLine()
        {
            bool b=tcpSynClient.Start();
            tcpSynClient.Stop();
            return b;
        }
        public  T2 Call<T1, T2>(String Route, string callfun, T1 parameter)
        {

            string datastr = setRpcdata(Route, callfun, parameter, null);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(call(datastr));


        }
        public  String Call<T>(String Route, string callfun, T parameter)
        {

            string datastr = setRpcdata(Route, callfun, parameter, null);

            return  call(datastr);


        }
        public  String Call(String Route, string callfun, params object[] parameter)
        {


            string datastr = setRpcdata(Route, callfun, parameter, null, 1);

            return  call(datastr);


        }
        public  T Call<T>(String Route, string callfun, params object[] parameter)
        {


            string datastr = setRpcdata(Route, callfun, parameter, null, 1);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>( call(datastr));


        }
        public  T Call<T>(String Route, string callfun, dynamic parameter, HttpContext context = null)
        {


            string datastr = setRpcdata(Route, callfun, parameter  , context, 1);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>( call(datastr));


        }
        String setRpcdata(String Route, String callfun, object parameter, HttpContext context, int type = 0)
        {
            
            Rpcdata<object[]> rpcdata = new wRPC.Rpcdata<object[]>();
            //httpmode httpmode = new httpmode();
            //if (context != null)
            //{
            //    if (context.Request.Headers!=null)
            //   // httpmode.Headers = Headers;
            //    if (context.Request.HasFormContentType )
            //        httpmode.From = context.Request.Form;
            //    if (context.Request.Query != null)
            //        httpmode.Query = context.Request.Query;
            //}
            //rpcdata.Headers = Headers;
            if (parameter is object[])
                rpcdata.parameter = parameter as object[];
            else
          rpcdata.parameter =new object[] { parameter };
            rpcdata.FunName = callfun;
            rpcdata.Route = Route;
            rpcdata.type = 1;
            string datastr = Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata);
            return datastr;
        }
        String call(String datastr)
        {
            DateTime dt = DateTime.Now;
            if (!isline)
                isline= connection();
            if (isline)
            {
                if (tcpSynClient.Send(0x01, GZIP.GZipCompress(datastr)))
                {
                  
                    var commdata = tcpSynClient.Receives(null);
                    DateTime dt2 = DateTime.Now;
                    //Console.WriteLine((dt2 - dt).TotalMilliseconds);
                    if (commdata == null)
                        throw new Exception("通信意外！");
                    if (commdata.comand == 0x01)
                    {
                        return GZIP.GZipDecompress(commdata.data);
                    }
                    else
                        throw new Exception(GZIP.GZipDecompress(commdata.data));
                }
            }
            return "无法发送到服务器";
        }
        
        public void Dispose()
        {
            isline = false;
            tcpSynClient.Stop();
        }
    }
}
