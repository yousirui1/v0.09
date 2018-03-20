using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**************************************
*FileName: TextureManage.cs
*User: ysr 
*Data: 2017/12/7
*Describe: 图集处理，使用图集下的小图
**************************************/


//Sprite sprite = TextureManage.getInstance().LoadAtlasSprite("common/game/CommPackAltas","小图名字");  
public class TextureManage : MonoBehaviour {
    //private	static GameObject ToolsObject;
    //public	 GameObject ToolsObject;
    private static TextureManage instance = null;

    public static TextureManage getInstance()
    {
        if (instance == null)
        {
			instance = new GameObject("ToolsObject").AddComponent<TextureManage>();
        }
        return instance;
    }


    private Dictionary<string ,Object[]> AtlasDir; //图集的集合
	
	void Awake()
	{
		initData();
	}

	private void initData()
	{

		//TextureManage.ToolsObject = gameObject;
		AtlasDir = new Dictionary<string, Object[]>();
	}
	
	void Start()
	{
		
	}

	//加载图集上的一个精灵
	public Sprite LoadAtlasSprite(string spriteAtlasPath, string spriteName)
	{
		Sprite sprite = FindSpriteFormBuff(spriteAtlasPath, spriteName);
		if(null == sprite)
		{
			Object[] atlas = Resources.LoadAll(spriteAtlasPath);
            if (null == atlas)
            {
                Debug.Log("图片为空" );
            }
            else
            {
                AtlasDir.Add(spriteAtlasPath, atlas);
                sprite = SpriteFormAtlas(atlas, spriteName);
            }
		}
		return sprite;
	}

	//删除图集缓存
	public void DeleteAtlas(string spriteAtlasPath)
	{
		if(AtlasDir.ContainsKey(spriteAtlasPath))
		{	
			AtlasDir.Remove(spriteAtlasPath);
		}
	}

	//从缓存中查找图集,并找出sprite
	private Sprite FindSpriteFormBuff(string spriteAtlasPath, string spriteName)
	{
		if(AtlasDir.ContainsKey(spriteAtlasPath))
		{
			Object[] atlas = AtlasDir[spriteAtlasPath];
			Sprite sprite = SpriteFormAtlas(atlas, spriteName);
			return sprite;
		}
        else
        {
            //Debug.Log("缓存没找到图集" + spriteAtlasPath);
            return null;
        }

	}

	private Sprite SpriteFormAtlas(Object[] atlas, string spriteName)
	{
		for(int i =0;i <atlas.Length; i++)
		{
			if(atlas[i].GetType() == typeof(UnityEngine.Sprite))
			{
				if(atlas[i].name ==spriteName)
				{
					return (Sprite)atlas[i];
				}
			}
		}
		Debug.Log("图片名"+ spriteName+"在图集找不到");
		return null;
	}
	

}
