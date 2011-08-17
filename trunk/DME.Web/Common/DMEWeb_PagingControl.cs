using System;
using System.Text;

namespace DME.Web.Common
{
    /// <summary>
    /// 分页控件类
    /// </summary>
    public class DMEWeb_PagingControl
    {
        #region 方法
        
        /// <summary>
        /// 显示默认的分页控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="pageCount">每页显示的数量</param>
        /// <param name="pageIndex">要显示的页</param>
        /// <param name="isWriteCount">是否允许自定义每页数量</param>
        /// <param name="isWriteIndex">是否允许自定义每屏显示几页的链接</param>
        /// <returns>配置好的分页控件</returns>
        public string Show(string url, int pageCount, int pageIndex, bool isWriteCount, bool isWriteIndex)
        {
            PageCount = pageCount;
            PageTotal = CountTotal / PageCount;
            if (CountTotal % PageCount != 0)
            {
                PageTotal++;
            }
            string strCountLable = "";
            string strCountText = "";
            if (isWriteCount)
            {
                strCountLable = CreateLabelControl(PerPageCountStr, PerPageCountCss, PerPageCountStyle);
                strCountText = CreateTextControl(PageCountParamStr, TextCss, " width:24px;height:17px;");
            }

            string strFirstPage = CreateLinkControl(GetUrl(url, 1, PageCount), FirstPageStr, FirstPageCss, FirstPageStyle);

            int iPrePage = pageIndex - 1;
            string prePageDisable = "";
            string prePageUrl = url;
            if (pageIndex - 1 < 1)
            {
                iPrePage = 1;
                //prePageDisable = "disabled";
                prePageUrl = "";
            }
            string strPrePage = CreateLinkControl(GetUrl(prePageUrl, iPrePage, PageCount), "", PrePageStr, PrePageCss, PrePageStyle, prePageDisable);

            int iPreNPage = (((pageIndex - 1) / NPage - 1) * NPage + 1);
            string preNPageDisable = "";
            string preNPageUrl = url;
            if ((((pageIndex - 1) / NPage - 1) * NPage + 1) < 1)
            {
                iPreNPage = 1;
                //preNPageDisable = "disabled";
                prePageUrl = "";
            }
            string strPreNPage = CreateLinkControl(GetUrl(prePageUrl, iPreNPage, PageCount), "", PreNPageStr, PreNPageCss, PreNPageStyle, preNPageDisable);
            string strLink = FormatNLinkArray(pageIndex, PageTotal, PageCount, NPage, url);

            int next = pageIndex + 1;
            string nextPageDisable = "";
            string nextPageUrl = url;
            if (pageIndex + 1 > PageTotal)
            {
                next = PageTotal;
                //nextPageDisable = "disabled";
                nextPageUrl = "";
            }
            string strNextPage = CreateLinkControl(GetUrl(nextPageUrl, next, PageCount), "", NextPageStr, NextPageCss, NextPageStyle, nextPageDisable);

            int iNextPage = (((pageIndex - 1) / NPage + 1) * NPage + 1);
            string nextNPageDisable = "";
            string nextNPageUrl = url;
            if ((((pageIndex - 1) / NPage + 1) * NPage + 1) >= PageTotal)
            {
                iNextPage = 1;
                //nextNPageDisable = "disabled";
                nextNPageUrl = "";
            }
            string strNextNPage = CreateLinkControl(GetUrl(nextNPageUrl, iNextPage, PageCount), "", NextNPageStr, NextNPageCss, NextNPageStyle, nextNPageDisable);

            string strLastPage = CreateLinkControl(GetUrl(url, PageTotal, PageCount), LastPageStr, LastPageCss, LastPageStyle);

            string strPageText = "";
            string strGo = "";
            if (isWriteIndex)
            {
                strPageText = CreateTextControl(PageIndexParamStr, TextCss, " width:24px;height:17px;");
                strGo = CreateImgButtonControl("", GOStr, GOCss, GOStyle, GOImg);
            }

            string empty = CreateEmptyControl();

            //string strPageTotal = CreateLabelControl(PageTotalStr + PageTotal.ToString(), PageTotalCss, PageTotalStyle);
            //string strCountTotal = CreateLabelControl(CountTotalStr + CountTotal.ToString(), CountTotalCss, CountTotalStyle);
            string strCountTotal = CreateLinkControl("", "", CountTotal.ToString(), CountTotalCss, CountTotalStyle, "");

            string strDIV = CreateDIVControl("", DivCss, DivStyle, strCountLable, strCountText, empty, strFirstPage, empty, strPreNPage, empty, strPrePage, empty, strLink, empty, strNextPage, empty, strNextNPage, empty, strLastPage, empty, strPageText, empty, strGo, empty, strCountTotal);

            return strDIV;
        }

        
        /// <summary>
        /// 显示默认的分页控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="pageCount">每页显示的数量</param>
        /// <param name="pageIndex">要显示的页</param>
        /// <returns>配置好的分页控件</returns>
        public string Show(string url, int pageCount, int pageIndex)
        {
            return Show(url, pageCount, pageIndex, false, false);
        }

        
        /// <summary>
        /// 显示默认的分页控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="pageIndex">要显示的页</param>
        /// <returns>配置好的分页控件</returns>
        public string Show(string url, int pageIndex)
        {
            return Show(url, PageCount, pageIndex, false, false);
        }

        
        /// <summary>
        /// 生成Text控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="attributes">要添加的attributes属性</param>
        /// <returns>生成的Text控件</returns>
        public string CreateTextControl(string strID, string strCSS, string strStyle, params string[] attributes)
        {
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.Append("<input type=\"text\" ");
            for (int i = 0; i < attributes.Length; i++)
            {
                textBuilder.Append(attributes[i]);
            }
            textBuilder.Append(AppandIDCSSStyle(strID, strCSS, strStyle));
            textBuilder.Append("/>");
            return textBuilder.ToString();
        }

        
        /// <summary>
        /// 生成Text控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Text控件</returns>
        public string CreateTextControl(string strID, string strCSS, string strStyle)
        {
            return CreateTextControl(strID, CountTotalStr, strStyle, "");
        }

        
        /// <summary>
        /// 生成Text控件
        /// </summary>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Text控件</returns>
        public string CreateTextControl(string strCSS, string strStyle)
        {
            return CreateTextControl(null, strCSS, strStyle, "");
        }

        
        /// <summary>
        /// 生成Text控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <returns>生成的Text控件</returns>
        public string CreateTextControl(string strID)
        {
            return CreateTextControl(strID, null, null, "");
        }

        
        /// <summary>
        /// 生成Text控件
        /// </summary>
        /// <returns>生成的Text控件</returns>
        public string CreateTextControl()
        {
            return CreateTextControl("", "", "", "");
        }

        
        /// <summary>
        /// 生成Link控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="attributes">要添加的attributes属性</param>
        /// <returns>生成的Link控件</returns>
        public string CreateLinkControl(string url, string strID, string strText, string strCSS, string strStyle, params string[] attributes)
        {
            StringBuilder linkBuilder = new StringBuilder();
            linkBuilder.Append("<a ");
            if (!String.IsNullOrEmpty(url))
            {
                linkBuilder.Append(" href=\"");
                linkBuilder.Append(url);
                linkBuilder.Append("\"");
            }
            for (int i = 0; i < attributes.Length; i++)
            {
                linkBuilder.Append(attributes[i]);
            }
            linkBuilder.Append(AppandIDCSSStyle(strID, strCSS, strStyle));
            linkBuilder.Append(">");
            linkBuilder.Append(strText);
            linkBuilder.Append("</a>");
            return linkBuilder.ToString();
        }

        
        /// <summary>
        /// 生成Link控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Link控件</returns>
        public string CreateLinkControl(string url, string strID, string strText, string strCSS, string strStyle)
        {
            return CreateLinkControl(url, strID, strText, strCSS, strStyle, "");
        }

        
        /// <summary>
        /// 生成Link控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Link控件</returns>
        public string CreateLinkControl(string url, string strText, string strCSS, string strStyle)
        {
            return CreateLinkControl(url, null, strText, strCSS, strStyle, "");
        }

        
        /// <summary>
        /// 生成Link控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <returns>生成的Link控件</returns>
        public string CreateLinkControl(string url, string strID, string strText)
        {
            return CreateLinkControl(url, strID, strText, null, null, "");
        }

        
        /// <summary>
        /// 生成Link控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strText">要显示的文字</param>
        /// <returns>生成的Link控件</returns>
        public string CreateLinkControl(string url, string strText)
        {
            return CreateLinkControl(url, null, strText, null, null, "");
        }

        
        /// <summary>
        ///  生成Img控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="attributes">要添加的attributes属性</param>
        /// <returns>生成的Img控件</returns>
        public string CreateImgControl(string url, string strID, string strCSS, string strStyle, params string[] attributes)
        {
            StringBuilder imgBuilder = new StringBuilder();
            imgBuilder.Append("<img src=\"");
            imgBuilder.Append(url);
            imgBuilder.Append("\" ");
            for (int i = 0; i < attributes.Length; i++)
            {
                imgBuilder.Append(attributes[i]);
            }
            imgBuilder.Append(AppandIDCSSStyle(strID, strCSS, strStyle));
            imgBuilder.Append("/>");
            return imgBuilder.ToString();
        }

        
        /// <summary>
        ///  生成Img控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Img控件</returns>
        public string CreateImgControl(string url, string strID, string strCSS, string strStyle)
        {
            return CreateImgControl(url, strID, strCSS, strStyle, "");
        }

        
        /// <summary>
        ///  生成Img控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Img控件</returns>
        public string CreateImgControl(string url, string strCSS, string strStyle)
        {
            return CreateImgControl(url, null, strCSS, strStyle, "");
        }

        
        /// <summary>
        ///  生成Img控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <param name="strID">ID和Name的值</param>
        /// <returns>生成的Img控件</returns>
        public string CreateImgControl(string url, string strID)
        {
            return CreateImgControl(url, strID, null, null, "");
        }

        
        /// <summary>
        ///  生成Img控件
        /// </summary>
        /// <param name="url">要跳转到的页面的路径和参数</param>
        /// <returns>生成的Img控件</returns>
        public string CreateImgControl(string url)
        {
            return CreateImgControl(url, null, null, null, "");
        }

        
        /// <summary>
        ///  生成Label控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="attributes">要添加的attributes属性</param>
        /// <returns>生成的Label控件</returns>
        public string CreateLabelControl(string strID, string strText, string strCSS, string strStyle, params string[] attributes)
        {
            StringBuilder labelBuilder = new StringBuilder();
            labelBuilder.Append("<span ");
            for (int i = 0; i < attributes.Length; i++)
            {
                labelBuilder.Append(attributes[i]);
            }
            labelBuilder.Append(AppandIDCSSStyle(strID, strCSS, strStyle));
            labelBuilder.Append(">");
            labelBuilder.Append(strText);
            labelBuilder.Append("</span>");
            return labelBuilder.ToString();
        }

        
        /// <summary>
        ///  生成Label控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Label控件</returns>
        public string CreateLabelControl(string strID, string strText, string strCSS, string strStyle)
        {
            return CreateLabelControl(strID, strText, strCSS, strStyle, "");
        }

        
        /// <summary>
        ///  生成Label控件
        /// </summary>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的Label控件</returns>
        public string CreateLabelControl(string strText, string strCSS, string strStyle)
        {
            return CreateLabelControl(null, strText, strCSS, strStyle, "");
        }

        
        /// <summary>
        ///  生成Label控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <returns>生成的Label控件</returns>
        public string CreateLabelControl(string strID, string strText)
        {
            return CreateLabelControl(strID, strText, null, null, "");
        }

        
        /// <summary>
        ///  生成Label控件
        /// </summary>
        /// <param name="strText">要显示的文字</param>
        /// <returns>生成的Label控件</returns>
        public string CreateLabelControl(string strText)
        {
            return CreateLabelControl(null, strText, null, null, "");
        }

        
        /// <summary>
        /// 生成DIV控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="strText">要显示的内容</param>
        /// <returns>生成的DIV控件</returns>
        public string CreateDIVControl(string strID, string strCSS, string strStyle, params string[] strText)
        {
            StringBuilder divBuilder = new StringBuilder();
            divBuilder.Append("<div ");
            divBuilder.Append(AppandIDCSSStyle(strID, strCSS, strStyle));
            divBuilder.Append(">");
            StringBuilder textBuilder = new StringBuilder();
            for (int i = 0; i < strText.Length; i++)
            {
                textBuilder.Append(strText[i]);
            }
            divBuilder.Append(textBuilder.ToString());
            divBuilder.Append("</div>");
            return divBuilder.ToString();
        }

        
        /// <summary>
        /// 生成DIV控件
        /// </summary>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的DIV控件</returns>
        public string CreateDIVControl(string strCSS, string strStyle)
        {
            return CreateDIVControl(null, strCSS, strStyle, "");
        }

        
        /// <summary>
        /// 生成DIV控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <returns>生成的DIV控件</returns>
        public string CreateDIVControl(string strID)
        {
            return CreateDIVControl(strID, null, null, "");
        }

        
        /// <summary>
        /// 生成空格
        /// </summary>
        /// <param name="count">生成的空格的数量</param>
        /// <returns>生成的空格</returns>
        public string CreateEmptyControl(int count)
        {
            string empty = "";
            for (int i = 0; i < count; i++)
            {
                empty += "&nbsp;";
            }
            return empty;
        }

        
        /// <summary>
        /// 生成空格
        /// </summary>
        /// <returns>生成的空格</returns>
        public string CreateEmptyControl()
        {
            return CreateEmptyControl(1);
        }

        
        /// <summary>
        /// 生成图像按钮控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="strImg">要显示的图片的路径</param>
        /// <param name="attributes">要添加的属性</param>
        /// <returns>生成的图像按钮控件</returns>
        public string CreateImgButtonControl(string strID, string strText, string strCSS, string strStyle, string strImg, params string[] attributes)
        {
            StringBuilder imgBtnBuilder = new StringBuilder();
            imgBtnBuilder.Append("<input type=\"submit\" ");
            imgBtnBuilder.Append(" value=\"");
            if (!String.IsNullOrEmpty(strText))
            {
                imgBtnBuilder.Append(strText);
            }
            imgBtnBuilder.Append("\" ");
            for (int i = 0; i < attributes.Length; i++)
            {
                imgBtnBuilder.Append(attributes[i]);
            }
            imgBtnBuilder.Append(AppandIDCSSStyle(strID, strCSS, ""));
            if (!String.IsNullOrEmpty(strStyle) && String.IsNullOrEmpty(strImg))
            {
                imgBtnBuilder.Append(" style=\"");
                imgBtnBuilder.Append(strStyle);
                imgBtnBuilder.Append("\" ");
            }
            if (!String.IsNullOrEmpty(strImg) && String.IsNullOrEmpty(strStyle))
            {
                imgBtnBuilder.Append(" style=\" background-image:url('");
                imgBtnBuilder.Append(strImg);
                imgBtnBuilder.Append("'); \" ");
            }
            if (!String.IsNullOrEmpty(strStyle) && !String.IsNullOrEmpty(strImg))
            {
                imgBtnBuilder.Append(" style=\"");
                imgBtnBuilder.Append(strStyle);
                imgBtnBuilder.Append("; background-image:url('");
                imgBtnBuilder.Append(strImg);
                imgBtnBuilder.Append("'); ");
                imgBtnBuilder.Append("\" ");
            }
            imgBtnBuilder.Append("/>");
            return imgBtnBuilder.ToString();
        }

        
        /// <summary>
        /// 生成图像按钮控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="strImg">要显示的图片的路径</param>
        /// <returns>生成的图像按钮控件</returns>
        public string CreateImgButtonControl(string strID, string strCSS, string strStyle, string strImg)
        {
            return CreateImgButtonControl(strID, null, strCSS, strStyle, strImg, "");
        }

        
        /// <summary>
        /// 生成图像按钮控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strImg">要显示的图片的路径</param>
        /// <returns>生成的图像按钮控件</returns>
        public string CreateImgButtonControl(string strID, string strImg)
        {
            return CreateImgButtonControl(strID, null, null, null, strImg, "");
        }

        
        /// <summary>
        /// 生成按钮控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <param name="attributes">要添加的属性</param>
        /// <returns>生成的按钮控件</returns>
        public string CreateButtonControl(string strID, string strText, string strCSS, string strStyle, params string[] attributes)
        {
            return CreateImgButtonControl(strID, strText, strCSS, strStyle, "", attributes);
        }

        
        /// <summary>
        /// 生成按钮控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的按钮控件</returns>
        public string CreateButtonControl(string strID, string strText, string strCSS, string strStyle)
        {
            return CreateButtonControl(strID, strText, strCSS, strStyle, "");
        }

        
        /// <summary>
        /// 生成按钮控件
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strText">要显示的文字</param>
        /// <returns>生成的按钮控件</returns>
        public string CreateButtonControl(string strID, string strText)
        {
            return CreateButtonControl(strID, strText, null, null, "");
        }

        
        /// <summary>
        /// 生成按钮控件
        /// </summary>
        /// <param name="strText">要显示的文字</param>
        /// <returns>生成的按钮控件</returns>
        public string CreateButtonControl(string strText)
        {
            return CreateButtonControl(null, strText, null, null, "");
        }

        
        /// <summary>
        /// 合成链接路径
        /// </summary>
        /// <param name="url">要合成的路径</param>
        /// <param name="pageIndex">要显示的页码</param>
        /// <param name="pageCount">每页要显示的数量</param>
        /// <returns>合成的链接路径</returns>
        public string GetUrl(string url, int pageIndex, int pageCount)
        {
            if (String.IsNullOrEmpty(url))
            {
                return "";
            }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(url);
            if (url.IndexOf("?") <= 0)
            {
                strBuilder.Append("?");
            }
            else
            {
                strBuilder.Append("&");
            }
            strBuilder.Append(PageIndexParamStr);
            strBuilder.Append("=");
            strBuilder.Append(pageIndex);
            //strBuilder.Append("&");
            //strBuilder.Append(PageCountParamStr);
            //strBuilder.Append("=");
            //strBuilder.Append(pageCount);
            return strBuilder.ToString();
        }

        
        /// <summary>
        /// 合成链接路径
        /// </summary>
        /// <param name="url">要合成的路径</param>
        /// <param name="nameArray">要显示的页码数组</param>
        /// <param name="valueArray">每页要显示的数量数组</param>
        /// <returns>合成的链接路径</returns>
        public string GetUrl(string url, string[] nameArray, string[] valueArray)
        {
            if (String.IsNullOrEmpty(url))
            {
                return "";
            }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(url);
            if (url.IndexOf("?") <= 0)
            {
                strBuilder.Append("?");
            }
            else
            {
                strBuilder.Append("&");
            }
            for (int i = 0; i < nameArray.Length; i++)
            {
                strBuilder.Append(nameArray[i]);
                strBuilder.Append("=");
                strBuilder.Append(valueArray[i]);
                strBuilder.Append("&");
            }
            string result = strBuilder.ToString();
            return result.Substring(0, result.Length - 2);
        }

        
        /// <summary>
        /// 生成链接数组 形式如: 1 2 3 4 5 6 7 8 9 
        /// </summary>
        /// <param name="pageIndex">要显示的页码</param>
        /// <param name="pageTotal">总的页数</param>
        /// <param name="pageCount">每页要显示的数量</param>
        /// <param name="NPage">每屏要显示的链接娄</param>
        /// <param name="url">要跳转的页面和参数</param>
        /// <returns>生成的链接数组</returns>
        public string FormatNLinkArray(int pageIndex, int pageTotal, int pageCount, int NPage, string url)
        {
            string temp = "";
            int length = pageTotal > NPage ? NPage : pageTotal;
            int NCount = (pageIndex - 1) / NPage;
            for (int i = 1; i < length + 1; i++)
            {
                if (NCount * NPage + i > PageTotal)
                {
                    break;
                }
                if ((NCount * NPage + i) == pageIndex)
                {
                    temp += CreateLinkControl(null, null, (pageIndex).ToString(), CurrentPageLinkCss, CurrentPageLinkStyle, "disable");
                    temp += CreateEmptyControl();
                    continue;
                }
                temp += CreateLinkControl(GetUrl(url, NCount * NPage + i, pageCount), (NCount * NPage + i).ToString(), LinkCss, LinkStyle);
                temp += CreateEmptyControl();
            }
            return temp;
        }

        
        /// <summary>
        /// 生成链接数组 形式如: 1 2 3 ... 7 8 9 
        /// </summary>
        /// <param name="pageIndex">要显示的页码</param>
        /// <param name="pageTotal">总的页数</param>
        /// <param name="pageCount">每页要显示的数量</param>
        /// <param name="NPage">每屏要显示的链接娄</param>
        /// <param name="url">要跳转的页面和参数</param>
        /// <returns>生成的链接数组</returns>
        public string FormatMiddleLinkArray(int pageIndex, int pageTotal, int pageCount, int NPage, string url)
        {
            string temp = "";
            int length = pageTotal - pageIndex;
            int NCount = (pageIndex - 1) / NPage;
            if (length < NPage)
            {
                for (int i = 0; i < length; i++)
                {
                    if ((NCount * NPage + i) == pageIndex)
                    {
                        temp += CreateLinkControl(null, (pageIndex).ToString(), CurrentPageLinkCss, CurrentPageLinkStyle);
                        temp += CreateEmptyControl();
                        continue;
                    }
                    temp += CreateLinkControl(GetUrl(url, i, pageCount), i.ToString(), LinkCss, LinkStyle);
                    temp += CreateEmptyControl();
                }
                return temp;
            }
            temp += CreateLinkControl(GetUrl(url, pageIndex - 1, pageCount), (pageIndex - 1).ToString(), LinkCss, LinkStyle);
            temp += CreateEmptyControl();
            temp += CreateLinkControl(null, (pageIndex).ToString(), CurrentPageLinkCss, CurrentPageLinkStyle);
            temp += CreateEmptyControl();
            temp += CreateLinkControl(GetUrl(url, pageIndex + 1, pageCount), (pageIndex + 1).ToString(), LinkCss, LinkStyle);
            temp += CreateEmptyControl();
            temp += " ... ";
            temp += CreateLinkControl(GetUrl(url, pageTotal - 2, pageCount), (pageTotal - 2).ToString(), LinkCss, LinkStyle);
            temp += CreateEmptyControl();
            temp += CreateLinkControl(GetUrl(url, pageTotal - 1, pageCount), (pageTotal - 1).ToString(), LinkCss, LinkStyle);
            temp += CreateEmptyControl();
            temp += CreateLinkControl(GetUrl(url, pageTotal, pageCount), pageTotal.ToString(), LinkCss, LinkStyle);
            return temp;
        }

        
        /// <summary>
        /// 生成ID，CSS，Style等属性的字符串
        /// </summary>
        /// <param name="strID">ID和Name的值</param>
        /// <param name="strCSS">CSS样式</param>
        /// <param name="strStyle">页面上的Style样式</param>
        /// <returns>生成的ID，CSS，Style等属性的字符串</returns>
        public string AppandIDCSSStyle(string strID, string strCSS, string strStyle)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (!String.IsNullOrEmpty(strID))
            {
                strBuilder.Append(" id=\"");
                strBuilder.Append(strID);
                strBuilder.Append("\" ");
                strBuilder.Append(" name=\"");
                strBuilder.Append(strID);
                strBuilder.Append("\" ");
            }
            if (!String.IsNullOrEmpty(strCSS))
            {
                strBuilder.Append(" class=\"");
                strBuilder.Append(strCSS);
                strBuilder.Append("\" ");
            }
            if (!String.IsNullOrEmpty(strStyle))
            {
                strBuilder.Append(" style=\"");
                strBuilder.Append(strStyle);
                strBuilder.Append("\" ");
            }
            return strBuilder.ToString();
        }
        #endregion 方法

        #region 变量
        /// <summary>
        /// 第一页的显示文字
        /// </summary>
        protected string _firstPageStr;
        /// <summary>
        /// 下一页的显示文字
        /// </summary>
        protected string _nextPageStr;
        /// <summary>
        /// 下N页的显示文字
        /// </summary>
        protected string _nextNPageStr;
        /// <summary>
        /// 最末页的显示文字
        /// </summary>
        protected string _lastPageStr;
        /// <summary>
        /// 前一页的显示文字
        /// </summary>
        protected string _prePageStr;
        /// <summary>
        /// 前N页的显示文字
        /// </summary>
        protected string _preNPageStr;
        /// <summary>
        /// 总页数的显示文字
        /// </summary>
        protected string _pageTotalStr;
        /// <summary>
        /// 总条数的显示文字
        /// </summary>
        protected string _countTotalStr;
        /// <summary>
        /// 跳转的显示文字
        /// </summary>
        protected string _GOStr;
        /// <summary>
        /// 每页显示几条的显示文字
        /// </summary>
        protected string _perPageCountStr;

        /// <summary>
        /// 总页数
        /// </summary>
        protected int _pageTotal;
        /// <summary>
        /// 总条数
        /// </summary>
        protected int _countTotal;
        /// <summary>
        /// N页数
        /// </summary>
        protected int _nPage;
        /// <summary>
        /// 每页显示条数
        /// </summary>
        protected int _pageCount;

        /// <summary>
        /// 第一页的图片路径
        /// </summary>
        protected string _firstPageImg;
        /// <summary>
        /// 下一页的图片路径
        /// </summary>
        protected string _nextPageImg;
        /// <summary>
        /// 下N页的图片路径
        /// </summary>
        protected string _nextNPageImg;
        /// <summary>
        /// 最末页的图片路径
        /// </summary>
        protected string _lastPageImg;
        /// <summary>
        /// 前一页的图片路径
        /// </summary>
        protected string _prePageImg;
        /// <summary>
        /// 前N页的图片路径
        /// </summary>
        protected string _preNPageImg;
        /// <summary>
        /// 跳转的图片路径
        /// </summary>
        protected string _GOImg;
        /// <summary>
        /// 中间链接页的图片路径
        /// </summary>
        protected string _linkImg;
        /// <summary>
        /// 每页显示几条的图片路径
        /// </summary>
        protected string _perPageCountImg;
        /// <summary>
        /// 当前链接页的图片路径
        /// </summary>
        protected string _currentPageLinkImg;

        /// <summary>
        /// 第一页的CSS
        /// </summary>
        protected string _firstPageCss;
        /// <summary>
        /// 下一页的CSS
        /// </summary>
        protected string _nextPageCss;
        /// <summary>
        /// 下N页的CSS
        /// </summary>
        protected string _nextNPageCss;
        /// <summary>
        /// 最末页的CSS
        /// </summary>
        protected string _lastPageCss;
        /// <summary>
        /// 前一页的CSS
        /// </summary>
        protected string _prePageCss;
        /// <summary>
        /// 前N页的CSS
        /// </summary>
        protected string _preNPageCss;
        /// <summary>
        /// 总页数的CSS
        /// </summary>
        protected string _pageTotalCss;
        /// <summary>
        /// 总条数的CSS
        /// </summary>
        protected string _countTotalCss;
        /// <summary>
        /// 跳转的CSS  
        /// </summary>
        protected string _GOCss;
        /// <summary>
        /// 中间链接页的CSS
        /// </summary>
        protected string _linkCss;
        /// <summary>
        /// 跳转的文本框的CSS 
        /// </summary>
        protected string _textCss;
        /// <summary>
        /// DIV的CSS
        /// </summary>
        protected string _divCss;
        /// <summary>
        /// 每页显示几条的CSS
        /// </summary>
        protected string _perPageCountCss;
        /// <summary>
        /// 当前链接页的CSS
        /// </summary>
        protected string _currentPageLinkCss;

        /// <summary>
        /// 第一页的Style
        /// </summary>
        protected string _firstPageStyle;
        /// <summary>
        /// 下一页的Style
        /// </summary>
        protected string _nextPageStyle;
        /// <summary>
        /// 下N页的Style
        /// </summary>
        protected string _nextNPageStyle;
        /// <summary>
        /// 最末页的Style
        /// </summary>
        protected string _lastPageStyle;
        /// <summary>
        /// 前一页的Style
        /// </summary>
        protected string _prePageStyle;
        /// <summary>
        /// 前N页的Style
        /// </summary>
        protected string _preNPageStyle;
        /// <summary>
        /// 总页数的Style
        /// </summary>
        protected string _pageTotalStyle;
        /// <summary>
        /// 总条数的Style
        /// </summary>
        protected string _countTotalStyle;
        /// <summary>
        /// 跳转的Style  
        /// </summary>
        protected string _GOStyle;
        /// <summary>
        /// 中间链接页的Style
        /// </summary>
        protected string _linkStyle;
        /// <summary>
        /// 跳转的文本框的Style 
        /// </summary>
        protected string _textStyle;
        /// <summary>
        /// DIV的Style 
        /// </summary>
        protected string _divStyle;
        /// <summary>
        /// 每页显示几条的Style 
        /// </summary>
        protected string _perPageCountStyle;
        /// <summary>
        /// 当前链接页的Style
        /// </summary>
        protected string _currentPageLinkStyle;

        /// <summary>
        /// 要查询页的参数名称
        /// </summary>
        protected string _pageIndexParamStr;
        /// <summary>
        /// 每页条数的参数名称
        /// </summary>
        protected string _pageCountParamStr;
        /// <summary>
        /// N页的参数名称
        /// </summary>
        protected string _nPageParamStr;
        #endregion 变量

        #region 属性
        #region Str
        
        /// <summary>
        /// 第一页的显示文字
        /// </summary>
        public string FirstPageStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_firstPageStr))
                {
                    _firstPageStr = "|&lt&lt";
                }
                return _firstPageStr;
            }
            set { _firstPageStr = value; }
        }
        
        /// <summary>
        /// 下一页的显示文字
        /// </summary>
        public string NextPageStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_nextPageStr))
                {
                    _nextPageStr = "&gt";
                }
                return _nextPageStr;
            }
            set { _nextPageStr = value; }
        }
        
        /// <summary>
        /// 下N页的显示文字
        /// </summary>
        public string NextNPageStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_nextNPageStr))
                {
                    _nextNPageStr = "&gt&gt";
                }
                return _nextNPageStr;
            }
            set { _nextNPageStr = value; }
        }
        
        /// <summary>
        /// 最末页的显示文字
        /// </summary>
        public string LastPageStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_lastPageStr))
                {
                    _lastPageStr = "&gt&gt|";
                }
                return _lastPageStr;
            }
            set { _lastPageStr = value; }
        }
        
        /// <summary>
        /// 前一页的显示文字
        /// </summary>
        public string PrePageStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_prePageStr))
                {
                    _prePageStr = "&lt";
                }
                return _prePageStr;
            }
            set { _prePageStr = value; }
        }
        
        /// <summary>
        /// 前N页的显示文字
        /// </summary>
        public string PreNPageStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_preNPageStr))
                {
                    _preNPageStr = "&lt&lt";
                }
                return _preNPageStr;
            }
            set { _preNPageStr = value; }
        }
        
        /// <summary>
        /// 总页数的显示文字
        /// </summary>
        public string PageTotalStr
        {
            get
            {
                if (String.IsNullOrEmpty(_pageTotalStr))
                {
                    _pageTotalStr = "Page:";
                }
                return _pageTotalStr;
            }
            set { _pageTotalStr = value; }
        }
        
        /// <summary>
        /// 总条数的显示文字
        /// </summary>
        public string CountTotalStr
        {
            get
            {
                if (String.IsNullOrEmpty(_countTotalStr))
                {
                    _countTotalStr = "Count:";
                }
                return _countTotalStr;
            }
            set { _countTotalStr = value; }
        }
        
        /// <summary>
        /// 跳转的显示文字
        /// </summary>
        public string GOStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_GOStr))
                {
                    _GOStr = "GO";
                }
                return _GOStr;
            }
            set { _GOStr = value; }
        }
        
        /// <summary>
        /// 每页显示几条的显示文字
        /// </summary>
        public string PerPageCountStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_perPageCountStr))
                {
                    _perPageCountStr = "PerCount:";
                }
                return _perPageCountStr;
            }
            set { _perPageCountStr = value; }
        }
        #endregion Str

        #region Total
        
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageTotal
        {
            get { return _pageTotal; }
            set { _pageTotal = value; }
        }
        
        /// <summary>
        /// 总条数
        /// </summary>
        public int CountTotal
        {
            get { return _countTotal; }
            set { _countTotal = value; }
        }
        
        /// <summary>
        /// N页数
        /// </summary>
        public int NPage
        {
            protected get
            {
                if (_nPage <= 0)
                {
                    _nPage = 10;
                }
                return _nPage;
            }
            set { _nPage = value; }
        }
        
        /// <summary>
        /// 每页显示的条数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (_pageCount <= 0)
                {
                    _pageCount = 20;
                }
                return _pageCount;
            }
            set
            {
                _pageCount = value;
            }
        }
        #endregion Total

        #region Img
        
        /// <summary>
        /// 第一页的图片路径
        /// </summary>
        public string FirstPageImg
        {
            protected get { return _firstPageImg; }
            set { _firstPageImg = value; }
        }
        
        /// <summary>
        /// 下一页的图片路径
        /// </summary>
        public string NextPageImg
        {
            protected get { return _nextPageImg; }
            set { _nextPageImg = value; }
        }
        
        /// <summary>
        /// 下N页的图片路径
        /// </summary>
        public string NextNPageImg
        {
            protected get { return _nextNPageImg; }
            set { _nextNPageImg = value; }
        }
        
        /// <summary>
        /// 最末页的图片路径
        /// </summary>
        public string LastPageImg
        {
            protected get { return _lastPageImg; }
            set { _lastPageImg = value; }
        }
        
        /// <summary>
        /// 前一页的图片路径
        /// </summary>
        public string PrePageImg
        {
            protected get { return _prePageImg; }
            set { _prePageImg = value; }
        }
        
        /// <summary>
        /// 前N页的CSS
        /// </summary>
        public string PreNPageImg
        {
            protected get { return _preNPageImg; }
            set { _preNPageImg = value; }
        }
        
        /// <summary>
        /// 跳转的图片路径
        /// </summary>
        public string GOImg
        {
            protected get { return _GOImg; }
            set { _GOImg = value; }
        }
        
        /// <summary>
        /// 中间链接页的图片路径
        /// </summary>
        public string LinkImg
        {
            protected get { return _linkImg; }
            set { _linkImg = value; }
        }
        
        /// <summary>
        /// 每页显示几条的图片路径
        /// </summary>
        public string PerPageCountImg
        {
            protected get { return _perPageCountImg; }
            set { _perPageCountImg = value; }
        }
        
        /// <summary>
        /// 当前链接页的图片路径
        /// </summary>
        public string CurrentPageLinkImg
        {
            protected get { return _currentPageLinkImg; }
            set { _currentPageLinkImg = value; }
        }
        #endregion Img

        #region Css
        
        /// <summary>
        /// 第一页的CSS
        /// </summary>
        public string FirstPageCss
        {
            protected get { return _firstPageCss; }
            set { _firstPageCss = value; }
        }
        
        /// <summary>
        /// 下一页的CSS
        /// </summary>
        public string NextPageCss
        {
            protected get { return _nextPageCss; }
            set { _nextPageCss = value; }
        }
        
        /// <summary>
        /// 下N页的CSS
        /// </summary>
        public string NextNPageCss
        {
            protected get { return _nextNPageCss; }
            set { _nextNPageCss = value; }
        }
        
        /// <summary>
        /// 最末页的CSS
        /// </summary>
        public string LastPageCss
        {
            protected get { return _lastPageCss; }
            set { _lastPageCss = value; }
        }
        
        /// <summary>
        /// 前一页的CSS
        /// </summary>
        public string PrePageCss
        {
            protected get { return _prePageCss; }
            set { _prePageCss = value; }
        }
        
        /// <summary>
        /// 前N页的CSS
        /// </summary>
        public string PreNPageCss
        {
            protected get { return _preNPageCss; }
            set { _preNPageCss = value; }
        }
        
        /// <summary>
        /// 总页数的CSS
        /// </summary>
        public string PageTotalCss
        {
            protected get { return _pageTotalCss; }
            set { _pageTotalCss = value; }
        }
        
        /// <summary>
        /// 总条数的CSS
        /// </summary>
        public string CountTotalCss
        {
            protected get { return _countTotalCss; }
            set { _countTotalCss = value; }
        }
        
        /// <summary>
        /// 跳转的CSS
        /// </summary>
        public string GOCss
        {
            protected get { return _GOCss; }
            set { _GOCss = value; }
        }
        
        /// <summary>
        /// 中间链接页的CSS
        /// </summary>
        public string LinkCss
        {
            protected get { return _linkCss; }
            set { _linkCss = value; }
        }
        
        /// <summary>
        /// 跳转的文本框的CSS 
        /// </summary>
        public string TextCss
        {
            protected get { return _textCss; }
            set { _textCss = value; }
        }
        
        /// <summary>
        /// DIV的CSS
        /// </summary>
        public string DivCss
        {
            protected get
            {
                if (String.IsNullOrEmpty(_divCss))
                {
                    _divCss = "common_paging";
                }
                return _divCss;
            }
            set { _divCss = value; }
        }
        
        /// <summary>
        /// 每页显示几条的CSS
        /// </summary>
        public string PerPageCountCss
        {
            protected get { return _perPageCountCss; }
            set { _perPageCountCss = value; }
        }
        
        /// <summary>
        /// 当前链接页的CSS
        /// </summary>
        public string CurrentPageLinkCss
        {
            protected get
            {
                if (String.IsNullOrEmpty(_currentPageLinkCss))
                {
                    _currentPageLinkCss = "common_current";
                }
                return _currentPageLinkCss;
            }
            set { _currentPageLinkCss = value; }
        }
        #endregion Css

        #region Style
        
        /// <summary>
        /// 第一页的Style
        /// </summary>
        public string FirstPageStyle
        {
            protected get { return _firstPageStyle; }
            set { _firstPageStyle = value; }
        }
        
        /// <summary>
        /// 下一页的Style
        /// </summary>
        public string NextPageStyle
        {
            protected get { return _nextPageStyle; }
            set { _nextPageStyle = value; }
        }
        
        /// <summary>
        /// 下N页的Style
        /// </summary>
        public string NextNPageStyle
        {
            protected get { return _nextNPageStyle; }
            set { _nextNPageStyle = value; }
        }
        
        /// <summary>
        /// 最末页的Style
        /// </summary>
        public string LastPageStyle
        {
            protected get { return _lastPageStyle; }
            set { _lastPageStyle = value; }
        }
        
        /// <summary>
        /// 前一页的Style
        /// </summary>
        public string PrePageStyle
        {
            protected get { return _prePageStyle; }
            set { _prePageStyle = value; }
        }
        
        /// <summary>
        /// 前N页的Style
        /// </summary>
        public string PreNPageStyle
        {
            protected get { return _preNPageStyle; }
            set { _preNPageStyle = value; }
        }
        
        /// <summary>
        /// 总页数的Style
        /// </summary>
        public string PageTotalStyle
        {
            protected get { return _pageTotalStyle; }
            set { _pageTotalStyle = value; }
        }
        
        /// <summary>
        /// 总条数的Style
        /// </summary>
        public string CountTotalStyle
        {
            protected get { return _countTotalStyle; }
            set { _countTotalStyle = value; }
        }
        
        /// <summary>
        /// 跳转的Style  
        /// </summary>
        public string GOStyle
        {
            protected get { return _GOStyle; }
            set { _GOStyle = value; }
        }
        
        /// <summary>
        /// 中间链接页的Style
        /// </summary>
        public string LinkStyle
        {
            protected get { return _linkStyle; }
            set { _linkStyle = value; }
        }
        
        /// <summary>
        /// 跳转的文本框的Style 
        /// </summary>
        public string TextStyle
        {
            get { return _textStyle; }
            set { _textStyle = value; }
        }
        
        /// <summary>
        /// DIV的Style 
        /// </summary>
        public string DivStyle
        {
            get { return _divStyle; }
            set { _divStyle = value; }
        }
        
        /// <summary>
        /// 每页显示几条的Style 
        /// </summary>
        public string PerPageCountStyle
        {
            protected get { return _perPageCountStyle; }
            set { _perPageCountStyle = value; }
        }
        
        /// <summary>
        /// 当前链接页的Style
        /// </summary>
        public string CurrentPageLinkStyle
        {
            protected get
            {
                if (String.IsNullOrEmpty(_currentPageLinkStyle))
                {
                    _currentPageLinkStyle = "BORDER-RIGHT:#e89954 1px solid; PADDING-RIGHT:5px; BORDER-TOP:#e89954 1px solid; PADDING-LEFT:5px; FONT-WEIGHT:bold; PADDING-BOTTOM:2px;";
                    _currentPageLinkStyle += "BORDER-LEFT:#e89954 1px solid; COLOR:#000; MARGIN-RIGHT:2px; PADDING-TOP:2px; BORDER-BOTTOM:#e89954 1px solid; BACKGROUND-COLOR:#ffca7d";
                }
                return _currentPageLinkStyle;
            }
            set { _currentPageLinkStyle = value; }
        }
        #endregion Style

        #region  Parameter
        
        /// <summary>
        /// 要查询页的参数名称
        /// </summary>
        public string PageIndexParamStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_pageIndexParamStr))
                {
                    _pageIndexParamStr = "pageindex";
                }
                return _pageIndexParamStr;
            }
            set { _pageIndexParamStr = value; }
        }
        
        /// <summary>
        /// 每页条数的参数名称
        /// </summary>
        public string PageCountParamStr
        {
            protected get
            {
                if (String.IsNullOrEmpty(_pageCountParamStr))
                {
                    _pageCountParamStr = "pagecount";
                }
                return _pageCountParamStr;
            }
            set { _pageCountParamStr = value; }
        }
        
        /// <summary>
        /// N页的参数名称
        /// </summary>
        public string NPageParamStr
        {
            protected get { return _nPageParamStr; }
            set { _nPageParamStr = value; }
        }
        #endregion parameter
        #endregion 属性
    }
}
