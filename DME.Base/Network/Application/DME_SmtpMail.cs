using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net.Sockets;

namespace DME.Base.Network.Application
{
    public class DME_SmtpMail
    {
        private string enter = "\r\n";
        /// <summary> 
        /// 设定语言代码，默认设定为GB2312，如不需要可设置为"" 
        /// </summary> 
        public string Charset = "GB2312";
        /// <summary> 
        /// 发件人地址 
        /// </summary> 
        public string From = "";
        /// <summary> 
        /// 发件人姓名 
        /// </summary> 
        public string FromName = "";
        /// <summary> 
        /// 回复邮件地址 
        /// </summary> 
        //public string ReplyTo=""; 
        /// <summary> 
        /// 收件人姓名 
        /// </summary> 
        public string RecipientName = "";
        /// <summary> 
        /// 收件人列表 
        /// </summary> 
        private Hashtable Recipient = new Hashtable();
        /// <summary> 
        /// 邮件服务器域名 
        /// </summary> 
        private string mailserver = "";
        /// <summary> 
        /// 邮件服务器端口号 
        /// </summary> 
        private int mailserverport = 25;
        /// <summary> 
        /// SMTP认证时使用的用户名 
        /// </summary> 
        private string username = "";
        /// <summary> 
        /// SMTP认证时使用的密码 
        /// </summary> 
        private string password = "";
        /// <summary> 
        /// 是否需要SMTP验证 
        /// </summary> 
        private bool ESmtp = false;
        /// <summary> 
        /// 是否Html邮件 
        /// </summary> 
        public bool Html = false;
        /// <summary> 
        /// 邮件附件列表 
        /// </summary> 
        private System.Collections.ArrayList Attachments;
        /// <summary> 
        /// 邮件发送优先级，可设置为"High","Normal","Low"或"1","3","5" 
        /// </summary> 
        private string priority = "Normal";
        /// <summary> 
        /// 邮件主题 
        /// </summary> 
        public string Subject = "";
        /// <summary> 
        /// 邮件正文 
        /// </summary> 
        public string Body = "";
        /// <summary> 
        /// 收件人数量 
        /// </summary> 
        private int RecipientNum = 0;
        /// <summary> 
        /// 最多收件人数量 
        /// </summary> 
        private int recipientmaxnum = 1;
        /// <summary> 
        /// 密件收件人数量 
        /// </summary> 
        //private int RecipientBCCNum=0; 
        /// <summary> 
        /// 错误消息反馈 
        /// </summary> 
        private string errmsg;
        /// <summary> 
        /// TcpClient对象，用于连接服务器 
        /// </summary> 
        private TcpClient tc;
        /// <summary> 
        /// NetworkStream对象 
        /// </summary> 
        private NetworkStream ns;
        /// <summary> 
        /// SMTP错误代码哈希表 
        /// </summary> 
        private Hashtable ErrCodeHT = new Hashtable();
        /// <summary> 
        /// SMTP正确代码哈希表 
        /// </summary> 
        private Hashtable RightCodeHT = new Hashtable();

        public DME_SmtpMail() 
        { 
            Attachments = new System.Collections.ArrayList(); 
        }

        /// <summary> 
        /// 邮件服务器域名和验证信息 
        /// 形如："user:pass@www.server.com:25"，也可省略次要信息。如"user:pass@www.server.com"或"www.server.com" 
        /// </summary> 
        public string MailDomain
        {
            set
            {
                string maidomain = value.Trim();
                int tempint;

                if (maidomain != "")
                {
                    tempint = maidomain.IndexOf("@");
                    if (tempint != -1)
                    {
                        string str = maidomain.Substring(0, tempint);
                        MailServerUserName = str.Substring(0, str.IndexOf(":"));
                        MailServerPassWord = str.Substring(str.IndexOf(":") + 1, str.Length - str.IndexOf(":") - 1);
                        maidomain = maidomain.Substring(tempint + 1, maidomain.Length - tempint - 1);
                    }

                    tempint = maidomain.IndexOf(":");
                    if (tempint != -1)
                    {
                        mailserver = maidomain.Substring(0, tempint);
                        mailserverport = System.Convert.ToInt32(maidomain.Substring(tempint + 1, maidomain.Length - tempint - 1));
                    }
                    else
                    {
                        mailserver = maidomain;

                    }


                }

            }
        }

        // <summary> 
        /// 邮件服务器端口号 
        /// </summary> 
        public int MailDomainPort
        {
            set
            {
                mailserverport = value;
            }
        }



        /// <summary> 
        /// SMTP认证时使用的用户名 
        /// </summary> 
        public string MailServerUserName
        {
            set
            {
                if (value.Trim() != "")
                {
                    username = value.Trim();
                    ESmtp = true;
                }
                else
                {
                    username = "";
                    ESmtp = false;
                }
            }
        }

        /// <summary> 
        /// SMTP认证时使用的密码 
        /// </summary> 
        public string MailServerPassWord
        {
            set
            {
                password = value;
            }
        }

        /// <summary> 
        /// 邮件发送优先级，可设置为"High","Normal","Low"或"1","3","5" 
        /// </summary> 
        public string Priority
        {
            set
            {
                switch (value.ToLower())
                {
                    case "high":
                        priority = "High";
                        break;

                    case "1":
                        priority = "High";
                        break;

                    case "normal":
                        priority = "Normal";
                        break;

                    case "3":
                        priority = "Normal";
                        break;

                    case "low":
                        priority = "Low";
                        break;

                    case "5":
                        priority = "Low";
                        break;

                    default:
                        priority = "Normal";
                        break;
                }
            }
        }


        /// <summary> 
        /// 错误消息反馈 
        /// </summary> 
        public string ErrorMessage
        {
            get
            {
                return errmsg;
            }
        }



        /// <summary> 
        /// 服务器交互记录 
        /// </summary> 
        private string logs = "";

        /// <summary> 
        /// 服务器交互记录，如发现本组件不能使用的SMTP服务器，请将出错时的Logs发给我（lion-a@sohu.com），我将尽快查明原因。 
        /// </summary> 
        public string Logs
        {
            get
            {
                return logs;
            }
        }




        /// <summary> 
        /// SMTP回应代码哈希表 
        /// </summary> 
        private void SMTPCodeAdd()
        {
            ErrCodeHT.Add("500", "邮箱地址错误");
            ErrCodeHT.Add("501", "参数格式错误");
            ErrCodeHT.Add("502", "命令不可实现");
            ErrCodeHT.Add("503", "服务器需要SMTP验证");
            ErrCodeHT.Add("504", "命令参数不可实现");
            ErrCodeHT.Add("421", "服务未就绪，关闭传输信道");
            ErrCodeHT.Add("450", "要求的邮件操作未完成，邮箱不可用（例如，邮箱忙）");
            ErrCodeHT.Add("550", "要求的邮件操作未完成，邮箱不可用（例如，邮箱未找到，或不可访问）");
            ErrCodeHT.Add("451", "放弃要求的操作；处理过程中出错");
            ErrCodeHT.Add("551", "用户非本地，请尝试<forward-path>");
            ErrCodeHT.Add("452", "系统存储不足，要求的操作未执行");
            ErrCodeHT.Add("552", "过量的存储分配，要求的操作未执行");
            ErrCodeHT.Add("553", "邮箱名不可用，要求的操作未执行（例如邮箱格式错误）");
            ErrCodeHT.Add("432", "需要一个密码转换");
            ErrCodeHT.Add("534", "认证机制过于简单");
            ErrCodeHT.Add("538", "当前请求的认证机制需要加密");
            ErrCodeHT.Add("454", "临时认证失败");
            ErrCodeHT.Add("530", "需要认证");

            RightCodeHT.Add("220", "服务就绪");
            RightCodeHT.Add("250", "要求的邮件操作完成");
            RightCodeHT.Add("251", "用户非本地，将转发向<forward-path>");
            RightCodeHT.Add("354", "开始邮件输入，以<enter>.<enter>结束");
            RightCodeHT.Add("221", "服务关闭传输信道");
            RightCodeHT.Add("334", "服务器响应验证Base64字符串");
            RightCodeHT.Add("235", "验证成功");
        }


        /// <summary> 
        /// 将字符串编码为Base64字符串 
        /// </summary> 
        /// <param name="estr">要编码的字符串</param> 
        private string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }


        /// <summary> 
        /// 将Base64字符串解码为普通字符串 
        /// </summary> 
        /// <param name="dstr">要解码的字符串</param> 
        private string Base64Decode(string str)
        {
            byte[] barray;
            barray = Convert.FromBase64String(str);
            return Encoding.Default.GetString(barray);
        }


        /// <summary> 
        /// 得到上传附件的文件流 
        /// </summary> 
        /// <param name="FilePath">附件的绝对路径</param> 
        private string GetStream(string FilePath)
        {
            //建立文件流对象 
            System.IO.FileStream FileStr = new System.IO.FileStream(FilePath, System.IO.FileMode.Open);
            byte[] by = new byte[System.Convert.ToInt32(FileStr.Length)];
            FileStr.Read(by, 0, by.Length);
            FileStr.Close();
            return (System.Convert.ToBase64String(by));
        }


        /// <summary> 
        /// 添加邮件附件 
        /// </summary> 
        /// <param name="path">附件绝对路径</param> 
        public void AddAttachment(string path)
        {
            Attachments.Add(path);
        }



        /// <summary> 
        /// 添加一个收件人 
        /// </summary> 
        /// <param name="str">收件人地址</param> 
        public bool AddRecipient(string str)
        {
            str = str.Trim();
            if (str == null || str == "" || str.IndexOf("@") == -1)
                return true;
            if (RecipientNum < recipientmaxnum)
            {
                Recipient.Add(RecipientNum, str);
                RecipientNum++;
                return true;
            }
            else
            {
                errmsg += "收件人过多";
                return false;
            }
        }


        /// <summary> 
        /// 最多收件人数量 
        /// </summary> 
        public int RecipientMaxNum
        {
            set
            {
                recipientmaxnum = value;
            }
        }


        /// <summary> 
        /// 添加一组收件人（不超过recipientmaxnum个），参数为字符串数组 
        /// </summary> 
        /// <param name="str">保存有收件人地址的字符串数组（不超过recipientmaxnum个）</param> 
        public bool AddRecipient(string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!AddRecipient(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary> 
        /// 发送SMTP命令 
        /// </summary> 
        private bool SendCommand(string str)
        {
            byte[] WriteBuffer;
            if (str == null || str.Trim() == "")
            {
                return true;
            }
            logs += str;
            WriteBuffer = Encoding.Default.GetBytes(str);
            try
            {
                ns.Write(WriteBuffer, 0, WriteBuffer.Length);
            }
            catch
            {
                errmsg = "网络连接错误";
                return false;
            }
            return true;
        }

        /// <summary> 
        /// 接收SMTP服务器回应 
        /// </summary> 
        private string RecvResponse()
        {
            int StreamSize;
            string ReturnValue = "";
            byte[] ReadBuffer = new byte[1024];
            try
            {
                StreamSize = ns.Read(ReadBuffer, 0, ReadBuffer.Length);
            }
            catch
            {
                errmsg = "网络连接错误";
                return "false";
            }

            if (StreamSize == 0)
            {
                return ReturnValue;
            }
            else
            {
                ReturnValue = Encoding.Default.GetString(ReadBuffer).Substring(0, StreamSize);
                logs += ReturnValue;
                return ReturnValue;
            }
        }


        /// <summary> 
        /// 与服务器交互，发送一条命令并接收回应。 
        /// </summary> 
        /// <param name="Command">一个要发送的命令</param> 
        /// <param name="errstr">如果错误，要反馈的信息</param> 
        private bool Dialog(string str, string errstr)
        {
            if (str == null || str.Trim() == "")
            {
                return true;
            }
            if (SendCommand(str))
            {
                string RR = RecvResponse();
                if (RR == "false")
                {
                    return false;
                }
                string RRCode = RR.Substring(0, 3);
                if (RightCodeHT[RRCode] != null)
                {
                    return true;
                }
                else
                {
                    if (ErrCodeHT[RRCode] != null)
                    {
                        errmsg += (RRCode + ErrCodeHT[RRCode].ToString());
                        errmsg += enter;
                    }
                    else
                    {
                        errmsg += RR;
                    }
                    errmsg += errstr;
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        /// <summary> 
        /// 与服务器交互，发送一组命令并接收回应。 
        /// </summary> 

        private bool Dialog(string[] str, string errstr)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Dialog(str[i], ""))
                {
                    errmsg += enter;
                    errmsg += errstr;
                    return false;
                }
            }

            return true;
        }



        private bool SendEmail()
        {
            //连接网络 
            try
            {
                tc = new TcpClient(mailserver, mailserverport);
            }
            catch (Exception e)
            {
                errmsg = e.ToString();
                return false;
            }

            ns = tc.GetStream();
            SMTPCodeAdd();

            //验证网络连接是否正确 
            if (RightCodeHT[RecvResponse().Substring(0, 3)] == null)
            {
                errmsg = "网络连接失败";
                return false;
            }


            string[] SendBuffer;
            string SendBufferstr;

            //进行SMTP验证 
            if (ESmtp)
            {
                SendBuffer = new String[4];
                SendBuffer[0] = "EHLO " + mailserver + enter;
                SendBuffer[1] = "AUTH LOGIN" + enter;
                SendBuffer[2] = Base64Encode(username) + enter;
                SendBuffer[3] = Base64Encode(password) + enter;
                if (!Dialog(SendBuffer, "SMTP服务器验证失败，请核对用户名和密码。"))
                    return false;
            }
            else
            {
                SendBufferstr = "HELO " + mailserver + enter;
                if (!Dialog(SendBufferstr, ""))
                    return false;
            }

            // 
            SendBufferstr = "MAIL FROM:<" + From + ">" + enter;
            if (!Dialog(SendBufferstr, "发件人地址错误，或不能为空"))
                return false;

            // 
            SendBuffer = new string[recipientmaxnum];
            for (int i = 0; i < Recipient.Count; i++)
            {

                SendBuffer[i] = "RCPT TO:<" + Recipient[i].ToString() + ">" + enter;

            }
            if (!Dialog(SendBuffer, "收件人地址有误"))
                return false;

            SendBufferstr = "DATA" + enter;
            if (!Dialog(SendBufferstr, ""))
                return false;

            SendBufferstr = "From:" + FromName + "<" + From + ">" + enter;

            SendBufferstr += "To:=?" + Charset.ToUpper() + "?B?" + Base64Encode(RecipientName) + "?=" + "<" + Recipient[0] + ">" + enter;
            SendBufferstr += "CC:";
            for (int i = 0; i < Recipient.Count; i++)
            {
                SendBufferstr += Recipient[i].ToString() + "<" + Recipient[i].ToString() + ">,";
            }
            SendBufferstr += enter;


            if (Charset == "")
            {
                SendBufferstr += "Subject:" + Subject + enter;
            }
            else
            {
                SendBufferstr += "Subject:" + "=?" + Charset.ToUpper() + "?B?" + Base64Encode(Subject) + "?=" + enter;
            }

            SendBufferstr += "X-Priority:" + priority + enter;
            SendBufferstr += "X-MSMail-Priority:" + priority + enter;
            SendBufferstr += "Importance:" + priority + enter;
            SendBufferstr += "X-Mailer: Huolx.Pubclass" + enter;
            SendBufferstr += "MIME-Version: 1.0" + enter;

            SendBufferstr += "Content-Type: multipart/mixed;" + enter;//内容格式和分隔符 
            SendBufferstr += " boundary=\"----=_NextPart_000_00D6_01C29593.AAB31770\"" + enter;
            SendBufferstr += "------=_NextPart_000_00D6_01C29593.AAB31770" + enter;

            if (Html)
            {
                SendBufferstr += "Content-Type: text/html;" + enter;
            }
            else
            {
                SendBufferstr += "Content-Type: text/plain;" + enter;
            }

            if (Charset == "")
            {
                SendBufferstr += " charset=\"iso-8859-1\"" + enter;
            }
            else
            {
                SendBufferstr += " charset=\"" + Charset.ToLower() + "\"" + enter;
            }
            //SendBufferstr += "Content-Transfer-Encoding: base64"+enter; 

            SendBufferstr += "Content-Transfer-Encoding: base64" + enter + enter;

            SendBufferstr += Base64Encode(Body) + enter;
            if (Attachments.Count != 0)
            {
                foreach (string filepath in Attachments)
                {

                    SendBufferstr += "------=_NextPart_000_00D6_01C29593.AAB31770" + enter;
                    SendBufferstr += "Content-Type: application/octet-stream" + enter;
                    SendBufferstr += " name=\"=?" + Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\") + 1)) + "?=\"" + enter;
                    SendBufferstr += "Content-Transfer-Encoding: base64" + enter;
                    SendBufferstr += "Content-Disposition: attachment;" + enter;
                    SendBufferstr += " filename=\"=?" + Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\") + 1)) + "?=\"" + enter + enter;
                    SendBufferstr += GetStream(filepath) + enter + enter;
                }
            }
            SendBufferstr += "------=_NextPart_000_00D6_01C29593.AAB31770--" + enter + enter;


            SendBufferstr += enter + "." + enter;

            if (!Dialog(SendBufferstr, "错误信件信息"))
                return false;


            SendBufferstr = "QUIT" + enter;
            if (!Dialog(SendBufferstr, "断开连接时错误"))
                return false;


            ns.Close();
            tc.Close();
            return true;
        }


        /// <summary> 
        /// 发送邮件方法，所有参数均通过属性设置。 
        /// </summary> 
        public bool Send()
        {
            if (Recipient.Count == 0)
            {
                errmsg = "收件人列表不能为空";
                return false;
            }

            if (mailserver.Trim() == "")
            {
                errmsg = "必须指定SMTP服务器";
                return false;
            }

            return SendEmail();

        }


        /// <summary> 
        /// 发送邮件方法 
        /// </summary> 
        /// <param name="smtpserver">smtp服务器信息，如"username:password@www.smtpserver.com:25"，也可去掉部分次要信息，如"www.smtpserver.com"</param> 
        public bool Send(string smtpserver)
        {

            MailDomain = smtpserver;
            return Send();
        }


        /// <summary> 
        /// 发送邮件方法 
        /// </summary> 
        /// <param name="smtpserver">smtp服务器信息，如"username:password@www.smtpserver.com:25"，也可去掉部分次要信息，如"www.smtpserver.com"</param> 
        /// <param name="from">发件人mail地址</param> 
        /// <param name="fromname">发件人姓名</param> 
        /// <param name="to">收件人地址</param> 
        /// <param name="toname">收件人姓名</param> 
        /// <param name="html">是否HTML邮件</param> 
        /// <param name="subject">邮件主题</param> 
        /// <param name="body">邮件正文</param> 
        public bool Send(string smtpserver, string from, string fromname, string to, string toname, bool html, string subject, string body)
        {
            MailDomain = smtpserver;
            From = from;
            FromName = fromname;
            AddRecipient(to);
            RecipientName = toname;
            Html = html;
            Subject = subject;
            Body = body;
            return Send();
        } 
    }
}
