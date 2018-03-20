using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using tpgm.UI;
using tpgm;

public  class UISkillPage : UIPage {
	GameObject showObj1;
	GameObject showObj2;

	int i =0;

	public UISkillPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		uiPath = "Prefab/UI/UISkillPage";
	}

	public override void Awake(GameObject go)
	{
		showObj1 = this.transform.Find("tx1").gameObject;


		showObj2 = this.transform.Find("tx2").gameObject;
		showObj2.GetComponent<Button>().onClick.AddListener(OnClickUpgrade);

		showObj1.GetComponent<Text>().text = "1";
		showObj2.transform.Find ("Text").GetComponent<Text> ().text = "2";
		//UIRoot.Instance.StartCoroutine(Timer());
	}

	//定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
			Refresh ();
		}
	}

	public override void Refresh()
	{
		//showObj1.GetComponent<Text>().text = "1......"+ConectData.Instance.NewTime;
	}

	public void RefreshDesc()
	{
		//showObj1.GetComponent<Text>().text = "1......"+ConectData.Instance.NewTime;
	}


	public void OnClickUpgrade()
	{
		//showObj2.transform.Find ("Text").GetComponent<Text> ().text = "2......"+ConectData.Instance.NewTime;

	}

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}
	
}
