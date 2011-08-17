using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DME.Base.Helper
{
    /// <summary>
    /// 日期时间帮助类
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
    public static class DME_DateTime
    {
        #region 私有变量
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
        /// <summary>判断当前年份是否是闰年，私有函数</summary>
        /// <param name="iYear">年份</param>
        /// <returns>是闰年：True ，不是闰年：False</returns>
        private static bool IsRuYear(int iYear)
        {
            //形式参数为年份
            //例如：2003
            int n;
            n = iYear;

            if ((n % 400 == 0) || (n % 4 == 0 && n % 100 != 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 公开函数
        /// <summary>本地时间转换成UTC时间</summary>
        /// <param name="vDate">待转换的时间</param>
        /// <param name="Milliseconds">是否精确到毫秒</param>
        /// <returns>UTC时间</returns>
        public static long DateTimeToUTC(DateTime vDate, bool Milliseconds)
        {
            TimeZone tz = TimeZone.CurrentTimeZone;
            vDate = vDate.ToUniversalTime();
            DateTime dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Milliseconds)
            {
                return (long)vDate.Subtract(dtZone).TotalMilliseconds;
            }
            else
            {
                return (long)vDate.Subtract(dtZone).TotalSeconds; //如果你觉得有误差，此处返回TotalMilliseconds,都精确到毫秒了。。。
            }

        }

        /// <summary>UTC时间转换成本地时间</summary>
        /// <param name="l">UTC时间</param>
        /// <param name="Milliseconds">是否精确到毫秒</param>
        /// <returns>DateTime</returns>
        public static DateTime UTCToDateTime(long l, bool Milliseconds)
        {
            DateTime dtZone = new DateTime(1970, 1, 1, 0, 0, 0);

            if (Milliseconds)
            {
                dtZone = dtZone.AddMilliseconds((double)l);
            }
            else
            {
                dtZone = dtZone.AddSeconds((double)l);
            }
            return dtZone.ToLocalTime();
        }

        /// <summary>验证是否为时间格式</summary>
        /// <param name="timeval"></param>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>判断用户输入是否为日期</summary>
        /// <param name="strln"></param>
        /// <returns></returns>
        /// <remarks>
        /// 可判断格式如下（其中-可替换为/，不影响验证)
        /// YYYY | YYYY-MM | YYYY-MM-DD | YYYY-MM-DD HH:MM:SS | YYYY-MM-DD HH:MM:SS.FFF
        /// </remarks>
        public static bool IsDateTime(string strln)
        {
            if (null == strln)
            {
                return false;
            }
            string regexDate = @"[1-2]{1}[0-9]{3}((-|\/|\.){1}(([0]?[1-9]{1})|(1[0-2]{1}))((-|\/|\.){1}((([0]?[1-9]{1})|([1-2]{1}[0-9]{1})|(3[0-1]{1})))( (([0-1]{1}[0-9]{1})|2[0-3]{1}):([0-5]{1}[0-9]{1}):([0-5]{1}[0-9]{1})(\.[0-9]{3})?)?)?)?$";
            if (Regex.IsMatch(strln, regexDate))
            {
                //以下各月份日期验证，保证验证的完整性
                int _IndexY = -1;
                int _IndexM = -1;
                int _IndexD = -1;
                if (-1 != (_IndexY = strln.IndexOf("-")))
                {
                    _IndexM = strln.IndexOf("-", _IndexY + 1);
                    _IndexD = strln.IndexOf(":");
                }
                else
                {
                    _IndexY = strln.IndexOf("/");
                    _IndexM = strln.IndexOf("/", _IndexY + 1);
                    _IndexD = strln.IndexOf(":");
                }
                //不包含日期部分，直接返回true
                if (-1 == _IndexM)
                    return true;
                if (-1 == _IndexD)
                {
                    _IndexD = strln.Length + 3;
                }
                int iYear = DME_TypeParse.StringToInt32(strln.Substring(0, _IndexY));
                int iMonth = DME_TypeParse.StringToInt32(strln.Substring(_IndexY + 1, _IndexM - _IndexY - 1));
                int iDate = DME_TypeParse.StringToInt32(strln.Substring(_IndexM + 1, _IndexD - _IndexM - 4));
                //判断月份日期
                if ((iMonth < 8 && 1 == iMonth % 2) || (iMonth > 8 && 0 == iMonth % 2))
                {
                    if (iDate < 32)
                        return true;
                }
                else
                {
                    if (iMonth != 2)
                    {
                        if (iDate < 31)
                            return true;
                    }
                    else
                    {
                        //闰年
                        if ((0 == iYear % 400) || (0 == iYear % 4 && 0 < iYear % 100))
                        {
                            if (iDate < 30)
                                return true;
                        }
                        else
                        {
                            if (iDate < 29)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>将字符转成DateTime,如果转换异常则返回 今天</summary>
        /// <param name="str">字符</param>
        /// <returns>DateTime</returns>
        public static DateTime StringToDateTime(string str)
        {
            DateTime _Str;
            if (DateTime.TryParse(str, out _Str))
            {
                return _Str;
            }
            else
            {
                return DateTime.Today;
            }
        }

        /// <summary>返回本年有多少天</summary>
        /// <param name="iYear">年份</param>
        /// <returns>本年的天数</returns>
        public static int GetDaysOfYear(int iYear)
        {
            int cnt = 0;
            if (IsRuYear(iYear))
            {
                //闰年多 1 天 即：2 月为 29 天
                cnt = 366;

            }
            else
            {
                //--非闰年少1天 即：2 月为 28 天
                cnt = 365;
            }
            return cnt;
        }

        /// <summary>本年有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>本年的天数</returns>
        public static int GetDaysOfYear(DateTime idt)
        {
            int n;

            //取得传入参数的年份部分，用来判断是否是闰年

            n = idt.Year;
            if (IsRuYear(n))
            {
                //闰年多 1 天 即：2 月为 29 天
                return 366;
            }
            else
            {
                //--非闰年少1天 即：2 月为 28 天
                return 365;
            }

        }

        /// <summary>本月有多少天</summary>
        /// <param name="iYear">年</param>
        /// <param name="Month">月</param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(int iYear, int Month)
        {
            int days = 0;
            switch (Month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    if (IsRuYear(iYear))
                    {
                        //闰年多 1 天 即：2 月为 29 天
                        days = 29;
                    }
                    else
                    {
                        //--非闰年少1天 即：2 月为 28 天
                        days = 28;
                    }

                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }

            return days;


        }

        /// <summary>本月有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(DateTime dt)
        {
            //--------------------------------//
            //--从dt中取得当前的年，月信息  --//
            //--------------------------------//
            int year, month, days = 0;
            year = dt.Year;
            month = dt.Month;

            //--利用年月信息，得到当前月的天数信息。
            switch (month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    if (IsRuYear(year))
                    {
                        //闰年多 1 天 即：2 月为 29 天
                        days = 29;
                    }
                    else
                    {
                        //--非闰年少1天 即：2 月为 28 天
                        days = 28;
                    }

                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }

            return days;

        }

        /// <summary>取每月的第一/最末一天</summary>
        /// <param name="time">传入时间</param>
        /// <param name="firstDay">第一天还是最末一天</param>
        /// <returns></returns>
        public static DateTime DayOfMonth(DateTime time, bool firstDay)
        {
            DateTime time1 = new DateTime(time.Year, time.Month, 1);
            if (firstDay) return time1;
            else return time1.AddMonths(1).AddDays(-1);
        }

        /// <summary>取每季度的第一/最末一天</summary>
        /// <param name="time">传入时间</param>
        /// <param name="firstDay">第一天还是最末一天</param>
        /// <returns></returns>
        public static DateTime DayOfQuarter(DateTime time, bool firstDay)
        {
            int m = 0;
            switch (time.Month)
            {
                case 1:
                case 2:
                case 3:
                    m = 1; break;
                case 4:
                case 5:
                case 6:
                    m = 4; break;
                case 7:
                case 8:
                case 9:
                    m = 7; break;
                case 10:
                case 11:
                case 12:
                    m = 11; break;
            }

            DateTime time1 = new DateTime(time.Year, m, 1);
            if (firstDay) return time1;
            else return time1.AddMonths(3).AddDays(-1);
        }

        /// <summary>取每年的第一/最末一天</summary>
        /// <param name="time">传入时间</param>
        /// <param name="firstDay">第一天还是最末一天</param>
        /// <returns></returns>
        public static DateTime DayOfYear(DateTime time, bool firstDay)
        {
            if (firstDay) return new DateTime(time.Year, 1, 1);
            else return new DateTime(time.Year, 12, 31);
        }

        /// <summary>返回当前日期的星期名称</summary>
        /// <param name="dt">日期</param>
        /// <returns>星期名称</returns>
        public static string GetWeekNameOfDay(DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Mondy":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;

            }
            return week;
        }

        /// <summary>返回当前日期的星期编号</summary>
        /// <param name="dt">日期</param>
        /// <returns>星期数字编号</returns>
        public static string GetWeekNumberOfDay(DateTime idt)
        {
            string dt, week = "";

            dt = idt.DayOfWeek.ToString();
            switch (dt)
            {
                case "Mondy":
                    week = "1";
                    break;
                case "Tuesday":
                    week = "2";
                    break;
                case "Wednesday":
                    week = "3";
                    break;
                case "Thursday":
                    week = "4";
                    break;
                case "Friday":
                    week = "5";
                    break;
                case "Saturday":
                    week = "6";
                    break;
                case "Sunday":
                    week = "7";
                    break;

            }

            return week;
        }

        /// <summary>获取两个日期之间的差值,获取年数使用 (int)DateDiff(string howtocompare, DateTime startDate, DateTime endDate)</summary>
        /// <param name="howtocompare">比较的方式：year month day hour minute second</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>时间差</returns>
        public static double DateDiff(string howtocompare, DateTime startDate, DateTime endDate)
        {
            double diff = 0;
            try
            {
                TimeSpan TS = new TimeSpan(endDate.Ticks - startDate.Ticks);

                switch (howtocompare.ToLower())
                {
                    case "year":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "month":
                        diff = Convert.ToDouble((TS.TotalDays / 365) * 12);
                        break;
                    case "day":
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                    case "hour":
                        diff = Convert.ToDouble(TS.TotalHours);
                        break;
                    case "minute":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "second":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                }
            }
            catch (Exception)
            {
                diff = 0;
            }
            return diff;
        }

        /// <summary>将日期对象转化为格式字符串 如：yyyy-MM-dd HH:mm:ss:fffffff</summary>
        /// <param name="oDateTime">日期对象</param>
        /// <param name="strFormat">
        /// 格式：
        ///        "SHORTDATE"===短日期
        ///        "LONGDATE"==长日期
        ///        其它====自定义格式
        /// </param>
        /// <returns>日期字符串</returns>
        public static string DateTimeFormat(DateTime oDateTime, string strFormat)
        {
            string strDate = "";
            try
            {
                switch (strFormat.ToUpper())
                {
                    case "SHORTDATE":
                        strDate = oDateTime.ToShortDateString();
                        break;
                    case "LONGDATE":
                        strDate = oDateTime.ToLongDateString();
                        break;
                    default:
                        strDate = oDateTime.ToString(strFormat);
                        break;
                }
            }
            catch (Exception)
            {
                strDate = oDateTime.ToShortDateString();
            }

            return strDate;
        }

        /// <summary>获取上周的今天</summary>
        /// <returns></returns>
        public static DateTime GetPreviousWeek()
        {
            return DateTime.Now.AddDays(-7);
        }

        /// <summary>获取上周的今天</summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetPreviousWeek(DateTime datetime)
        {
            return datetime.AddDays(-7);
        }

        /// <summary>获取下周的今天</summary>
        /// <returns></returns>
        public static DateTime GetNextWeek()
        {
            return DateTime.Now.AddDays(7);
        }

        /// <summary>获取下周的今天</summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetNextWeek(DateTime datetime)
        {
            return datetime.AddDays(7);
        }

        /// <summary>在获取离dt最近的一个周一 </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetLastMondayDate(DateTime dt)
        {
            DateTime temp = dt;

            while (temp.DayOfWeek != DayOfWeek.Monday)
            {
                temp = temp.AddDays(-1);
            }

            return temp;
        }

        /// <summary>返回远程国际标准时间(如果网络不能会有异常记得try)</summary>
        /// <returns></returns>
        public static DateTime ServerDateTime()
        {
            //只使用的时间服务器的IP地址，未使用域名
            string[,] 时间服务器 = new string[14, 2];
            int[] 搜索顺序 = new int[] { 3, 2, 4, 8, 9, 6, 11, 5, 10, 0, 1, 7, 12 };
            时间服务器[0, 0] = "time-a.nist.gov";
            时间服务器[0, 1] = "129.6.15.28";
            时间服务器[1, 0] = "time-b.nist.gov";
            时间服务器[1, 1] = "129.6.15.29";
            时间服务器[2, 0] = "time-a.timefreq.bldrdoc.gov";
            时间服务器[2, 1] = "132.163.4.101";
            时间服务器[3, 0] = "time-b.timefreq.bldrdoc.gov";
            时间服务器[3, 1] = "132.163.4.102";
            时间服务器[4, 0] = "time-c.timefreq.bldrdoc.gov";
            时间服务器[4, 1] = "132.163.4.103";
            时间服务器[5, 0] = "utcnist.colorado.edu";
            时间服务器[5, 1] = "128.138.140.44";
            时间服务器[6, 0] = "time.nist.gov";
            时间服务器[6, 1] = "192.43.244.18";
            时间服务器[7, 0] = "time-nw.nist.gov";
            时间服务器[7, 1] = "131.107.1.10";
            时间服务器[8, 0] = "nist1.symmetricom.com";
            时间服务器[8, 1] = "69.25.96.13";
            时间服务器[9, 0] = "nist1-dc.glassey.com";
            时间服务器[9, 1] = "216.200.93.8";
            时间服务器[10, 0] = "nist1-ny.glassey.com";
            时间服务器[10, 1] = "208.184.49.9";
            时间服务器[11, 0] = "nist1-sj.glassey.com";
            时间服务器[11, 1] = "207.126.98.204";
            时间服务器[12, 0] = "nist1.aol-ca.truetime.com";
            时间服务器[12, 1] = "207.200.81.113";
            时间服务器[13, 0] = "nist1.aol-va.truetime.com";
            时间服务器[13, 1] = "64.236.96.53";
            int portNum = 13;
            string hostName;
            byte[] bytes = new byte[1024];
            int bytesRead = 0;
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            for (int i = 0; i < 13; i++)
            {
                hostName = 时间服务器[搜索顺序[i], 1];
                try
                {
                    client.Connect(hostName, portNum);
                    System.Net.Sockets.NetworkStream ns = client.GetStream();
                    bytesRead = ns.Read(bytes, 0, bytes.Length);
                    client.Close();
                    break;
                }
                catch (System.Exception)
                {
                }
            }
            char[] sp = new char[1];
            sp[0] = ' ';
            System.DateTime dt = new DateTime();
            string str1;
            str1 = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

            string[] s;
            s = str1.Split(sp);
            dt = System.DateTime.Parse(s[1] + " " + s[2]);//得到标准时间
            dt = dt.AddHours(8);//得到北京时间*/
            return dt;

        }
        #endregion
    }
}
