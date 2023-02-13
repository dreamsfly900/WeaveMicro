using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using wRPC;

namespace WeaveDoc
{
    /// <summary>
    /// 网关信息，与框架保持一致
    /// </summary>
    internal class Gateway
    {
        public string Sid { get; set; }
        public string IP { get; set; }
        public int port { get; set; }
        public Socket socket { get; set; }
    }
    /// <summary>
    /// Api服务信息，与框架保持一致
    /// </summary>
    internal class Server
    {
        public String Name { get; set; }
        public String IP { get; set; }
        public int Port { get; set; }
        public service[] services { get; set; }
    }
    /// <summary>
    /// 接口文档附属数据模型
    /// </summary>
    internal class DocInfoData
    {
        /// <summary>
        /// 网关列表
        /// </summary>
        public List<Gateway> Gateways { get; set; } = new List<Gateway>();
        /// <summary>
        /// 认证服务列表，当前未启用。应该从网关中获取或者配置文件获取
        /// </summary>
        public List<string> AuthServers { get; set; } = new List<string>();
    }
    /// <summary>
    /// Swagger 自定配置类
    /// </summary>
    internal class ConfigManage
    {
        /// <summary>
        /// 读取根目录下的json文件，转换为指定的模型类
        /// </summary>
        /// <typeparam name="T">指定的模型类型</typeparam>
        /// <param name="file">json文件名，不包括路径（固定路径根目录下）,如：temp.json</param>
        /// <returns>指定的模型类</returns>
        private static T GetJson<T>(string file) where T : new()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + file;
            if (!File.Exists(path)) return new T();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }
        /// <summary>
        /// 网关信息列表
        /// </summary>
        public static IEnumerable<Gateway> Gateways { get; } = GetJson<List<Gateway>>("gateway.json");
        /// <summary>
        /// API服务信息列表
        /// </summary>
        public static IEnumerable<Server> Servers { get; } = GetJson<List<Server>>("temp.json");
        /// <summary>
        /// 认证服务列表，当前未启用。
        /// <code>
        /// //应该从网关中获取或者配置文件获取
        /// //1.如果网关保存了认证服务器
        /// Gateways.ForEach(o => list.Add(o.AuthServer));
        /// //2.如果保存在单独的配置文件中
        /// GetJson&lt;List&lt;string&gt;&gt;("auth.json");
        /// //3.也可以保存到config.json中
        /// </code>
        /// </summary>
        public static IEnumerable<string> AuthServers { get; } = GetJson<List<string>>("auth.json");
        /// <summary>
        /// API文档资源列表，每个服务一条数据
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> Resources
        {
            get
            {
                //要传送的参数对象转json并Base64编码，方便传输；传输的参数包括网关和认证服务列表
                string base64 = Convert.ToBase64String(
                                    Encoding.UTF8.GetBytes(
                                        JsonSerializer.Serialize(
                                            new { Gateways, AuthServers })));
                //文档地址使用第一个网关传送
                var gateway = Gateways.FirstOrDefault();
                //防止网关未启动
                if (gateway == null) return new List<KeyValuePair<string, string>>();
                return Servers.Select(d => new KeyValuePair<string, string>(d.Name
                    , $"http://{gateway.IP}:{gateway.port}/{d.Name}/swagger/info?data={base64}"));
            }
        }
    }
}