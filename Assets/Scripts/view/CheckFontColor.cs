using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CheckFontColor : MonoBehaviour {

	string text = "";
	// Use this for initialization
	void Start () {
		text = this.GetComponent<Text> ().text;
	}
	
	// Update is called once per frame
	void Update () {
		if(this.transform.parent.gameObject.GetComponent<Button> ().interactable)
		{

			this.GetComponent<Text> ().text = "<color=#DCC3A5FF>" + this.text + "</color> ";
		}
		else
		{
			this.GetComponent<Text> ().text =  "<color=#733B2EFF>" + this.text + "</color> "; 
		}
	}
}
