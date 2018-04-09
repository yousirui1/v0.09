using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
*FileName: Translucent.cs
*User: ysr 
*Data: 2018/3/19
*Describe: 易碎品 被子弹击中播放一次动画
**************************************/

public class Fragile : MonoBehaviour {

	Animator animator;
	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();
		animator.speed = 0;
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == Tags.bullet) {
			animator.speed = 1;
			//改变层级让人物能穿过
			this.gameObject.layer = LayerMask.NameToLayer ("Default");

		}
	}

}
