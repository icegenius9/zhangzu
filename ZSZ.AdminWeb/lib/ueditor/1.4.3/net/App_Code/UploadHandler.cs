using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ZSZ.Common;

/// <summary>
/// UploadHandler 的摘要说明
/// </summary>
public class UploadHandler : Handler
{

    public UploadConfig UploadConfig { get; private set; }
    public UploadResult Result { get; private set; }

    public UploadHandler(HttpContext context, UploadConfig config)
        : base(context)
    {
        this.UploadConfig = config;
        this.Result = new UploadResult() { State = UploadState.Unknown };
    }

    public override void Process()
    {
        byte[] uploadFileBytes = null;
        string uploadFileName = null;

        if (UploadConfig.Base64)
        {
            uploadFileName = UploadConfig.Base64Filename;
            uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
        }
        else
        {
            var file = Request.Files[UploadConfig.UploadFieldName];
            //上传文件的文件名
            uploadFileName = file.FileName;

            if (!CheckFileType(uploadFileName))
            {
                Result.State = UploadState.TypeNotAllow;
                WriteResult();
                return;
            }
            if (!CheckFileSize(file.ContentLength))
            {
                Result.State = UploadState.SizeLimitExceed;
                WriteResult();
                return;
            }
            //读取出来的文件内容
            uploadFileBytes = new byte[file.ContentLength];
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                WriteResult();
            }
        }

        Result.OriginFileName = uploadFileName;
        //ueditor上传到本地代码
        //var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);
        //var localPath = Server.MapPath(savePath);
        //try
        //{
        //    if (!Directory.Exists(Path.GetDirectoryName(localPath)))
        //    {
        //        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
        //    }
        //    File.WriteAllBytes(localPath, uploadFileBytes);
        //    Result.Url = savePath;
        //    Result.State = UploadState.Success;
        //}
        //catch (Exception e)
        //{
        //    Result.State = UploadState.FileAccessError;
        //    Result.ErrorMessage = e.Message;
        //}
        //finally
        //{
        //    WriteResult();
        //}

        //ueditor上传到七牛云       
        try
        {
            //先安装Install-Package Qiniu.Shared
            //密钥(AccessKey=AK,SecretKey=SK)
            Mac mac = new Mac("hQt1huUHIPXwKRvfakbvgZLvN2-oPNIUmEcu61WO",
                "95pmXjtZOZXTNgAk2qr7o5IqYR_lYV51qwXIe_Ij");
            //bucket指的是七牛云的存储空间的名字
            string bucket = "icegenius9";
            //上传到七牛云服务器的文件名
            //string saveKey = "1.jpg";
            //也可以把文件名设定有目录的
            //string saveKey = "girl/gf/1.jpg";
            //本地文件路径是什么
            //string localFile = "D:\\1.jpg";
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

            //用“年/月/日/文件内容md5.文件类型”做上传到七牛云的文件名,Path.GetExtension(uploadFileName)是取到文件的后缀
            string saveKey = DateTime.Now.ToString("yyyy/MM/dd") + "/"
                + CommonHelper.CalcMD5(uploadFileBytes)
                + Path.GetExtension(uploadFileName);


            //HttpResult result = um.UploadFile(localFile, saveKey, token);
            //把bytes数组文件(图片)上传到七牛云
            HttpResult result = um.UploadData(uploadFileBytes, saveKey, token);
                       
            Result.Url = saveKey;
            Result.State = UploadState.Success;
        }
        catch (Exception e)
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = e.Message;
        }
        finally
        {
            WriteResult();
        }
    }

    private void WriteResult()
    {
        this.WriteJson(new
        {
            state = GetStateMessage(Result.State),
            url = Result.Url,
            title = Result.OriginFileName,
            original = Result.OriginFileName,
            error = Result.ErrorMessage
        });
    }

    private string GetStateMessage(UploadState state)
    {
        switch (state)
        {
            case UploadState.Success:
                return "SUCCESS";
            case UploadState.FileAccessError:
                return "文件访问出错，请检查写入权限";
            case UploadState.SizeLimitExceed:
                return "文件大小超出服务器限制";
            case UploadState.TypeNotAllow:
                return "不允许的文件格式";
            case UploadState.NetworkError:
                return "网络错误"; 
        }
        return "未知错误";
    }

    private bool CheckFileType(string filename)
    {
        var fileExtension = Path.GetExtension(filename).ToLower();
        return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
    }

    private bool CheckFileSize(int size)
    {
        return size < UploadConfig.SizeLimit;
    }
}

public class UploadConfig
{
    /// <summary>
    /// 文件命名规则
    /// </summary>
    public string PathFormat { get; set; }

    /// <summary>
    /// 上传表单域名称
    /// </summary>
    public string UploadFieldName { get; set; }

    /// <summary>
    /// 上传大小限制
    /// </summary>
    public int SizeLimit { get; set; }

    /// <summary>
    /// 上传允许的文件格式
    /// </summary>
    public string[] AllowExtensions { get; set; }

    /// <summary>
    /// 文件是否以 Base64 的形式上传
    /// </summary>
    public bool Base64 { get; set; }

    /// <summary>
    /// Base64 字符串所表示的文件名
    /// </summary>
    public string Base64Filename { get; set; }
}

public class UploadResult
{
    public UploadState State { get; set; }
    public string Url { get; set; }
    public string OriginFileName { get; set; }

    public string ErrorMessage { get; set; }
}

public enum UploadState
{
    Success = 0,
    SizeLimitExceed = -1,
    TypeNotAllow = -2,
    FileAccessError = -3,
    NetworkError = -4,
    Unknown = 1,
}

