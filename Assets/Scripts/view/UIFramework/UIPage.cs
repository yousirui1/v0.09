using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.UI;
//using Umeng;
using System.Threading;
using System.Linq;
using DG.Tweening;
/**************************************
*FileName: UIPage.cs
*User: ysr 
*Data: 2017/12/28
*Describe: 栈存储页面，
			隐藏显示来切换页面 同步异步
**************************************/



namespace tpgm.UI
{


    public enum UIType
    {
        Normal,
        Fixed,      //固定窗口
        PopUp,      //弹出的窗口
        None,       //独立的窗口
    }


    public enum UIMode
    {
        DoNothing,
        HideOther,  //关闭其他界面
        NeedBack,   //点击返回按钮关闭当前页面，不关闭其他界面
        NoNeedBack, //关闭ToBar, 关闭其他页面,不加入backSequence队列
    }


    public enum UICollider
    {
        None,       //显示该界面不包含背景
        Normal,     //碰撞透明背景
        WithBg,     //碰撞非透明背景
    }


    public abstract class UIPage 
    {
        public string name = string.Empty;

        public int id = -1;

        //页面的属性
        public UIType type = UIType.Normal;

        //页面的模式
        public UIMode mode = UIMode.DoNothing;

        //the backgroup collider mode
        public UICollider collider = UICollider.None;

        //载入ui的路径
        public string uiPath = string.Empty;

        //this ui's gameobject
        public GameObject gameObject;
        public Transform transform;


        //用字典存储所以得页面数据
        private static Dictionary<string, UIPage> m_allPages;
        public static Dictionary<string, UIPage> allPages
        { get { return m_allPages; } }


        //控制页面的显示层次关系 1>2>3>4
        private static List<UIPage> m_currentPageNodes;
        public static List<UIPage> currentPageNodes
        { get { return m_currentPageNodes; } }

        //同步 =false、异步 = true  flag
        private bool isAsyncUI = false;


        //隐藏 =false、显示 = true  flag
        protected bool isActived = false;

        //刷新页面数据
        private object m_data = null;
        protected object data { get { return m_data; } }

        //delegate load ui function 同步、异步载入方式
        public static Func<string, Object> delegateSyncLoadUI = null;
        public static Action<string, Action<Object>> delegateAsyncLoadUI = null;


		private Toast m_toast = null;
		protected Toast toast { get { return m_toast;}}

     

        #region virtual api

        //when Instance UI ony once
        public virtual void Awake(GameObject go) {
			

        }
	


    


        //show ui refresh eachtime
        public virtual void Refresh() { }

        //active this ui
        public virtual void Active()
        {
            this.gameObject.SetActive(true);
            isActived = true;
        }

        //only Deactive UI wont clear Data
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
            isActived = false;
            this.m_data = null;
			unloadRes (SavedContext.s_texCache, SavedContext.s_valTableCache);

        }



		//载入资源
		protected abstract void loadRes(TexCache texCache, ValTableCache valCache);
		//卸载资源
		protected abstract void unloadRes(TexCache texCache, ValTableCache valCache);

		//text文件
		protected TexCache getTexCache()
		{
			return SavedContext.s_texCache;
		}

		//数值表
		protected ValTableCache getValTableCache()
		{
			return SavedContext.s_valTableCache;
		}

		//主线程
		protected MainLooper getMainLooper()
		{
			return MainLooper.instance();
		}


	

		public bool hasDestroy()
		{
			if (0 != m_destroyUtcMs)
			{
				long now = TimeUtils.utcNowMs();
				Debug.Log("has destroy: " + (now - m_destroyUtcMs));

				return true;
			}

			return false;
		}


		protected long m_destroyUtcMs;

		//消息处理
		protected void handleMsgDispatch(HandlerMessage msg)
		{
			if (hasDestroy())
			{
				return;
			}

			onHandleMsg(msg);
		}

		protected virtual void onHandleMsg(HandlerMessage msg)
		{
		}


		//#网络请求n毫秒内没响应就会把等待框显示出来;
		public void postShowWaitDialogMessage()
		{
			MainLooper looper = getMainLooper();
			HandlerMessage msg = MainLooper.obtainMessage(looper_ShowWaitDialog);
			looper.postMessageDelay(msg, 200);
		}

		public void removeShowWaitDialogMessage()
		{
			MainLooper looper = getMainLooper();
			looper.removeMessagesByToExecute(looper_ShowWaitDialog);
		}


		void looper_ShowWaitDialog(HandlerMessage msg)
		{
			//this.Show<> ();
		}


		//#服务器返回的数据格式错误;
		public void serverRespDataError(Exception ex)
		{
			//#提示: 服务器数据出错, 请重新登录;
			//m_toast.create(" 服务器数据出错, 请重新登录");
		}

		//#服务器出现逻辑错误;
		public void serverLogicError(int code)
		{

		}

		//#游戏数据异常: 该有的数据没有;
		public void gameDataCorrupt()
		{
			//Toast.create("游戏数据异常: " + ex.Message);
		}

		//#游戏数据损坏;
		public void gameDataDamage(Exception ex)
		{
			//直接显示: 游戏数据损坏, 请重新启动游戏;
			//Toast.create("游戏数据损坏: " + ex.Message);
		}



        #endregion



		#region 通知
		public class Toast : BaseController<UIPage>
		{
			UIPage m_page;

			//public static List<Params> m_queue = new List<Params>();

			internal long m_sec = 3500;


			public GameObject toastObj;

			public MessageHandlerProxy m_msgHandlerProxy;

			public float start_time;

			public Toast(UIPage iview):base(null)
			{
				m_page = iview;
				m_msgHandlerProxy = new MessageHandlerProxy(handleMsg);
			}

			public void onDestroy()
			{
				//m_queue.RemoveAt(0);

				//checkToastQueue();
			}
				
			public void InitToast(GameObject parentObj)
			{
				toastObj = parentObj.transform.Find("toast").gameObject;
				toastObj.SetActive (false);

			}

			public void showToast(string text)
			{
				
				toastObj.transform.Find ("bg_toast/tx_toast").GetComponent<Text> ().text = text;
				toastObj.transform.localPosition = new Vector2 (0.0f, -100.0f);
				Sequence seq = DOTween.Sequence();
				//#先快, 后慢(快落到底的时候), 然后快速回弹;
				/*seq.Append(m_page.toastObj.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f, 13.0f), 1.8f).SetRelative())
					.Append(m_page.toastObj.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f, -13.0f), 2.0f).SetRelative())
					.SetDelay(0.1f)
					.SetLoops(-1);*/  //翻转位置
				//m_page.toastObj.GetComponent<Renderer> ().material.color = new Color (0, 1, 0, TimeUtils.utcNowMs());
				toastObj.SetActive (true);
				seq.Append (toastObj.GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (0.0f, 100.0f), 1.5f).SetRelative ())
					.Append (toastObj.GetComponent<Renderer>().material.DOFade(1,1).SetLoops(-1,LoopType.Yoyo))
					.Append(toastObj.GetComponent<RectTransform> ().DOAnchorPos (new Vector2 (0.0f, 100.0f), 1.5f).SetRelative ())
					.SetDelay (1.0f);
				
				MainLooper looper = m_page.getMainLooper();
				//渐变

				HandlerMessage msg1 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 2);
				looper.postMessageDelay (msg1, 500);

				HandlerMessage msg2 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 3);
				looper.postMessageDelay (msg2, 1000);

				HandlerMessage msg3 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 4);
				looper.postMessageDelay (msg3, 1500);

				HandlerMessage msg4 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 5);
				looper.postMessageDelay (msg4, 2000);

				HandlerMessage msg5 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 6);
				looper.postMessageDelay (msg5, 2500);

				HandlerMessage msg6 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 7);
				looper.postMessageDelay (msg6, 3000);


				HandlerMessage msg = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 1);
				looper.postMessageDelay (msg, m_sec);


			}
		
			private void looper_ShowEndToast()
			{
				Debug.Log ("looper_ShowEndToast");
				toastObj.SetActive (false);
			}

			private void looper_CheckAlpha(float bg_alpha,float tx_alpha)
			{
				Color bg_color = toastObj.transform.Find ("bg_toast").GetComponent<Image> ().color;
				bg_color = new Color (bg_color.r, bg_color.g, bg_color.b, bg_alpha);
				toastObj.transform.Find ("bg_toast").GetComponent<Image> ().color = bg_color;

				Color tx_color = toastObj.transform.Find ("bg_toast/tx_toast").GetComponent<Text> ().color;
				tx_color = new Color (tx_color.r, tx_color.g, tx_color.b, tx_alpha);
				toastObj.transform.Find ("bg_toast/tx_toast").GetComponent<Text> ().color = tx_color;
			}

		
			void handleMsg(HandlerMessage msg)
			{
				switch (msg.m_what) {
				case 1:
					{
						looper_ShowEndToast ();
					}
					break;

				case 2:
					{
						looper_CheckAlpha (0.6f, 0.9f);
					}
					break;

				case 3:
					{
						looper_CheckAlpha (0.5f, 0.8f);
					}
					break;
				case 4:
					{
						looper_CheckAlpha (0.4f, 0.7f);
					}
					break;

				case 5:
					{
						looper_CheckAlpha (0.3f, 0.6f);
					}
					break;
				case 6:
					{
						looper_CheckAlpha (0.2f, 0.5f);
					}
					break;

				case 7:
					{
						looper_CheckAlpha (0.1f, 0.3f);
					}
					break;

				}

			}

		}






		#endregion

        #region internal api	

        private UIPage() { }
		//页面id用于资源加载索引
		public readonly string m_pageID;
		//页面总数
		private static long s_createCount;

		//GameObject toastObj = null;




        public UIPage(UIType type, UIMode mode, UICollider col)
        {
            this.type = type;
            this.mode = mode;
            this.collider = col;
            this.name = this.GetType().ToString();

			m_toast =new Toast (this);
			//m_toast.InitToast (this.gameObject);

            //Init();
            UIBind.Bind();
			//页面唯一id
			m_pageID = string.Format("{0}{1}", GetType().FullName, s_createCount);


			loadRes(SavedContext.s_texCache, SavedContext.s_valTableCache);


			//Interlocked.Increment(ref s_createCount);

        }

	
	




        //Sync show UI logic
        protected void Show()
        {
            if (this.gameObject == null && string.IsNullOrEmpty(uiPath) == false)
            {
                GameObject go = null;
                //绑定资源管理器
                if (delegateSyncLoadUI != null)
                {
                    Object o = delegateSyncLoadUI(uiPath);
                    go = o != null ? GameObject.Instantiate(o) as GameObject : null;
                }
                else
                {

                    go = GameObject.Instantiate(Resources.Load(uiPath)) as GameObject;
                }

                //protected 
                if (go == null)
                {
                    Debug.LogError("[UI] Cant sync load your ui prefab");
                    return;
                }


                AnchorUIGameObject(go);


                //after instance should awake init
                Awake(go);


                //mark this ui sync ui
                isAsyncUI = false;
            }

            //animation or init when active
            Active();


            //refresh ui component
            Refresh();

            //popup this node to top if need back
            PopNode(this);

        }



        //Async Show UI logic
        protected void Show(Action callback)
        {
            UIRoot.Instance.StartCoroutine(AsyncShow(callback));
        }

        IEnumerator AsyncShow(Action callback)
        {
            //instance ui manager multi gameObject 
            if (this.gameObject == null && string.IsNullOrEmpty(uiPath) == false)
            {
                GameObject go = null;
                bool _loading = true;

                //载入
                delegateAsyncLoadUI(uiPath, (o) =>
                {
                    go = o != null ? GameObject.Instantiate(o) as GameObject : null;
                    AnchorUIGameObject(go);
                    isAsyncUI = true;
                    _loading = false;


                    Active();

                    Refresh();

                    PopNode(this);

                    if (callback != null) callback();
                });


                //自游戏开始时间只读
                float _t0 = Time.realtimeSinceStartup;


                while (_loading)
                {
                    if (Time.realtimeSinceStartup - _t0 >= 10.0f)
                    {
                        Debug.LogError("[UI] WTF async load your ui prefab timeout!");
                        yield break;
                    }
                    yield return null;
                }


            }
            else
            {
                Active();

                Refresh();

                PopNode(this);

                if (callback != null) callback();

            }

        }

        //检测是否能返回上一级
        internal bool CheckIfNeedBack()
        {
            if (type == UIType.Fixed || type == UIType.PopUp || type == UIType.None)
                return false;
            else if (mode == UIMode.NoNeedBack || mode == UIMode.DoNothing)
                return false;

            return true;

        }


        //设置物体的锚点
        protected void AnchorUIGameObject(GameObject ui)
        {
            if (UIRoot.Instance == null || ui == null)
                return;

            this.gameObject = ui;
            this.transform = ui.transform;

            //check if this is ugui or ngui	
            Vector3 anchorPos = Vector3.zero;
            Vector2 sizeDel = Vector2.zero;
            Vector3 scale = Vector3.one;
            if (ui.GetComponent<RectTransform>() != null)
            {
                //anchoredPosition + sizeDelta 来设置位置和大小,不能自动拉伸
                //Pivot相对Anchor refernce点的位置	
                anchorPos = ui.GetComponent<RectTransform>().anchoredPosition;
                sizeDel = ui.GetComponent<RectTransform>().sizeDelta;
                scale = ui.GetComponent<RectTransform>().localScale;
            }
            else
            {
                anchorPos = ui.transform.localPosition;
                scale = ui.transform.localScale;
            }

            if (type == UIType.Fixed)
            {
                ui.transform.SetParent(UIRoot.Instance.fixedRoot);
            }
            else if (type == UIType.Normal)
            {
                ui.transform.SetParent(UIRoot.Instance.normalRoot);
            }
            else if (type == UIType.PopUp)
            {
                ui.transform.SetParent(UIRoot.Instance.popupRoot);
            }

            if (ui.GetComponent<RectTransform>() != null)
            {
                //anchoredPosition + sizeDelta 来设置位置和大小,不能自动拉伸
                //Pivot相对Anchor refernce点的位置	
                ui.GetComponent<RectTransform>().anchoredPosition = anchorPos;
                ui.GetComponent<RectTransform>().sizeDelta = sizeDel;
                ui.GetComponent<RectTransform>().localScale = scale;
            }
            else
            {
                ui.transform.localPosition = anchorPos;
                ui.transform.localScale = scale;
            }

        }

        //重写修改格式
        public override string ToString()
        {

            return ">Name:" + name + ",ID:" + id + ",Type:" + type.ToString() + ",ShowMode:" + mode.ToString() + ",Collider:" + collider.ToString();
        }

        //是否激活状态
        public bool isActive()
        {
            bool ret = gameObject != null && gameObject.activeSelf;
            return ret || isActived;
        }
        #endregion

        #region static api
        private static bool CheckIfNeedBack(UIPage page)
        {
            return page != null && page.CheckIfNeedBack();
        }


        //页面出栈
        //make the target node to the top
        private static void PopNode(UIPage page)
        {
            if (m_currentPageNodes == null)
            {
                m_currentPageNodes = new List<UIPage>();
            }

            if (page == null)
            {

            }


            if (CheckIfNeedBack(page) == false)
            {
                return;
            }


            bool _isFound = false;

            //在当前栈查找该页面	
            for (int i = 0; i < m_currentPageNodes.Count; i++)
            {
                if (m_currentPageNodes[i].Equals(page))
                {
                    m_currentPageNodes.RemoveAt(i);
                    m_currentPageNodes.Add(page);
                    _isFound = true;
                    break;
                }
            }

            //没有找到页面就添加该页面
            if (!_isFound)
            {
                m_currentPageNodes.Add(page);
            }

            //after pop should hide the old node if need
            HideOldNodes();
        }

        //隐藏旧的栈
        private static void HideOldNodes()
        {
            if (m_currentPageNodes.Count < 0) return;

            UIPage topPage = m_currentPageNodes[m_currentPageNodes.Count - 1];

            if (topPage.mode == UIMode.HideOther)
            {
                //自下而上检测页面的激活状态
                for (int i = m_currentPageNodes.Count - 2; i >= 0; i--)
                {
                    if (m_currentPageNodes[i].isActive())
                        //隐藏
                        m_currentPageNodes[i].Hide();
                }

            }
        }


        public static void ClearNodes()
        {
            m_currentPageNodes.Clear();
        }

        //where 约束泛型T  where T new() 析构函数	
        private static void ShowPage<T>(Action callback, object pageData, bool isAsync) where T : UIPage, new()
        {
            Type t = typeof(T);
            string pageName = t.ToString();

            //查找所以页面是否含有需要显示的页面	
            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
                ShowPage(pageName, m_allPages[pageName], callback, pageData, isAsync);

            }
            else
            {
                //没有就创建然后显示
                T instance = new T();
                ShowPage(pageName, instance, callback, pageData, isAsync);
            }

        }

        private static void ShowPage(string pageName, UIPage pageInstance, Action callback, object pageData, bool isAsync)
        {
            if (string.IsNullOrEmpty(pageName) || pageInstance == null)
            {
                Debug.LogError("[UI] show page error with :" + pageName + " maybe null instance");
                return;
            }

            if (null == m_allPages)
            {
                m_allPages = new Dictionary<string, UIPage>();
            }

            UIPage page = null;
            if (m_allPages.ContainsKey(pageName))
            {
                page = m_allPages[pageName];
            }
            else
            {
                m_allPages.Add(pageName, pageInstance);
                page = pageInstance;
            }

            page.m_data = pageData;

            if (isAsync)
                page.Show(callback);
            else
                page.Show();

        }

        //对外接口

        //同步显示页面
        public static void ShowPage<T>() where T : UIPage, new()
        {
            ShowPage<T>(null, null, false);
        }

        //input page Data 同步显示
        public static void ShowPage<T>(object pageData) where T : UIPage, new()
        {
            ShowPage<T>(null, pageData, false);
        }

        public static void ShowPage(string pageName, UIPage pageInstance)
        {
            ShowPage(pageName, pageInstance, null, null, false);
        }

        public static void ShowPage(string pageName, UIPage pageInstance, object pageData)
        {
            ShowPage(pageName, pageInstance, null, pageData, false);
        }


        //异步显示,先绑定 TTUIBind.Bind()
        public static void ShowPage<T>(Action callback) where T : UIPage, new()
        {
            ShowPage<T>(callback, null, true);
        }

        //input page Data 异步显示
        public static void ShowPage<T>(Action callback, object pageData) where T : UIPage, new()
        {
            ShowPage<T>(callback, pageData, true);
        }

        public static void ShowPage(string pageName, UIPage pageInstance, Action callback)
        {
            ShowPage(pageName, pageInstance, callback, null, true);
        }

        public static void ShowPage(string pageName, UIPage pageInstance, Action callback, object pageData)
        {
            ShowPage(pageName, pageInstance, callback, pageData, true);
        }


        //关闭当前页面
        public static void ClosePage()
        {

            if (m_currentPageNodes == null || m_currentPageNodes.Count <= 1)
                return;

            UIPage closePage = m_currentPageNodes[m_currentPageNodes.Count - 1];
            m_currentPageNodes.RemoveAt(m_currentPageNodes.Count - 1);

            //显示上一页面 隐藏当前页面
            if (m_currentPageNodes.Count > 0)
            {
                UIPage page = m_currentPageNodes[m_currentPageNodes.Count - 1];
                if (page.isAsyncUI)
                    ShowPage(page.name, page, () =>
                    {
                        closePage.Hide();
                    });
                else
                {
                    ShowPage(page.name, page);
                    closePage.Hide();
                }


            }
        }

        //关闭指定target页面
        public static void ClosePage(UIPage target)
        {
            if (target == null)
                return;

            if (target.isActive() == false)
            {
                if (m_currentPageNodes != null)
                {
                    //查找页面
                    for (int i = 0; i < m_currentPageNodes.Count; i++)
                    {
                        if (m_currentPageNodes[i] == target)
                        {
                            m_currentPageNodes.RemoveAt(i);
                            break;
                        }
                    }
                    return;
                }
            }

            //检查是否是当前显示页面,并且有没有上层隐藏页面
            if (m_currentPageNodes != null && m_currentPageNodes.Count >= 1 && m_currentPageNodes[m_currentPageNodes.Count - 1] == target)
            {
                m_currentPageNodes.RemoveAt(m_currentPageNodes.Count - 1);

                //显示上层页面	
                if (m_currentPageNodes.Count > 0)
                {
                    UIPage page = m_currentPageNodes[m_currentPageNodes.Count - 1];
                    if (page.isAsyncUI)
                        ShowPage(page.name, page, () =>
                        {
                            target.Hide();
                        });
                    else
                    {
                        ShowPage(page.name, page);
                        target.Hide();
                    }

                    return;
                }

            }
            //是否可以返回主界面
            else if (target.CheckIfNeedBack())
            {
                for (int i = 0; i < m_currentPageNodes.Count; i++)
                {
                    if (m_currentPageNodes[i] == target)
                    {
                        m_currentPageNodes.RemoveAt(i);
                        target.Hide();
                        break;
                    }
                }

            }
            target.Hide();


        }

        //关闭泛型页面	
        public static void ClosePage<T>() where T : UIPage
        {
            Type t = typeof(T);
            string pageName = t.ToString();

            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
                ClosePage(m_allPages[pageName]);
            }
            else
            {
                Debug.LogError(pageName + "have not show yet!");

            }
        }

        //根据页面名关闭页面
        public static void ClosePage(string pageName)
        {
            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
                ClosePage(m_allPages[pageName]);
            }
            else
            {

                Debug.LogError(pageName + " have not shwo yet !");
            }

        }

        #endregion

    }
}


#if false
/*Transform[] touch;
touch = this.gameObject.transform.GetComponentsInChildren<Transform>();


foreach (Transform child in touch)
{
if (child.GetComponent<Button>())
{
child.GetChild(0).GetComponent<Text>().text = UIButtonID.GetName(typeof(UIButtonID), btnId);

child.name = UIButtonID.GetName(typeof(UIButtonID), btnId);
child.GetComponent<Button>().onClick.AddListener(delegate () {
UIEventMgr.Instance().OnClickEvent(child.gameObject);
});
btnId++;
}

if (child.GetComponent<InputField>())
{
child.name = UIInputFieldID.GetName(typeof(UIInputFieldID), inputId);

child.GetChild(0).GetComponent<Text>().text = UIInputFieldID.GetName(typeof(UIInputFieldID), inputId);
inputId++;
}

if (child.tag == "UILabel")
{
child.name = UITextID.GetName(typeof(UITextID), txId);
child.GetComponent<Text>().text = UITextID.GetName(typeof(UITextID), txId);
txId++;
}

if (child.GetComponent<SpriteRenderer>())
{
child.name = UIIamgeID.GetName(typeof(UIIamgeID), imgId);

if (child.tag == "AtlasSprite")
{
child.GetComponent<SpriteRenderer>().sprite = TextureManage.getInstance().LoadAtlasSprite(UIValue.UIPathTexture[imgId], "General_icon_0");
}
else
{
child.GetComponent<SpriteRenderer>().sprite = ResourceMgr.Instance().Load<Sprite>(UIValue.UIPathTexture[imgId], false);
}
child.GetComponent<SpriteRenderer>().sprite = ResourceMgr.Instance().Load<Sprite>(UIValue.UIPathTexture[imgId], false);
imgId++;
}
}*/




public  void create(string text)
{
Params p = new Params(text);

queueToast(p);
}

public  void create(string text, long sec)
{
Params p = new Params(text);
p.m_sec = sec;

queueToast(p);
}



void queueToast(Params p)
{
if (m_queue.Count > 0)
{
}
else
{
showToast(p.m_text, p.m_sec);
}

m_queue.Add(p);
}

//#检查是否还有要显示的toast;
void checkToastQueue()
{
if (m_queue.Count > 0)
{
Params p = m_queue[0];
showToast(p.m_text, p.m_sec);
}
}


public class Params
{
public Params(string str)
{
m_text = str;
}

internal string m_text = "";
internal long m_sec = 3500;
}




public void postShowWaitDialogMessage()
{
MainLooper looper = MainLooper.instance();
HandlerMessage msg = MainLooper.obtainMessage(looper_ShowEndToast);
looper.postMessageDelay(msg, 4600);
}

public void removeShowToast()
{
MainLooper looper = MainLooper.instance();
looper.removeMessagesByToExecute (looper_ShowEndToast);
}



public void showToast(string text, long sec)
{

m_page.toastObj.transform.Find ("bg_toast/tx_toast").GetComponent<Text> ().text = text;
m_page.toastObj.transform.localPosition = new Vector2 (0.0f, -100.0f);

Sequence seq = DOTween.Sequence();
MainLooper looper = m_page.getMainLooper();
HandlerMessage msg = MainLooper.obtainMessage(looper_ShowEndToast);
looper.postMessageDelay(msg, sec);

}
#endif