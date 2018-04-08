using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm;
public class FollowPlayer : MonoBehaviour {
    
  
    private bool isFind = false;

	GameObject userObj ;

	private string uid;

	private Vector3 vector3;

	// Use this for initialization
	void Start () {
    }

  

	public bool SetUid(GameObject userObj)
	{
		this.userObj = userObj;
		if (null != userObj)
		{
			this.transform.localPosition = new Vector3(userObj.transform.localPosition.x, userObj.transform.localPosition.y, -400);
			isFind = true;
		}
		return isFind;
	}

    //LateUpdate晚于所有Update执行
	void LateUpdate()
    {
		if(isFind)
		{
			vector3 = new Vector3(userObj.transform.localPosition.x, userObj.transform.localPosition.y, -400);

			if (userObj.transform.localPosition.y >= 6330) {
				vector3 = new Vector3(vector3.x, 6330, vector3.z);
			}

			if(userObj.transform.localPosition.y <= 330)
			{
				vector3 = new Vector3(vector3.x, 330, vector3.z);
			}

			if(userObj.transform.localPosition.x >= 6050)
			{
				vector3 = new Vector3(6050, vector3.y, vector3.z);
			}

			if(userObj.transform.localPosition.x <= 610)
			{
				vector3 = new Vector3(610, vector3.y, vector3.z);
			}
				
			this.transform.localPosition = vector3;
        }
    }




}
