using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**************************************
*FileName: ATKAndDamage.cs
*User: ysr 
*Data: 2017/12/12
*Describe: 收到伤害的公共接口
**************************************/

public class HpBar : MonoBehaviour
{
    private Image sliderHpBar;
    private Image sliderSpBar;

    private Image sliderExp;


    private Text txLevel;

	private Text txName;

    private PlayerATKAndDamage playerATK;//主角的攻击和伤害脚本s

    void Awake()
    {
        //获取血条


		sliderHpBar = this.transform.Find ("img_hp").GetComponent<Image> ();
		sliderSpBar = this.transform.Find ("img_sp").GetComponent<Image> ();
		sliderExp = this.transform.Find ("img_exp").GetComponent<Image> ();

		txLevel = this.transform.Find ("tx_level").GetComponent<Text> ();
		txName = this.transform.Find ("tx_name").GetComponent<Text> ();


		playerATK = gameObject.transform.parent.GetComponent<PlayerATKAndDamage> ();

    }

	public void SetName(string nickname)
	{	
		txName.text = nickname;
	}


    private void Update()
    {
        //设置状态数值

		sliderHpBar.fillAmount = playerATK.hp / playerATK.hp_Max;
		sliderSpBar.fillAmount = playerATK.sp / playerATK.sp_Max;
		sliderExp.fillAmount = playerATK.exp / playerATK.exp_Max;

        txLevel.text = ""+ playerATK.level;

    }


  
   

   
}