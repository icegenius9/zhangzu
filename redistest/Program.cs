using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace redistest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Install-Package ServiceStack.Redis
            //memcache是保存在内存中(运行效率更高但服务器重启memcache中的数据都没了,redis则数据还在),redis是保存在硬盘中,数据就保存在dump.rdb中
            PooledRedisClientManager redisMgr = new PooledRedisClientManager("127.0.0.1");
            //拿到一个连接
            using (IRedisClient redisClient = redisMgr.GetClient())
            {
                var p = new Person { Id = 3, Name = "yzk" };
                //redisClient.Set("p", p,DateTime.Now.AddSeconds(3));//3秒过期
                //Set(Key,value,过期时间)
                redisClient.Set("p3", p, TimeSpan.FromSeconds(3));//3秒过期
                Thread.Sleep(2000);
                var p1 = redisClient.Get<Person>("p3");
                Console.WriteLine(p1.Name);
                Thread.Sleep(2000);
                p1 = redisClient.Get<Person>("p3");
                Console.WriteLine(p1.Name);
            }
        }
    }
}
