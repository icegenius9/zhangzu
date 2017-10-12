using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace modelBinder
{
    //一定要使用using System.Web.Mvc下的DefaultModelBinder
    //ModelBinder负责把表单提交的用户名和密码赋值到model上
    //写一个类继承自DefaultModelBinder
    //然后在 Global 中：表示我这个ModelBinders对string类型的数据由我来处理，其他的类型不管
    //ModelBinders.Binders.Add(typeof(string),
    // new TrimToDBCModelBinder());
    public class TrimToDBCModelBinder : DefaultModelBinder
    {
        //override父类的BindModel方法
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext
       bindingContext)
        {
            //先调用父类的方法获得返回值
            object value = base.BindModel(controllerContext, bindingContext);
            //如果获得返回值是一个string类型
            if (value is string)
            {
                string strValue = (string)value;
                //转成半角字符串
                string value2 = ToDBC(strValue).Trim();
                return value2;
            }
            else
            {
                return value;
            }
        }
        /// <summary> 全角转半角的函数(DBC case) </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        private static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }


    }
}