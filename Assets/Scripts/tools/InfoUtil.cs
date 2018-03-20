using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System.Net.NetworkInformation;
using tpgm;
using System.IO;  

public class InfoUtil
{
   


    //GPS服务
    private static LocationService locationServer;
    private static LocationServiceStatus locationServerStatus;
    private static LocationInfo locationInfo;

   

	public static string  GetMac()
    {
		string Mac = "";
        bool isFind = false;
        NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface adaper in nis)
        {
			//ios
			if (adaper.Description == "en0") {
				Mac = adaper.GetPhysicalAddress ().ToString ();
			} else {
				//pc
				Mac = adaper.GetPhysicalAddress().ToString();
				if (Mac != "") {
					break;
				}
			}
        }

		if (Mac == "") {
			var fileAddress = System.IO.Path.Combine("/sys/class/net/wlan0/", "address");  
			FileInfo fInfo0 = new FileInfo(fileAddress);  
			string s = "";  
			if (fInfo0.Exists)  
			{  
				StreamReader r = new StreamReader(fileAddress);  
				//StreamReader默认的是UTF8的不需要转格式了，因为有些中文字符的需要有些是要转的，下面是转成String代码  
				//byte[] data = new byte[1024];  
				// data = Encoding.UTF8.GetBytes(r.ReadToEnd());  
				// s = Encoding.UTF8.GetString(data, 0, data.Length);  
				s = r.ReadToEnd();  
				Mac = s;
				Mac.Replace("\n","");
				Mac.Replace(":","");
				Debug.Log(s);  
			}  
		}
        return Mac;
    }

    public static void GetGPS(GPSVal gps)
    {
        //GPS Sever
        locationServer = Input.location;
        gps.isCould = locationServer.isEnabledByUser; //用户是否可以设置定位服务        
        locationServerStatus = locationServer.status; //返回设备服务状态  
        //参数1 服务所需的精度，以米为单位，参数2 最小更新距离  
        locationServer.Start(1, 1);//开始位置更新服务，最后的位置坐标  
                                   //locationServer.Stop();//停止位置服务更新，节省电池寿命 
                                   //调用该方法之前确保调用了 Input.location.Start()

        //GPS Info
        locationInfo = locationServer.lastData; //设备最后检测的位置  
        gps.altitude = locationInfo.altitude;//设备高度  
        gps.horizontalAccuracy = locationInfo.horizontalAccuracy; //水平精确度  
        gps.verticalAccuracy = locationInfo.verticalAccuracy; //垂直精确度  
        gps.latitude = locationInfo.latitude; //设备纬度  
        gps.longitude = locationInfo.longitude;//设备纬度  
        gps.timestamp = locationInfo.timestamp;//时间戳(自1970年以来以秒为单位)位置时最后一次更新。
    }

	//获取电池电量
	public static float GetBettery()
	{
		float bettery = SystemInfo.batteryLevel;
		return bettery;
	}

	//获取信号强度
	public static void GetSignal()
	{

	}

}
