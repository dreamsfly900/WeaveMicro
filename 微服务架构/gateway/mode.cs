using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace gateway
{ 
  public  class httpmode
    {
        public IQueryCollection Query { get; set; }
        public IFormCollection From { get; set; }

        public IHeaderDictionary Headers { get; set; }
    }
}
