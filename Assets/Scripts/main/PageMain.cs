using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm.UI;
using tpgm;
using Pomelo.DotNetClient;

public class PageMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		//主线程检查并启动
		MainLooper.checkSetup ();

		if (null == SavedData.s_instance) {
			SavedData.s_instance = new SavedData ();
		}

		//设置数据存储目录
		SavedContext.setup ("tpgm");
	
		MusicPlay.login ();

		//远程打印
		NetLog.Instance();

		UIRoot.Instance.InitRoot();

		UIPage.ShowPage<LoadNoticeUIPage> ();
	

	}



}
