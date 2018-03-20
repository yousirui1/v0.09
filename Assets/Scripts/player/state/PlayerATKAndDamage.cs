using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**************************************
*FileName: PlayerATKAndDamage.cs
*User: ysr 
*Data: 2017/12/12
*Describe: 玩家攻击和伤害计算脚步
**************************************/

public class PlayerATKAndDamage : ATKAndDamage{

    //GameObject singleSwordGo;

    //攻击距离
    public float  attackRange = 0;

    //攻击力
    public float attack = 10;

    //捡到奖励物品音效
    public AudioClip pickeUpItem;

	//人物移动方向
	private int[,] direction = new int[9, 2] { {0,0 },{0, -1},{1, -1},{1, 0},{1, 1},
		{0, 1},{-1, 1},{-1, 0},{-1, -1}};

	//保存最后的玩家的方向
	public int d = 0;

	//0 idle 1 left 2 right -1 死亡
	//Animator animator;

	//public int ifire = 0;


	protected SpriteRenderer m_sprite;

	public Sprite[] m_clips;

	public int m_state = 0;

	//public int m_oldstate = 0;


	//当前帧
	protected  int m_frame = 0;

	void Start()
	{
		//animator = this.gameObject.GetComponent<Animator> ();
		m_clips =   Resources.LoadAll<Sprite>("images/heros/roles/role1/walk");
		m_sprite = this.gameObject.transform.Find ("role").GetComponent<SpriteRenderer> ();
	}

	JoyControl joy = new JoyControl();

	void FixedUpdate()
	{
		switch (m_state) {
			case 0:
			{
				
			}
			break;

			case 1:
			{
				m_sprite.transform.eulerAngles = new Vector3 (0, 0, 0);
				m_frame++;
				if (m_frame >= m_clips.Length)
					m_frame = 0;
				m_sprite.sprite = m_clips [m_frame];
			}
			break;

			case 2:
			{
				m_sprite.transform.eulerAngles = new Vector3 (0, 180f, 0);
				m_frame++;
				if (m_frame >= m_clips.Length)
					m_frame = 0;
				m_sprite.sprite = m_clips [m_frame];
			}
			break;

		}

	}




}
