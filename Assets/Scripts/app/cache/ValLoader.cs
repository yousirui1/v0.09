using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using LitJson;

/**************************************
*FileName: ValLoader.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 数值表载入
**************************************/

namespace tpgm
{
    public class ValLoader
    {

          //#valFileName => ValList
        Dictionary<string, object> m_dict = new Dictionary<string, object>();
        //#valFileName => ValDict
        Dictionary<string, object> m_dict2 = new Dictionary<string, object>();

        List<Listener> m_listeners = new List<Listener>();
        
        public interface Listener
        {
            void onTableLoadErr(ValLoader sender);
        }

        public class ListenerProxy : Listener
        {
            public void setupListener(Listener l)
            {
                m_listenerRef = l;
            }

            //#MonoBehavior.OnDestroy中调用;
            public void onGameObjectDestory()
            {
                m_listenerRef = null;
            }

            public void onTableLoadErr(ValLoader sender)
            {
                if (null != m_listenerRef)
                {
                    m_listenerRef.onTableLoadErr(sender);
                }
            }

            Listener m_listenerRef;
        }

        //**************************************************
        // ValLoader
        //**************************************************

        public ValLoader()
        {
        }

        //#该数值表是否已加载到内存中;
        public bool isLoaded(string valFileName)
        {
            //string path = SavedContext.getExternalPath("data/" + valFileName);
            return m_dict.ContainsKey(valFileName);
        }

//        //#如果加载失败, 没法告诉外部????
//        public void loadTable<T_Val>(string valFileName) where T_Val : BaseVal
//        {
//            List<T_Val> list;
//            Dictionary<int, T_Val> dict;
//
//            loadTable<T_Val>(valFileName, out list, out dict);
//        }

        //#如果加载失败, 则out的为null;
        void loadTableOrThrow<T_Val>(string valFileName, out List<T_Val> outList, out Dictionary<int, T_Val> outDict) where T_Val : BaseVal
        {
            outList = null;
            outDict = null;

            string path = "";
            try
            {
				path = SavedContext.getExternalPath("data/" + valFileName);
                
				string text = File.ReadAllText(path, Encoding.UTF8);
                
                //List<T_Val> list = SimpleJson.SimpleJson.DeserializeObject<List<T_Val>>(text);
                List<T_Val> list = JsonMapper.ToObject<List<T_Val>>(text);

                outList = list;
                m_dict.Add(valFileName, list);

                Dictionary<int, T_Val> dict = ValUtils.listToDict(list);
                outDict = dict;    
                m_dict2.Add(valFileName, dict);

                return;
            }
            catch (IOException ex)
            {
                //直接显示: 游戏数据损坏, 请重新启动游戏;
                Log.w<ValUtils>(ex.Message);
				Debug.Log("IOException ysr"+ex.Message);
                tellOnTableLoadErr();
            }
            catch (SerializationException ex)
            {
                //直接显示: 游戏数据损坏, 请重新启动游戏;
                Log.w<ValUtils>(ex.Message);
				Debug.Log("SerializationException ysr"+ex.Message);
                tellOnTableLoadErr();
            }
            catch (Exception ex)
            {
				Debug.Log("Exception ysr"+ ex.Message + ", " + ex.GetType().FullName);
                Log.w<ValUtils>(ex.Message + ", " + ex.GetType().FullName);
                tellOnTableLoadErr();
            }

            throw new DataDamageException("valTable load fail: " + valFileName);
        }

        public void unloadTable(string valFileName)
        {
            //string path = SavedContext.getExternalPath("data/" + valFileName);
            m_dict.Remove(valFileName);
            m_dict2.Remove(valFileName);
        }

        //#主要用于预加载的;
        public void loadTableAsync<T_Val>(string valFileName) where T_Val : BaseVal
        {
        }

        //#无法加载数值表就抛出DataDamageException;
        public List<T_Val> loadValListOrThrow<T_Val>(string valFileName) where T_Val : BaseVal
        {
            if (isLoaded(valFileName))
            {
                object list = m_dict[valFileName];
                return (List<T_Val>)list;
            }

            {
                List<T_Val> list;
                Dictionary<int, T_Val> dict;
                loadTableOrThrow<T_Val>(valFileName, out list, out dict);

                //#will not be here;
                //return new List<T_Val>();
                return list;
            }
        }

        public List<T_Val> getValList<T_Val>(string valFileName) where T_Val : BaseVal
        {
            object list = m_dict[valFileName];
            return (List<T_Val>)list;
        }

        //#无法加载数值表就抛出DataDamageException;
        public Dictionary<int, T_Val> loadValDictOrThrow<T_Val>(string valFileName) where T_Val : BaseVal
        {
            if (isLoaded(valFileName))
            {
                object dict = m_dict2[valFileName];
                return (Dictionary<int, T_Val>)dict;
            }

            {
                List<T_Val> list;
                Dictionary<int, T_Val> dict;
                loadTableOrThrow<T_Val>(valFileName, out list, out dict);

                //#will not be here;
                //return new List<T_Val>();
                return dict;
            }
        }

        public Dictionary<int, T_Val> getValDict<T_Val>(string valFileName) where T_Val : BaseVal
        {
            object dict = m_dict2[valFileName];
			return (Dictionary<int, T_Val>)dict;
        }

        //************************************************** 监听器 begin;

        void tellOnTableLoadErr()
        {
            for (int i = 0, size = m_listeners.Count; i < size; i++)
            {
                Listener l = m_listeners[i];
                l.onTableLoadErr(this);
            }
        }

        public void addListener(Listener l)
        {
            m_listeners.Add(l);
        }

        public void removeListener(Listener l)
        {
            m_listeners.Remove(l);
        }

        //************************************************** 监听器 end;

      
    }
}

