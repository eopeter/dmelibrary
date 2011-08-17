using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DME.Base.Helper
{
    /// <summary>
    /// 字符串处理类
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
    public static class DME_String
    {
        #region 私有变量
        private static int[] pyValue = new int[]
{
-20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
-20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
-19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
-19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
-19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
-19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
-18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
-18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
-17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
-17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
-17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
-16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
-16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
-16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
-15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
-15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
-15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
-15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
-14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
-14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
-14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
-14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
-14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
-13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
-13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
-13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
-13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
-12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
-12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
-11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
-11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
-10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
-10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
};
        private static string[] pyName = new string[]
{
"A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
"Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
"Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
"Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
"Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
"Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
"Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
"Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
"Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
"Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
"Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
"Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
"Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
"La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
"Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
"Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
"Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
"Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
"Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
"Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
"Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
"Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
"Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
"Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
"Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
"Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
"Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
"Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
"Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
"Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
"Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
"Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
"Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
};
        #endregion

        #region 公有变量
        #endregion

        #region 构造
        #endregion

        #region 析构
        #endregion

        #region 属性
        #endregion

        #region 私有函数
        /// <summary>取单个字符的拼音声母</summary>
        /// <param name="singleChinese">要转换的单个汉字</param>
        /// <returns>拼音首字母</returns>
        private static string GetFirstLetterFromSingleChinese(string singleChinese)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(singleChinese);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));

            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "g";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";

            return "*";
        }
        #endregion

        #region 公开函数
        
        /// <summary>取指定长度的字符串</summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {


            string myResult = p_SrcString;

            //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") ||
                System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
            {
                //当截取的起始位置超出字段串长度时
                if (p_StartIndex >= p_SrcString.Length)
                {
                    return "";
                }
                else
                {
                    return p_SrcString.Substring(p_StartIndex,
                                                   ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }


            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }



                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {

                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                    {
                        nRealLength = p_Length + 1;
                    }

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);

                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }

        /// <summary>字符串如果操过指定长度则将超出的部分用指定字符串代替</summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        /// <summary>比较两个字符串是否相等</summary>
        /// <param name="a">字符串a</param>
        /// <param name="b">字符串b</param>
        /// <param name="capital">是否区分大小写</param>
        /// <returns>true or false</returns>
        public static bool StringCompare(string a, string b, bool capital)
        {
            return string.Compare(a, b, (capital) ? false : true, CultureInfo.InvariantCulture) == 0;
        }

        /// <summary>获取一个GUID</summary>
        /// <returns>GUID</returns>
        public static string NewGuid()
        {
            return System.Guid.NewGuid().ToString();
        } 

        /// <summary>清除给定字符串中的回车及换行符</summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBR(string str)
        {
            Match m = null;
            Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
            for (m = RegexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }
            return str;
        }

        /// <summary>删除字符串尾部的回车/换行/空格</summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (int i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }

        /// <summary>自定义的替换字符串函数</summary>
        /// <param name="SourceString">要操作的字符串</param>
        /// <param name="SearchString">要查找的字符</param>
        /// <param name="ReplaceString">替换的字符</param>
        /// <param name="IsCaseInsensetive">是否区分大小写</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>分割字符串</summary>
        /// <param name="strContent">待分割字符串</param>
        /// <param name="strSplit">分隔符</param>
        /// <returns>string[]</returns>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }

        /// <summary>分割字符串</summary>
        /// <param name="strContent">待分割字符串</param>
        /// <param name="strSplit">分隔符</param>
        /// <param name="p_3">指定分割数组大小</param>
        /// <returns>string[]</returns>
        public static string[] SplitString(string strContent, string strSplit, int p_3)
        {
            string[] result = new string[p_3];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < p_3; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>判断指定字符串在指定字符串数组中的位置</summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else
                {
                    if (strSearch == stringArray[i])
                    {
                        return i;
                    }
                }

            }
            return -1;
        }

        /// <summary>从右边填充字符串</summary>   
        /// <param name="strOriginal">要操作的字符串</param>   
        /// <param name="totalWidth">填充的字符数</param>   
        /// <param name="paddingChar">填充的字符</param>   
        /// <returns>string</returns>   
        public static string RightPadStr(string strOriginal, int totalWidth, char paddingChar)
        {
            if (strOriginal.Length < totalWidth)
                return strOriginal.PadRight(totalWidth, paddingChar);
            return strOriginal;
        }

        /// <summary>从左边填充字符串 </summary>   
        /// <param name="strOriginal">要操作的字符串</param>   
        /// <param name="totalWidth">填充的字符数</param>   
        /// <param name="paddingChar">填充的字符</param>   
        /// <returns>string</returns>   
        public static string LeftPadStr(string strOriginal, int totalWidth, char paddingChar)
        {
            if (strOriginal.Length < totalWidth)
                return strOriginal.PadLeft(totalWidth, paddingChar);
            return strOriginal;
        }

        /// <summary>裁切字符串（中文按照两个字符计算）</summary>    
        /// <param name="str">旧字符串</param>
        /// <param name="len">新字符串长度</param>
        /// <param name="HtmlEnable">为 false 时过滤 Html 标签后再进行裁切，反之则保留 Html 标签。</param>
        /// <remarks>
        /// <para>注意：<ol>
        /// <li>若字符串被截断则会在末尾追加“...”，反之则直接返回原始字符串。</li>
        /// <li>参数 <paramref name="HtmlEnable"/> 为 false 时会先调用<see cref="Common.Functions.HtmlFilter"/>过滤掉 Html 标签再进行裁切。</li>
        /// <li>中文按照两个字符计算。若指定长度位置恰好只获取半个中文字符，则会将其补全，如下面的例子：<br/>
        /// <code><![CDATA[
        /// string str = "感谢使用。";
        /// string A = CutStr(str,4);   // A = "感谢..."
        /// string B = CutStr(str,5);   // B = "感谢使..."
        /// ]]></code></li>
        /// </ol>
        /// </para>
        /// </remarks>       
        public static string CutStr(string str, int len, bool HtmlEnable)
        {
            if (str == null || str.Length == 0 || len <= 0) { return string.Empty; }

            if (HtmlEnable == false) str = HTMLToString(str);
            int l = str.Length;

            #region 计算长度
            int clen = 0;//当前长度 
            while (clen < len && clen < l)
            {
                //每遇到一个中文，则将目标长度减一。
                if ((int)str[clen] > 128) { len--; }
                clen++;
            }
            #endregion

            if (clen < l)
            {
                return str.Substring(0, clen) + "...";
            }
            else
            {
                return str;
            }
        }

        /// <summary>获取字符串长度。与string.Length不同的是，该方法将中文作 2 个字符计算。</summary>
        /// <param name="str">目标字符串</param>
        /// <returns></returns>
        public static int GetLength(string str)
        {
            if (str == null || str.Length == 0) { return 0; }

            int l = str.Length;
            int realLen = l;

            #region 计算长度
            int clen = 0;//当前长度 
            while (clen < l)
            {
                //每遇到一个中文，则将实际长度加一。
                if ((int)str[clen] > 128) { realLen++; }
                clen++;
            }
            #endregion

            return realLen;
        }

        /// <summary>把汉字转换成拼音(全拼)</summary>
        /// <param name="chinese">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string ChineseToFullPinYin(string chinese)
        {
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
            byte[] array = new byte[2];
            string pyString = "";
            int chrAsc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] noWChar = chinese.ToCharArray();

            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    array = System.Text.Encoding.Default.GetBytes(noWChar[j].ToString());
                    i1 = (short)(array[0]);
                    i2 = (short)(array[1]);
                    chrAsc = i1 * 256 + i2 - 65536;
                    if (chrAsc > 0 && chrAsc < 160)
                    {
                        pyString += noWChar[j];
                    }
                    else
                    {
                        // 修正部分文字
                        if (chrAsc == -9254) // 修正"圳"字
                            pyString += "Zhen";
                        else
                        {
                            for (int i = (pyValue.Length - 1); i >= 0; i--)
                            {
                                if (pyValue[i] <= chrAsc)
                                {
                                    pyString += pyName[i];
                                    break;
                                }
                            }
                        }
                    }
                }
                // 非中文字符
                else
                {
                    pyString += noWChar[j].ToString();
                }
            }
            return pyString;
        }

        /// <summary> 获得某个字符串在另个字符串中出现的次数</summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strSymbol">符号</param>   
        /// <returns>返回值</returns>   
        public static int GetStrCount(string strOriginal, string strSymbol)
        {
            int count = 0;
            for (int i = 0; i < (strOriginal.Length - strSymbol.Length + 1); i++)
            {
                if (strOriginal.Substring(i, strSymbol.Length) == strSymbol)
                {
                    count = count + 1;
                }
            }
            return count;
        }

        /// <summary>获得某个字符串在另个字符串第一次出现时前面所有字符  </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strSymbol">符号</param>   
        /// <returns>返回值</returns>   
        public static string GetFirstStr(string strOriginal, string strSymbol)
        {
            int strPlace = strOriginal.IndexOf(strSymbol);
            if (strPlace != -1)
                strOriginal = strOriginal.Substring(0, strPlace);
            return strOriginal;
        }

        /// <summary> 获得某个字符串在另个字符串最后一次出现时后面所有字符 </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strSymbol">符号</param>   
        /// <returns>返回值</returns>   
        public static string GetLastStr(string strOriginal, string strSymbol)
        {
            int strPlace = strOriginal.LastIndexOf(strSymbol) + strSymbol.Length;
            strOriginal = strOriginal.Substring(strPlace);
            return strOriginal;
        }

        /// <summary> 获得两个字符之间第一次出现时前面所有字符 </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strFirst">最前哪个字符</param>   
        /// <param name="strLast">最后哪个字符</param>   
        /// <returns>返回值</returns>   
        public static string GetTwoMiddleFirstStr(string strOriginal, string strFirst, string strLast)
        {
            strOriginal = GetFirstStr(strOriginal, strLast);
            strOriginal = GetLastStr(strOriginal, strFirst);
            return strOriginal;
        }

        /// <summary>获得两个字符之间最后一次出现时的所有字符 </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strFirst">最前哪个字符</param>   
        /// <param name="strLast">最后哪个字符</param>   
        /// <returns>返回值</returns>   
        public static string GetTwoMiddleLastStr(string strOriginal, string strFirst, string strLast)
        {
            strOriginal = GetLastStr(strOriginal, strFirst);
            strOriginal = GetFirstStr(strOriginal, strLast);
            return strOriginal;
        }

        /// <summary>将全角数字转换为数字</summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }

        /// <summary>转全角的函数(SBC case)</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DBCToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>转半角的函数(DBC case)</summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        public static string SBCToDBC(string input)
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
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>将 GB2312 值转换为 UTF8 字符串(如：测试 -> 娴嬭瘯 )</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GB2312ToUTF8(string source)
        {
            return Encoding.GetEncoding("GB2312").GetString(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>将 UTF8 值转换为 GB2312 字符串 (如：娴嬭瘯 -> 测试)</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string UTF8ToGB2312(string source)
        {
            return Encoding.UTF8.GetString(Encoding.GetEncoding("GB2312").GetBytes(source));
        }

        ///<summary>字符串转为unicode字符串（如：测试 -> &#27979;&#35797;）</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StringToUnicode(string source)
        {
            StringBuilder sa = new StringBuilder();//Unicode
            string s1;
            string s2;
            for (int i = 0; i < source.Length; i++)
            {
                byte[] bt = System.Text.Encoding.Unicode.GetBytes(source.Substring(i, 1));
                if (bt.Length > 1)//判断是否汉字
                {
                    s1 = Convert.ToString((short)(bt[1] - '\0'), 16);//转化为16进制字符串
                    s2 = Convert.ToString((short)(bt[0] - '\0'), 16);//转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1;//不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2;//不足位补0
                    sa.Append("&#" + Convert.ToInt32(s1 + s2, 16) + ";");
                }
            }

            return sa.ToString();
        }

        /// <summary>字符串转为UTF8字符串(如：测试 -> \u6d4b\u8bd5)</summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StringToUTF8(string source)
        {
            StringBuilder sb = new StringBuilder();//UTF8
            string s1;
            string s2;
            for (int i = 0; i < source.Length; i++)
            {
                byte[] bt = System.Text.Encoding.Unicode.GetBytes(source.Substring(i, 1));
                if (bt.Length > 1)//判断是否汉字
                {
                    s1 = Convert.ToString((short)(bt[1] - '\0'), 16);//转化为16进制字符串
                    s2 = Convert.ToString((short)(bt[0] - '\0'), 16);//转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1;//不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2;//不足位补0
                    sb.Append("\\u" + s1 + s2);
                }
            }

            return sb.ToString();
        }

        /// <summary>转化为ASC码方法</summary>
        /// <param name="txt">字符</param>
        /// <returns>Ascii码</returns>
        public static string StringToAsc(string source)
        {
            string newtxt = "";
            foreach (char c in source)
            {

                newtxt += Convert.ToString((int)c);
            }
            return newtxt;

        }

        /// <summary>转化为金额数据</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToMoneyCHN(object obj)
        {
            return ToMoneyCHN(DME_TypeParse.StringToDouble(obj.ToString()));
        }

        /// <summary>转化为金额数据</summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string ToMoneyCHN(double money)
        {
            return money.ToString("c");
        }

        /// <summary>删除指定位置指定长度字符串</summary>   
        /// <param name="strOriginal">要操作的字符串</param>   
        /// <param name="startIndex">开始删除字符的位置</param>   
        /// <param name="count">要删除的字符数</param>   
        /// <returns>string</returns>   
        public static string RemoveStr(string strOriginal, int startIndex, int count)
        {
            return strOriginal.Remove(startIndex, count);
        }

        /// <summary>移除字符串首尾某些字符</summary>   
        /// <param name="strOriginal">要操作的字符串</param>   
        /// <param name="startStr">要在字符串首部移除的字符串</param>   
        /// <param name="endStr">要在字符串尾部移除的字符串</param>   
        /// <returns>string</returns>   
        public static string RemoveStartOrEndStr(string strOriginal, string startStr, string endStr)
        {
            char[] start = startStr.ToCharArray();
            char[] end = endStr.ToCharArray();
            return strOriginal.TrimStart(start).TrimEnd(end);
        }

        /// <summary>过滤html标签</summary>
        /// <param name="strHtml">html的内容</param>
        /// <returns></returns>
        public static string HTMLToString(string strHtml)
        {
            string[] aryReg ={
								  @"<script[^>]*?>.*?</script>",

								  @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
								  @"([\r\n])[\s]+",
								  @"&(quot|#34);",
								  @"&(amp|#38);",
								  @"&(lt|#60);",
								  @"&(gt|#62);", 
								  @"&(nbsp|#160);", 
								  @"&(iexcl|#161);",
								  @"&(cent|#162);",
								  @"&(pound|#163);",
								  @"&(copy|#169);",
								  @"&#(\d+);",
								  @"-->",
								  @"<!--.*\n"
							  };

            string[] aryRep = {
								   "",
								   "",
								   "",
								   "\"",
								   "&",
								   "<",
								   ">",
								   " ",
								   "\xa1",//chr(161),
								   "\xa2",//chr(162),
								   "\xa3",//chr(163),
								   "\xa9",//chr(169),
								   "",
								   "\r\n",
								   ""
							   };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        /// <summary>将HTML代码替换为页面文本形式。</summary>
        /// <param name="str"></param>
        /// <remarks>
        /// <![CDATA[替换了：&、>、<、'、"、Tab、空格、换行符、回车符。]]>
        /// </remarks>
        public static string HtmlEnCode(string str)
        {
            if (DME_Validation.IsNull(str))
            {
                return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder(str);
                sb.Replace(@"&", @"&amp;");
                sb.Replace(@">", @"&gt;");
                sb.Replace(@"<", @"&lt;");
                sb.Replace(Char.ConvertFromUtf32(32), @"&nbsp;");
                sb.Replace(Char.ConvertFromUtf32(9), @"&nbsp;&nbsp;&nbsp;&nbsp;");
                sb.Replace(Char.ConvertFromUtf32(34), @"&quot;");
                sb.Replace(Char.ConvertFromUtf32(39), @"&#39;");
                sb.Replace(Char.ConvertFromUtf32(13), @"");
                sb.Replace(Char.ConvertFromUtf32(10), @"<br />");
                return sb.ToString();
            }
        }

        /// <summary>将页面文本形式字符串还原成HTML代码。</summary>
        /// <param name="str"></param>
        /// <remarks>
        /// <![CDATA[替换了：&、>、<、'、"、Tab、空格、换行符、回车符。]]>
        /// </remarks>
        public static string HtmlDeCode(string str)
        {
            if (DME_Validation.IsNull(str.Trim()))
            {
                return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder(str);
                sb.Replace(@"&amp;", @"&");
                sb.Replace(@"&gt;", @">");
                sb.Replace(@"&lt;", @"<");
                sb.Replace(@"&nbsp;&nbsp;&nbsp;&nbsp;", Char.ConvertFromUtf32(9));
                sb.Replace(@"&nbsp;", Char.ConvertFromUtf32(32));
                sb.Replace(@"&#39;", Char.ConvertFromUtf32(39));
                sb.Replace(@"<br />", Char.ConvertFromUtf32(10) + Char.ConvertFromUtf32(13));
                return sb.ToString();
            }
        }

        /// <summary>将HTML字符串格式化为JavaScirpt字符串</summary>
        /// <param name="str">待格式化的字符串</param>
        /// <returns>格式化后字符串</returns>
        public static string HtmlToJs(string str)
        {
            if (DME_Validation.IsNull(str.Trim())) { return string.Empty; }
            else
            {
                StringBuilder sb = new StringBuilder(str);
                sb.Replace(Char.ConvertFromUtf32(34), @"\" + Char.ConvertFromUtf32(34));
                sb.Replace(@"\n", @"\\n");
                sb.Replace(Environment.NewLine, @"\n");
                sb.Replace(@"\r", @"\\r");
                sb.Replace(Char.ConvertFromUtf32(9), "\r");
                sb.Replace(@"\/", @"\\/");
                sb.Replace(@"\'", @"\\\'");
                return sb.ToString();
            }
        }

        /// <summary>各配置项采用分号分隔，使用"="连接key与value</summary>
        /// <param name="configStr"></param>
        /// <returns></returns>
        public static IDictionary<string, string> AnalyzeConfigString(string configStr) //IP=127.0.0.1; DataSource=TestMap ;User=sa ;Password=chenqi;DataBase=TestMap
        {
            if ((configStr == null) && (configStr.Trim() == ""))
            {
                return null;
            }

            string[] groups = configStr.Split(';');
            IDictionary<string, string> dic = new Dictionary<string, string>();

            foreach (string group in groups)
            {
                string groupOk = group.Trim();
                if (groupOk != "")
                {
                    string[] keyVal = groupOk.Split('=');
                    if (keyVal.Length > 1)
                    {
                        dic.Add(keyVal[0].Trim(), keyVal[1].Trim());
                    }
                    else
                    {
                        dic.Add(keyVal[0].Trim(), "");
                    }
                }
            }

            return dic;
        }

        /// <summary>把List数组合并成一个List</summary>
        /// <param name="strLists"></param>
        /// <returns></returns>
        public static IList<string> CombineStringList(params IList<string>[] strLists)
        {
            IList<string> list = new List<string>();
            foreach (IList<string> strList in strLists)
            {
                foreach (string str in strList)
                {
                    if (!list.Contains(str))
                    {
                        list.Add(str);
                    }
                }
            }

            return list;
        } 
        #endregion
    }
}
