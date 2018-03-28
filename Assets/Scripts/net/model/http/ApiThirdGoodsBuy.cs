using System;

/**************************************
*FileName: ApiThirdGoodsBuy.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 商城购买数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdGoodsBuy
	{
		public ReqThirdGoodsBuy()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_goodsID ;   
	}

	[ProtoBuf.ProtoContract]
	public class RespThirdGoodsBuy
	{
		public RespThirdGoodsBuy()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  


	}



}

