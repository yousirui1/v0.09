using UnityEngine;
using System.Collections;

//#程序运行期间需要保留下来的玩家数据，比如：玩家所拥有的物品，某个倒计时情况；
using System.Collections.Generic;


namespace tpgm
{
    public class SavedData
    {

		public static SavedData s_instance;

		public int m_mode = 1;

		//#服务端当前的时间;
		public long m_serverNow_UtcMs;

		//#以数量衡量的物品;
		public Dictionary<int, int> m_gsidNum = new Dictionary<int, int>();
		List<int> m_sortedGsidList = new List<int>();

		//#玩家的金币;
		public int m_gold;

		//#玩家的珍珠;
		public int m_pearl;

		//#玩家的金币，珍珠，经验，等级等数据
		//public UserData m_userdata=null;

		//#某个新手引导是否执行过的标识;
		public Dictionary<int, int> m_guideFlags = new Dictionary<int, int>();

		public User m_user = new User();

		//#是否要检测宝箱时间等;
		public Msg_Box m_box = new Msg_Box();

		//#是否要检测签到等;
		public Msg_SignIn m_signIn = new Msg_SignIn();


		//#是否要检测邮箱等;
		public Msg_Email m_email = new Msg_Email();

		//#是否要检测占星等;
		public Msg_Astrology m_astrology = new Msg_Astrology();

	
	
		//保存当前这一局游戏的玩家列表
		public string m_roomNum ;


		//public List<UserGunInfo> m_gunData;

		//上次显示升级奖励的等级;
		public int m_lastShowRewardLevel;

		public  string s_clientUrl = "121.40.149.87";
		//public  string s_clientUrl = "192.168.52.1";
		public  int s_clientPort = 7014;
	
		public  int m_playerMax = 25;

		//技能id
		public int m_skillID = 0;  

		//技能使用次数
		public int m_skillCount = 0;

		//技能等级 0 - 2
		public int m_skillLevel = 0;

		public string m_skillUid = "";

		//玩家数据字典
		public  Dictionary<string, RespThirdUserData> m_userCache = new Dictionary<string, RespThirdUserData>();

		//保存当前这一局游戏的玩家列表uid
		public List<string> m_userlist ;

		//保存当前这一局游戏的玩家列表排行榜用于显示
		public List<UserRank> m_userrank = new List<UserRank>();

        //#该gsid我所拥有的数量;
        public int getNum(int gsid)
        {
            int myNum = 0;
            if (!m_gsidNum.TryGetValue(gsid, out myNum))
            {
                myNum = 0;
            }

            return myNum;
        }

        public int somethingEnough(int gsid, int needNum)
        {
            int myNum = 0;
            if (!m_gsidNum.TryGetValue(gsid, out myNum))
            {
                myNum = 0;
            }

            return myNum - needNum;
        }
		#if false
        public void gainSomething(List<Reward> list)
        {
            //if (null != list)
            {
                for (int i = 0, size = list.Count; i < size; i++)
                {
                    Reward item = list[i];
                    gainSomething(item.gsid, item.num);
                }
            }
        }

        public void gainSomething(List<GsidNumPair> list)
        {
            //if (null != things) //#从数值表解析的, 可能为null的; 算是防御性编程吧;
            {
                for (int i = 0, size = list.Count; i < size; i++)
                {
                    GsidNumPair item = list[i];
                    gainSomething(item.m_gsid, item.m_num);
                }
            }
        }

        public void gainSomething(Dictionary<int, int> things)
        {
            foreach (var item in things)
            {
                gainSomething(item.Key, item.Value);
            }
        }

        public void gainSomething(int gsid, int num)
        {
            if (num > 0)
            {
                if (gsid == (int)ConstsGsid.Gold_1)
                {
                    Log.d<SavedData>("add gold: " + num);

                    m_gold += num;
                }
                else
                {
                    Log.d<SavedData>("add: " + gsid + ", " + num);
                    if (m_gsidNum.ContainsKey(gsid))
                    {
                        m_gsidNum[gsid] += num;
                    }
                    else
                    {
                        m_gsidNum[gsid] = num;
                    }
                }
            }
        }

        public void costSomething(int gsid, int num)
        {
            if (num > 0)
            {
                if (gsid == (int)ConstsGsid.Gold_1)
                {
                    m_gold -= num;
                }
                else
                {
                    if (m_gsidNum.ContainsKey(gsid))
                    {
                        m_gsidNum[gsid] -= num;
                    }
                    else
                    {
                        //warn: 没有该物品;
                    }
                }
            }
        }

        public void setItems(Reward[] items)
        {
            m_gsidNum.Clear();

            foreach (Reward item in items)
            {
                m_gsidNum.Add(item.gsid, item.num);
            }
        }

		public void setUserData(UserData data)
		{
			if (m_userdata != null) 
			{
				m_userdata = null;
			}
			m_userdata = data;
		}
		#endif
        class GsidSorter : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x - y;
            }
        }

        //#获取按gsid排序的物品列表;
        public void getSortedMyGsidList(List<int> outList)
        {
            outList.Clear();

            foreach (var entry in m_gsidNum)
            {
                outList.Add(entry.Key);
            }

            outList.Sort(new GsidSorter());
        }


      

    }


    //**************************************************
    // User
    //**************************************************

    //玩家信息;
    public class User
    {
        //#用户id;
        public string m_uid = "";

        //#授权token;
        public string m_token = "";

		public int m_head;    //头像

		public string m_nickname  = "";  //昵称

		public int m_level;    			//等级

		public int m_fans ;				//粉丝数
						
		public int m_follow;			//关注数

		public int m_like;				//被赞数

		public string m_signature =  "";  //签名
	

    }

    //**************************************************
    // Msg
    //**************************************************




	//**************************************************
	// Msg 宝箱时间
	//**************************************************

	public class Msg_Box
	{
		public bool checkNeedReload()
		{
			if (m_needReload)
			{
			}
			else
			{
				long diffMs = TimeUtils.utcNowMs() - m_lastMsgCheckUtcMs;
				if (diffMs >= 3 * 60 * 1000)
				{
					m_needReload = true;
				}
			}

			//return m_needReload;
			return true;
		}

		public void reloadOk()
		{
			long now = TimeUtils.utcNowMs();
			m_lastMsgCheckUtcMs = now;
			m_needReload = false;
		}

		//#上次检查宝箱的时间;
		long m_lastMsgCheckUtcMs;

		//#是否有可以领取;
		public bool m_hasNewMsg;

		bool m_needReload;
	}



	//**************************************************
	// Msg 签到
	//**************************************************

	public class Msg_SignIn
	{
		//检查是否需要请求服务器
		public bool checkNeedReload()
		{
			if (m_needReload)
			{
			}
			else
			{
				long diffMs = TimeUtils.utcNowMs() - m_lastMsgCheckUtcMs;
				if (diffMs >= 3 * 60 * 1000)  
				{
					m_needReload = true;
				}
			}
			//return m_needReload;
			return true;
		}

		public void reloadOk()
		{
			long now = TimeUtils.utcNowMs();
			m_lastMsgCheckUtcMs = now;
			m_needReload = false;
		}

		//#上次检查签到的时间;
		long m_lastMsgCheckUtcMs;

		//#是否有新邮件;
		public bool m_hasNewMsg;

		bool m_needReload;
	}



	//**************************************************
	// Msg Email
	//**************************************************

	public class Msg_Email
	{
		public bool checkNeedReload()
		{
			if (m_needReload)
			{
			}
			else
			{
				long diffMs = TimeUtils.utcNowMs() - m_lastMsgCheckUtcMs;
				if (diffMs >= 3 * 60 * 1000)
				{
					m_needReload = true;
				}
			}

			//return m_needReload;
			return true;
		}

		public void reloadOk()
		{
			long now = TimeUtils.utcNowMs();
			m_lastMsgCheckUtcMs = now;
			m_needReload = false;
		}

		//#上次检查邮件的时间;
		long m_lastMsgCheckUtcMs;

		//#是否有新邮件;
		public bool m_hasNewMsg;

		bool m_needReload;
	}


	//**************************************************
	// Msg Astrology 占星
	//**************************************************

	public class Msg_Astrology
	{
		public bool checkNeedReload()
		{
			if (m_needReload)
			{
			}
			else
			{
				long diffMs = TimeUtils.utcNowMs() - m_lastMsgCheckUtcMs;
				if (diffMs >= 3 * 60 * 1000)     
				{
					m_needReload = true;
				}
			}

			//return m_needReload;
			return true;
		}

		public void reloadOk()
		{
			long now = TimeUtils.utcNowMs();
			m_lastMsgCheckUtcMs = now;
			m_needReload = false;
		}

		//#上次检查邮件的时间;
		long m_lastMsgCheckUtcMs;

		//#是否有新邮件;
		public bool m_hasNewMsg;

		bool m_needReload;
	}

    //**************************************************
    // Guide
    //**************************************************

    public class Guide
    {
        public bool checkGuide(string name)
        {
            int flag;
            if (m_flags.TryGetValue(name, out flag))
            {
                if (0 == flag)
                {
                    //#引导的触发条件是否满足;
                }
            }

            return false;
        }

        //#已经通过的引导;
        public Dictionary<string, int> m_flags = new Dictionary<string, int>();
    }
}

