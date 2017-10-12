using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZSZ.IService;

namespace ZSZ.FrontWeb
{
    public class MemcacheMgr
    {
        private MemcachedClient client;

        //C#6.0可以这么写，有且只有一个实例
        //public static MemcacheMgr Instance { get; private set; } = new MemcacheMgr();
        //单例模式
        public static MemcacheMgr Instance { get; private set; }

        //静态构造函数,第一次创建类的时候执行
        static MemcacheMgr()
        {
            Instance = new MemcacheMgr();
        }
        //构造函数
        private MemcacheMgr()
        {
            //手动注入
            var settingService =
                DependencyResolver.Current.GetService<ISettingService>();
            //取得到数据库中配置表的MemCachedServers对应的服务器地址(可能会有多个服务器地址)
            string[] servers
                = settingService.GetValue("MemCachedServers").Split(';');

            MemcachedClientConfiguration config =
                new MemcachedClientConfiguration();
            foreach (var server in servers)
            {
                config.Servers.Add(new IPEndPoint(IPAddress.Parse(server), 11211));
            }
            //用Binary,类必须标注[Serializable]可序列化
            config.Protocol = MemcachedProtocol.Binary;
            client = new MemcachedClient(config);
        }

        /// <summary>
        /// memcache中存数据
        /// </summary>
        /// <param name="key">memcache的key</param>
        /// <param name="value">memcache的value</param>
        /// <param name="expires">过期时间</param>
        public void SetValue(string key, object value, TimeSpan expires)
        {
            //判断类是不是可序列化
            if (!value.GetType().IsSerializable)
            {
                throw new ArgumentException("value必须是可序列化的对象");
            }
            client.Store(StoreMode.Set, key, value, expires);
        }

      /// <summary>
      /// memcache中取数据
      /// </summary>
      /// <typeparam name="T">任意类型</typeparam>
      /// <param name="key">memcache中的key</param>
      /// <returns></returns>
        public T GetValue<T>(string key)
        {
            return client.Get<T>(key);
        }
    }
}