using System;
using UnityEngine;
using System.IO;
using ProtoBuf;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

/**************************************
*FileName: TimeUtils.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 时间打印和转换类
**************************************/


namespace tpgm
{
	public class TimeUtils 
	 {
		 
		//打印时间
		public static void printLogUseTime()
		{
			long t1 = TimeUtils.utcNowMs();
			Debug.Log("log use time");
			long t2 = TimeUtils.utcNowMs();
		}
	
		//打印时间
		public static void printDateTime(DateTime dt)
        {
            //new DateTime(Utils.utcMsToUtcTicks(resp.utcMsLong), DateTimeKind.Utc); //ToString的话, 直接打印utc的;
            //new DateTime(Utils.utcMsToUtcTicks(resp.utcMsLong), DateTimeKind.Local); //ToString的话, 直接打印local的;

            //Log.d<Utils>("dt kind: " + dt.ToString("yyyy-MM-dd HH:mm:ss"));
            //Log.d<Utils>("dt utc: " + dt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            //Log.d<Utils>("dt local: " + dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
        }


		 //#距离1970-01-01的毫秒数;
        public static long utcNowMs()
        {
            //CurrentTimeMillis: 636138779700945450, 636138491700945450
            //Debug.Log("CurrentTimeMillis: " + System.DateTime.Now.Ticks + ", " + System.DateTime.UtcNow.Ticks);

            long millis = (long) ((System.DateTime.UtcNow.Ticks - 621355968000000000) * 0.0001);
            return millis;
        }

        //#毫秒转纳秒;
        public static long msToTicks(long ms)
        {
            return ms * 10000;
        }

        //#utc时间转tick时间;
        public static long utcMsToUtcTicks(long utcMs)
        {
            long utcTicks = utcMs * 10000 + 621355968000000000;

//            DateTime dt = new DateTime(utcTicks, DateTimeKind.Utc);
//            Debug.Log("dt: " + dt.ToUniversalTime().ToString("yyyy-MM-dd HH-mm-ss"));
//
//            var epoch = new DateTime(1970, 1, 1, 13, 0, 0, DateTimeKind.Utc);
//            Debug.Log("aa: " + epoch.Ticks + ", " + epoch.ToString("yyyy-MM-dd HH-mm-ss"));
//            Debug.Log("aa: " + epoch.ToUniversalTime().Ticks + ", " + epoch.ToUniversalTime().ToString("yyyy-MM-dd HH-mm-ss"));
//
//            var epoch2 = new DateTime(1970, 1, 1, 0, 0, 0); //认为是东8区的时间;
//            Debug.Log("aa: " + epoch2.Ticks + ", " + epoch2.ToString("yyyy-MM-dd HH-mm-ss"));
//            Debug.Log("aa: " + epoch2.ToUniversalTime        ().Ticks + ", " + epoch2.ToUniversalTime().ToString("yyyy-MM-dd HH-mm-ss"));

            return utcTicks;
        }

//        public DateTime FromUnixTime(long unixTime)
//        {
//            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//            return epoch.AddMilliseconds(unixTime);
//        }
//
//        public long ToUnixTime(DateTime date)
//        {
//            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalMilliseconds);
//        }
	}
}
