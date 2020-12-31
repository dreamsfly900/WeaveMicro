using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
    public class LBconfig
    {
        public String ip { get; set; }
        public int port { get; set; }
        public service[] services { get; set; }

    }
    public class service
    {
        public String funname { get; set; }
        public String[] parameter { get; set; }
        public string annotation { get; set; }
        public string Method { get; set; }


    }
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
        private static ServiceCenterModel serviceCenter = new ServiceCenterModel();
        private static ConcurrentDictionary<string, WeightAlgorithmItem> _serviceDic = new ConcurrentDictionary<string, WeightAlgorithmItem>();
        private static SpinLock _spinLock = new SpinLock();
        public static ServiceCenterModel Get(List<ServiceCenterModel> serviceList, string serviceName)
        {
            if (serviceList == null)
                return null;
            if (serviceList.Count == 1)
                return serviceList[0];

            bool locked = false;
            _spinLock.Enter(ref locked);//获取锁

            WeightAlgorithmItem weightAlgorithmItem = null;
            if (!_serviceDic.ContainsKey(serviceName))
            {
                weightAlgorithmItem = new WeightAlgorithmItem()
                {
                    Index = -1,
                    Urls = new List<string>(),
                    Port = new List<int>()
                };
                BuildWeightAlgorithmItem(weightAlgorithmItem, serviceList);
                _serviceDic.TryAdd(serviceName, weightAlgorithmItem);
            }
            else
            {
                _serviceDic.TryGetValue(serviceName, out weightAlgorithmItem);
                weightAlgorithmItem.Urls.Clear();
                weightAlgorithmItem.Port.Clear();
                BuildWeightAlgorithmItem(weightAlgorithmItem, serviceList);
            }

            string url = string.Empty;
            ++weightAlgorithmItem.Index;
          
            if (weightAlgorithmItem.Index > weightAlgorithmItem.Urls.Count - 1) //当前索引 > 最新服务最大索引
            {
                weightAlgorithmItem.Index = 0;
                serviceCenter.Url= serviceList[0].Url;
                serviceCenter.port = serviceList[0].port;
                //url = serviceList[0].Url;
            }
            else
            {
               // url = weightAlgorithmItem.Urls[weightAlgorithmItem.Index];
                serviceCenter.Url = weightAlgorithmItem.Urls[weightAlgorithmItem.Index];
                serviceCenter.port = weightAlgorithmItem.Port[weightAlgorithmItem.Index];

            }
            _serviceDic[serviceName] = weightAlgorithmItem;

            Console.WriteLine(serviceName + "-----" + url);
            if (locked) //释放锁
                _spinLock.Exit();
            return serviceCenter;
        }

        private static void BuildWeightAlgorithmItem(WeightAlgorithmItem weightAlgorithmItem, List<ServiceCenterModel> serviceList)
        {
            serviceList.ForEach(service => //有几个权重就加几个实例
            {
                int weight = 1;
                if (service.ServiceTags != null && service.ServiceTags.Length > 0)
                {
                    //int.TryParse(service.ServiceTags[0], out weight);//获取权重值
                    weight = service.weight;
                }
                for (int i = 0; i < weight; i++)
                {
                    weightAlgorithmItem.Urls.Add(service.Url);
                    weightAlgorithmItem.Port.Add(service.port);
                }
            });
        }
    }

    internal class WeightAlgorithmItem
    {
        public List<string> Urls { get; set; }
        public List<int> Port { get; set; }
        public int Index { get; set; }
    }
}
