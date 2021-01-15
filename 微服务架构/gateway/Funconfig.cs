using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wRPC;

namespace gateway
{
    public static class Funconfig
    {
        static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("funconfig.json");



        public static server[] getConfig()
        {
            server[] serverlist = new server[0];
            var config = builder.Build();
            serverlist = config.GetSection("server").Get<server[]>();
            if (serverlist != null)
                foreach (server ser in serverlist)
                {
                    foreach (service serice in ser.services)
                    {
                        if (!ser.servicesDic.ContainsKey(serice.Route))
                            ser.servicesDic.GetOrAdd(serice.Route, serice);
                    }
                }
            return serverlist;
        }
    }
}
