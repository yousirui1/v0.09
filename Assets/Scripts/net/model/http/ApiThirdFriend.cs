using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 签到领取奖励取数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdFriend
	{
		public ReqThirdFriend()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_type ;   //1关注，2好友，3粉丝，4附近的人

		[ProtoBuf.ProtoMember(5, IsRequired=true)]
		public int m_page ;   //第几页
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdFriend
	{
		public RespThirdFriend()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(3, IsRequired = true)]   
		public string m_friends = "";  						//奖励 JsonObject

	}

	public class JsonFriends
	{

		public string uid = "";
		public int head;
		public string nickname = "";
		public int section;
		public int status;		//		状态 1：在线，2：离线，3：组队，4：匹配，5：战斗中
		public float winPre;
	}


}

