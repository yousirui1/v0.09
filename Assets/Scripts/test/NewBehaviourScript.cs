using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm.UI;

public class NewBehaviourScript : MonoBehaviour {
	GameObject obj = null;
	// Use this for initialization
	void Start () {
		obj = ResourceMgr.Instance ().CreateGameObject ("GameObject", false);


		UIPage.ShowPage<PublicUINotice> ();
		Invoke ("agent", 3.0f);

	}

	void agent()
	{
		//Destroy (obj);
		UIRoot.Instance.gameObject.SetActive(false);
		//Invoke ("End", 3.0f);
	}

	void End()
	{
		//Destroy (obj);
		UIRoot.Instance.gameObject.SetActive(true);
		//UIPage.ShowPage<PublicUINotice> ();
		//Invoke ("End", 3.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
