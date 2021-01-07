using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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
    }
    public class Rpcdata<T>
    {
        //  public httpmode HttpContext { get; set; }
        public IDictionary<string, IList<String>> Headers { get; set; }
        public T parameter { get; set; }
        public string FunName { get; set; }
        public string Route { get;  set; }

        /// <summary>
        /// 0为泛型参数，1为多参数
        /// </summary>
        public FunAttribute type = 0;

    }
}
