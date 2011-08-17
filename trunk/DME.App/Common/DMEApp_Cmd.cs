using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DME.App.Common
{
    /// <summary>
    /// CMDִ��������
    /// 
    /// �޸ļ�¼
    ///
    ///		2010.12.18 �汾��1.0 lance ������
    /// 
    /// �汾��1.0
    /// 
    /// <author>
    ///		<name>lance</name>
    ///		<date>2010.12.18</date>
    /// </author> 
    /// </summary>
    public class DMEApp_Cmd
    {
        #region ˽�б���
        #endregion

        #region ���б���
        #endregion

        #region ����
        #endregion

        #region ����
        #endregion

        #region ����
        #endregion

        #region ˽�к���
        #endregion

        #region ��������
        /// <summary>ִ��cmd.exe����</summary>
        /// <param name="commandText">�����ı�</param>
        /// <returns>��������ı�</returns>
        public string ExeCommand(string commandText)
        {
            return ExeCommand(new string[] { commandText });
        }

        /// <summary>ִ�ж���cmd.exe����</summary>
        /// <param name="commandTexts">�����ı�����</param>
        /// <returns>��������ı�</returns>
        public string ExeCommand(string[] commandTexts)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            string strOutput = null;
            try
            {
                p.Start();
                foreach (string item in commandTexts)
                {
                    p.StandardInput.WriteLine(item);
                }
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();
                //strOutput = Encoding.UTF8.GetString(Encoding.Default.GetBytes(strOutput));
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }

        /// <summary>�����ⲿWindowsӦ�ó������س������</summary>
        /// <param name="appName">Ӧ�ó���·������</param>
        /// <returns> true��ʾ�ɹ���false��ʾʧ��</returns>
        public bool StartApp(string appName)
        {
            return StartApp(appName, ProcessWindowStyle.Hidden);
        }

        /// <summary>�����ⲿӦ�ó���</summary>
        /// <param name="appName">Ӧ�ó���·������</param>
        /// <param name="style">���̴���ģʽ</param>
        /// <returns>true��ʾ�ɹ���false��ʾʧ��</returns>
        public bool StartApp(string appName, ProcessWindowStyle style)
        {
            return StartApp(appName, null, style);
        }

        /// <summary>�����ⲿӦ�ó���</summary>
        /// <param name="appName">Ӧ�ó���·������</param>
        /// <param name="arguments">��������</param>
        /// <param name="style">���̴���ģʽ</param>
        /// <returns> true��ʾ�ɹ���false��ʾʧ��</returns>
        public bool StartApp(string appName, string arguments, ProcessWindowStyle style)
        {
            bool blnRst = false;
            Process p = new Process();
            p.StartInfo.FileName = appName;//exe,bat and so on
            p.StartInfo.WindowStyle = style;
            p.StartInfo.Arguments = arguments;
            try
            {
                p.Start();
                p.WaitForExit();
                p.Close();
                blnRst = true;
            }
            catch
            {
            }
            return blnRst;
        }

        /// <summary>ʵ��ѹ��</summary>
        /// <param name="s">Ҫѹ����Ŀ¼·��</param>
        /// <param name="d">ѹ������ļ�·��</param>
        /// <param name="rarpath">rar.ext���ļ�·��</param>
        /// <example>rar("e:/www.svnhost.cn/", "e:/www.svnhost.cn.rar");</example>
        public void Rar(string s, string d,string rarpath)
        {
            ExeCommand(rarpath + " a \"" + d + "\" \"" + s + "\" -ep1");
        }

        /// <summary>ʵ�ֽ�ѹ��</summary>
        /// <param name="s">ѹ���ļ�·��</param>
        /// <param name="d">��ѹ��Ŀ¼·��</param>
        /// <param name="rarpath">rar.ext���ļ�·��</param>
        public void UnRar(string s, string d, string rarpath)
        {
            ExeCommand(rarpath + " x \"" + s + "\" \"" + d + "\" -o+");
        }
        #endregion
    }
}
