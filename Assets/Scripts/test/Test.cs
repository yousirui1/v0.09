using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomelo.DotNetClient;
using System;
using SimpleJson;
using tpgm;

public class Test : MonoBehaviour {
	int i =0;

	void Start()
	{
		i++;
	}
	//private PomeloClient pClient;
	#if false
	private static Test instance = null;

	//	private List<string> UserNameList = new List<string>();


	public static Test Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}


	void Start()
	{
		
	}

	void OnDestroy()
	{
		Debug.Log (".................");
	}
	#endif

}