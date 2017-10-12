using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 锁
{
    class Program
    {
        /// <summary>
        /// 纯EF写乐观锁,不用出现rowver，但需要配置上
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string bf = Console.ReadLine();
            using (MyDbContext ctx = new MyDbContext())
            {
                ctx.Database.Log = (sql) =>
                {
                    Console.WriteLine(sql);
                };
                var g = ctx.Girls.First();
                if (!string.IsNullOrEmpty(g.BF))
                {
                    if (g.BF == bf)
                    {
                        Console.WriteLine("早已经是你的人了呀，还抢啥？");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("来晚了，早就被别人抢走了");
                        Console.ReadKey();
                        return;
                    }
                }
                Console.WriteLine("点击任意键，开抢（模拟耗时等待并发）");
                Console.ReadKey();
                g.BF = bf;
                try
                {
                    ctx.SaveChanges();
                    Console.WriteLine("抢媳妇成功");
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("抢媳妇失败");
                }
            }
            Console.ReadKey();
        }
            /// <summary>
            /// EF(有ado)写乐观锁(没有事务),在数据库表中增加一个字段类型为timestamp(时间戳)
            /// </summary>
            /// <param name="args"></param>
            static void Main4(string[] args)
        {
            //先创建DbContext
            using (MyDbContext ctx = new MyDbContext())
            {
                Console.WriteLine("请输入你的名字");
                string bf = Console.ReadLine();
                //EF用原声sql写法
                var g = ctx.Database.SqlQuery<Girl>("select * from T_Girls where id=1").Single();
                //查出来的rowver(timestamp)
                byte[] rowver = g.rowver;
                if (!string.IsNullOrEmpty(g.BF))
                {
                    if (g.BF == bf)
                    {
                        Console.WriteLine("早已经是你的人了呀，还抢啥？");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("来晚了，早就被别人抢走了");
                        Console.ReadKey();
                        return;
                    }
                }
                //并发时2个程序同时执行到这步，2个程序取到的rowver都是2，当其中一个程序update，那rowver的值就变3,另外一个程序执行update根据rowver=2去更新，可rowver已经变成3,就找不到rowver=2这条数据了,受影响就变0
                Console.WriteLine("点击任意键，开抢（模拟耗时等待并发）");
                Console.ReadKey();
                Thread.Sleep(3000);
                int affectRow = ctx.Database.ExecuteSqlCommand("update T_Girls set BF={0} where id=1 and RowVer={1}",
                    bf, rowver);
                if (affectRow == 0)
                {
                    Console.WriteLine("抢媳妇失败");
                }
                else if (affectRow == 1)
                {
                    Console.WriteLine("抢媳妇成功");
                }
                else
                {
                    Console.WriteLine("什么鬼");
                }
            }

            Console.ReadKey();

        }

        /// <summary>
        /// 乐观锁没有事务,在数据库表中增加一个字段类型为timestamp(时间戳) 此代码有问题
        /// </summary>
        /// <param name="args"></param>
        static void Main3(string[] args)
        {
            Console.WriteLine("请输入您的名字");
            string myname = Console.ReadLine();
            string connstr = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open();
                long rowver = 0;
                using (var cmd = conn.CreateCommand())
                {
                    //查询数据库表中的一行数据其中包括timestamp类型的那列数据(取timestamp类型用CONVERT(BIGINT,rowver))
                    cmd.CommandText = "select id,name,bf,CONVERT(BIGINT,rowver) from T_Girls where Id=1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("没找到id=1的");
                            return;
                        }
                        string bf = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("BF")))
                        {
                            bf = reader.GetString(reader.GetOrdinal("BF"));
                        }
                        if (!string.IsNullOrEmpty(bf))
                        {
                            if (bf == myname)
                            {
                                Console.WriteLine("早就是我的人了");
                            }
                            else
                            {
                                Console.WriteLine(bf + "妻不客气");
                            }
                            Console.ReadKey();
                            return;
                        }
                        //把数据库读到的时间戳赋给rowver
                        rowver = reader.GetInt64(reader.GetOrdinal("rowver"));
                    }
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "update T_Girls set BF=@bf where Id=1 and rowver=@rv";
                    cmd.Parameters.Add(new SqlParameter("@bf", myname));
                    cmd.Parameters.Add(new SqlParameter("@rv", rowver));
                    //返回值是受影响的行数
                    int affectedRow = cmd.ExecuteNonQuery();
                    if (affectedRow <= 0)
                    {
                        Console.WriteLine("被别人抢先啦");
                    }
                    else
                    {
                        Console.WriteLine("被我抢到了");
                    }
                }
            }

            Console.ReadKey();
        }


        /// <summary>
        /// 悲观锁
        /// </summary>
        /// <param name="args"></param>
        static void Main1(string[] args)
        {
            Console.WriteLine("请输入您的名字");
            string myname = Console.ReadLine();
            //取得appconfig的连接字符串
            string connstr = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                //打开数据库连接
                conn.Open();
                //开启一个事务
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("开始查询");
                        //创建一个command
                        using (var selectCmd = conn.CreateCommand())
                        {
                            //给command赋刚创建的Transaction
                            selectCmd.Transaction = tx;
                            //给commandText赋执行的sql语句,加了with(xlock,ROWLOCK)排他锁，ROWLOCK表示行锁(有的表锁把一个表都锁定)，指定这行数据不能修改也不能查询(读写都不行)
                            selectCmd.CommandText = "select * from T_Girls with(xlock,ROWLOCK) where id=1";
                            //读数据 ,用bin启动2个exe程序发现,第一个程序执行之后加锁，第二个程序输入名字后，只显示开始查询,在读取数据的时候等待第一个程序事务结束
                            using (var reader = selectCmd.ExecuteReader())
                            {
                                //如果没有数据
                                if (!reader.Read())
                                {
                                    Console.WriteLine("没有id为1的女孩");
                                    return;
                                }
                                string bf = null;
                                //如果BF这列不为null
                                if (!reader.IsDBNull(reader.GetOrdinal("BF")))
                                {
                                    //把数据库中BF这列的值赋给bf
                                    bf = reader.GetString(reader.GetOrdinal("BF"));
                                }
                                if (!string.IsNullOrEmpty(bf))//已经有男朋友
                                {
                                    if (bf == myname)
                                    {
                                        Console.WriteLine("早已经是我的人了");
                                    }
                                    else
                                    {
                                        Console.WriteLine("早已经被" + bf + "抢走了");
                                    }
                                    Console.ReadKey();
                                    return;
                                }
                                //如果bf==null，则继续向下抢
                            }
                            Console.WriteLine("查询完成，开始update");
                            //执行update的Command
                            using (var updateCmd = conn.CreateCommand())
                            {
                                //也要给command赋刚创建的Transaction
                                updateCmd.Transaction = tx;
                                //把女孩的男朋友设置成我的名字
                                updateCmd.CommandText = "Update T_Girls set BF=@bf where id=1";
                                //sql参数化，防止sql注入
                                updateCmd.Parameters.Add(new SqlParameter("@bf", myname));
                                updateCmd.ExecuteNonQuery();
                            }
                            Console.WriteLine("结束Update");
                            Console.WriteLine("按任意键结束事务");
                            Console.ReadKey();
                        }
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        tx.Rollback();
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
