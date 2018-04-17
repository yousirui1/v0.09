using System;

/**************************************
*FileName: ApiThirdMage.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 登录数据定义类
**************************************/


namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdMage
	{
		public ReqThirdMage()
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
	public class RespThirdMage
	{
		public RespThirdMage()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";


		[ProtoBuf.ProtoMember(3, IsRequired = false)]
		public int m_skin;

		[ProtoBuf.ProtoMember(4, IsRequired = false)]
		public int m_pet;

		[ProtoBuf.ProtoMember(5, IsRequired = false)]
		public int m_attackSkin;

		[ProtoBuf.ProtoMember(6, IsRequired = false)]
		public int m_footprint;

		[ProtoBuf.ProtoMember(7, IsRequired = false)]
		public int m_signBox;

		[ProtoBuf.ProtoMember(8, IsRequired = false)]
		public int m_magicBook;


		[ProtoBuf.ProtoMember(9, IsRequired = false)]
		public string m_genius = "";


		[ProtoBuf.ProtoMember(10, IsRequired = false)]
		public string m_items = "";

	

	}


}
