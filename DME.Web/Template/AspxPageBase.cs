using System;
using System.Collections.Generic;
using System.Web;
using DME.Web.Template;
using System.Text;
using System.IO;
namespace DME.Web.Template
{
    /// <summary>
    ///  模板页面基类
    ///  注: PageBase只是提供一个简单的用于测试VTemplate速度的基类,而TemplatePageBase则是根据某些朋友的需要而增加的.有需要的朋友则可以用此基类代替PageBase并可在实际项目中使用.
    ///      TemplatePageBase 提供了详细的页面流程事件控制和缓存处理等.其页面流程类似于System.Web.UI.Page的流程
    ///      页面流程： OnInit(页面初始化) -> OnLoad(页面装载) -> OnRender(页面数据呈现) -> OnUnLoad(页面卸载)
    ///      一般要处理页面模板数据则是在InitPageTemplate方法进行。而需要进行其它需要则可以重载上面的流程事件方法以便进行页面流程控制（比如需要进行页面权限判断，则可重载OnInit事件函数进行判断）
    ///      
    ///  注意： LoadCurrentTemplate 这个方法请根据你的实际项目编写！
    /// </summary>
    public abstract class AspxPageBase : System.Web.UI.Page
    {
        #region 页面处理流程

        //private void PageBase_Load(object sender, EventArgs e)
        //{
        //    this.LoadCurrentTemplate();
        //    this.Document.Render(Response.Output);

        //}


        /// <summary>
        /// 页面初始化开始
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnInit(EventArgs e)
        //{
        //    this.Load += new EventHandler(PageBase_Load);
        //    base.OnInit(e);

        //}

        protected override void OnLoad(EventArgs e)
        {
            this.LoadCurrentTemplate();
            this.PageBase_OnLoad(e);
            base.OnLoad(e);
            this.OnRender();
        }

        /// <summary>
        /// 装载页面数据
        /// </summary>
        /// <param name="e"></param>
        protected virtual void PageBase_OnLoad(EventArgs e)
        {
            if (this.IsPostBack)
            {
                //POST方式则不处理缓存数据
                this.InitPageTemplate();
            }
            else
            {
                //优先装载缓存数据
                if (!this.LoadPageCache())
                {
                    this.InitPageTemplate();
                }
            }
        }

        /// <summary>
        /// 是否已呈现过缓存文件
        /// </summary>
        private bool _IsCacheFilePage = false;
        /// <summary>
        /// 呈现页面数据
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRender()
        {
            if (!this.IsPostBack)
            {
                //GET方式访问才保存页面缓存
                this.SavePageCache();
            }
            if (this.IsValid && !_IsCacheFilePage)
            {
                this.Document.Render(Response.Output);
            }
            _IsCacheFilePage = false;
        }

        /// <summary>
        /// 初始化当前访问页面的模板数据
        /// </summary>
        protected abstract void InitPageTemplate();


        #endregion

        #region 模板处理
        private DMEWeb_TemplateDocument _Document = null;
        /// <summary>
        /// 当前页面的模板文档对象
        /// </summary>
        public DMEWeb_TemplateDocument Document
        {
            get
            {
                return _Document;
            }
            protected set
            {
                _Document = value;
            }
        }

        /// <summary>
        /// 当前页面的模板文档的配置参数
        /// </summary>
        protected virtual DMEWeb_TemplateDocumentConfig DocumentConfig
        {
            get
            {
                return DMEWeb_TemplateDocumentConfig.Default;
            }
        }
        /// <summary>
        /// 是否读取缓存模板
        /// </summary>
        protected virtual bool IsLoadCacheTemplate
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 装载当前页面的模板文档
        /// </summary>
        public virtual void LoadCurrentTemplate()
        {
            //注: 以下模板路径只是参考!请根据你的现实情况进行修改.
            //    建议是配合配置文件一起使用.
            string fileName = Path.GetFileNameWithoutExtension(this.Request.FilePath);
            this.LoadTemplateFile(this.Server.MapPath("/Themes/Manager/template/" + fileName + ".html"));
        }
        /// <summary>
        /// 装载模板文件
        /// </summary>
        /// <param name="fileName"></param>
        protected virtual void LoadTemplateFile(string fileName)
        {
            this.Document = null;
            if (this.IsLoadCacheTemplate)
            {
                //缓存模板文档
                this.Document = DMEWeb_TemplateDocument.FromFileCache(fileName, Encoding.UTF8, this.DocumentConfig);
            }
            else
            {
                //实例模板文档
                this.Document = new DMEWeb_TemplateDocument(fileName, Encoding.UTF8, this.DocumentConfig);
            }

            //设置当前页面对象的值
            this.Document.SetValue("Page", this);
        }
        #endregion

        #region 常用属性
        /// <summary>
        /// 返回判断当前页面是否有效(模板是否已初始化)
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _Document != null;
            }
        }


        /// <summary>
        /// 当前页面是否是以POST方式访问
        /// </summary>
        public bool IsPostBack
        {
            get
            {
                return "POST".Equals(Request.RequestType, StringComparison.InvariantCultureIgnoreCase);
            }
        }
        #endregion

        #region 处理页面缓存
        /// <summary>
        /// 当前页面的缓存过期时效(单位:秒钟,默认为1小时,如果小于等于0则不缓存)
        /// </summary>
        protected virtual int PageCacheExpireTime
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 当前页面的缓存文件绝对地址(如果需要使用页面缓存则必须重写此属性并返回真实地址)
        /// </summary>
        protected virtual string PageCacheFileName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 装载页面缓存,此方法默认在OnLoad事件发生时自动调用
        /// </summary>
        /// <returns>是否装载页面缓存成功</returns>
        protected virtual bool LoadPageCache()
        {
            if (this.PageCacheExpireTime > 0 && !string.IsNullOrEmpty(this.PageCacheFileName))
            {
                try
                {
                    FileInfo cacheFile = new FileInfo(this.PageCacheFileName);
                    if (cacheFile.Exists)
                    {
                        //判断文件是否没过期
                        TimeSpan span = DateTime.Now.Subtract(cacheFile.LastWriteTime);
                        if (span.TotalSeconds >= 0 && span.TotalSeconds <= this.PageCacheExpireTime)
                        {
                            //输出缓存文件数据
                            HttpResponse response = (HttpContext.Current != null ? HttpContext.Current.Response : null);
                            if (response != null)
                            {
                                response.Clear();
                                response.WriteFile(this.PageCacheFileName);
                                _IsCacheFilePage = true;
                                return true;
                            }
                        }
                    }

                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// 保存页面缓存,此方法默认在OnRender事件发生时自动调用
        /// </summary>
        protected virtual void SavePageCache()
        {
            if (this.PageCacheExpireTime > 0 && !string.IsNullOrEmpty(this.PageCacheFileName))
            {
                //获取模板文档的数据
                if (this.IsValid)
                {
                    try
                    {
                        string path = Path.GetDirectoryName(this.PageCacheFileName);
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        FileInfo cacheFile = new FileInfo(this.PageCacheFileName);
                        if (cacheFile.Exists)
                        {
                            TimeSpan span = DateTime.Now.Subtract(cacheFile.LastWriteTime);
                            if (span.TotalSeconds >= this.PageCacheExpireTime)
                            {
                                this.Document.RenderTo(this.PageCacheFileName, this.Document.Charset);
                            }
                        }
                        else
                        {
                            this.Document.RenderTo(this.PageCacheFileName, this.Document.Charset);
                        }

                    }
                    catch { }
                }
            }
        }
        #endregion

    }
}