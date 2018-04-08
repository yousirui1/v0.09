using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tpgm;

public class ActivityTestData
{
	private static ActivityTestData m_instance;
	public static ActivityTestData Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new ActivityTestData();
			}
			return m_instance;
		}

	}


	public UDActivity playerActivity;
	private ActivityTestData()
	{
		playerActivity = new UDActivity();
		playerActivity.activitys = new List<UDActivity.Activity>();
		string[] name = new string[4] {"百度", "bilibili", "优酷","360"};
		string[] urls = new string[4] {"https://www.baidu.cn/",  "https://www.bilibili.com","https://www.youku.com/","https://hao.360.cn/"};
		playerActivity.activitys = new List<UDActivity.Activity>();
		for(int i =0 ; i< 4; i++)
		{
			UDActivity.Activity activity = new UDActivity.Activity();
			activity.item_tx = name[i];
			activity.url = urls [i];
			playerActivity.activitys.Add(activity);	
		}
	}


}
