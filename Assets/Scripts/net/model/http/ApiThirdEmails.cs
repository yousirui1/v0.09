using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 宝箱奖励领取数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdEmails
	{
		public ReqThirdEmails()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdEmails
	{
		public RespThirdEmails()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(3, IsRequired = true)]   
		public string m_emailList = "";  						//Json arr 邮件列表
	
	}

	//Json arr 邮件列表
	public class JsonThirdEmail
	{
		public string id ;
		public int status;
		public string rewards;
		public string title;   
		public string content;
		public string createDate;
		public string loseTime;
	}
}

