using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

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
        public IDictionary<string, StringValues> Headers { get; set; }
        public service[] GetService()
        {
            List<service> listservice = new List<service>();
            Type tt = this.GetType();
            MethodInfo[] mis = tt.GetMethods();
            foreach (MethodInfo mi in mis)
            {

                if (mi != null)
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
                            serv.Route = RouteAttr.Route+"/"+ mi.Name;
                        else
                            serv.Route = tt.FullName.Replace(".", @"/")+"/"+ mi.Name;
                        serv.annotation = myattribute.Annotation;
                        serv.Method = myattribute.Type.ToString();
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
            }

            return listservice.ToArray();
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
    }
    public class Rpcdata<T>
    {
        //  public httpmode HttpContext { get; set; }
        public IDictionary<string, IList<String>> Headers { get; set; }
        public T parameter { get; set; }
        public string FunName { get; set; }
        public string Route { get; set; }

        /// <summary>
        /// 0为泛型参数，1为多参数
        /// </summary>
        public FunAttribute type = 0;

    }
}
