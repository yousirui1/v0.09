using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour {
	private Button btn_tab;
	private Text tx_tab;

	// Use this for initialization
	void Start () {
		btn_tab = this.gameObject.GetComponent<Button> ();
		tx_tab = this.gameObject.transform.Find ("Text").GetComponent<Text> ();
	}


	
	// Update is called once per frame
	void Update () {
		if (btn_tab.interactable) {
			tx_tab.text = "<color=#DCC3A5FF>"+tx_tab.text+"</color>\n";
		} else {
			tx_tab.text = "<color=#733B2EFF>"+tx_tab.text+"</color>\n";
		}
	}
}
