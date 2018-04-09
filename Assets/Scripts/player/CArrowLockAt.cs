using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm;
public class CArrowLockAt : MonoBehaviour {

	//离屏幕的边缘距离
	float devValue = 50f;

	float showWidth;
	float showHeight;

	float Width;
	float Height;

	Transform self;

	Transform group1Tr;
	Transform group2Tr;

	Transform bastTr;


	Transform arrow_group1Tr;
	Transform arrow_group2Tr;
	Transform arrow_bastTr;
	Transform bg_bastTr;

	Quaternion originRot_group1;
	Quaternion originRot_group2;

	Quaternion originRot_bast;

	void Start()
	{
		Width = 1280f;
		Height = 720f;
	}

	//设置自身
	public void SetSelf(Transform transform)
	{
		self = transform;
	}

	//设置队友1
	public void SetGroup1(Transform transform)
	{
		arrow_group1Tr = this.transform.Find ("arrow1");
		//箭头的原角度
		originRot_group1 = arrow_group1Tr.rotation;
		group1Tr = transform;
	}

	//设置队友2
	public void SetGroup2(Transform transform)
	{
		arrow_group2Tr = this.transform.Find ("arrow2");
		//箭头的原角度
		originRot_group2 = arrow_group2Tr.rotation;
		group2Tr = transform;

	}

	//设置最佳玩家
	public void SetBast(Transform transform)
	{
		arrow_bastTr = this.transform.Find ("arrow0");
		bg_bastTr = this.transform.Find ("arrow_bast");
		//箭头的原角度
		originRot_bast = arrow_bastTr.rotation;	
		bastTr = transform;
	}


	void LateUpdate()
	{
		if (self != null) {
			if (group1Tr != null ) {
				ArrowLockAt (self, group1Tr, arrow_group1Tr, originRot_group1);
			} 

			if (group2Tr != null ) {
				ArrowLockAt (self, group2Tr, arrow_group2Tr, originRot_group2);
			} 
			
			if (bastTr != null ) {
				ArrowLockAt (self, bastTr, arrow_bastTr, originRot_bast);
				//bg_bastAt (bg_bastTr, arrow_bastTr);
			} 
		}
	}

	//自身,目标,箭头
	void ArrowLockAt(Transform self, Transform target, Transform arrow, Quaternion originRot)
	{
		//Debug.Log (target.position);
		float direction;
		Vector3 u;

	
		showWidth = Width/ 2 - devValue ;
		showHeight = Height / 2 - devValue ;

	

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

		//transform.RotateAround(Vector3.zero, Vector3.up, 20 * Time.deltaTime); 

		//给与旋转值
		arrow.rotation = originRot * Quaternion.Euler(new Vector3(0f, 0f, direction * (u.z > 0 ? 1: -1)));


		//计算当前物体在屏幕上的位置
		//Vector2 screenPos = camera.WorldToScreenPoint(target.position);


		Vector2 screenPos = new Vector2 (self.transform.position.x, self.transform.position.y) ;



		if(screenPos.x < devValue || screenPos.x > Width - devValue || screenPos.y < devValue || screenPos.y > Height - devValue || Vector3.Dot(forVec, angVec) < 0)
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
					if (direction > 90) {
						result.y = -showHeight;
						result.x = result.x * -1;
					} else if(direction < 63){

						result.y = showHeight;
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
			arrow.localPosition = result;

		}
		else  //在屏幕内的情况
		{
			//Debug.Log ("在屏幕内的情况"+screenPos.x);
			arrow.localPosition = screenPos;
		}	
	}








	void bg_bastAt(Transform self, Transform target)
	{

		float direction;
		Vector3 u;

		showWidth = Width / 2 - devValue;
		showHeight = Height / 2 - devValue;

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

		Vector2 screenPos = new Vector2(self.position.x, self.position.y);

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
					if (direction > 90) {
						result.y = -showHeight;
						result.x = result.x * -1;
					} else if(direction < 63){

						result.y = showHeight;
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
			self.localPosition = result;

		}
		else  //在屏幕内的情况
		{
			self.localPosition = screenPos;
		}	
	}

}



