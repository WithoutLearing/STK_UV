using System;
using System.Collections.Generic;

namespace BCM检测工装
{
    public class PublicData
    {

        public static string IniPath = Environment.CurrentDirectory + @"\MyINI.ini";
        /// <summary>
        /// 定义数据库连接字符串
        /// </summary>
        public static string ConnString = "data source=" + System.AppDomain.CurrentDomain.BaseDirectory + @"myDB.db";
        /// <summary>
        /// 用户登陆标志位
        /// </summary>
        public static bool LoginOnflag = false;
        /// <summary>
        /// 产品选型标志位
        /// </summary>
        public static bool Selectionflag = false;
        /// <summary>
        /// 保存当前用户名
        /// </summary>
        public static string LoginUsername;
        /// <summary>
        /// 当前选择产品型号
        /// </summary>
        public static string ProductModelName;
        /// <summary>
        /// 当前选择二维码信息
        /// </summary>
        public static string ProductCode;

        public static string Productdatatime1;
        public static string Productdatatime2;
        public static string Productdatatime3;
        public static string Productdatatime4;
        public static string Productdatatime5;
        public static string Productdatatime6;
        public static string Productdatatime7;
        public static string Productdatatime8;
        public static string Productdatatime9;
        public static string ProductID;
        public static int ProductSeqNumber;
        public static int ProductState;
        public static int ProductTimes;
        public static int ProductBackTemperatureTimes;
        public static int ProductBackTemperatureTimesCnt;
        public static int ProductNormalTemperatureTimes;
        public static int Cnt1;
        public static int Cnt2;
        public static bool Writeflag = false;//产品信息录入标志

        public static bool BackTemperatureFlag1;//回温计时中标志位
        public static bool BackTemperatureFlag2;//回温计时中标志位
        public static bool NormalTemperatureFlag1;//常温计时中标志位
        public static bool NormalTemperatureFlag2;//常温计时中标志位
        public static bool BackTemperatureCompleteFlag1;//回温完成标志位
        public static bool BackTemperatureCompleteFlag2;//回温完成标志位
        public static bool ReturnContainerFlag1;//回箱完成标志位
        public static bool ReturnContainerFlag2;//回箱完成标志位
        public static bool OverFlag;//用毕完成标志位
        public static int BackTemperatureTimeSet;//回温时间设置值(s)
        public static int NormalTemperatureLimitTimeSet;//常温极限时间设置值(s)
        public static int WorkLimitTimesSet;//工作极限次数设置值

        public static int BackTemperatureSendCnt1;
        public static int BackTemperatureSendCnt2;

        public static string ProductModel;//产品编码

        public struct ProductInformationStruct
        {
            public static string Model;
            public static string Batch;
            public static int Numbers;
            public static string Informtion;
        }

        public static List<int> GlueSeqNumberList = new List<int> { };//胶水序号
        public static List<string> GlueIDList = new List<string> { };//胶水ID
        public static List<int> BackTemperatureTimingList = new List<int> { };//回温计时
        public static List<int> NormalTemperatureTimingList = new List<int> { };//常温计时
        public static List<int> BackTemperatureNumberList = new List<int> { };//回温次数
        public static List<int> GlueStateList = new List<int> { };//胶水状态


  

    }
}