using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using tpgm;
using SimpleJson;

/**************************************
*FileName: UIPackageItem.cs
*User: ysr 
*Data: 2018/2/5
*Describe: 法师item逻辑处理和显示
**************************************/

public class UIPackageItem : MonoBehaviour 
{
	public ValStore data = null;




	public void Refresh(ValStore val)
	{
		this.data = val;
		//this.transform.Find("item_bg").GetComponent<Image>().sprite = "累计"+ val.day+"日登录奖励";
		//this.transform.Find("img_type").GetComponent<Image>().sprite = "累计"+ val.day+"日登录奖励";
		this.transform.Find ("tx_itemhead").GetComponent<Text> ().text = val.name;
		this.transform.Find("img_materials").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+0);

		switch (val.buy_type) {
		case  0:  //立即购买
			{
				//string[] rewards = val.reward.Split (';');

				this.transform.Find ("tx_itemcount").GetComponent<Text> ().text = val.reward;

			}break;
		case 1: //延时购买
			{
				this.transform.Find ("tx_itemcount").GetComponent<Text> ().text = "延迟"+val.start+val.validity;
			}
			break;
		case 2: //限时购买
			{
				this.transform.Find ("tx_itemcount").GetComponent<Text> ().text ="限时"+ val.start+val.validity;
			}
			break;
		}

		//string[] rewards = val.reward.Split (':');
		this.transform.Find("tx_itemmoney").GetComponent<Text>().text = "￥"+val.price;


		//this.transform.Find("img").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("Public/Atlases/Icon/General_icon","General_icon_"+0);


		/*
		//背景
		//this.transform.Find("item_bg").GetComponent<Text>().text = friend.name + "lv." + friend.level+ "]"; 
		//头像
		this.transform.Find("head_img").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("Public/Atlases/Icon/General_icon","General_icon_"+friend.head);
		//名字
		this.transform.Find("item_tx").GetComponent<Text>().text = friend.nickname;
		//段位图标
		this.transform.Find("level_img").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("Public/Atlases/ui_atlas_battle","ui_atlas_battle_16");
		//段位显示
		this.transform.Find("level_tx").GetComponent<Text>().text = ""+friend.level;

		//段位显示
		this.transform.Find ("invite_btn").GetComponent<Button>().onClick.AddListener(() =>
			{
				if (ConectData.Instance.pClient != null) {
					JsonObject userMsg= new JsonObject();
					userMsg.Add ("roomNum", ConectData.Instance.roomNum);
					userMsg.Add ("uid", ConectData.Instance.Uid);
					ConectData.Instance.pClient.request("area.gloryHandler.invite", userMsg, (data) =>
						{
							Debug.Log("Entry" + data);

						});
				} else {
					Debug.LogError ("pClient null");
				}
			});

		//玩家状态 1：在线，2：离线，3：组队中，4：匹配中，5：战斗中
		switch (friend.status) {
		case 1:
			//邀请按钮
			this.transform.Find ("invite_btn").gameObject.SetActive (true);
			this.transform.Find ("status_tx").gameObject.SetActive  (false);
			break;
		case 2:
			//邀请按钮
			this.transform.Find ("invite_btn").gameObject.SetActive  (false);
			this.transform.Find ("status_tx").gameObject.SetActive  (true);
			this.transform.Find ("status_tx").GetComponent<Text> ().text = "离线";
			break;
		case 3:
			//邀请按钮
			this.transform.Find ("invite_btn").gameObject.SetActive  (true);
			this.transform.Find ("status_tx").gameObject.SetActive (false);
			break;
		case 4:
			//邀请按钮
			this.transform.Find ("invite_btn").gameObject.SetActive  (false);
			this.transform.Find ("status_tx").gameObject.SetActive  (true);
			this.transform.Find ("status_tx").GetComponent<Text> ().text = "匹配中";
			break;
		case 5:
			//邀请按钮
			this.transform.Find ("invite_btn").gameObject.SetActive  (false);
			this.transform.Find ("status_tx").gameObject.SetActive  (true);
			this.transform.Find ("status_tx").GetComponent<Text> ().text = "战斗中";
			break;
		}*/



	}



} 
