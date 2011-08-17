using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Drawing;

namespace DME.Web.Common
{
    /// <summary>
    /// 使用此HttpHandler可以为您生成一张验证码图片。
    /// </summary>
    /// <example>
    /// <p>使用此HttpHandler可以为您生成一张验证码图片。图片正常显示后，您可以使用 Session["ValidateCode"] 来获取产生的验证码。
    /// 为避免难以辨认的情况，验证码中不会产生如下字符：0(数字)、1(数字)、l(字母)、I(字母)、o(字母)、O(字母)</p>
    /// <p>要使用此模块，您只需要配置web.config中httpHandlers一节即可：</p>
    /// <code><![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <configuration>
    ///     <system.web>
    ///         <httpHandlers>
    ///             <add verb="*" path="/ValidateImg.aspx" type="DME.Web.Common.DMEWeb_ValidateImg,DME.Web"/>
    ///         </httpHandlers>
    ///     </system.web>
    /// </configuration>]]></code>
    /// <p>其中“/ValidateImg.aspx”可以是您希望的任意路径，不必存在这个.aspx文件。</p>
    /// <p>如此配置后您便可以像普通图片用于，直接在您的网页上使用如下代码来显示验证码了：</p>
    /// <code><![CDATA[<img src="/ValidateImg.aspx?n=4" border="0" onclick="var d=new Date();this.src='/ValidateImg.aspx?n=4&amp;t='+d.toString();" style="cursor:pointer;" alt="看不清？点击换一个！" />]]></code>
    /// <p>其中图片路径就是您在web.config中配置的路径。其中参数“n”用于指定验证码的字符数，不指定则默认为 5 。</p>
    /// </example>
    public class DMEWeb_ValidateImg : IHttpHandler, IRequiresSessionState
    {
        #region IHttpHandler 成员

        /// <summary>
        /// 是否可以被多线程同时使用
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            int n = 0; int.TryParse(context.Request["n"], out n);
            n = n == 0 ? 5 : n;
            string checkCode = CreateCode(n);

            //用于验证
            context.Session["ValidateCode"] = checkCode;
            CreateImages(checkCode, context);
        }
        #endregion

        private string CreateCode(int codeLength)
        {

            string so = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] strArr = so.Split(',');
            string code = "";
            Random rand = new Random();
            for (int i = 0; i < codeLength; i++)
            {
                code += strArr[rand.Next(0, strArr.Length)];
            }
            return code;
        }

        /*产生验证图片*/
        private void CreateImages(string checkCode, HttpContext context)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty) return;

            int height = 0; int.TryParse(context.Request["h"], out height);
            height = height == 0 ? 22 : height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 13.0)), height);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器 
                Random random = new Random();
                //清空图片背景色 
                g.Clear(Color.White);

                //画图片的背景噪音线
                for (int i = 0; i < 12; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
                }

                Font font2 = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Regular);
                System.Drawing.Drawing2D.LinearGradientBrush brush2 = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, image.Width, image.Height),
                    Color.Gray, Color.Gray,
                    2f, true);
                g.DrawString("DME", font2, brush2, 0, 0);

                //画图片的前景噪音点 
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                Font font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, image.Width, image.Height),
                    Color.Blue, Color.Red,
                    2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的边框线 
                g.DrawRectangle(new Pen(Color.Gray), 0, 0, image.Width - 1, image.Height - 1);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                context.Response.ClearContent();
                context.Response.ContentType = "image/Gif";
                context.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }

        }
    }
}
