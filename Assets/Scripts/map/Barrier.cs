using UnityEngine;
using System.Collections;
using tpgm;


/**************************************
*FileName: Barrier.cs
*User: ysr 
*Data: 2018/3/19
*Describe: 不可通过位移穿过的建筑物
**************************************/


public class Barrier : MonoBehaviour
{

	void OnTriggerStay2D(Collider2D collider)
	{
		int speed = 0;
		//计算建筑在人物的那个方向上，禁止人物继续朝建筑移动
		if (collider.tag == Tags.player) {
			int d = 0;
			if (collider.transform.position.x > this.transform.position.x) {
				
			}
			if (collider.transform.position.y > this.transform.position.x) {

			}
			#if false
			speed = collider.GetComponent<ATKAndDamage> ().speed;
			if (d == collider.GetComponent<ATKAndDamage> ().d) {
			collider.GetComponent<ATKAndDamage> ().speed = 0;
			} else {

			}
			#endif

		}

	}



		#if false

		Vector2 vec = new Vector2 (0,0);

		switch (players[id].d) {
		case 1:
		{
		vec = new Vector2 (-1.0f, 0);
		}
		break;
		case 2:
		{
		vec = new Vector2 (-1.0f,1.0f);
		}
		break;
		case 3:
		{
		vec = new Vector2 (0,1.0f);
		}
		break;
		case 4:
		{
		vec = new Vector2 (1.0f,1.0f);
		}
		break;
		case 5:
		{
		vec = new Vector2 (1.0f,0);
		}
		break;
		case 6:
		{
		vec = new Vector2 (1.0f,-1.0f);
		}
		break;
		case 7:
		{
		vec = new Vector2 (0,-1.0f);
		}
		break;
		case 8:
		{
		vec = new Vector2 (-1.0f,-1.0f);
		}
		break;
		}

		//发射射线判断障碍物
		RaycastHit2D hit = Physics2D.Raycast(gameObj.transform.position, vec, 3, 1<<LayerMask.NameToLayer("barrier"));

		if (hit.collider == null) {
		shadow.craft_move (players [id], 1);
		}

		else if(players[id].skill == 500)
		{
		shadow.craft_move (players [id], 1);
		shadow.craft_move (players [id], 1);
		shadow.craft_move (players [id], 1);
		shadow.craft_move (players [id], 1);
		}

		#endif


}