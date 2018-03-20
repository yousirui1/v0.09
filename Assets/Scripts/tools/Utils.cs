using System;
using UnityEngine;
using System.IO;
using ProtoBuf;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

/**************************************
*FileName: Utils.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 公共工具类
**************************************/

namespace tpgm
{
	public class Utils
	{
		//        //#canvs嵌套在canvas中;
		//        public static void canvasNestCanvas(GameObject uiRootLayerGo, Transform uiRootTrans)
		//        {
		//            uiRootLayerGo.transform.SetParent(uiRootTrans);
		//            RectTransform rectTrans = uiRootLayerGo.transform as RectTransform;
		//            {
		//                Vector3 v3 = rectTrans.localScale;
		//                v3.Set(1, 1, 1);
		//                rectTrans.localScale = v3;
		//            }
		//            Utils.setAnchoredPos(rectTrans, 0, 0);
		//            Utils.setPivot(rectTrans, 0, 0);
		//        }


		private Utils()
		{
		}

		public static string uuid()
		{
			return Guid.NewGuid().ToString("N");
		}


		public static void setTag(GameObject gameObj, string tag)
		{
			gameObj.tag = tag;
		}

		public static void setImageSprite(GameObject gameObj, string pathInRes)
		{
			Image img = gameObj.GetComponent<Image>();
			img.sprite = Resources.Load<Sprite>(pathInRes);
		}


		//#将整张大图作为sprite;
		public static Sprite loadSprite(string texPath)
		{
			Sprite sp = Resources.Load<Sprite>(texPath);
			return sp;
		}

		//#大图中的某张小图作为sprite;
		public static Sprite loadSprite(string texPath, string spName)
		{
			//long t1 = Utils.utcNowMs();
			Sprite[] sps = Resources.LoadAll<Sprite>(texPath);
			for (int i = 0, size = sps.Length; i < size; i++)
			{
				Sprite sp = sps[i];
				if (sp.name.Equals(spName))
				{
					//long t2 = Utils.utcNowMs();
					//Log.d<Utils>("loadSprite diffMs: " + (t2 - t1)); 

					return sp;
				}
			}

			return null;
		}


		public static string bytesToReadableUnit(long bytes)
		{
			string[] unitStrs = new string[] {"B", "KB", "MB", "GB", "TB", "PB"};
			int unit = 0;
			float tmpSpeed = bytes;
			while (tmpSpeed >= 1024)
			{
				tmpSpeed /= 1024;
				unit++;
				if (unit >= unitStrs.Length - 1)
				{
					break;
				}
			}

			return String.Format("{0:0.00}{1}", tmpSpeed, unitStrs[unit]);
		} 


		//#获取对象占用的字节数;
		public static long memoryBytes(object obj)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;
				return ms.Length;
			}
		}


		public static T bytesToObject<T>(byte[] bytes)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(bytes, 0, bytes.Length);
				ms.Seek(0, SeekOrigin.Begin);

				return Serializer.Deserialize<T>(ms);
			}
		}

		//检索value
		public static T ensureValue<T>(T t, T defVal)
		{
			if (null == t)
			{
				return defVal;
			}

			return t;
		}

	}
}