using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using tpgm;
using SimpleJson;

/**************************************
*FileName: UIEmailItem.cs
*User: ysr 
*Data: 2018/1/2
*Describe: 活动邮件item逻辑处理和显示
**************************************/

public class UIRankItem : MonoBehaviour 
{
	public UserRank data  = null;

	public void Refresh(UserRank data)
	{
		this.data = data;
		this.transform.Find ("tx_name").GetComponent<Text> ().text = data.m_uid;
		this.transform.Find ("tx_score").GetComponent<Text> ().text = "" +data.m_score;

	}

} 
