using System;
using SimpleJson;

/**************************************
*FileName: ApiThirdgetData.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 获取玩家信息数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdGetData
	{
		public ReqThirdGetData()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=false)]
		public int m_user ;

		[ProtoBuf.ProtoMember(5, IsRequired=false)]
		public int m_box ;

		[ProtoBuf.ProtoMember(6, IsRequired=false)]
		public int m_signIn ;


		[ProtoBuf.ProtoMember(7, IsRequired=false)]
		public int m_email ;


		[ProtoBuf.ProtoMember(8, IsRequired=false)]
		public int m_astrology ;

		[ProtoBuf.ProtoMember(9, IsRequired=false)]
		public float m_Lng ;

		[ProtoBuf.ProtoMember(10, IsRequired=false)]
		public float m_Lat;
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdGetData
	{
		public RespThirdGetData()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";

		[ProtoBuf.ProtoMember(3, IsRequired = false)]
		public string m_userData  = "";

		[ProtoBuf.ProtoMember(4, IsRequired = false)]
		public int m_boxData ;

		[ProtoBuf.ProtoMember(5, IsRequired = false)]
		public string m_signInData;

		[ProtoBuf.ProtoMember(6, IsRequired = false)]
		public int m_emailData;

		[ProtoBuf.ProtoMember(7, IsRequired = false)]
		public string m_astrologyData = "";
	}


	[ProtoBuf.ProtoContract]
	public class ReqThirdGetData2
	{
		public ReqThirdGetData2()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_gsid;
	}


	[ProtoBuf.ProtoContract]
	public class RespThirdGetData2
	{
		public RespThirdGetData2()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";

		[ProtoBuf.ProtoMember(3, IsRequired = false)]
		public int achieve;


	}

	public class JsonThirdUserData
	{
		public int head;             //图像
		public string nickname;         //昵称
		public int level;               //等级    
		public int section;             //段位    
		public string mac;              //mac地址
		public int gold;                //金钱
		public int diamond;             //钻石
		public int prestige;            //声望
		public int fragment;            //碎片
		public int fans;				//粉丝
		public int follow;   			//关注
		public int like;   				//被赞数
		public string signature;   		//签名
	}


	public class JsonThirdFriendData
	{
		public int fans;			//粉丝数量
		public int follow;			//关注数量
	}

	public class JsonThirdSignInData
	{
		public string signIn7;						//7天签到信息  ,为分隔符
		public int signInAdd;                  //累计签到次数
		public string signInAddIndex;             //累计签到的奖励领取
		public string createDate;
	}
}
