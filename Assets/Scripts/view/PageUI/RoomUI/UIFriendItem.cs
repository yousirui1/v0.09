using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using tpgm;
using SimpleJson;

/**************************************
*FileName: UIFriendItem.cs
*User: ysr 
*Data: 2018/1/2
*Describe: 好友Item逻辑处理和显示
**************************************/

public class UIFriendItem : MonoBehaviour 
{
	public JsonFriends data = null;

	//邀请好友
	public void onPomeloEvent_Invite(string uid)
	{
		if (SavedContext.s_client != null) {
			JsonObject jsMsg = new JsonObject ();
			jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
			jsMsg["uid"] = uid;
			SavedContext.s_client.request ("area.gloryHandler.invite",jsMsg, (data) => {
				Debug.Log(data);
			});
		} else {
			Debug.LogError ("pClient null");
		}
	}

	public void Refresh(JsonFriends friend)
	{
		this.data = friend;
		//背景
		//this.transform.Find("item_bg").GetComponent<Text>().text = friend.name + "lv." + friend.level+ "]"; 
		//头像
		this.transform.Find("img_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+friend.head);
		//名字
		this.transform.Find("tx_name").GetComponent<Text>().text = friend.nickname;
		//段位图标
		//this.transform.Find("level_img").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("Public/Atlases/ui_atlas_battle","ui_atlas_battle_16");
		//段位显示
		this.transform.Find("tx_section").GetComponent<Text>().text = ""+friend.section;

		//邀请好友
		this.transform.Find ("status_on/status_1").GetComponent<Button>().onClick.AddListener(() =>
		{
			onPomeloEvent_Invite(friend.uid);
		});

		//玩家状态 1：在线，2：离线，3：组队中，4：匹配中，5：战斗中
		switch (friend.status) {
		case 1:
			//在线
			this.transform.Find ("status_on").gameObject.SetActive  (true);
			this.transform.Find ("status_off").gameObject.SetActive  (false);

			this.transform.Find ("status_on/status_1").gameObject.SetActive  (true);
			this.transform.Find ("status_on/status_3").gameObject.SetActive  (false);
			this.transform.Find ("status_on/status_5").gameObject.SetActive  (false);
			break;
		case 2:
			//离线
			this.transform.Find ("status_on").gameObject.SetActive  (false);
			this.transform.Find ("status_off").gameObject.SetActive  (true);

			break;
		case 3:
			//组队中
			this.transform.Find ("status_on").gameObject.SetActive  (true);
			this.transform.Find ("status_off").gameObject.SetActive  (false);

			this.transform.Find ("status_on/status_1").gameObject.SetActive  (false);
			this.transform.Find ("status_on/status_3").gameObject.SetActive  (true);
			this.transform.Find ("status_on/status_5").gameObject.SetActive  (false);
			break;
		case 4:
			//匹配中
			this.transform.Find ("status_on").gameObject.SetActive  (true);
			this.transform.Find ("status_off").gameObject.SetActive  (false);

			this.transform.Find ("status_on/status_1").gameObject.SetActive  (true);
			this.transform.Find ("status_on/status_3").gameObject.SetActive  (false);
			this.transform.Find ("status_on/status_5").gameObject.SetActive  (false);
			break;
		case 5:
			//战斗中
			this.transform.Find ("status_on").gameObject.SetActive  (true);
			this.transform.Find ("status_off").gameObject.SetActive  (false);

			this.transform.Find ("status_on/status_1").gameObject.SetActive  (false);
			this.transform.Find ("status_on/status_3").gameObject.SetActive  (false);
			this.transform.Find ("status_on/status_5").gameObject.SetActive  (true);
			break;
		}


		
	}



} 
