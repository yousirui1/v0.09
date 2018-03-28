using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm;

namespace tpgm
{


	public class GPSVal
	{
		public bool isCould;  //是否开启定位服务，即开启GPS定位  
		public float altitude; //海拔高度  
		public float horizontalAccuracy; //水平精度  
		public float verticalAccuracy;  //垂直精度  
		public float latitude;       //纬度  
		public float longitude;      //经度  
		public double timestamp;     //最近一次定位的时间戳，从 1970年开始  
	}



	public class TabIndex
	{
		public int id;
		public string tabname;
		public string panelPath;

		public TabIndex(int id, string tabname, string panelPath)
		{
			this.id = id;
			this.tabname = tabname;
			this.panelPath = panelPath;
		}
	}

	public class UDFriend 
	{
		public class Friend
		{
			public string nickname;
			public string uid;
			public int gender; 	//性别
			public int head;     //头像
			public int level;		//段位
			public int status;		//状态 1：在线，2：离线，3：组队中，4：匹配中，5：战斗中


		}
		public class FriendBuf
		{
			public int code;
			public string roomNum;
			public Friend[] friendArr;
			public string utcMs;
		}

		public List<Friend> friends;
	}


	public class UDStore 
	{
		public class Materials
		{
			public string nickname;
			public string uid;
			public int gender; 	//性别
			public int head;     //头像
			public int level;		//段位
			public int status;		//状态 1：在线，2：离线，3：组队中，4：匹配中，5：战斗中


		}

	}



	public class UDActivity
	{
		public class Activity
		{
			public string image;
			public string item_tx;
			public string url;

		}
		/*public class FriendBuf
		{
			public int code;
			public string roomNum;
			public Friend[] friendArr;
		}*/

		public List<Activity> activitys;
	}


	public class UDGroup
	{
		public class Group
		{
			public string uid;            //
			public string isReady;		  //
			public string nickname;		  //

			public string group;		  //
			public int head;
			public int level;			
			public int mmr;   //隐藏分

		}
		public class PreparePlayerBuf
		{
			public Group[] message;
			public string type;
		}



		public class StartPlayerBuf
		{
			public Group[] newUser;
		}

		public List<Group> groups;


	}

}
