using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm;
public class CArrowLockAt : MonoBehaviour {
	//目标
	public Transform target; 
	
	//自己
	public Transform self;
	
	//箭头的旋转方向,只有正的值
	public float direction;
	//叉乘结果，用于判断上述角度正负值
	public Vector3 u;

	//离屏幕的边缘距离
	float devValue = 50f;
	
	float showWidth;
	float showHeight;

	//箭头的原角度
	Quaternion originRot;

	Camera camera;
	
	void Start()
	{
		
		originRot = transform.rotation;
		camera = GameObject.Find ("Camera").GetComponent<Camera> ();
		//self = GameObject.Find (SavedData.s_instance.m_user.m_uid).transform;
	}

	void Update()
	{
		showWidth = Screen.width / 2 - devValue;
		showHeight = Screen.height / 2 - devValue;
	
		//计算向量和角度
		Vector3 forVec = self.forward;
		//本物体和目标之间的单位向量
		Vector3 angVec = (target.position - self.position).normalized;

		//向量投影到本事的xy平面	
		Vector3 targetVec = Vector3.ProjectOnPlane(angVec - forVec, forVec).normalized;
		
		Vector3 originVec = self.up;
		
		//再跟y轴正方向做点积和叉积,求出箭头需要旋转的角度和正负
		direction = Vector3.Dot(originVec, targetVec);

		u = Vector3.Cross(originVec, targetVec);
		
		//转换为角度
		direction = Mathf.Acos(direction) * Mathf.Rad2Deg;

		//叉积结果转换为本物体坐标
		u = self.InverseTransformDirection(u);

		//给与旋转值
		transform.rotation = originRot * Quaternion.Euler(new Vector3(0f, 0f, direction * (u.z > 0 ? 1: -1)));
		
		//计算当前物体在屏幕上的位置
		Vector2 screenPos = camera.WorldToScreenPoint(target.position);


		if(screenPos.x < devValue || screenPos.x > Screen.width - devValue || screenPos.y < devValue || screenPos.y > Screen.height - devValue || Vector3.Dot(forVec, angVec) < 0)
		{
			Vector3 result = Vector3.zero;
			if(direction == 0)
			{

				result.y = showHeight;
			}
			else if (direction == 180)
			{
				result.y = -showHeight;
			}
			else //非特殊角
			{
				//转换角度
				float direction_new = 90 - direction;
				float k = Mathf.Tan(Mathf.Deg2Rad * direction_new);
			
				//矩形
				result.x = showHeight / k;
				if((result.x > (-showWidth) )	&& (result.x < showWidth))
				{
					result.y = -showHeight;
					if(direction > 90)
					{
						result.y = -showHeight;
						result.x = result.x * -1;
					}
				}
				else   //角度在左右底边的情况
				{
					result.y = showWidth * k;
					if((result.y > -showHeight) && (result.y < showHeight))
					{
						result.x = result.y / k;
					
					}

				}

				if(u.z >0)
				{
					result.x = -result.x;
				}
			

			}
			transform.localPosition = result;

		}
		else  //在屏幕内的情况
		{
			Debug.Log ("在屏幕内的情况"+screenPos.x);

			transform.localPosition = screenPos;
		}

	}
}
