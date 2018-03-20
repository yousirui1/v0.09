using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 登录数据定义类
**************************************/


namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdLogin
	{
		public ReqThirdLogin()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID ;

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public int m_type; 

		[ProtoBuf.ProtoMember(4, IsRequired=false)]
		public string m_mac = "";

		[ProtoBuf.ProtoMember(5, IsRequired=false)]
		public string m_account = "";

		[ProtoBuf.ProtoMember(6, IsRequired=false)]
		public string m_password = "";
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdLogin
	{
		public RespThirdLogin()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = false)]
		public string m_uid = "";

		[ProtoBuf.ProtoMember(3, IsRequired = false)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired = false)]
		public int m_isFirst;

		[ProtoBuf.ProtoMember(5, IsRequired = true)]
		public string m_utcMs = "";




	}


}
