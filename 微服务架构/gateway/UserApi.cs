using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Weave.Client;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using wRPC;
using Newtonsoft.Json;
using WeaveMicroClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace gateway
{


    public static class Proccessor
    {
        private static string _noSafe = "exec|execute|insert|select|delete|" +
                                                   "declare|net user|xp_cmdshell|create|drop|grant|group_concat|column_name|" +
                                                   "information_schema.columns|table_schema|truncate|;|--|#|\'";//查询时危险字符；
        public static server[] servers;
        public static String[] filetype;
        public async static Task agent(HttpContext context)
        {
            if (context.Request.Method == "NONE")
                context.Request.Method = "POST";
            if (context.Request.Method != "GET" && context.Request.Method != "POST")
                return;
            RouteLog rlog = new RouteLog();
            wRPCclient.filedata FDATA = new wRPCclient.filedata();
            try
            {
                Dictionary<string, String> servicesDic = new Dictionary<string, String>();
                dynamic contentFromBody = "";
                //    await context.Response.WriteAsync(Encoding.GetEncoding("GB2312").ToString());
                context.Response.ContentType = "application/json;charset=utf-8";
                if (servers == null)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 999, msg = "非法请求" }));
                    return;
                }
                if (context.Request.ContentLength != null)
                {

                    var body = "";
                    var filebyte = new byte[0];

                    context.Request.EnableBuffering();
                    using (var mem = new MemoryStream())
                    using (var reader = new StreamReader(mem))
                    {
                        context.Request.Body.Seek(0, SeekOrigin.Begin);
                        await context.Request.Body.CopyToAsync(mem);
                        mem.Seek(0, SeekOrigin.Begin);

                        if (context.Request.ContentType.IndexOf("multipart/form-data") >= 0)
                        {
                            string WebKitForm = reader.ReadLine();

                            string cd = reader.ReadLine();
                            string ct = reader.ReadLine();
                            String[] Content_Disposition = cd.Split(";");
                            String[] Content_Type = ct.Split(":");
                            try
                            {
                                foreach (String sstype in Content_Disposition)
                                {
                                    if (sstype.Split("=")[0].Trim() == "filename")
                                    {
                                        FDATA.filename = sstype.Split("=")[1].Replace("\"", "");
                                        FDATA.filetype = FDATA.filename.Substring(FDATA.filename.LastIndexOf("."));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 510, msg = "传输非法文件" }));
                                //context.Abort();
                                return;
                            }
                            bool fileisok = false;
                            foreach (string ftype in filetype)
                            {
                                if (FDATA.filetype == ftype)
                                {
                                    fileisok = true;
                                    break;
                                }

                            }
                            if (!fileisok)
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 555, msg = "非法文件" }));
                                //context.Abort();
                                return;
                            }

                            string a = reader.ReadLine();
                            mem.Position =System.Text.UTF8Encoding.UTF8.GetBytes (WebKitForm + "\r\n" + cd + "\r\n" + ct + "\r\n" + "\r\n").Length ;

                            byte[] data = new byte[mem.Length - mem.Position - System.Text.UTF8Encoding.UTF8.GetBytes("\r\n" + WebKitForm + "--\r\n").Length];
                            mem.Read(data, 0, data.Length);
                            //System.IO.FileStream streamWriter = new System.IO.FileStream(FDATA.filename, System.IO.FileMode.Create);
                            //streamWriter.Write(data, 0, data.Length);
                            //streamWriter.Close();
                            // String str = reader.ReadToEnd() ;

                            //String tempstr=str.Split("\r\n" + WebKitForm+ "--")[0];
                            //filebyte = System.Text.Encoding.Default.GetBytes(tempstr);
                            FDATA.data = data;
                            //  reader.ReadLine();

                            string endWebKitForm = WebKitForm + "--";
                        }
                        else
                        {

                            body = reader.ReadToEnd();

                        }
                    }
                    if (context.Request.ContentType != null)
                    {
                        if (context.Request.ContentType.IndexOf("application/json") >= 0)
                        {
                            contentFromBody = body;
                        }
                        else if (context.Request.ContentType.IndexOf("application/x-www-form-urlencoded") >= 0)
                        {
                            contentFromBody = body.Split("&");

                            foreach (string datastr in contentFromBody)
                            {
                                servicesDic.Add(datastr.Split("=")[0], datastr.Split("=")[1]);
                            }
                        }
                        else if (context.Request.ContentType.IndexOf("multipart/form-data") >= 0)
                        {
                            if (context.Request.QueryString.ToString().Length > 1)
                            {
                                contentFromBody = context.Request.QueryString.ToString().Substring(1).Split("&");

                                foreach (string datastr in contentFromBody)
                                {
                                    servicesDic.Add(datastr.Split("=")[0], datastr.Split("=")[1]);
                                }
                            }
                        }
                    }

                }

                rlog.Route = context.Request.Path.Value.Trim('/');
                rlog.gayway = Program.applicationUrl;

                rlog.requestIP = context.Connection.RemoteIpAddress.ToString();
                server ser = await WeightAlgorithm.Get(servers, context.Request.Path.Value.Trim('/'));

                if (ser == null)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 404, msg = "非法请求" }));
                    //context.Abort();
                    return;
                }
                else
                {
                    rlog.RouteIP = ser.IP + ser.Port;
                    if (!IsAuthenticated(context, ser.services[0].Authorize))
                    {
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 401, msg = "非法请求" }));
                        // context.Abort();
                        return;
                    }

                    try
                    {
                        DateTime dt = DateTime.Now;
                        //ser.services[0].parameter
                        object[] objs = new object[0];
                        if (ser.services[0].parameter != null)
                        {
                            objs = new object[ser.services[0].parameter.Length];
                            for (int i = 0; i < objs.Length; i++)
                            {

                                if (ser.services[0].Method == "NONE")
                                {
                                    if (context.Request.ContentType != null)
                                        if (context.Request.ContentType.ToLower().Contains("application/json"))
                                        {
                                            objs[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(contentFromBody);
                                        }
                                        else if (context.Request.ContentType.ToLower() == "application/x-www-form-urlencoded")
                                        {
                                            objs[i] = servicesDic[ser.services[0].parameter[i]];
                                        }
                                }
                                else if (ser.services[0].Method.ToUpper() == "GET")
                                {
                                    if (context.Request.Query == null)
                                    {

                                    }
                                    else
                                    {
                                        objs[i] = context.Request.Query[ser.services[0].parameter[i]].ToString();
                                    }
                                }
                                else if (ser.services[0].Method.ToUpper() == "POST")
                                {
                                    if (!context.Request.HasFormContentType)
                                    {

                                    }
                                    else
                                    {
                                        if (context.Request.ContentType.ToLower().IndexOf("application/x-www-form-urlencoded") >= 0)
                                        {
                                            objs[i] = servicesDic[ser.services[0].parameter[i]];
                                        }
                                        else
                                            objs[i] = context.Request.Form[ser.services[0].parameter[i]].ToString();
                                    }
                                }
                                else if (ser.services[0].Method.ToUpper() == "FILE")
                                {
                                    if (context.Request.ContentType.IndexOf("multipart/form-data") >= 0)
                                    {
                                        if (servicesDic.Count > 0)
                                            objs[i] = servicesDic[ser.services[0].parameter[i]];
                                    }
                                }
                                if (context.Request.ContentType != null && context.Request.ContentType.ToLower().Contains("application/json"))
                                {
                                    continue;
                                }
                                else
                                {
                                    Regex reg = new Regex(_noSafe, RegexOptions.IgnoreCase);
                                    if (objs[i] != null && reg.IsMatch(objs[i].ToString()))
                                    {
                                        await context.Response.WriteAsync($" ~, {"警告！！不安全SELECT!已记录IP，等待报警"}");
                                        return;

                                    }
                                }
                            }

                        }

                        string datastr = Newtonsoft.Json.JsonConvert.SerializeObject(context.Request.Headers);

                        String rl = context.Request.Path.Value.Trim('/');
                        String[] rls = rl.Split('/');
                        rl = "";
                        for (int i = 0; i < rls.Length - 1; i++)
                        {
                            rl += rls[i] + "/";
                        }
                        rl = rl.Substring(0, rl.Length - 1);
                        String[] Headers = Startup.config.GetSection("Headers").Get<String[]>();
                        Dictionary<string, String> keysh = new Dictionary<string, string>();
                        foreach (string hh in Headers)
                        {
                            Regex reg = new Regex(_noSafe, RegexOptions.IgnoreCase);
                            if (reg.IsMatch(context.Request.Headers[hh].ToString()))
                            {
                                await context.Response.WriteAsync($" ~, {"警告！！不安全SELECT!已记录IP，等待报警"}");
                                return;

                            }
                            keysh.Add(hh, context.Request.Headers[hh]);
                        }
                        String[] Cookies = Startup.config.GetSection("Cookies").Get<String[]>();
                        Dictionary<string, String> keysCookies = new Dictionary<string, string>();

                        if (context.User.Identity.IsAuthenticated)
                        {
                            foreach (string hh in Cookies)
                            {
                                if (context.User.Claims.Count(c => c.Type == hh) > 0)
                                {

                                    keysCookies.Add(hh, context.User.Claims.Single(c => c.Type == hh).Value);
                                }
                            }
                        }
                        //  context.Request.Headers
                        //await context.Response.WriteAsync($"");
                        String retun = CallServer.CallService(ser, rl, rls[rls.Length - 1], objs, keysh, keysCookies, FDATA);
                        ////String retun =  clientChannel.Call<String>(rl, rls[rls.Length - 1], objs);
                        //Encoding utf8 = Encoding.ASCII;
                        //Encoding ISO = Encoding.UTF8;//换成你想转的编码
                        //byte[] temp = utf8.GetBytes(retun);
                        //string result = ISO.GetString(temp);
                        await context.Response.WriteAsync($"{retun}");

                        // await context.Response.WriteAsync($"{ retun}{ objs.Length},{rl},{rls[rls.Length - 1]}，{ser.ToString()},{context.Request.ContentType}");
                        DateTime dt2 = DateTime.Now;
                        rlog.time = (dt2 - dt).TotalMilliseconds.ToString();
                        //    Console.WriteLine((dt2 - dt).TotalMilliseconds);
                        return;
                        //string httpstr = Newtonsoft.Json.JsonConvert.SerializeObject(httpmode);
                    }
                    catch (Exception ex)
                    {

                        await context.Response.WriteAsync($" ~服务调用 异常");
                        return;
                    }
                    finally
                    {
                        if (Program.mc != null)
                            Program.mc.SendLog(rlog);
                    }

                }
            }
            catch (Exception e)
            { await context.Response.WriteAsync($" ~, {e.Message}"); }
            finally
            {

            }


        }

        static bool IsAuthenticated(HttpContext context, bool Authorize)
        {
            if (Convert.ToBoolean(Startup.config["Authentication"]))
            {
                if (Authorize)
                {
                    if (!context.User.Identity.IsAuthenticated)
                    {


                        // await context.Response.WriteAsync(JsonConvert.SerializeObject(new { code = 0, msg = "非法请求" }));
                        //  context.Abort();
                        return false;
                    }
                    else
                        return true;
                }
                else
                    return true;
            }
            else
                return true;
        }
    }
}
