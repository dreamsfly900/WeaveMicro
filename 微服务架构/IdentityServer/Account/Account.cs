using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Account
{
    public class Account
    {
        
        public static bool GetLogin(Dictionary<string, String> servicesDic, HttpContext context)
        {
            return true;
        }
    
    }
}
