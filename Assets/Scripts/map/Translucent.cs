using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
*FileName: Translucent.cs
*User: ysr 
*Data: 2018/3/19
*Describe: 建筑物半透明
**************************************/

public class Translucent : MonoBehaviour {

	Animator animator;
	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();
		animator.speed = 0;
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == Tags.player) {
			animator.speed = 1;
			this.transform.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, 0.43f);

			collider.GetComponent<PlayerATKAndDamage> ().Hide();
		}
	}
		
	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.tag == Tags.player) {
			animator.speed = 0;
			this.transform.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, 1f);

			collider.GetComponent<PlayerATKAndDamage> ().Show();
		}
	}





}
