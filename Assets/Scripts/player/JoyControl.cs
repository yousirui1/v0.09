using UnityEngine;
using System.Collections;
using System;
using SimpleJson;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;  


public class JoyControl : MonoBehaviour {
    public RectTransform rect_Viewport;
    //public RectTransform rect_Joy;//将获取坐标作为摇杆键值
    public int iR;

    //虚拟摇杆的值-1 ~1
	public float vertical { set; get; }

    public float horizontal { set; get; }

    //技能和普攻
    private int skill = 0;

    private int fire = 0;
	
	private GameObject obj;

	public bool isMove = false;

	RectTransform joy;

	GameObject pointerObj;  //箭头
	
    void Start()
    {
		rect_Viewport = this.gameObject.transform.Find ("Viewport").GetComponent<RectTransform> ();
		joy = rect_Viewport.gameObject.transform.Find ("Joy").GetComponent<RectTransform> ();

		pointerObj = this.gameObject.transform.Find ("pointer").gameObject;

		//pointerObj.transform.rotation =   Quaternion.Euler(0, 0, 45);
		//Adds a listener to the main slider and invokes a method when the value changes.
		this.gameObject.GetComponent<ScrollRect> ().onValueChanged.AddListener(
			delegate { On_Move(joy); }
		);

		AddTriggersListener(EventTriggerType.BeginDrag, On_Begin);  

		AddTriggersListener(EventTriggerType.EndDrag, On_End);  



        iR = (int)rect_Viewport.sizeDelta.x / 2;

    }

	private void AddTriggersListener(EventTriggerType eventType, UnityAction<BaseEventData> action)
	{
		EventTrigger trigger = this.gameObject.GetComponent<EventTrigger> ();
		if (null == trigger) {
			trigger =  this.gameObject.AddComponent<EventTrigger>(); 
		}
			
		//实例化delegates  
		if (trigger.triggers.Count == 0)  
		{  
			trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();  
		}  

		//定义所要绑定的事件类型   
		EventTrigger.Entry entry = new EventTrigger.Entry();  
		//设置事件类型    
		entry.eventID = eventType;  
		//定义回调函数    
		UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(action);  
		//设置回调函数    
		entry.callback.AddListener(callback);  

		//添加事件触发记录到GameObject的事件触发组件    
		trigger.triggers.Add(entry); 
	}

	//手指结束滑动
	void On_End(BaseEventData eventData)
	{
		isMove = false;
	}

	//手指开始滑动
	void On_Begin(BaseEventData eventData)
	{
		isMove = true;
	}



	//计算滑动的方向
    void On_Move(RectTransform rect)
    {

      	if (rect.anchoredPosition.magnitude > iR)
      	{//将摇杆限制在 半径 iR 以内
            rect.anchoredPosition = rect.anchoredPosition.normalized * iR;
      	}
		if(isMove)
		{
			
			pointerObj.transform.localEulerAngles =  new Vector3(0, 0, -(float)(Math.Atan2(rect.anchoredPosition.x, rect.anchoredPosition.y) * 180 / Math.PI-45));

			horizontal = rect.anchoredPosition.x / 45.0f;
			vertical = rect.anchoredPosition.y / 45.0f;

			#if false
			if (rect.anchoredPosition.x > 20)
            {
                horizontal = 1;
            }
            else if(rect.anchoredPosition.x <-20)
            {
				
                horizontal = -1;
            }
            else{
				
                horizontal = 0;
            }

            if (rect.anchoredPosition.y > 20)
            {
				
                vertical = 1;
            }
            else if(rect.anchoredPosition.y <-20)
            {
				
                vertical = -1;
            }
            else
            {
                vertical = 0;
            }
			#endif
		
		}		
		else
		{	
			vertical = 0;
			horizontal = 0;
            
        }
        //Debug.Log("vertical"+vertical+"horizontal"+horizontal);
   }

    public Vector2 getPos()
    {
		return joy.anchoredPosition;
    }

    public JsonObject jsonPos;
    //事件发送给服务器
  



}
