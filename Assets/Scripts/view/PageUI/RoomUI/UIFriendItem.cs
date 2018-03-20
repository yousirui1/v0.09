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
	public UDFriend.Friend data = null;

	public void Refresh(UDFriend.Friend friend)
	{
		this.data = friend;
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
				#if false
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
				#endif
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
		}


		
	}

} 
