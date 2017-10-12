using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace 锁
{
    class MyDbContext:DbContext
    {
        public MyDbContext() : base("name=connstr")
        {


        }
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //FltAPI配置方法 需要对每个表创建一个对应的Config配置
            Assembly asm = Assembly.GetExecutingAssembly();
            modelBuilder.Configurations.AddFromAssembly(asm);

            //EF配置其中一种方法
            //modelBuilder.Entity<Girl>().ToTable("T_Girls");
            //配置rowver列是IsRowVersion类型的
            //modelBuilder.Entity<Girl>().Property(g => g.rowver).IsRowVersion();

        }
        public DbSet<Girl> Girls { get; set; }
    }
}
