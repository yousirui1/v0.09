using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour
{
	private static Create instance = null;

	public static Create Instance
	{

		get {

			return instance;
		}
	}


	void Awake()
	{
		instance = this;
	}


	#if false
	private static Create m_Instance = null;
	public static Create Instance
	{
		get
		{
			if(null == m_Instance)
			{
				InitRoot();
			}

			return m_Instance;
		}
	}


	// Use this for initialization
	static void InitRoot(){
		GameObject obj = new GameObject("GameRoot").AddComponent<Test> ().gameObject;
	}
		
	void OnDestroy()
	{
		m_Instance = null;
	}
	#endif
}
