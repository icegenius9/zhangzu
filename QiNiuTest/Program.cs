using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qiniu.Util;
using Qiniu.IO.Model;
using Qiniu.IO;
using Qiniu.Http;

namespace QiNiuTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Qiniu.Common.Config.SetZone(Qiniu.Common.ZoneID.CN_East, true);
            //先安装Install-Package Qiniu.Shared
            //密钥(AccessKey=AK,SecretKey=SK)
            Mac mac = new Mac("hQt1huUHIPXwKRvfakbvgZLvN2-oPNIUmEcu61WO",
                "95pmXjtZOZXTNgAk2qr7o5IqYR_lYV51qwXIe_Ij");
            //bucket指的是七牛云的存储空间的名字
            string bucket = "icegenius9";
            //上传到七牛云服务器的文件名
            //string saveKey = "1.jpg";
            //也可以把文件名设定有目录的
            string saveKey = "girl/gf/1.jpg";
            //本地文件路径是什么
            string localFile = "D:\\1.jpg";
            //自动配置所属区域(连哪个机房)
            Qiniu.Common.Config.AutoZone("hQt1huUHIPXwKRvfakbvgZLvN2-oPNIUmEcu61WO",
                bucket, true);
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            putPolicy.DeleteAfterDays = 1;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            UploadManager um = new UploadManager();
            HttpResult result = um.UploadFile(localFile, saveKey, token);
            Console.WriteLine(result);

            //上传上去以后访问通过(域名后面加上传时文件的名称) http://ofsm1mlxm.bkt.clouddn.com/1.jpg
            //用户上传图片的过程:先把图片给到我们自己的服务器,但不把图片保存到本地,服务器把图片存到七牛云
            //用户访问的时候直接从七牛云访问图片

            Console.ReadKey();
        }
    }
}
