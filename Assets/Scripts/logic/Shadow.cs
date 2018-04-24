using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tpgm;


/**************************************
*FileName: Shadow.cs
*User: ysr 
*Data: 2017/12/19
*Describe: 影子同步算法(DR)
**************************************/

public class Shadow  {
    //移动方向
    private int[,] direction = new int[9, 2] { {0,0 },{0, -1},{1, -1},{1, 0},{1, 1},
    {0, 1},{-1, 1},{-1, 0},{-1, -1}};

    //影子移动
    public void shadow_move(PlayerVal player_shadow, int step)
    {

        //Debug.Log("shadow_move step" + step);
        int inc = 1;

        if (step < 0)
        {
            step = -step;
            inc = -1;
        }

        for (int i = 0; i < step; i++)
        {
            player_shadow.sx += player_shadow.sv * direction[player_shadow.sd, 0] * inc;
            player_shadow.sy += player_shadow.sv * direction[player_shadow.sd, 1] * inc;

            //三个数取中间值 限定移动范围
            player_shadow.sx = middle(80, player_shadow.sx, 6660);
            player_shadow.sy = middle(128, player_shadow.sy, 6660);
        }



    }


    //飞机移动
    public void craft_move(PlayerVal player_craft, int step)
    {
        int inc = 1;

        if (step < 0)
        {
            step = -step;
            inc = -1;
        }

        for (int i = 0; i < step; i++)
        {
            player_craft.x += player_craft.v * direction[player_craft.d, 1] * inc;
            player_craft.y += player_craft.v * direction[player_craft.d, 0] * inc;

			//player_craft.x += player_craft.v * player_craft.dx * Time.fixedDeltaTime;
			//player_craft.y += player_craft.v * player_craft.dy * Time.fixedDeltaTime;


            //三个数取中间值 限定移动范围
            player_craft.x = middle(80, player_craft.x, 6660);
            player_craft.y = middle(128, player_craft.y, 6660);
			
        }
       
    }

	//飞机移动
	public void craft_flash(PlayerVal player_craft, int step)
	{
		int inc = 1;

		if (step < 0)
		{
			step = -step;
			inc = -1;
		}

		for (int i = 0; i < step; i++)
		{

			if(player_craft.d != 0)
			{
				player_craft.x += 300 * direction[player_craft.d, 1] * inc;
				player_craft.y += 300 * direction[player_craft.d, 0] * inc;
			}
			else
			{
				player_craft.x += 300 * direction[player_craft.old_d, 1] * inc;
				player_craft.y += 300 * direction[player_craft.old_d, 0] * inc;
			}

			//三个数取中间值 限定移动范围
			player_craft.x = middle(80, player_craft.x, 6660);
			player_craft.y = middle(128, player_craft.y, 6660);
		}

	}



    public void shadow_refresh(PlayerVal player_shadow )
    {

		player_shadow.sx = player_shadow.x;
        player_shadow.sy = player_shadow.y;
        player_shadow.sd = player_shadow.d;
        player_shadow.sv = player_shadow.v;
        //adjust(player_shadow, curframe, oldframe);

    }

    //插值平滑
    void adjust(PlayerVal buf, double curframe, double oldframe)
    {
       
        Debug.Log("frame"+ (int)(curframe - oldframe - 1));
        //根据时间差计算帧数
        shadow_move(buf, 1);
    }

    /*
    //时间到了执行
    Player_Entite ontimer()
      {
          Debug.Log("ontimer");
       // GameObject playerObject = GameObject.Find(ConectData.Instance.UserName);
        //Player_Entite entite = new Player_Entite();
        //entite.x = playerObject.transform.position.x;
        //entite.y = playerObject.transform.position.x;
        //Check_phase();
        //return entite;
      }*/

    // 跟随方式1：同步跟随  延迟大的时候使用
    void trace1(PlayerVal entite, int step)
    {

        for (int i = 0; i < step; i++)
        {
            if (entite.x < entite.sx)
            {
                entite.x += min(entite.sx - entite.x, entite.v * 2);
            }
            else if (entite.x > entite.sx)
            {
                entite.x -= min(entite.x - entite.sx, entite.v * 2);
            }
            if (entite.y < entite.sy)
            {
                entite.y += min(entite.sy - entite.y, entite.v * 2);
            }
            else if (entite.y > entite.sy)
            {
                entite.y -= min(entite.y - entite.sy, entite.v * 2);
            }
        }


      

    }

    //跟随方式2：相位滞后  延迟较小的时候使用
    void trace2(PlayerVal entite, int step)
    {

        entite.v = entite.sv;

        int inc = 1;

        if (step < 0)
        {
            step = -step;
            inc = -1;
        }


        for (int i = 0; i < step; i++)
        {
            entite.x = newPos(entite.x, entite.sx, entite.v, entite.sv);
            entite.y = newPos(entite.y, entite.sy, entite.v, entite.sv);
        }
	
    }


    //跟随方式判断
    public void trace(int mode ,PlayerVal entite, int step)
    {
       switch(mode)
       {
           case 1:
            trace1(entite, step);
            break;
           case 2:
            trace1(entite, step);
            break;
            default :
            trace1(entite, step);
            break;

       } 
    }

    //计算最新的位置
    int newPos(int x, int sx, int v, int sv)
    {
        int d1, d2;
        if (x == sx)
            return x;
        if (x < sx)
        {
            d1 = min(sx - x, 2 * v);
            d2 = min(sx - x, sv);
            if (sx - x > sv * 35)
            {
                x += d1;
            }
            else
            {
                x += d2;
            }
        }
        else if (x > sx)
        {
            d1 = min(x - sx, v * 2);
            d2 = min(x - sx, sv);
            if (x - sx > sv * 35)
            {
                x -= d1;
            }
            else
            {
                x -= d2;
            }
        }
        return x;

    }

    //三个数取中间数
    int middle(int a, int b, int c)
    {
        int max = a > b ? a : b;
        max = max > c ? max : c;
        int min = a < b ? a : b;
        min = min < c ? min : c;
        int sum = a + b + c;
        int mid = sum - max - min;
        return mid;
    }
    //最小数
    int min(int a, int b)
    {
        if (a < b)
            return a;
        else
            return b;
    }


}
