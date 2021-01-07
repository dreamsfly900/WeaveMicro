using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace wRPC
{
   
  public  class httpmode
    {
        public IEnumerable<KeyValuePair<string, StringValues>> Query { get; set; }
        public IEnumerable<KeyValuePair<string, StringValues>> From { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; }
    }
    public class Rpcdata<T>
    {
        public IHeaderDictionary Headers { get; set; }
       // public httpmode HttpContext { get; set; }
        public T parameter { get; set; }
        public string FunName { get; set; }
        public string Route { get; internal set; }

        /// <summary>
        /// 0为泛型参数，1为多参数
        /// </summary>
        public byte type = 0;

    }
}
