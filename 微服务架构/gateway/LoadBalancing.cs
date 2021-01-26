using gateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wRPC
{
    //class Program
    //{ 

    //    static void Main(string[] args)
    //    {
    //        List<ServiceCenterModel> list = new List<ServiceCenterModel>();
    //        ServiceCenterModel service = new ServiceCenterModel();
    //        service.Url = "127.0.0.1";
    //        service.port = 9001;
    //        service.ServiceTags = new string[] { "aabbcc" };
    //        service.weight = 1;
    //        list.Add(service);
    //        service = new ServiceCenterModel();
    //        service.Url = "127.0.0.2";
    //        service.port = 9002;
    //        service.weight = 2;
    //        service.ServiceTags = new string[] { "aabbcc" };
    //        list.Add(service);
    //        service = new ServiceCenterModel();
    //        service.Url = "127.0.0.3";
    //        service.port = 9003;
    //        service.weight = 3;
    //        service.ServiceTags = new string[] { "aabbcc" };
    //        list.Add(service);
    //        ServiceCenterModel sm = WeightAlgorithm.Get(list, "aabbcc");
    //    }
    //}
 
    internal class ServiceCenterModel
    {
        public string Url { get; set; }
        public int port { get; set; }
        public int weight { get; set; }
        public string[] ServiceTags { get; set; }
    }
        /// <summary>
        /// 权重
        /// </summary>
      internal class WeightAlgorithm
    {
        private static server serviceCenter = new server();
        public static ConcurrentDictionary<string, WeightAlgorithmItem> _serviceDic = new ConcurrentDictionary<string, WeightAlgorithmItem>();
        private static SpinLock _spinLock = new SpinLock();
        public static async Task<server>  Get(server[] serviceList, string serviceName)
        {
            
            if (serviceList == null)
                return null;
            //if (serviceList.Length == 1)
            //    return serviceList[0];

            //bool locked = false;
            //_spinLock.Enter(ref locked);//获取锁

            WeightAlgorithmItem weightAlgorithmItem = null;
            if (!_serviceDic.ContainsKey(serviceName ))
            {
                weightAlgorithmItem = new WeightAlgorithmItem()
                {
                    Index = -1,
                    Urls = new List<string>(),
                    Port = new List<int>()
                };
                BuildWeightAlgorithmItem(weightAlgorithmItem, serviceList, serviceName);
                if (weightAlgorithmItem.Urls.Count == 0)
                    return null;
                _serviceDic.TryAdd(serviceName, weightAlgorithmItem);
            }
            else
            {
                _serviceDic.TryGetValue(serviceName , out weightAlgorithmItem);
                //weightAlgorithmItem.Urls.Clear();
                //weightAlgorithmItem.Port.Clear();
                //BuildWeightAlgorithmItem(weightAlgorithmItem, serviceList, serviceName, Method);
            }

            string url = string.Empty;
            ++weightAlgorithmItem.Index;
          
            if (weightAlgorithmItem.Index > weightAlgorithmItem.Urls.Count - 1) //当前索引 > 最新服务最大索引
            {
                weightAlgorithmItem.Index = 0;
                serviceCenter.IP = weightAlgorithmItem.Urls[0];
                serviceCenter.Port = weightAlgorithmItem.Port[0];
                serviceCenter.services = new service[] { weightAlgorithmItem.servic };
                //url = serviceList[0].Url;
            }
            else
            {
               // url = weightAlgorithmItem.Urls[weightAlgorithmItem.Index];
                serviceCenter.IP = weightAlgorithmItem.Urls[weightAlgorithmItem.Index];
                serviceCenter.Port = weightAlgorithmItem.Port[weightAlgorithmItem.Index];
                serviceCenter.services = new service[] { weightAlgorithmItem.servic };
            }
            _serviceDic[serviceName ] = weightAlgorithmItem;

            //  Console.WriteLine(serviceName + "-----" + url);
            //if (locked) //释放锁
            //    _spinLock.Exit();
            return serviceCenter;
        }

        private static void BuildWeightAlgorithmItem(WeightAlgorithmItem weightAlgorithmItem, 
            server[] serviceList,string serviceName)
        {
            //serviceList.ForEach(service => //有几个权重就加几个实例
            //{
            foreach (server servic in serviceList)
            {
                int weight = 1;
                if (servic.services != null && servic.services.Length > 0)
                {
                    if (servic.servicesDic.ContainsKey(serviceName))
                    {
                       // if(servic.servicesDic[serviceName].Method == Method)
                        { 
                             weight = servic.weight;
                            for (int i = 0; i < weight; i++)
                            {
                                weightAlgorithmItem.Urls.Add(servic.IP);
                                weightAlgorithmItem.Port.Add(servic.Port);
                                weightAlgorithmItem.servic = servic.servicesDic[serviceName];
                            }
                        }
                    }
                    //foreach (service seric in servic.services)  
                    //{ 
                    //   if(seric.Route+ seric.funname)
                    //}
                    //int.TryParse(service.ServiceTags[0], out weight);//获取权重值

                }
                
            }
            //});
        }
    }

    internal class WeightAlgorithmItem
    {
        public List<string> Urls { get; set; }
        public List<int> Port { get; set; }
        public service servic;

        public int Index { get; set; }
    }
}
