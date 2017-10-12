using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace memcachedtest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Install-Package EnyimMemcached
            MemcachedClientConfiguration config = new MemcachedClientConfiguration();
            //Parse(服务的ip地址),代表连接哪台服务器
            config.Servers.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11211));
            //用Binary,类必须标注[Serializable]可序列化
            config.Protocol = MemcachedProtocol.Binary;
            //为了利用连接池不要using MemcachedClient ,而是使用一个static对象
            MemcachedClient client = new MemcachedClient(config);
            //创建一个对象
            var p = new Person { Id = 3, Name = "yzk" };
            //保存到缓存中
            //HttpContext.Cache.Insert(cacheKey, model, null,
            //DateTime.Now.AddSeconds(1),TimeSpan.Zero);
            // "p" + p.Id这个缓存的key,p是缓存的值
            client.Store(StoreMode.Set, "p" + p.Id, p, DateTime.Now.AddSeconds(3));//还可以指定第四个参数指定数据的过期时间。
            Thread.Sleep(2000);

            //从memcache中取出来
            Person p1 = client.Get<Person>("p3");//等价于HttpContext.Cache[cacheKey]
            Console.WriteLine(p1.Name);
            Thread.Sleep(2000);
            p1 = client.Get<Person>("p3");
            Console.WriteLine(p1.Name);
            Console.ReadKey();
        }
    }
}
