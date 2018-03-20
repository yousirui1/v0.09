using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 修改用户信息数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdNewUser
	{
		public ReqThirdNewUser()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";       

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int  m_type;       				//1：注册，2：绑定mac地址

		[ProtoBuf.ProtoMember(5, IsRequired=false)]
		public string m_name = "";

		[ProtoBuf.ProtoMember(6, IsRequired=false)]
		public string m_signature = "";

		[ProtoBuf.ProtoMember(7, IsRequired=false)]
		public int m_head;
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdNewUser
	{
		public RespThirdNewUser()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

	}
}

