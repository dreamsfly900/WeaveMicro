using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using wRPC;

namespace gateway
{ 
  public  class httpmode
    {
        public IQueryCollection Query { get; set; }
        public IFormCollection From { get; set; }

        public IHeaderDictionary Headers { get; set; }
    }
   
    public class server
    {
        public int weight=1;

        public String IP { get; set; }
        public int Port { get; set; }
        public service[] services { get; set; }

        public ConcurrentDictionary<string, service> servicesDic = new ConcurrentDictionary<string, service>();
    }
    
}
