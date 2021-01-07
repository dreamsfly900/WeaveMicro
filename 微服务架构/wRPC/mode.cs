using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace wRPC
{
   
  public  class httpmode
    {
        public IQueryCollection Query { get; set; }
        public IFormCollection From { get; set; }

        public IHeaderDictionary Headers { get; set; }
    }
    public class Rpcdata<T>
    {
         
        public httpmode HttpContext { get; set; }
        public T parameter { get; set; }
        public string FunName { get; set; }
        public string Route { get; internal set; }

        /// <summary>
        /// 0为泛型参数，1为多参数
        /// </summary>
        public byte type = 0;

    }
}
