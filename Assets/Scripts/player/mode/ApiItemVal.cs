using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 宝箱奖励领取数据定义类
**************************************/

namespace tpgm
{
	public class ItemJs
	{   
		public int id; 
		public int X;
		public int Y;
	} 


	//资源物品
	public class ItemVal
	{


		public int score = 0;
		public int rehp = 0;
		public int exp = 0;

		public ItemVal(int score, int rehp, int exp)
		{
			this.score = score;
			this.rehp = rehp;
			this.exp = exp;
		}

		//1 min 2 mid 3 max 水晶
		/*public ItemVal(int type)
        {
            switch (type)
            {
                case 1:
                    {
                        score = 1;
                        rehp = 1;
                        exp = 1;
                    }
                    break;
                case 2:
                    {
                        score = 10;
                        rehp = 10;
                        exp = 10;
                    }
                    break;
                case 3:
                    {
                        score = 50;
                        rehp = 50;
                        exp = 50;
                    }
                    break;
                default:
                    break;
            }
        }*/
	}
}

