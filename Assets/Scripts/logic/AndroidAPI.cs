using UnityEngine;
using System.Collections;

public class AndroidAPI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NetLog.Instance ();


	}

	private AndroidJavaObject androidObj = null;


	private AndroidJavaObject InitAndroidJavaObj(string path)
	{
	 	AndroidJavaObject m_ANObj = null;
		if (m_ANObj == null)
		{
			try
			{
				m_ANObj = new AndroidJavaObject(path);
			}
			catch
			{
				Debug.Log("Init AndroidNotificator Fail");
	
				return null;
			}
		}

		if (m_ANObj == null)
		{
			Debug.Log("AndroidNotificator Not Found.");
			return null;
		}

		return m_ANObj;
	}
		

	
	// Update is called once per frame
	void Update () {
		// 返回键退出
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();

	}

	void OnGUI ()   
	{  
		// 通过API调用对话框  
		if(GUILayout.Button("调用安卓Jar中的函数 ShowDialog ！",GUILayout.Height(50)))  
		{  
			Debug.Log ("ShowDialog");
			//获取Android的Java接口  
			AndroidJavaClass jc=new AndroidJavaClass("com.unity3d.player.UnityPlayer");  
			AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("currentActivity");  
			//构造参数  
			string[] mObject=new string[2];  
			mObject[0]="Jar4Android";  
			mObject[1]="Wow,Amazing!It's worked!";  
			//调用方法  
			jo.Call("ShowDialog",mObject);  
		}  
		// 通过API调用Toast
		if(GUILayout.Button("调用安卓Jar中的函数 ShowToast !",GUILayout.Height(50)))  
		{ 
			Debug.Log ("Toast");
			AndroidJavaClass jc=new AndroidJavaClass("com.unity3d.player.UnityPlayer");  
			AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("currentActivity"); 
			jo.Call("ShowToast","Showing on Toast"); 
		}
	
		// 通过API调用手机震动的方法
		if(GUILayout.Button("调用安卓Jar中的函数 SetVibrator !",GUILayout.Height(50)))  
		{ 
			
			AndroidJavaClass jc=new AndroidJavaClass("com.unity3d.player.UnityPlayer");  
			AndroidJavaObject jo=jc.GetStatic<AndroidJavaObject>("currentActivity"); 
			jo.Call("SetVibrator"); 
		}

		// 通过API调用手机震动的方法
		if(GUILayout.Button("调用安卓Jar中的函数通知栏",GUILayout.Height(50)))  
		{ 
			AndroidJavaObject jo = InitAndroidJavaObj ("tpgm.com.utilcode.AndroidNotificator");

			if(jo != null)
			{
				//tile, content , 延迟 定时显示  静态函数
				jo.CallStatic("ShowNotification",Application.productName,"unity","test",10,false); 
			}
			else
			{
				Debug.Log ("jo erro");
			}
		}
	} 
}
