﻿using Microsoft.AspNetCore.Http;
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
        public delegate void recdata(string data);
        public recdata recs = null;
        public delegate void recdataStream(byte[] data);
        public recdataStream recsStream = null;
        String IP;
        int Port;
        Weave.Client.TcpSynClient tcpSynClient;
        public Dictionary<string, String> Headers { get; set; }
        public Dictionary<string, String> Cookies { get; set; }
        public filedata Filedata { get; set; }
        bool isline = false;
        public ClientChannel(String _IP, int port)
        {
            IP = _IP;
            Port = port;
            tcpSynClient = new TcpSynClient(Weave.Client.DataType.bytes, IP, Port);
            isline = tcpSynClient.Start();
            
            if (!isline)
                throw new Exception("无法连接服务器");
        }
       
        public bool connection()
        {
          
            return isline=tcpSynClient.Start();

        }
        public bool IsLine()
        {
            
            return isline;
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
        DateTime dt;
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
            rpcdata.Headers = Headers;
            rpcdata.Cookies = Cookies;
            if (parameter is object[])
                rpcdata.parameter = parameter as object[];
            else
             rpcdata.parameter =new object[] { parameter };
            rpcdata.Filedata = Filedata;
            rpcdata.FunName = callfun;
            rpcdata.Route = Route;
            rpcdata.type = 1;
            
            string datastr = Newtonsoft.Json.JsonConvert.SerializeObject(rpcdata);
            return datastr;
        }
        myreceivebitobj funobj;
        String call(String datastr)
        {
            funobj = new myreceivebitobj(reca);


            if (!isline)
                isline= connection();
            if (isline)
            {
                if (tcpSynClient.Send(0x01, GZIP.GZipCompress(datastr)))
                {
                   
                   
                    
                    while (true)
                    { dt = DateTime.Now;
                        var commdatas = tcpSynClient.ReceiveList(funobj);
                        if(commdatas==null)
                            return "";
                        foreach (var commdata in commdatas)
                        {
                            DateTime dt3 = DateTime.Now;
                            //   Console.WriteLine("Receives:" + (dt3 - dt).TotalMilliseconds);
                            //   Console.WriteLine("call:" + (dt2 - dt).TotalMilliseconds);
                            if (commdata == null)
                                throw new Exception("通信意外！");
                            if (commdata.comand == 0x01)
                            {
                                DateTime dt2 = DateTime.Now;
                                Console.WriteLine("0x01:" + (dt2 - dt).TotalMilliseconds);
                                return GZIP.GZipDecompress(commdata.data);


                            }
                            if (commdata.comand == 0x10)
                            {
                                DateTime dt2 = DateTime.Now;
                                Console.WriteLine("0x10:" + (dt2 - dt).TotalMilliseconds);
                                return "";
                            }
                            //if (commdata.comand == 0x11) { 

                            //    recs(GZIP.GZipDecompress(commdata.data));
                            //}
                            //if (commdata.comand == 0x12) { recsStream(GZIP.Decompress(commdata.data)); }
                            else if (commdata.comand == 0x2)
                            {
                                throw new Exception(GZIP.GZipDecompress(commdata.data));

                            }
                            else if (commdata.comand == 0x5)
                            {
                                throw new Exception(GZIP.GZipDecompress(commdata.data));

                            }
                        }
                    }
                   
                   
                }

            }
            
            return "无法发送到服务器";
        }

        private void reca(byte command, byte[] data, TcpSynClient soc)
        {
           
           
            if (command == 0x11)
            {

                recs(GZIP.GZipDecompress(data));
            }
            if (command == 0x12) { recsStream(GZIP.Decompress(data)); }
            
        }

        public void Dispose()
        {
            isline = false;
            tcpSynClient.Stop();
        }
    }
}
