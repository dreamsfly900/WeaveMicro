﻿ 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Weave.Server;
using wRPCService;

namespace wRPC
{
    
    public class httpmode
    {
        public httpmode()
        {
           
        }
       

        public KeyValuePair<string, String[]> From { get; set; }
        //public KeyValuePair<string, StringValues> Query { get; set; }
        //public KeyValuePair<string, StringValues> From { get; set; }
        public CookieCollection Cookies { get; set; }
        public IDictionary<string, String[]> Headers { get; set; }
    }
    public class FunctionBase
    {
        public IDictionary<string, String> Headers { get; set; }
        public Dictionary<string, string> Cookies { get; internal set; }
        public filedata Filedata { get; set; }
        public service[] GetService()
        {
            List<service> listservice = new List<service>();
            Type tt = this.GetType();
            MethodInfo[] mis = tt.GetMethods();
           
            foreach (MethodInfo mi in mis)
            {
                //if (tt.FullName == "Forecast_Query.Forecast")
                //{ 
                //    Console.WriteLine("---999-:" + mis.Length);
                //    Console.WriteLine("---999-:" + mi.Name);
                //}
                 
                if (mi != null)
                {

                 
                    try
                    {
                        InstallFunAttribute myattribute = (InstallFunAttribute)Attribute.GetCustomAttribute(mi, typeof(InstallFunAttribute));

                        if (myattribute != null)
                        {
                            service serv = new service();
                            AuthorizeAttribute Authorizeattribute = (AuthorizeAttribute)Attribute.GetCustomAttribute(mi, typeof(AuthorizeAttribute));
                            if (Authorizeattribute != null)
                                serv.Authorize = true;
                            else
                                serv.Authorize = false;

                            RouteAttribute RouteAttr = (RouteAttribute)Attribute.GetCustomAttribute(tt, typeof(RouteAttribute));
                            if (RouteAttr != null)
                                serv.Route = RouteAttr.Route + "/" + mi.Name;
                            else
                                serv.Route = tt.FullName.Replace(".", @"/") + "/" + mi.Name;
                            serv.annotation = myattribute.Annotation;
                            serv.Method = myattribute.Type.ToString();
                            serv.ContentType = myattribute.ContentType;
                            ParameterInfo[] paramsInfo = mi.GetParameters();//得到指定方法的参数列表 
                            serv.parameter = new string[paramsInfo.Length];
                            serv.parameterexplain = new string[paramsInfo.Length];
                            for (int i = 0; i < paramsInfo.Length; i++)

                            {
                                ParamAttribute ParamAttr = (ParamAttribute)Attribute.GetCustomAttribute(paramsInfo[i], typeof(ParamAttribute));
                                Type tType = paramsInfo[i].ParameterType;
                                FieldInfo[] fis = tType.GetFields();
                                foreach (FieldInfo fi in fis)
                                    if (fi.Name != "Empty")
                                        serv.parameterexplain[i] += fi.FieldType.Name + " " + fi.Name + ",";
                                    else
                                        serv.parameterexplain[i] += fi.FieldType.Name + ",";


                                if (ParamAttr != null)
                                {

                                    serv.parameterexplain[i] += "@" + ParamAttr.explain + "|";
                                }
                                else
                                    serv.parameterexplain[i] += "@|";



                                //如果它是值类型,或者String   

                                serv.parameter[i] = paramsInfo[i].Name;

                            }

                            listservice.Add(serv);
                        }

                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(mi.Name+"-特别说明：" +ee.Message);
                    }
                }
            }

            return listservice.ToArray();
        }
        public WeaveP2Server P2Server;
        public Socket soc;
        public bool PushStream(String data)
        {
            return P2Server.Send(soc,0x11, GZIP.GZipCompress(data));
          //  return false;
        }
        public bool PushStream(byte[] data)
        {
           return P2Server.Send(soc, 0x12, GZIP.Compress(data));
          //  return false;
        }
    }
    public class service
    {
        public string Route { get; set; }
        public string Method { get; set; }
        public String[] parameter { get; set; }
        public String[] parameterexplain { get; set; }
        public string annotation { get; set; }
        public bool Authorize { get;  set; }
        public string ContentType { get;  set; }
    }
    public class Rpcdata<T>
    {
        //  public httpmode HttpContext { get; set; }
        public Dictionary<string, String> Headers { get; set; }
        
        public Dictionary<string, String> Cookies { get; set; }
        public filedata Filedata { get; set; }
        public T parameter { get; set; }
        public string FunName { get; set; }
        public string Route { get; set; }
      
        /// <summary>
        /// 0为泛型参数，1为多参数
        /// </summary>
        public FunAttribute type = 0;

    }
    public class filedata
    {
        public string filename { get; set; }
        public byte[] data { get; set; }
        public string filetype { get; set; }
    }
}
