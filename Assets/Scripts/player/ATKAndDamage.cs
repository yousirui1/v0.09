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
	public float speed_Max = 10;

    public float exp = 1;
	public float exp_Max = 100;

    public int level = 1;

    public float score = 0;

	public int at_sp = 0;   //每秒钟回复的体力

	//方向
	public int d = 0;

    //动画控制器    
    private Animator animator;
    //死亡音效
    public AudioClip death;

	//数值表缓存
	ValTableCache valCache;

	public const string m_gameID = "1";

	Dictionary<int, ValRoleBattle> valDict1;
	Dictionary<int, ValNum> valDict2;

	//助攻记录
	public Queue<string> queue_assit= new Queue<string>();

	private int assit_time = 0;

	public int state = 0;  //buffer状态
	public float buffer_time = 0;


    //申请为受保护的方便子类调用
    protected void Awake()
    {

		animator = this.transform.Find("role").GetComponent<Animator>();
		valCache = SavedContext.s_valTableCache;

		valDict1 = valCache.getValDictInPageScopeOrThrow<ValRoleBattle>(m_gameID, ConstsVal.val_role_battle);
		ValRoleBattle val = ValUtils.getValByKeyOrThrow (valDict1, level);
		SetMaxValue (val.hp, val.spirit, val.speed, val.res);

		valDict2 = valCache.getValDictInPageScopeOrThrow<ValNum>(m_gameID, ConstsVal.val_num);
		ValNum valspeed = ValUtils.getValByKeyOrThrow (valDict2, 28);
		this.speed_Max = 50;//valspeed.num; //设置速度上限

		//每秒钟回复体力
		ValNum valat_sp = ValUtils.getValByKeyOrThrow (valDict2, 3);
		at_sp = valat_sp.num;

		AtTime ();
    }

	//没秒钟回复体力
	void AtTime()
	{
		if(this.sp + at_sp > sp_Max)
		{
			this.sp = sp_Max;
		}
		else
		{
			this.sp += at_sp;
		}
		if (state != 0) {
			buffer_time--;
		}

		if (buffer_time <= 0) {
			state = 0;
		}

		//助攻数据
		assit_time++;
		if (assit_time == 6) {
			if(queue_assit.Count >0)
			queue_assit.Dequeue();	//出队列
			assit_time = 0;
		}

		Invoke ("AtTime", 1.0f);
	}

	void Destroy()
	{
		

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
				
			if(this.speed + speed > speed_Max)
			{
				this.speed = speed_Max;
				//Invoke ();
			}
			else
			{
				this.speed += speed;
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
		if (this.state != 3) {
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
					animator.SetInteger ("state",-1);
				}
			
            
				//死亡后掉落的资源比例
				ValNum val = ValUtils.getValByKeyOrThrow (valDict2, 5);
				//死亡后爆装备
				SpawnAwardItem (val.num);

				Invoke ("OnDeathEnd", 1.5f);
				//Remove移除角色

			}
		}
    }


	//
	public virtual void TakeBuffer(int state,float time)
	{
		switch (state) {
		case 1:
			{
				//雪人
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
				this.state = 2;
				this.buffer_time = time;
			}
			break;

		case 3:
			{
				Debug.Log ("土遁术");
				//土遁术
				this.state = 3;
				this.buffer_time = time;
			}
			break;

		case 4:
			{
				Debug.Log ("禁止移动");
				SaveState ();
	
			}
			break;
		}
	}

	//保存当前状态
	private void SaveState()
	{


	}

	//状态结束回到保存的状态
	private void ReState()
	{


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
