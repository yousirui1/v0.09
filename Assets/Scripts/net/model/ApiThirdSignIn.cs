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
	public class ReqThirdSignin7
	{
		public ReqThirdSignin7()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_type ;   //1:签到，2：补签
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdSignin7
	{
		public RespThirdSignin7()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(3, IsRequired = true)]   
		public string materials = "";  						//奖励 JsonObject

	}


	[ProtoBuf.ProtoContract]
	public class ReqThirdSigninAdd
	{
		public ReqThirdSigninAdd()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_day ;   //天数 查json表
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdSigninAdd
	{
		public RespThirdSigninAdd()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(3, IsRequired = true)]   
		public string materials = "";  						//奖励 JsonObject

	}


	public class JsonMaterials
	{
		public int gsid;
		public int num;
	}
}

