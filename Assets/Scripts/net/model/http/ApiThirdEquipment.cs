using System;

/**************************************
*FileName: ApiThirdEquipment.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 魔法师界面装备
**************************************/


namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdEquipment
	{
		public ReqThirdEquipment()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID  = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = ""; 

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_gsid ;


	}

	[ProtoBuf.ProtoContract]
	public class RespThirdEquipment
	{
		public RespThirdEquipment()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";
	
	}


}
