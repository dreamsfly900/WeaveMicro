using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public ClientChannel(String _IP, int port)
        {
            IP = _IP;
            Port = port;
            tcpSynClient = new TcpSynClient(Weave.Client.DataType.bytes, IP, Port);
            if (!tcpSynClient.Start())
                throw new Exception("无法连接服务器");
        }
        public async Task<T2> Call<T1, T2>(String Route, string callfun, T1 parameter, HttpContext context = null)
        {

            string datastr = setRpcdata<T1>(Route, callfun, parameter, context);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T2>(await call(datastr));


        }
        public async Task<String> Call<T>(String Route, string callfun, T parameter, HttpContext context = null)
        {

            string datastr = setRpcdata<T>(Route, callfun, parameter, context);

            return await call(datastr);


        }
        public async Task<String> Call(String Route, string callfun, HttpContext context = null, params object[] parameter)
        {


            string datastr = setRpcdata<object[]>(Route, callfun, parameter, context, 1);

            return await call(datastr);


        }
        public async Task<T> Call<T>(String Route, string callfun, HttpContext context = null, params object[] parameter)
        {


            string datastr = setRpcdata<object[]>(Route, callfun, parameter, context, 1);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(await call(datastr));


        }
        String setRpcdata<T>(String Route, String callfun, T parameter, HttpContext context, int type = 0)
        {
            Rpcdata<object[]> rpcdata = new wRPC.Rpcdata<object[]>();
            httpmode httpmode = new httpmode();
            if (context != null)
            {
                if (context.Request.Headers!=null)
                httpmode.Headers = context.Request.Headers;
                if (context.Request.Form != null)
                    httpmode.From = context.Request.Form;
                if (context.Request.Query != null)
                    httpmode.Query = context.Request.Query;
            }
            rpcdata.HttpContext = httpmode;
            rpcdata.parameter =new object[] { parameter };
            rpcdata.FunName = callfun;
            rpcdata.Route = Route;
            rpcdata.type = 1;
            string datastr = Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata);
            return datastr;
        }
        async Task<String> call(String datastr)
        {
            tcpSynClient.Send(0x01, GZIP.GZipCompress(datastr));
            var commdata = await tcpSynClient.Receives(null);
            if (commdata == null)
                throw new Exception("通信意外！");
            if (commdata.comand == 0x01)
            {
                return GZIP.GZipDecompress(commdata.data);
            }
            else
                throw new Exception(GZIP.GZipDecompress(commdata.data));
        }
        public void Dispose()
        {
            tcpSynClient.Stop();
        }
    }
}
