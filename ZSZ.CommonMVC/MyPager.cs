using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.CommonMVC
{
    //分页组件
    public class MyPager
    {   /// <summary>
        /// 每一页数据的条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总数据条数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 显示出来的页码最多个数
        /// </summary>
        public int MaxPagerCount { get; set; }

        /// <summary>
        /// 当前页的页码(从1开始算起始页)
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 连接的格式，约定其中页码用{pn}占位
        /// </summary>
        public string UrlPattern { get; set; }

        /// <summary>
        /// 当前页的页码的样式的名字
        /// </summary>
        public string CurrentPageClassName { get; set; }

        public string GetPagerHtml()
        {
            StringBuilder html = new StringBuilder();
            html.Append("<ul>");

            //总页数=总条数/页每页的条数(取天花板数Ceiling)
            int pageCount = (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            
            //显示出来的页码的起始页码( 1和当前页码减去显示出来最多页码数除以2对比谁大显示谁)
            //当前页是20页，总共显示出来10页，20-10/2=15 ，15比1大，起始页显示15
            int startPageIndex = Math.Max(1, PageIndex - MaxPagerCount / 2);

            //显示出来的页码的结束页码
            //起始页+显示出来最多的页码数 和 总页数比较谁小取谁
            //总页数15  起始页4+最多显示出来10条，那最后的页码是14
            int endPageIndex = Math.Min(pageCount, startPageIndex + MaxPagerCount);

            //起始页是1，最大也是10 当前页是5的话，5拼接样式属性，没有超链接
            for (int i = startPageIndex; i <= endPageIndex; i++)
            {
                //如果是当前页 
                if (i == PageIndex)
                {
                    //设置当前页的样式
                    html.Append("<li class='").Append(CurrentPageClassName).Append("'>")
                        .Append(i).Append("</li>");
                }
                else
                {
                    string href = UrlPattern.Replace("{pn}", i.ToString());
                    html.Append("<li><a href='").Append(href).Append("'>")
                        .Append(i).Append("</a></li>");
                }
            }
            html.Append("</ul>");
            return html.ToString();
        }
    }
}
