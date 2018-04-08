using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Control_ScrollFlow_Item : MonoBehaviour
{
    private UI_Control_ScrollFlow parent;
    [HideInInspector]
    public  RectTransform rect;
    public  float v = 0;
    public float sv; //缩放值
    Vector3 p, s;  //p是卡牌位置，s是卡牌大小
    
    //Color color;
    //public Image img;



    public void Init(UI_Control_ScrollFlow _parent)
    {
        rect =this. GetComponent<RectTransform>();
       // img = this.GetComponent<Image>();
        parent = _parent;
      //  color = img.color;
    }

    //滑动卡牌时控制位置和大小
    public void Drag(float value)
    {
        v += value;
        p=rect.localPosition;
        p.x=parent.GetPosition(v);
        rect.localPosition = p;

        //color.a = parent.GetApa(v);
        // img.color = color;
        sv = parent.GetScale(v);
        s.x = sv;
        s.y = sv;
        s.z=1;
        rect.localScale = s;
    }
}
