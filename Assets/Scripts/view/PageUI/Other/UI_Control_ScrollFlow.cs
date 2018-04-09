using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using tpgm;
public class UI_Control_ScrollFlow : MonoBehaviour, IDragHandler, IEndDragHandler
{
    RectTransform Rect;
    GameObject mainUI;

    public List<UI_Control_ScrollFlow_Item> Items;  //滑动卡牌列表
    private List<UI_Control_ScrollFlow_Item> GotoFirstItems = new List<UI_Control_ScrollFlow_Item>(), GotoLaserItems = new List<UI_Control_ScrollFlow_Item>();
    public float Width=810;
    public float MaxScale=1;
    public float StartValue = 0.2f, AddValue = 0.15f, VMin = 0.2f, VMax = 0.8f;
    public AnimationCurve PositionCurve;
    public AnimationCurve ScaleCurve;  
    public AnimationCurve ApaCurve;  
    private Vector2 start_point=Vector2.zero, add_vect;
    public bool _anim = false;  //动画状态 
    public float _anim_speed = 1f;  //动画速度
    private float v = 0;
   
    private void Start()
    {
        Rect = GetComponent<RectTransform>();
		mainUI = GameObject.Find ("ImageMain").gameObject;
        Refresh();
    }

    //初始化位置
    public void Refresh()
    {
        
        for (int i = 0; i < Rect.childCount; i++)
        {
            UI_Control_ScrollFlow_Item item = Rect.GetChild(i).GetComponent<UI_Control_ScrollFlow_Item>();
            if (item != null)
            {
                Items.Add(item);
                item.Init(this);
                item.Drag(StartValue + i * AddValue);
                if (item.v - 0.5 < 0.05f)
                {
                    Current = Items[i];
                }
            }
        }
        if (Rect.childCount < 5)
        {
            VMax = StartValue + 4 * AddValue;
        }
        else
        {
            VMax = StartValue + (Rect.childCount - 1) * AddValue;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
       	

		mainUI.SetActive(false);

        add_vect = eventData.position - start_point;
        v = eventData.delta.x * 1.00f / Width;
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Drag(v);
        }
        Check(v);
      
    }
    

    public void Check(float _v)
    {
		SoundPlay.cardClick ();
		if (_v < 0)
        {//向左运动
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].v < (VMin - AddValue / 2))
                {
                    GotoLaserItems.Add(Items[i]);
                }
            }
            if (GotoLaserItems.Count > 0)
            {
                for (int i = 0; i < GotoLaserItems.Count; i++)
                {
                    Items.Remove(GotoLaserItems[i]);
                    GotoLaserItems[i].v = Items[Items.Count - 1].v + AddValue;
                    
                    Items.Add(GotoLaserItems[i]);
                }
                GotoLaserItems.Clear();
            }
        }
        else if (_v > 0)
        {//向右运动，需要把右边的放到前面来

            for (int i = Items.Count-1; i >0; i--)
            {
                if (Items[i].v >= VMax)
                {
                    GotoFirstItems.Add(Items[i]);
                }
            }
            if (GotoFirstItems.Count > 0)
            {
                for (int i = 0; i < GotoFirstItems.Count; i++)
                {
                    GotoFirstItems[i].v = Items[0].v - AddValue;
                    Items.Remove(GotoFirstItems[i]);
                    Items.Insert(0, GotoFirstItems[i]);
                }
                GotoFirstItems.Clear();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float k = 0,v1;
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].v >= VMin)
            {
                v1 = (Items[i].v - VMin)%AddValue;
                if (add_vect.x >= 0)
                {
                    k = AddValue - v1;
                }
                else
                {
                    k = v1 * -1;
                }
                break;
            }
        }
        add_vect = Vector3.zero;
        AnimToEnd(k);
        mainUI.SetActive(true);
       
    }

    public float GetApa(float v)
    {
        return ApaCurve.Evaluate(v);
    }
    public float GetPosition(float v)
    {
        return PositionCurve.Evaluate(v) * Width;
    }
    public float GetScale(float v)
    {
        return ScaleCurve.Evaluate(v) * MaxScale;
    }


  
    public void LateUpdate()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].v >= 0.15f && Items[i].v <= 0.25f) Items[i].GetComponent<RectTransform>().SetSiblingIndex(0);
            else if (Items[i].v >= 0.75f && Items[i].v <= 0.85f) Items[i].GetComponent<RectTransform>().SetSiblingIndex(1);
            else if (Items[i].v >= 0.3f && Items[i].v <= 0.4f) Items[i].GetComponent<RectTransform>().SetSiblingIndex(2);
            else if (Items[i].v >= 0.6f && Items[i].v <= 0.7f) Items[i].GetComponent<RectTransform>().SetSiblingIndex(3);
            else if (Items[i].v >= 0.45f && Items[i].v <= 0.55f) Items[i].GetComponent<RectTransform>().SetSiblingIndex(4);
        }
		#if false
        for (int i = 0; i < Items.Count; i++)
        {
            Debug.Log(Items[i].name + ":" + Items[i].GetComponent<RectTransform>().GetSiblingIndex());
            if (i == Items.Count - 1)
            {
                //Debug.Log("-------------------------");
            }
        }
		#endif

        //替换资源
		mainUI.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load("images/ui/main/" + Current.transform.GetChild(1).GetComponent<Image>().sprite.name + "On", typeof(Sprite)) as Sprite;
        
		mainUI.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("images/ui/kapai/" + Current.transform.GetChild(0).GetComponent<Image>().sprite.name, typeof(Sprite)) as Sprite;
       
    }

    public void ToLaster(UI_Control_ScrollFlow_Item item)
    {
        item.v=Items[Items.Count - 1].v + AddValue;
        Items.Remove(item);
        Items.Add(item);
    }

  
    private float AddV = 0, Vk=0,CurrentV=0,Vtotal=0,VT=0;
 
    public UI_Control_ScrollFlow_Item Current;



    public void AnimToEnd(float k)
    {
        AddV= k;
        if (AddV > 0) { Vk = 1; }
        else if (AddV < 0) { Vk = -1; }
        else
        {
            return;
        }
        Vtotal = 0;
        _anim = true;

    }

    void Update()
    {
        if (_anim)
        {
            CurrentV = Time.deltaTime * _anim_speed * Vk;
            VT = Vtotal + CurrentV;
            if (Vk > 0 && VT >= AddV) { _anim = false; CurrentV = AddV - Vtotal; }
            if (Vk < 0 && VT <= AddV) { _anim = false; CurrentV = AddV - Vtotal; }
            //==============
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Drag(CurrentV);
                if(Items[i].v-0.5<0.05f)
                {
                    Current = Items[i];                 
                }
            }
            Check(CurrentV);
            Vtotal = VT;    
        }
    }
     
}
