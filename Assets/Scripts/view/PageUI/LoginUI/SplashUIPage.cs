using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;
using tpgm.val;

public class SplashUIPage : UIPage
{
	private const string TAG = "SplashUIPage";

	ValUpdateController m_ValController;
	SplashController m_SplashController;

	GameObject msgObj = null;
	GameObject stateObj = null;
	//进度条
	GameObject prgObj = null;

	public SplashUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoginUI/SplashUIPage";

	}

	public override void Awake(GameObject go)
	{
		//PrefValUpdate.clear ();


		//版本检查
		//m_SplashController = new SplashController(this);
		//m_SplashController.checkHasNewApk();
		
		//数值表检查
		m_ValController = new ValUpdateController(this);
		m_ValController.checkFileModified();
	

		stateObj = this.gameObject.transform.Find("tx_state").gameObject;
		msgObj = this.gameObject.transform.Find("bg_message/tx_message").gameObject;
		msgObj.SetActive(false);

		prgObj = this.gameObject.transform.Find("bg_message/tx_message").gameObject;
		prgObj.SetActive(false);

			
	}




	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	#region 版本更新
	
	//正常流程回调
	
	//有新版本
	public void findNewApk()
	{
		
		showUpdateConfirm();
	}

	void showUpdateConfirm()
	{

	}
	
	public void apkDownBegin()
	{
		prgObj.SetActive(true);
	}

	public void downPrg()
	{
		long nowKB = m_SplashController.m_nowBytes / 1000;
		long nowTotalKB = m_SplashController.m_totalBytes / 1000;
		string downInfoStr = "正在下载新版本:(" + nowKB + "k/" + nowTotalKB + "k)";
		msgObj.GetComponent<Text>().text = downInfoStr;

		//进度条
		prgObj.GetComponent<Image>().fillAmount = m_SplashController.m_prg;
	}

	public void installApk()
	{
		stateObj.GetComponent<Text>().text = "正在安装新版本";
		//数值表检查
		m_ValController = new ValUpdateController(this);
		m_ValController.checkFileModified();
	}


	//============================================================ 错误流程 begin;

	void retryCheckHasNewApk()
	{
		
		/*GameObject dialogObj = MonoBehaviour.Instantiate(m_prefabConfirmDialog);
		dialogObj.GetComponent<Canvas>().sortingOrder = m_dialogOrderInLayer++;
		ConfirmDialog dialog = dialogObj.GetComponent<ConfirmDialog>();
		dialog.m_titleView.text = "提示";
		dialog.m_msgView.text = "检查新版本出错，请重新尝试";
		dialog.m_positiveTextView.text = "重试";
		dialog.m_positiveView.onClick.AddListener(delegate {
			m_dialogOrderInLayer--;
			MonoBehaviour.Destroy(dialogObj);

			m_controller.checkHasNewApk();
		});

		dialog.m_negativeTextView.text = "取消";
		dialog.m_negativeView.onClick.AddListener(delegate {
			m_dialogOrderInLayer--;
			MonoBehaviour.Destroy(dialogObj);

			//显示点击检查更新;
		});*/
	}

	public void checkNewApkNetErr()
	{
		//#重新检测;
		retryCheckHasNewApk();
	}

	public void checkApkUpdateBakDirErr()
	{
		retryCheckHasNewApk();
	}

	public void parseLocalApkHashErr()
	{
		//#重新检测;
		retryCheckHasNewApk();
	}

	public void parseServerApkHashErr()
	{
		//#重新检测;
		retryCheckHasNewApk();
	}


	void retryDownload()
	{
		/*GameObject dialogObj = MonoBehaviour.Instantiate(m_prefabConfirmDialog);
		dialogObj.GetComponent<Canvas>().sortingOrder = m_dialogOrderInLayer++;
		ConfirmDialog dialog = dialogObj.GetComponent<ConfirmDialog>();
		dialog.m_titleView.text = "提示";
		dialog.m_msgView.text = "新版本下载出错，请重新尝试";
		dialog.m_positiveTextView.text = "重试";
		dialog.m_positiveView.onClick.AddListener(delegate {
			m_dialogOrderInLayer--;
			MonoBehaviour.Destroy(dialogObj);

			m_controller.downApk();
		});

		dialog.m_negativeTextView.text = "取消";
		dialog.m_negativeView.onClick.AddListener(delegate {
			m_dialogOrderInLayer--;
			MonoBehaviour.Destroy(dialogObj);

			//显示点击检查更新;
		});*/
	}

	public void downApkNetErr()
	{
		//#重新下载;
		retryDownload();
	}

	public void delOldUpdateApkTmp()
	{
		//#重新下载;
		retryDownload();
	}

	public void downDirCreateErr()
	{
		//#重新下载;
		retryDownload();
	}

	public void doDownErr()
	{
		//#重新下载;
		retryDownload();
	}

	public void md5VerifyErr()
	{
		//#重新下载;
		retryDownload();
	}


	void retryWriteOut()
	{
		/*
		GameObject dialogObj = MonoBehaviour.Instantiate(m_prefabConfirmDialog);
		dialogObj.GetComponent<Canvas>().sortingOrder = m_dialogOrderInLayer++;
		ConfirmDialog dialog = dialogObj.GetComponent<ConfirmDialog>();
		dialog.m_titleView.text = "提示";
		dialog.m_msgView.text = "新版本更新出错，请重新尝试";
		dialog.m_positiveTextView.text = "重试";
		dialog.m_positiveView.onClick.AddListener(delegate {
			m_dialogOrderInLayer--;
			MonoBehaviour.Destroy(dialogObj);

			m_controller.retryWriteOutServerApkHash();
		});

		dialog.m_negativeTextView.text = "取消";
		dialog.m_negativeView.onClick.AddListener(delegate {
			m_dialogOrderInLayer--;
			MonoBehaviour.Destroy(dialogObj);

			//显示点击检查更新;
		});*/
	}

	public void writeOutServerApkHashErr()
	{
		//#重新写出;
		retryWriteOut();
	}

	public void finishUpdateErr()
	{
		//#重新写出;
		retryWriteOut();
	}

	public void installApkErr()
	{
		//#重新写出;
		retryWriteOut();
	}

	//============================================================ 错误流程 end;


	#endregion


	#region 数值表更新
	//正常流程回调
	public void valDownBegin()
	{
			
		msgObj.SetActive(true);
		msgObj.GetComponent<Text>().text = "";
	}


	public void valDownPrg()
	{
		stateObj.GetComponent<Text>().text = "更新";
		long nowKB = m_ValController.m_nowBytes / 1000;
		long nowTotalKB = m_ValController.m_totalBytes / 1000;
		string downInfoStr = "正在更新数据包:(" + nowKB +"k/" + nowTotalKB + "k)";
		msgObj.GetComponent<Text>().text = downInfoStr;

		//进度条
		//prgObj.GetComponent<Image>().fillAmount = m_ValController.m_prg;
	}


	public void valUnzip()
	{
		
		stateObj.GetComponent<Text>().text = "正在解压数据包....";
	}

	public void valLoadVal()
	{

		stateObj.GetComponent<Text>().text = "正在加载数据资源....";
	}


	public void valEnd()
	{
		stateObj.GetComponent<Text>().text = "valEnd";
		//进入下一流程
		UIPage.ShowPage<StartUIPage>();
		
	}

	public void valNotModified()
	{
		stateObj.GetComponent<Text>().text = "valNotModified";
		UIPage.ShowPage<StartUIPage>();
		//进入下一流程
	}

	// 异常回调
	public void valErr()
	{
		switch(m_ValController.m_errCode)
		{
			case ErrCode.CHECK_DATA_TMP_BIN:
            case ErrCode.CHECK_VAL_MODIFIED_NET_ERR:
            case ErrCode.CREATE_TMP_BIN:
            case ErrCode.DOWN_FILE_NET_ERR:
            case ErrCode.DEL_OLD_DATA_TMP:
            case ErrCode.DOWN_DIR_CREATE:
            case ErrCode.DO_DOWN:
            case ErrCode.MD5_VERIFY:
            case ErrCode.UNZIP:
            case ErrCode.LOAD_VAL:
            case ErrCode.FINISH_ALL:
            {   
                showErrorConfirm();
            }   
            break;

            }   

		}

	  //异常弹窗
	  void showErrorConfirm()
	  {

		
	  }

	#endregion
}



	
