using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using DME.Base.Helper;
using System.IO;
using DME.Web.Helper;

namespace DME.Web.Common
{
    /// <summary>
    /// 上传类（图片）
    /// 
    /// 修改纪录
    ///
    ///		2010.12.18 版本：1.0 lance 创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DMEWeb_UploadImage
    {
        private int _Error = 0;//返回上传状态。
        private int _MaxSize = 1024 * 1024;//最大单个上传文件 (默认)
        private string _FileType = "jpg;gif;bmp;png";//所支持的上传类型用"/"隔开
        private string _SavePath = DME_Path.MapPath(@"\");//保存文件的实际路径
        private int _SaveType = 0;//上传文件的类型，0代表自动生成文件名
        private HtmlInputFile _FormFile = new HtmlInputFile();//上传控件。
        private string _InFileName = "";//非自动生成文件名设置。
        private string _OutFileName = "";//输出文件名。
        private bool _IsCreateImg = true;//是否生成缩略图。
        private int _Height = 0;//获取上传图片的高度
        private int _Width = 0;//获取上传图片的宽度
        private int _sHeight = 120;//设置生成缩略图的高度
        private int _sWidth = 120;//设置生成缩略图的宽度
        private int _DrawStyle = 0;//设置加水印的方式０：文字水印模式，１：图片水印模式,2:不加
        private string _AddText = "Do Maker Exchange";//设置水印内容
        private int _FileSize = 0;//获取已经上传文件的大小
        private string _CopyIamgePath = DME_Path.MapPath("/images/DMEImage.jpg");//图片水印模式下的覆盖图片的实际地址

        /// <summary>
        /// Error返回值，1、没有上传的文件。2、类型不允许。3、大小超限。4、未知错误。0、上传成功。
        /// </summary>
        public int Error
        {
            get { return _Error; }
        }
        /// <summary>
        /// 最大单个上传文件
        /// </summary>
        public int MaxSize
        {
            set { _MaxSize = value; }
        }
        /// <summary>
        /// 所支持的上传类型用";"隔开
        /// </summary>
        public string FileType
        {
            set { _FileType = value; }
        }
        /// <summary>
        /// //保存文件的实际路径
        /// </summary>
        public string SavePath
        {
            set { _SavePath = DME_Path.MapPath(value); }
            get { return _SavePath; }
        }
        /// <summary>
        /// 上传文件的类型，0代表自动生成文件名
        /// </summary>
        public int SaveType
        {
            set { _SaveType = value; }
        }
        /// <summary>
        /// 上传控件
        /// </summary>
        public HtmlInputFile FormFile
        {
            
            set { _FormFile = value; }
        }
        /// <summary>
        /// //非自动生成文件名设置。
        /// </summary>
        public string InFileName
        {
            set { _InFileName = value; }
        }
        /// <summary>
        /// 输出文件名
        /// </summary>
        public string OutFileName
        {
            get { return _OutFileName; }
            set { _OutFileName = value; }
        }
        /// <summary>
        /// 输出的缩略图文件名
        /// </summary>
        public string OutThumbFileName
        {
            get;
            set;
        }
        /// <summary>
        /// //获取上传图片的宽度
        /// </summary>
        public int Width
        {
            get { return _Width; }
        }
        /// <summary>
        /// //获取上传图片的高度
        /// </summary>
        public int Height
        {
            get { return _Height; }
        }
        /// <summary>
        /// 设置缩略图的宽度
        /// </summary>
        public int sWidth
        {
            get { return _sWidth; }
            set { _sWidth = value; }
        }
        /// <summary>
        /// 设置缩略图的高度
        /// </summary>
        public int sHeight
        {
            get { return _sHeight; }
            set { _sHeight = value; }
        }
        /// <summary>
        /// 是否生成缩略图
        /// </summary>
        public bool IsCreateImg
        {
            get { return _IsCreateImg; }
            set { _IsCreateImg = value; }
        }
        /// <summary>
        /// 设置加水印的方式０：文字水印模式，１：图片水印模式,2:不加
        /// </summary>
        public int DrawStyle
        {
            get { return _DrawStyle; }
            set { _DrawStyle = value; }
        }
        /// <summary>
        /// 设置文字水印内容
        /// </summary>
        public string AddText
        {
            get { return _AddText; }
            set { _AddText = value; }
        }
        public int FileSize
        {
            get { return _FileSize; }
            set { _FileSize = value; }
        }
        /// <summary>
        /// 图片水印模式下的覆盖图片的实际地址
        /// </summary>
        public string CopyIamgePath
        {
            set { _CopyIamgePath = DME_Path.MapPath(value); }
        }

        //获取文件的后缀名
        private string GetExt(string path)
        {
            return DME_Path.GetExtension(path);
        }
        //获取输出文件的文件名。
        private string FileName(string Ext)
        {
            if (_SaveType == 0 || _InFileName.Trim() == "")
                return DateTime.Now.ToString("yyyyMMddHHmmssfff") + Ext;
            else
                return _InFileName;
        }
        //检查上传的文件的类型，是否允许上传。
        private bool IsUpload(string Ext)
        {
            Ext = Ext.Replace(".", "");
            bool b = false;
            string[] arrFileType = _FileType.Split(';');
            foreach (string str in arrFileType)
            {
                if (str.ToLower() == Ext.ToLower())
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        //上传主要部分。
        public void Open()
        {
            System.Web.HttpPostedFile hpFile = _FormFile.PostedFile;
            if (hpFile == null || hpFile.FileName.Trim() == "")
            {
                _Error = 1;
                return;
            }

            string Ext = GetExt(hpFile.FileName);
            if (!IsUpload(Ext))
            {
                _Error = 2;
                return;
            }

            int iLen = hpFile.ContentLength;
            if (iLen > _MaxSize)
            {
                _Error = 3;
                return;
            }

            try
            {
                //创建目录
                DME_Files.InitFolder(DME_Path.GetDirectoryName(_SavePath));
                string FName;
                FName = FileName(Ext);
                int _FileSizeTemp = hpFile.ContentLength;
                System.Drawing.Image Img = System.Drawing.Image.FromStream(hpFile.InputStream, true);
                _Width = Img.Width;
                _Height = Img.Height;
                Img.Dispose();
                if (_DrawStyle == 0)
                {
                    DMEWeb_Image.ZoomAuto(hpFile, _SavePath + FName, _Width, _Height, _AddText, "");
                }
                else
                {
                    DMEWeb_Image.ZoomAuto(hpFile, _SavePath + FName, _Width, _Height, "", _CopyIamgePath);
                }
                if (_IsCreateImg)
                {
                    DMEWeb_Image.ZoomAuto(hpFile, _SavePath + FName, _sWidth, _sHeight, "", "");
                }
                _OutFileName = FName;
                _FileSize = _FileSizeTemp;
                _Error = 0;
                return;
            }
            catch (Exception ex)
            {
                _Error = 4;
                return;
            }
        }
    }
}
