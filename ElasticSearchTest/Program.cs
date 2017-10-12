using PlainElastic.Net;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace estest1
{
    //Install-Package PlainElastic.Net(elasticSearch的包)
    class Program
    {
        /// <summary>
        /// elasticSearch(搜索下面方法中添加的数据)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            SearchCommand cmd = new SearchCommand("verycd", "items");
            //首先创建我要查询的结果(VerycdItem)
            var query = new QueryBuilder<VerycdItem>()
            .Query(b =>
                    b.Bool(m =>
                    //并且关系
                    m.Must(t =>
                       //分词的最小单位或关系查询   //DefaultField是模糊匹配,Term是精确匹配(int,long类型),Must是必须(and),Should是或者(or)，MustNot是否定(不能含有)
                       t.QueryString(t1 => t1.DefaultField("content").Query("成龙"))
                         ).Must(t => t.QueryString(t1 => t1.DefaultField("category2").Query("纪录")))
                      )
                    ).Size(100)//默认返回10条,Size(100)表示返回最多100条
            .Build();
            //elasticSearch中没有更新操作,先删在加
            // DeleteCommand delCmd = new DeleteCommand()
            //得到查询结果
            var result = client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            //把查询结果序列化
            var searchResult = serializer.ToSearchResult<VerycdItem>(result);
            //searchResult.hits.total; //一共有多少匹配结果  10500
            // searchResult.Documents;//当前页的查询结果 
            foreach (var doc in searchResult.Documents)
            {
                Console.WriteLine(doc.title + "," + doc.category1 + "," + doc.category2);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 把sqllite中的数据存入elasticSearch中
        /// </summary>
        /// <param name="args"></param>
        static void Main3(string[] args)
        {
            //安装sqlLite的ado.net sdk:Install-Package System.Data.SQLite.Core
            //创建SQLite数据库连接(直接连接文件地址)
            using (SQLiteConnection conn
                = new SQLiteConnection(@"Data Source=C:\Users\Administrator\Desktop\掌上租项目\电驴数据库（全部电影）\verycd.sqlite3.db"))
            {
                //打开数据库连接
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from verycd";
                    //读取数据
                    using (var reader = cmd.ExecuteReader())
                    {
                        //连接elasticSearch
                        ElasticConnection client = new ElasticConnection("localhost", 9200);
                        var serializer = new JsonNetSerializer();
                        while (reader.Read())
                        {
                            //将读取的数据赋值
                            long verycdid = reader.GetInt64(reader.GetOrdinal("verycdid"));
                            string title = reader.GetString(reader.GetOrdinal("title"));
                            string status = reader.GetString(reader.GetOrdinal("status"));
                            string brief = reader.GetString(reader.GetOrdinal("brief"));
                            string pubtime = reader.GetString(reader.GetOrdinal("pubtime"));
                            string updtime = reader.GetString(reader.GetOrdinal("updtime"));
                            string category1 = reader.GetString(reader.GetOrdinal("category1"));
                            string category2 = reader.GetString(reader.GetOrdinal("category2"));
                            string ed2k = reader.GetString(reader.GetOrdinal("ed2k"));
                            string content = reader.GetString(reader.GetOrdinal("content"));
                            string related = reader.GetString(reader.GetOrdinal("related"));

                            //创建一个对象，把数据库每条数据给一个对象
                            VerycdItem item = new VerycdItem();
                            item.verycdid = verycdid;
                            item.title = title;
                            item.status = status;
                            item.brief = brief;
                            item.pubtime = pubtime;
                            item.updtime = updtime;
                            item.category1 = category1;
                            item.category2 = category2;
                            item.ed2k = ed2k;
                            item.content = content;
                            item.related = related;

                            Console.WriteLine("当前读取到id=" + verycdid);
                            //创建一个elasticSearch(数据库,表名,主键)
                            IndexCommand indexCmd = new IndexCommand("verycd", "items", verycdid.ToString());
                            //Put()第二个参数是要插入的数据(把数据序列化存入elasticSearch)
                            OperationResult result = client.Put(indexCmd, serializer.Serialize(item));
                            var indexResult = serializer.ToIndexResult(result.Result);
                            if (indexResult.created)
                            {
                                Console.WriteLine("创建了");
                            }
                            else
                            {
                                Console.WriteLine("没创建" + indexResult.error);
                            }
                        }
                    }
                }
            }



            Console.ReadKey();
        }

        /// <summary>
        /// 从ElasticSearch数据库中取数据(搜索)
        /// </summary>
        /// <param name="args"></param>
        static void Main2(string[] args)
        {
            //连接地址
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            //从哪个数据库哪个表取数据
            SearchCommand cmd = new SearchCommand("zsz", "persons");
            //首先创建我要查询的结果(Person)
            var query = new QueryBuilder<Person>()
             //查询的条件
            .Query(b =>
                    b.Bool(m =>
                    //并且关系
                    m.Must(t =>
                       //分词的最小单位或关系查询            //name字段中必须含有帅这个词
                       t.QueryString(t1 => t1.DefaultField("Name").Query("丑"))
                         )
                      )
                    )
            //分页(第0条开始取10条数据)
            /*
            .From(0)//Skip()
            .Size(10)//Take()*/
            //排序(根据年龄排序)
            //.Sort(c => c.Field("Age", SortDirection.desc))
            //添加高亮(查询结果高亮显示)
            /*
            .Highlight(h => h
            .PreTags("<b>")
            .PostTags("</b>")
            .Fields(
                 f => f.FieldName("Name").Order(HighlightOrder.score)
                 )
            )*/
            .Build();
            //查询结果
            var result = client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            //把查询结果序列化
            var searchResult = serializer.ToSearchResult<Person>(result);
            //searchResult.hits.total; //一共有多少匹配结果  10500
            // searchResult.Documents;//当前页的查询结果 
            foreach (var doc in searchResult.Documents)
            {
                Console.WriteLine(doc.Id + "," + doc.Name + "," + doc.Age);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 把数据放入ElasticSearch数据库中
        /// </summary>
        /// <param name="args"></param>
        static void Main1(string[] args)
        {

            //Person p1 = new Person();
            //p1.Id = 1;
            //p1.Age = 10;
            //p1.Name = "欧阳帅帅";
            //p1.Desc = "欧阳锋家的帅哥公子，人送外号‘小杨中科’";

            Person p1 = new Person();
            p1.Id = 2;
            p1.Age = 8;
            p1.Name = "丑娘娘";
            p1.Desc = "二丑家的姑娘，是最美丽的女孩";
            //有的电脑localhost不行改成127.0.0.1                                              
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            var serializer = new JsonNetSerializer();
            //第一个参数相当于“数据库”，第二个参数相当于“表”，第三个参数相当于“主键”
            IndexCommand cmd = new IndexCommand("zsz", "persons", p1.Id.ToString());
            //Put()第二个参数是要插入的数据(序列化之后插入(es接受json格式的数据))
            OperationResult result = client.Put(cmd, serializer.Serialize(p1));
            //解析json字符串
            var indexResult = serializer.ToIndexResult(result.Result);
            if (indexResult.created)
            {
                Console.WriteLine("创建了");
            }
            else
            {
                Console.WriteLine("没创建" + indexResult.error);
            }
            Console.ReadKey();
        }
    }
}
