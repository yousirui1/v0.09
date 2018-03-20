using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm;

/**************************************
*FileName: GameRoot.cs
*User: ysr 
*Data: 2018/2/28
*Describe: 游戏部分的主函数
**************************************/

public class GameMain : MonoBehaviour {

	private EventController eventController;

	private const string TAG = "GameMain";

	private AreaConect areaConect;

	//private LoginConect loginConect;
	private WlanClient wlanClient;

	private WlanServer wlanServer;

	void Start () {

		//初始化Canvas
		if (GameRoot.Instance == null)
			return;

		//主线程检查并启动
		MainLooper.checkSetup ();
	
		switch(SavedData.s_instance.m_mode)
		{
			case 1:
			{
				//网络长连接
				areaConect = new GameObject ("NetController").AddComponent<AreaConect> ();
			}
			break;

			case 2:
			{
				//
				wlanServer = new GameObject ("NetController").AddComponent<WlanServer> ();
			}
			break;

			case 3:
			{
				//
				wlanClient = new GameObject ("NetController").AddComponent<WlanClient> ();
			}
			break;
		}
			

		//游戏主要逻辑管理
		if(SavedData.s_instance.m_mode != 2)
		eventController = new GameObject ("EventController").AddComponent<EventController> ();
	
	}
		




}
