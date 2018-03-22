using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm.UI;
using tpgm;
using Pomelo.DotNetClient;

public class HomeMain : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//主线程检查并启动
		MainLooper.checkSetup ();

		if (null == SavedData.s_instance) {
			SavedData.s_instance = new SavedData ();
		}

		//设置数据存储目录
		SavedContext.setup ("tpgm");


		//远程打印
		NetLog.Instance();


		//PrefValUpdate.clear ();

		//UIPage.ShowPage<MainUIPage> ();
		if(SavedData.s_instance.m_home == 1)
		{
			UIPage.ShowPage<MainUIPage> ();
		}
		else
		{
			UIPage.ShowPage<RoomUIPrepare> ();
		}



	}


}
