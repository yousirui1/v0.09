using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using tpgm;

/**************************************
*FileName: ATKAndDamage.cs
*User: ysr 
*Data: 2017/12/12
*Describe: 收到伤害的公共接口
**************************************/

public class ATKAndDamage : MonoBehaviour{
	//当前值
    public float hp  = 100;
    public float sp = 100;
	public float speed = 20;

	//上线值
	public float hp_Max  = 100;
	public float sp_Max = 100;
	public float speed_Max = 50;

    public float exp = 1;
	public float exp_Max = 100;

    public int level = 1;

    public float score = 0;

	public int at_sp = 0;   //每秒钟回复的体力
	public int at_hp = 0;   //每秒钟回复的血量


	private int save_at_sp = 0;   //保存buff前每秒钟回复的体力
	private int save_at_hp = 0;   //保存buff前每秒钟回复的血量
	private float save_speed = 0; //保存buff前速度

	//方向
	public int d = 0;

    //动画控制器    
    private Animator animator;


	//数值表缓存
	ValTableCache valCache;

	public const string m_gameID = "1";

	Dictionary<int, ValRoleBattle> valDict1;
	Dictionary<int, ValNum> valDict2;

	//助攻记录
	public Queue<string> queue_assit= new Queue<string>();

	private int assit_time = 0;

	public bool isSpeed = false;  //加速buffer
	public float speed_buffer_time = 3; //加速buffer时间

	public bool isInvincible = false;  //无敌buffer
	public float invincible_buffer_time = 3; //加速buffer时间

	public bool isNoMove = false;  //无法移动buffer
	public float nomove_buffer_time = 3; //加速buffer时间

	public bool isReHp = false;  //回血buffer
	public float reHp_buffer_time = 3; //加速buffer时间

	public bool isReSp = false;  //回蓝buffer
	public float reSp_buffer_time = 3; //加速buffer时间

    //申请为受保护的方便子类调用
    protected void Awake()
    {
		animator = this.transform.Find("role").GetComponent<Animator>();
		valCache = SavedContext.s_valTableCache;

		//有问题偶然会报错

		if(this.level > 0)
		{
			valDict1 = valCache.getValDictInPageScopeOrThrow<ValRoleBattle>(m_gameID, ConstsVal.val_role_battle);
			ValRoleBattle val = ValUtils.getValByKeyOrThrow (valDict1, this.level);
			SetMaxValue (val.hp, val.spirit, val.speed, val.res);

			valDict2 = valCache.getValDictInPageScopeOrThrow<ValNum>(m_gameID, ConstsVal.val_num);
			ValNum valspeed = ValUtils.getValByKeyOrThrow (valDict2, 28);
			this.speed_Max = valspeed.num; //设置速度上限

			//每秒钟回复体力
			ValNum valat_sp = ValUtils.getValByKeyOrThrow (valDict2, 3);
			at_sp = valat_sp.num;
		}

		AtTime ();
    }



	//没秒钟回复体力
	void AtTime()
	{
		//持续回复
		if (at_sp > 0) {
			if (this.sp + at_sp > sp_Max) {
				this.sp = sp_Max;
			} else {
				this.sp += at_sp;
			}
		} else {
			if (this.sp + at_sp < 0) {
				this.sp = 0;
			}
			else {
				this.sp += at_sp;
			}
		}

		if (at_hp > 0) {
			if (this.hp + at_hp > hp_Max) {
				this.hp = hp_Max;
			}
		} else {
			if (this.hp + at_hp < 0) {
				this.hp = 0;
				Death ();
			}
		}

	
		//计算buffer时间
		//加速状态
		if (isSpeed) {
			speed_buffer_time--;
		}
		if (speed_buffer_time <= 0) {
			isSpeed = false;
			ReState ();
		}
		//无敌状态
		if (isInvincible) {
			invincible_buffer_time--;
		}
		if (invincible_buffer_time <= 0) {
			isInvincible = false;
		}

		//不能移动状态
		if (isNoMove) {
			nomove_buffer_time--;
		}
		if (nomove_buffer_time <= 0) {
			isNoMove = false;
		}

		//回血状态 (不是默认回血速度)
		if (isReHp) {
			reHp_buffer_time--;
		}
		if (reHp_buffer_time <= 0) {
			isReHp = false;
			ReState ();
		}

		//回蓝状态 (不是默认回蓝速度)
		if (isReSp) {
			reSp_buffer_time--;
		} 
		if (reSp_buffer_time <= 0) {
			isReSp = false;
			ReState ();
		}

		//助攻数据
		assit_time++;
		if (assit_time == 6) {
			if(queue_assit.Count > 0)
			queue_assit.Dequeue();	//出队列
			assit_time = 0;
		}

		Invoke ("AtTime", 1.0f);
	}

	void Destroy()
	{
		

	}


	//保存当前状态
	private void SaveState()
	{
		this.save_at_hp = this.at_hp;
		this.save_at_hp = this.at_sp;
		this.save_speed = this.speed;
	}

	//状态结束回到保存的状态
	private void ReState()
	{
		this.at_hp = this.save_at_hp;
		this.at_sp = this.save_at_hp;
		this.speed = this.save_speed;
	}


	//设置数值上线
	public virtual void SetMaxValue(int hp ,int sp ,int speed,int exp)
	{
		this.hp_Max = hp;
		this.sp_Max = sp;
		this.exp_Max = exp;

		this.hp = this.hp_Max;
		this.sp = this.sp_Max;
		this.speed = speed;

	}

	//捡到资源物体
	public virtual void TakeItem(int hp, int sp, int speed , int score, int exp)
    {
        //加血之前判断人物是否死亡
        if(this.hp > 0)
        {
			if(this.hp + hp > hp_Max)
			{
				this.hp = hp_Max;
			}
			else
			{
				this.hp += hp;
			}
     
			if(this.sp + sp > sp_Max)
			{

				this.sp = sp_Max;
			}
			else
			{
				this.sp += sp;
			}

			if (speed != 0) {
				
				TakeBuffer (4,3.0f);
				if (speed > 0) {
					if (this.speed + speed > speed_Max) {
						this.speed = speed_Max;
					} else {
						this.speed += speed;
					}
				} else {
					if (this.speed + speed < 0) {
						this.speed = 10;
					}
					else {
						this.speed += speed;
					}
				}
			}

			//升级
			if (this.exp + exp > exp_Max) {
				level++;
				ValRoleBattle val = ValUtils.getValByKeyOrThrow (valDict1, level);
				SetMaxValue (val.hp, val.spirit, val.speed, val.res);
			} 

			this.exp += exp;
			this.score += score;
        }
   }

     //收到伤害
	public virtual void TakeDamage(float hp,string user, EventController eventController)
    {
		//不是在土遁术状态
		if (this.isInvincible) {
			//造成伤害前判断人物是否死亡
			if (this.hp > 0) {
				if (this.hp + hp > hp_Max) {
					this.hp = hp_Max;
				} else {
					this.hp += hp;
				}
			}
			//造成伤害后没有死亡
			if (this.hp > 0) {
				//造成伤害放入助攻列表
				if (hp < 0 && this.name == SavedData.s_instance.m_user.m_uid) {
					bool isFind = false;
					foreach(string str in queue_assit)
					{
						if(str == user)
						{
							isFind = true;
						}
					}
					if(!isFind)
					queue_assit.Enqueue(user);
				}

			} 
			//造成伤害后死亡
			else {
				if (this.name == SavedData.s_instance.m_user.m_uid) {
					eventController.PlayerDead (queue_assit, user);
				}
				Death ();
			}
		}
    }

	public virtual void Death()
	{
		animator.SetInteger ("state",-1);
		//死亡后掉落的资源比例
		ValNum val = ValUtils.getValByKeyOrThrow (valDict2, 5);
		//死亡后爆装备
		SpawnAwardItem (val.num);

		Invoke ("OnDeathEnd", 1.5f);
		//Remove移除角色
	}


	//
	public virtual void TakeBuffer(int state,float time)
	{
		switch (state) {
		case 1:
			{
				//雪人
				this.isNoMove = true;
				this.nomove_buffer_time = time;

				GameObject stateObj = ResourceMgr.Instance().CreateGameObject("prefabs/heros/state/state_snowman",true);
				stateObj.transform.parent = this.transform;
				stateObj.transform.localPosition = this.transform.localPosition;
				Destroy(stateObj, 3.0f);

				this.transform.Find ("role").gameObject.SetActive (false);
				Invoke ("OnEffectEnd", time);
			}
			break;

		case 2:
			{
				//眩晕
				this.isNoMove = true;
				this.nomove_buffer_time = time;
			}
			break;

		case 3:
			{
				//无敌 (土遁术)
				this.isInvincible = true;
				this.invincible_buffer_time = time;
			}
			break;

		case 4:
			{
				//速度 (改变速度)
				this.isSpeed = true;
				this.speed_buffer_time = time;
				//保存buff前状态
				SaveState ();
			}
			break;

		case 5:
			{
				//回血 (改变回血、负值持续扣血)
				this.isReHp = true;
				this.reSp_buffer_time = time;
				//保存buff前状态
				SaveState ();
			}
			break;

		case 6:
			{
				//回蓝 (改变回血、负值持续扣蓝)
				this.isReSp = true;
				this.reSp_buffer_time = time;

				//保存buff前状态
				SaveState ();
			}
			break;
	
		}
	}



	public bool useFlash(int sp)
	{
		//人物是否死亡
		if (this.hp > 0) {
			//sp不为零
			if (this.sp > 0) {
				this.sp += sp;
				if (this.sp > 0) {
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}


	private void OnEffectEnd()
	{
		this.transform.Find ("role").gameObject.SetActive (true);

		GameObject effectObj = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_14_effect",true);
		effectObj.transform.parent = this.transform;
		effectObj.transform.localPosition = Vector3.zero;
		Destroy(effectObj, 3.0f);
	}


	private void OnDeathEnd()
	{
		this.gameObject.SetActive (false);
	}

    //死亡后爆装备
	void SpawnAwardItem(int val)
    {
		#if false
        //随机生成物品
        double score  = Math.Floor(this.score * 0.2);
        double max = Math.Floor(score / 50);
        double mid = Math.Floor ((score %50)/10);
        double min = Math.Floor ((score %50)%10);

        for(int i =0; i<max;i++)
        {
            int rid = UnityEngine.Random.Range(0,90);
            GameObject.Instantiate(Resources.Load("Item_DualSword"), this.transform.position + Vector3.up, Quaternion.identity);
        }

        for(int i =0; i< mid; i++ )
        {
            int rid = UnityEngine.Random.Range(0,90);
            GameObject.Instantiate(Resources.Load("Item_DualSword"), this.transform.position + Vector3.up, Quaternion.identity);
        }

         for(int i =0; i< min;i++)
        {
            int rid = UnityEngine.Random.Range(0,90);
            GameObject.Instantiate(Resources.Load("Item_DualSword"), this.transform.position + Vector3.up, Quaternion.identity);
        }
		#endif

    }
		

}
