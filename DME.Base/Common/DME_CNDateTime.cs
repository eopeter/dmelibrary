﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Helper
{
    /// <summary>
    /// 公历/农历类
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
    public class DME_CNDateTime
    {
        private const ushort START_YEAR = 1901;

        private const ushort END_YEAR = 2050;

        private string[] ConstellationName = 

  { 

	  "白羊座", "金牛座", "双子座", 

	  "巨蟹座", "狮子座", "处女座", 

	  "天秤座", "天蝎座", "射手座", 

	  "摩羯座", "水瓶座", "双鱼座"};

        private string[] LunarHolDayName = 

  { 

	  "小寒", "大寒", "立春", "雨水", 

	  "惊蛰", "春分", "清明", "谷雨", 

	  "立夏", "小满", "芒种", "夏至", 

	  "小暑", "大暑", "立秋", "处暑", 

	  "白露", "秋分", "寒露", "霜降", 

	  "立冬", "小雪", "大雪", "冬至"};



        //数组gLunarDay存入阴历1901年到2100年每年中的月天数信息， 

        //阴历每月只能是29或30天，一年用12（或13）个二进制位表示，对应位为1表30天，否则为29天 

        private int[] gLunarMonthDay = { 

										   //测试数据只有1901.1.1 --2050.12.31 

										   0x4ae0, 0xa570, 0x5268, 0xd260, 0xd950, 0x6aa8, 0x56a0, 0x9ad0, 0x4ae8, 0x4ae0, //1910 

										   0xa4d8, 0xa4d0, 0xd250, 0xd548, 0xb550, 0x56a0, 0x96d0, 0x95b0, 0x49b8, 0x49b0, //1920 

										   0xa4b0, 0xb258, 0x6a50, 0x6d40, 0xada8, 0x2b60, 0x9570, 0x4978, 0x4970, 0x64b0, //1930 

										   0xd4a0, 0xea50, 0x6d48, 0x5ad0, 0x2b60, 0x9370, 0x92e0, 0xc968, 0xc950, 0xd4a0, //1940 

										   0xda50, 0xb550, 0x56a0, 0xaad8, 0x25d0, 0x92d0, 0xc958, 0xa950, 0xb4a8, 0x6ca0, //1950 

										   0xb550, 0x55a8, 0x4da0, 0xa5b0, 0x52b8, 0x52b0, 0xa950, 0xe950, 0x6aa0, 0xad50, //1960 

										   0xab50, 0x4b60, 0xa570, 0xa570, 0x5260, 0xe930, 0xd950, 0x5aa8, 0x56a0, 0x96d0, //1970 

										   0x4ae8, 0x4ad0, 0xa4d0, 0xd268, 0xd250, 0xd528, 0xb540, 0xb6a0, 0x96d0, 0x95b0, //1980 

										   0x49b0, 0xa4b8, 0xa4b0, 0xb258, 0x6a50, 0x6d40, 0xada0, 0xab60, 0x9370, 0x4978, //1990 

										   0x4970, 0x64b0, 0x6a50, 0xea50, 0x6b28, 0x5ac0, 0xab60, 0x9368, 0x92e0, 0xc960, //2000 

										   0xd4a8, 0xd4a0, 0xda50, 0x5aa8, 0x56a0, 0xaad8, 0x25d0, 0x92d0, 0xc958, 0xa950, //2010 

										   0xb4a0, 0xb550, 0xb550, 0x55a8, 0x4ba0, 0xa5b0, 0x52b8, 0x52b0, 0xa930, 0x74a8, //2020 

										   0x6aa0, 0xad50, 0x4da8, 0x4b60, 0x9570, 0xa4e0, 0xd260, 0xe930, 0xd530, 0x5aa0, //2030 

										   0x6b50, 0x96d0, 0x4ae8, 0x4ad0, 0xa4d0, 0xd258, 0xd250, 0xd520, 0xdaa0, 0xb5a0, //2040 

										   0x56d0, 0x4ad8, 0x49b0, 0xa4b8, 0xa4b0, 0xaa50, 0xb528, 0x6d20, 0xada0, 0x55b0}; //2050 



        //数组gLanarMonth存放阴历1901年到2050年闰月的月份，如没有则为0，每字节存两年 

        byte[] gLunarMonth ={ 

							   0x00, 0x50, 0x04, 0x00, 0x20, //1910 

							   0x60, 0x05, 0x00, 0x20, 0x70, //1920 

							   0x05, 0x00, 0x40, 0x02, 0x06, //1930 

							   0x00, 0x50, 0x03, 0x07, 0x00, //1940 

							   0x60, 0x04, 0x00, 0x20, 0x70, //1950 

							   0x05, 0x00, 0x30, 0x80, 0x06, //1960 

							   0x00, 0x40, 0x03, 0x07, 0x00, //1970 

							   0x50, 0x04, 0x08, 0x00, 0x60, //1980 

							   0x04, 0x0a, 0x00, 0x60, 0x05, //1990 

							   0x00, 0x30, 0x80, 0x05, 0x00, //2000 

							   0x40, 0x02, 0x07, 0x00, 0x50, //2010 

							   0x04, 0x09, 0x00, 0x60, 0x04, //2020 

							   0x00, 0x20, 0x60, 0x05, 0x00, //2030 

							   0x30, 0xb0, 0x06, 0x00, 0x50, //2040 

							   0x02, 0x07, 0x00, 0x50, 0x03}; //2050 



        //数组gLanarHoliDay存放每年的二十四节气对应的阳历日期 

        //每年的二十四节气对应的阳历日期几乎固定，平均分布于十二个月中 

        // 1月 2月 3月 4月 5月 6月 

        //小寒 大寒 立春 雨水 惊蛰 春分 清明 谷雨 立夏 小满 芒种 夏至 

        // 7月 8月 9月 10月 11月 12月 

        //小暑 大暑 立秋 处暑 白露 秋分 寒露 霜降 立冬 小雪 大雪 冬至 

        //********************************************************************************* 

        // 节气无任何确定规律,所以只好存表,要节省空间,所以.... 

        //**********************************************************************************} 

        //数据格式说明: 

        //如1901年的节气为 

        // 1月 2月 3月 4月 5月 6月 7月 8月 9月 10月 11月 12月 

        // 6, 21, 4, 19, 6, 21, 5, 21, 6,22, 6,22, 8, 23, 8, 24, 8, 24, 8, 24, 8, 23, 8, 22 

        // 9, 6, 11,4, 9, 6, 10,6, 9,7, 9,7, 7, 8, 7, 9, 7, 9, 7, 9, 7, 8, 7, 15 

        //上面第一行数据为每月节气对应日期,15减去每月第一个节气,每月第二个节气减去15得第二行 

        // 这样每月两个节气对应数据都小于16,每月用一个字节存放,高位存放第一个节气数据,低位存放 

        //第二个节气的数据,可得下表 

        byte[] gLunarHolDay ={ 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1901 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x87, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1902 

								0x96, 0xA5, 0x87, 0x96, 0x87, 0x87, 0x79, 0x69, 0x69, 0x69, 0x78, 0x78, //1903 

								0x86, 0xA5, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x78, 0x87, //1904 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1905 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1906 

								0x96, 0xA5, 0x87, 0x96, 0x87, 0x87, 0x79, 0x69, 0x69, 0x69, 0x78, 0x78, //1907 

								0x86, 0xA5, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1908 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1909 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1910 

								0x96, 0xA5, 0x87, 0x96, 0x87, 0x87, 0x79, 0x69, 0x69, 0x69, 0x78, 0x78, //1911 

								0x86, 0xA5, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1912 

								0x95, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1913 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1914 

								0x96, 0xA5, 0x97, 0x96, 0x97, 0x87, 0x79, 0x79, 0x69, 0x69, 0x78, 0x78, //1915 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1916 

								0x95, 0xB4, 0x96, 0xA6, 0x96, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x87, //1917 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x77, //1918 

								0x96, 0xA5, 0x97, 0x96, 0x97, 0x87, 0x79, 0x79, 0x69, 0x69, 0x78, 0x78, //1919 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1920 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x87, //1921 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x77, //1922 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x87, 0x79, 0x79, 0x69, 0x69, 0x78, 0x78, //1923 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1924 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x87, //1925 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1926 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x87, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1927 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1928 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1929 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1930 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x87, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1931 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1932 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1933 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1934 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1935 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1936 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1937 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1938 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1939 

								0x96, 0xA5, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1940 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1941 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1942 

								0x96, 0xA4, 0x96, 0x96, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1943 

								0x96, 0xA5, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1944 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1945 

								0x95, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x77, //1946 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1947 

								0x96, 0xA5, 0xA6, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //1948 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x79, 0x78, 0x79, 0x77, 0x87, //1949 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x77, //1950 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x79, 0x79, 0x79, 0x69, 0x78, 0x78, //1951 

								0x96, 0xA5, 0xA6, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //1952 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1953 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x78, 0x79, 0x78, 0x68, 0x78, 0x87, //1954 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1955 

								0x96, 0xA5, 0xA5, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //1956 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1957 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1958 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1959 

								0x96, 0xA4, 0xA5, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1960 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1961 

								0x96, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1962 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1963 

								0x96, 0xA4, 0xA5, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1964 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1965 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1966 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1967 

								0x96, 0xA4, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1968 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1969 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1970 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x79, 0x69, 0x78, 0x77, //1971 

								0x96, 0xA4, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1972 

								0xA5, 0xB5, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1973 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1974 

								0x96, 0xB4, 0x96, 0xA6, 0x97, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x77, //1975 

								0x96, 0xA4, 0xA5, 0xB5, 0xA6, 0xA6, 0x88, 0x89, 0x88, 0x78, 0x87, 0x87, //1976 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //1977 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x78, 0x87, //1978 

								0x96, 0xB4, 0x96, 0xA6, 0x96, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x77, //1979 

								0x96, 0xA4, 0xA5, 0xB5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1980 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x77, 0x87, //1981 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1982 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x78, 0x79, 0x78, 0x69, 0x78, 0x77, //1983 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x87, //1984 

								0xA5, 0xB4, 0xA6, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //1985 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1986 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x79, 0x78, 0x69, 0x78, 0x87, //1987 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //1988 

								0xA5, 0xB4, 0xA5, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1989 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //1990 

								0x95, 0xB4, 0x96, 0xA5, 0x86, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1991 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //1992 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1993 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1994 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x76, 0x78, 0x69, 0x78, 0x87, //1995 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //1996 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //1997 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //1998 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //1999 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //2000 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2001 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //2002 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //2003 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //2004 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2005 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2006 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x69, 0x78, 0x87, //2007 

								0x96, 0xB4, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x87, 0x78, 0x87, 0x86, //2008 

								0xA5, 0xB3, 0xA5, 0xB5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2009 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2010 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x78, 0x87, //2011 

								0x96, 0xB4, 0xA5, 0xB5, 0xA5, 0xA6, 0x87, 0x88, 0x87, 0x78, 0x87, 0x86, //2012 

								0xA5, 0xB3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x87, //2013 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2014 

								0x95, 0xB4, 0x96, 0xA5, 0x96, 0x97, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //2015 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x87, 0x88, 0x87, 0x78, 0x87, 0x86, //2016 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x87, //2017 

								0xA5, 0xB4, 0xA6, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2018 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //2019 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x86, //2020 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //2021 

								0xA5, 0xB4, 0xA5, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2022 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x79, 0x77, 0x87, //2023 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x96, //2024 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //2025 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2026 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //2027 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x96, //2028 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //2029 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2030 

								0xA5, 0xB4, 0x96, 0xA5, 0x96, 0x96, 0x88, 0x78, 0x78, 0x78, 0x87, 0x87, //2031 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x96, //2032 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x86, //2033 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x78, 0x88, 0x78, 0x87, 0x87, //2034 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2035 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x96, //2036 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x86, //2037 

								0xA5, 0xB3, 0xA5, 0xA5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2038 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2039 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x96, //2040 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA5, 0xA6, 0x87, 0x88, 0x87, 0x78, 0x87, 0x86, //2041 

								0xA5, 0xB3, 0xA5, 0xB5, 0xA6, 0xA6, 0x88, 0x88, 0x88, 0x78, 0x87, 0x87, //2042 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2043 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x88, 0x87, 0x96, //2044 

								0xA5, 0xC3, 0xA5, 0xB4, 0xA5, 0xA6, 0x87, 0x88, 0x87, 0x78, 0x87, 0x86, //2045 

								0xA5, 0xB3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x88, 0x78, 0x87, 0x87, //2046 

								0xA5, 0xB4, 0x96, 0xA5, 0xA6, 0x96, 0x88, 0x88, 0x78, 0x78, 0x87, 0x87, //2047 

								0x95, 0xB4, 0xA5, 0xB4, 0xA5, 0xA5, 0x97, 0x87, 0x87, 0x88, 0x86, 0x96, //2048 

								0xA4, 0xC3, 0xA5, 0xA5, 0xA5, 0xA6, 0x97, 0x87, 0x87, 0x78, 0x87, 0x86, //2049 

								0xA5, 0xC3, 0xA5, 0xB5, 0xA6, 0xA6, 0x87, 0x88, 0x78, 0x78, 0x87, 0x87}; //2050 





        private DateTime m_Date;

        public DateTime Date
        {

            get { return m_Date; }

            set { m_Date = value; }

        }



        public DME_CNDateTime()
        {

            Date = DateTime.Today;

        }

        public DME_CNDateTime(DateTime dt)
        {

            Date = dt.Date;

        }

        //计算指定日期的星座序号 

        public int GetConstellation()
        {

            int Y, M, D;

            Y = m_Date.Year;

            M = m_Date.Month;

            D = m_Date.Day;

            Y = M * 100 + D;

            if (((Y >= 321) && (Y <= 419))) { return 0; }

            else if ((Y >= 420) && (Y <= 520)) { return 1; }

            else if ((Y >= 521) && (Y <= 620)) { return 2; }

            else if ((Y >= 621) && (Y <= 722)) { return 3; }

            else if ((Y >= 723) && (Y <= 822)) { return 4; }

            else if ((Y >= 823) && (Y <= 922)) { return 5; }

            else if ((Y >= 923) && (Y <= 1022)) { return 6; }

            else if ((Y >= 1023) && (Y <= 1121)) { return 7; }

            else if ((Y >= 1122) && (Y <= 1221)) { return 8; }

            else if ((Y >= 1222) || (Y <= 119)) { return 9; }

            else if ((Y >= 120) && (Y <= 218)) { return 10; }

            else if ((Y >= 219) && (Y <= 320)) { return 11; }

            else { return -1; };

        }



        //计算指定日期的星座名称 

        public string GetConstellationName()
        {

            int Constellation;

            Constellation = GetConstellation();

            if ((Constellation >= 0) && (Constellation <= 11))

            { return ConstellationName[Constellation]; }

            else

            { return ""; };

        }



        //计算公历当天对应的节气 0-23，-1表示不是节气 

        public int l_GetLunarHolDay()
        {

            byte Flag;

            int Day, iYear, iMonth, iDay;

            iYear = m_Date.Year;

            if ((iYear < START_YEAR) || (iYear > END_YEAR))

            { return -1; };

            iMonth = m_Date.Month;

            iDay = m_Date.Day;

            Flag = gLunarHolDay[(iYear - START_YEAR) * 12 + iMonth - 1];

            if (iDay < 15)

            { Day = 15 - ((Flag >> 4) & 0x0f); }

            else

            { Day = (Flag & 0x0f) + 15; };

            if (iDay == Day)
            {

                if (iDay > 15)

                { return (iMonth - 1) * 2 + 1; }

                else

                { return (iMonth - 1) * 2; }

            }

            else

            { return -1; };

        }



        public string FormatMonth(ushort iMonth, bool bLunar)
        {

            string szText = "正二三四五六七八九十";

            string strMonth;

            if ((!bLunar) && (iMonth == 1))

            { return "一月"; }

            if (iMonth <= 10)
            {

                strMonth = "";

                strMonth = strMonth + szText.Substring(iMonth - 1, 1);

                strMonth = strMonth + "月";

                return strMonth;

            }

            if (iMonth == 11)

            { strMonth = "十一"; }

            else

            { strMonth = "十二"; }

            return strMonth + "月";

        }





        public string FormatLunarDay(ushort iDay)
        {

            string szText1 = "初十廿三";

            string szText2 = "一二三四五六七八九十";

            string strDay;

            if ((iDay != 20) && (iDay != 30))
            {

                strDay = szText1.Substring((iDay - 1) / 10, 1);

                strDay = strDay + szText2.Substring((iDay - 1) % 10, 1);

            }

            else
            {

                //				strDay = szText1.Substring((iDay / 10) * 2 + 1, 2); 

                strDay = szText1.Substring((iDay / 10), 1);

                strDay = strDay + "十";

            }

            return strDay;

        }



        public string GetLunarHolDay()
        {

            ushort iYear, iMonth, iDay;

            int i;

            TimeSpan ts;

            iYear = (ushort)(m_Date.Year);

            if ((iYear < START_YEAR) || (iYear > END_YEAR))

            { return ""; };

            i = l_GetLunarHolDay();

            if ((i >= 0) && (i <= 23))

            { return LunarHolDayName[i]; }

            else
            {

                ts = m_Date - (new DateTime(START_YEAR, 1, 1));

                l_CalcLunarDate(out iYear, out iMonth, out iDay, (uint)(ts.Days));

                return FormatMonth(iMonth, true) + FormatLunarDay(iDay);

            }

        }



        //返回阴历iLunarYear年的闰月月份，如没有返回0 1901年1月---2050年12月 

        public int GetLeapMonth(ushort iLunarYear)
        {

            byte Flag;

            if ((iLunarYear < START_YEAR) || (iLunarYear > END_YEAR))

            { return 0; };

            Flag = gLunarMonth[(iLunarYear - START_YEAR) / 2];

            if ((iLunarYear - START_YEAR) % 2 == 0)

            { return Flag >> 4; }

            else

            { return Flag & 0x0F; }

        }


        //返回阴历iLunarYer年阴历iLunarMonth月的天数，如果iLunarMonth为闰月， 

        //高字为第二个iLunarMonth月的天数，否则高字为0 1901年1月---2050年12月 

        public uint LunarMonthDays(ushort iLunarYear, ushort iLunarMonth)
        {

            int Height, Low;

            int iBit;

            if ((iLunarYear < START_YEAR) || (iLunarYear > END_YEAR))

            { return 30; }

            Height = 0;

            Low = 29;

            iBit = 16 - iLunarMonth;

            if ((iLunarMonth > GetLeapMonth(iLunarYear)) && (GetLeapMonth(iLunarYear) > 0))

            { iBit--; }

            if ((gLunarMonthDay[iLunarYear - START_YEAR] & (1 << iBit)) > 0)

            { Low++; }

            if (iLunarMonth == GetLeapMonth(iLunarYear))
            {

                if ((gLunarMonthDay[iLunarYear - START_YEAR] & (1 << (iBit - 1))) > 0)

                { Height = 30; }

                else

                { Height = 29; }

            }

            return (uint)((uint)(Low) | (uint)(Height) << 16); //合成为uint 

        }



        //返回阴历iLunarYear年的总天数 1901年1月---2050年12月 

        public int LunarYearDays(ushort iLunarYear)
        {

            int Days;

            uint tmp;

            if ((iLunarYear < START_YEAR) || (iLunarYear > END_YEAR))

            { return 0; };

            Days = 0;

            for (ushort i = 1; i <= 12; i++)
            {

                tmp = LunarMonthDays(iLunarYear, i);

                Days = Days + ((ushort)(tmp >> 16) & 0xFFFF); //取高位 

                Days = Days + (ushort)(tmp); //取低位 

            }

            return Days;

        }


        //计算从1901年1月1日过iSpanDays天后的阴历日期 
        public void l_CalcLunarDate(out ushort iYear, out ushort iMonth, out ushort iDay, uint iSpanDays)
        {

            uint tmp;

            //阳历1901年2月19日为阴历1901年正月初一 

            //阳历1901年1月1日到2月19日共有49天 

            if (iSpanDays < 49)
            {

                iYear = START_YEAR - 1;

                if (iSpanDays < 19)
                {

                    iMonth = 11;

                    iDay = (ushort)(11 + iSpanDays);

                }

                else
                {

                    iMonth = 12;

                    iDay = (ushort)(iSpanDays - 18);

                }

                return;

            }

            //下面从阴历1901年正月初一算起 

            iSpanDays = iSpanDays - 49;

            iYear = START_YEAR;

            iMonth = 1;

            iDay = 1;

            //计算年 

            tmp = (uint)LunarYearDays(iYear);

            while (iSpanDays >= tmp)
            {

                iSpanDays = iSpanDays - tmp;

                iYear++;

                tmp = (uint)LunarYearDays(iYear);

            }

            //计算月 

            tmp = LunarMonthDays(iYear, iMonth); //取低位 

            while (iSpanDays >= tmp)
            {

                iSpanDays = iSpanDays - tmp;

                if (iMonth == GetLeapMonth(iYear))
                {

                    tmp = (LunarMonthDays(iYear, iMonth) >> 16) & 0xFFFF; //取高位 

                    if (iSpanDays < tmp)

                    { break; }

                    iSpanDays = iSpanDays - tmp;

                }

                iMonth++;

                tmp = LunarMonthDays(iYear, iMonth); //取低位 

            }

            //计算日 

            iDay = (ushort)(iDay + iSpanDays);

        }





        //把iYear年格式化成天干记年法表示的字符串 

        public string FormatLunarYear()
        {

            string strYear;

            string szText1 = "甲乙丙丁戊己庚辛壬癸";

            string szText2 = "子丑寅卯辰巳午未申酉戌亥";

            string szText3 = "鼠牛虎免龙蛇马羊猴鸡狗猪";

            ushort iYear;

            iYear = (ushort)(m_Date.Year);

            strYear = szText1.Substring((iYear - 4) % 10, 1);

            strYear = strYear + szText2.Substring((iYear - 4) % 12, 1);

            strYear = strYear + " ";

            strYear = strYear + szText3.Substring((iYear - 4) % 12, 1);

            strYear = strYear + "年";

            return strYear;

        }
    }
}
