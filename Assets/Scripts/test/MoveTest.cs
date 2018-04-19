using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour {
	





	#if false
	public GameObject obj;

	Shadow shadow = new Shadow();

	private int[,] direction = new int[9, 2] { {0,0 },{0, -1},{1, -1},{1, 0},{1, 1},
		{0, 1},{-1, 1},{-1, 0},{-1, -1}};
	
	// Use this for initialization
	void Start () {
		//Debug.Log (obj.transform.position);
		//obj.transform.Translate (1, 0, 0);
		//obj.transform.Translate (1, 0, 0);
		//obj.transform.Translate (1, 0, 0);

		//Debug.Log (obj.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
		#if false
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");


		obj.transform.Translate (horizontal * Time.fixedDeltaTime, vertical * Time.fixedDeltaTime, 0);
		#endif
	}



	void FixedUpdate()
	{
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		int d = 0;

		Vector3 startVec = obj.transform.position;

		//Debug.Log (d);
		Vector2  vec2 = new Vector2(horizontal,vertical);
		Vector3 endVec =  craft_move (startVec, 1, vec2);


		Debug.Log (endVec);
		Move (obj, obj.transform.position, endVec);
	}


	void Move(GameObject playObj, Vector3 oriVec, Vector3 endVec)
	{
		playObj.transform.Translate (endVec - oriVec);
		//Debug.Log (endVec - oriVec);
		//Debug.Log ("MOve");
	}

	//飞机移动
	public Vector3 craft_move(Vector3 vec, int step, Vector2 dVec)
	{
		int inc = 1;

		if (step < 0)
		{
			step = -step;
			inc = -1;
		}

		for (int i = 0; i < step; i++)
		{
			vec.x += 5 * dVec.x * inc * Time.fixedDeltaTime;
			vec.y += 5 * dVec.y * inc * Time.fixedDeltaTime;
		}

		return vec;
	}
	#endif
}
