using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 宝箱奖励领取数据定义类
**************************************/

namespace tpgm
{
	public class SkillJs
	{   
		public int id; 
		public int X;
		public int Y;
	} 


	//资源物品
	public class SkillVal
	{
		public int score = 0;
		public int rehp = 0;
		public int exp = 0;

		public SkillVal(int score, int rehp, int exp)
		{
			this.score = score;
			this.rehp = rehp;
			this.exp = exp;
		}
	
	}
}

