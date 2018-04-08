using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace tpgm
{
    //#用于Val部分的一些工具函数(公共代码);
    public class ValUtils
    {
		
        //#会抛出DataCorruptException;
        public static T getValByKeyOrThrow<T>(Dictionary<int, T> dict, int idKey)
        {
            T val;
            if (dict.TryGetValue(idKey, out val))
            {
                return val;
            }

            //Toast.create("游戏数据异常, 请重新启动游戏");
            throw new DataCorruptException("id not found in table " + idKey);
        }

        //#注意: 是通过sid来查找ValGlobal, 而不是通过gsid来获得ValGlobal;
        public static ValGlobal getGValBySidOrThrow(List<ValGlobal> gvalList, int sid)
        {
            for (int i = 0, size = gvalList.Count; i < size; i++)
            {
                ValGlobal gval = gvalList[i];

                if (gval.sid == sid)
                {
                    return gval;
                }
            }

            throw new DataCorruptException("sid not found in val_global: " + sid);
        }

//        public static void loadVal<T_Val>(string valFileName, out List<T_Val> pList) where T_Val : BaseVal
//        {
//            pList = null;
//            try
//            {
//                string path = SavedContext.getExternalPath("data/" + valFileName);
//                //string path = SavedContext.getExternalPath("data/val_signIn_15.json");
//                string text = File.ReadAllText(path, Encoding.UTF8);
//                //Log.d<ValUpdateLayer>("json: " + text);
//
//                List<T_Val> list = SimpleJson.SimpleJson.DeserializeObject<List<T_Val>>(text);
//                pList = list;
//
//                return;
//            }
//            catch (IOException ex)
//            {
//                //直接显示: 游戏数据损坏, 请重新启动游戏;
//                Log.w<ValUtils>(ex.Message);
//            }
//            catch (SerializationException ex)
//            {
//                //直接显示: 游戏数据损坏, 请重新启动游戏;
//                Log.w<ValUtils>(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                Log.w<ValUtils>(ex.Message + ", " + ex.GetType().FullName);
//            }
//
//            Toast.create("游戏数据损坏");
//            throw new DataDamageException("val read err: " + valFileName);
//        }

        public static Dictionary<int, T> listToDict<T>(List<T> list) where T : BaseVal
        {
            Dictionary<int, T> dict = new Dictionary<int, T>();

            foreach (T item in list)
            {
                if (dict.ContainsKey(item.id))
                {
                    //warn: will not be here;
                }
                else
                {
                    dict[item.id] = item;
                }
            }

            return dict;
        }

//        public static Dictionary<int, int> parseSidNumDictOrThrow(string toParseText)
//        {
//            //10001:10;10002:5;10003:2
//            Dictionary<int, int> dict = new Dictionary<int, int>();
//
//            int idx1 = 0;
//            while (true)
//            {
//                int idx2 = toParseText.IndexOf(';', idx1);
//                if (-1 == idx2)
//                {
//                    break;
//                }
//
//                int count = idx2 - idx1;
//                string pairStr = toParseText.Substring(idx1, count);
//                int idx3 = pairStr.IndexOf(':');
//                if (-1 != idx3)
//                {
//                    string sidStr = pairStr.Substring(0, idx3);
//                    string numStr = pairStr.Substring(idx3 + 1, pairStr.Length - (idx3 + 1));
//
//                    try
//                    {
//                        int sid = int.Parse(sidStr);
//                        int num = int.Parse(numStr);
//
//                        if (dict.ContainsKey(sid))
//                        {
//                            throw new DataInvalidException(toParseText + " contains exists key: " + sidStr);
//                        }
//
//                        dict[sid] = num;
//                    }
//                    catch (FormatException ex)
//                    {
//                        throw new DataInvalidException(pairStr + " can't parse int");
//                    }
//                }
//                else
//                {
//                    //数据格式不对;
//                    throw new DataInvalidException(pairStr + " contains no pair");
//                }
//
//                idx1 = idx2;
//            }
//
//            return dict;
//        }

//        /// <summary>
//        /// 会抛出 DataCorruptException
//        /// </summary>
//        public static List<SidNumPair> parseSidNumPairsOrThrow(string toParseText)
//        {
//            //10001:10;10002:5;10003:2
//            List<SidNumPair> list = new List<SidNumPair>();
//            HashSet<int> sidSet = new HashSet<int>();
//
//            int idx1 = 0;
//            while (true)
//            {
//                bool onlyOnePair = false;
//                int idx2 = toParseText.IndexOf(';', idx1);
//                if (-1 == idx2)
//                {
//                    if (0 == idx1)
//                    {
//                        idx2 = toParseText.Length;
//                        onlyOnePair = true;
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }
//
//                int count = idx2 - idx1;
//                string pairStr = toParseText.Substring(idx1, count);
//                int idx3 = pairStr.IndexOf(':');
//                if (-1 != idx3)
//                {
//                    string sidStr = pairStr.Substring(0, idx3);
//                    string numStr = pairStr.Substring(idx3 + 1, pairStr.Length - (idx3 + 1));
//
//                    try
//                    {
//                        int sid = int.Parse(sidStr);
//                        int num = int.Parse(numStr);
//
//                        if (sidSet.Contains(sid))
//                        {
//                            //id不允许重复;
//                            throw new DataCorruptException(toParseText + " contains exists key: " + sidStr);
//                        }
//                        else
//                        {
//                            sidSet.Add(sid);
//                        }
//
//                        list.Add(new SidNumPair(sid, num));
//
//                        if (onlyOnePair)
//                        {
//                            break;
//                        }
//                    }
//                    catch (FormatException ex)
//                    {
//                        throw new DataCorruptException(pairStr + " can't parse int");
//                    }
//                }
//                else
//                {
//                    //数据格式不对;
//                    throw new DataCorruptException(pairStr + " contains no pair");
//                }
//
//                idx1 = idx2 + 1;
//            }
//
//            if (list.Count <= 0)
//            {
//                throw new DataCorruptException("no sid num pair found: " + toParseText);
//            }
//
//            return list;
//        }

		#if false
        public static GsidNumPair parseGsidNumPairOrThrow(string pairStr)
        {
            int gsid;
            int num;
            parsePair(null, pairStr, out gsid, out num);
            return new GsidNumPair(gsid, num);
        }

        //#用例:
        //(#)只有一个的时候;
        //(#)多个的时候不会出现少一个的情况;
        /// <summary>
        /// 会抛出 DataCorruptException
        /// </summary>
        public static List<GsidNumPair> parseGsidNumPairsOrThrow(string toParseText)
        {
            //10001:10;10002:5;10003:2
            List<GsidNumPair> list = new List<GsidNumPair>();
            HashSet<int> sidSet = new HashSet<int>();

            int idx1 = 0;
            while (true)
            {
                int idx2 = toParseText.IndexOf(';', idx1);
                if (-1 == idx2)
                {
                    string pairStr = toParseText.Substring(idx1);

                    int gsid;
                    int num;
                    parsePair(sidSet, pairStr, out gsid, out num);
                    list.Add(new GsidNumPair(gsid, num));
                    break;
                }

                {
                    int count = idx2 - idx1;
                    string pairStr = toParseText.Substring(idx1, count);

                    int gsid;
                    int num;
                    parsePair(sidSet, pairStr, out gsid, out num);
                    list.Add(new GsidNumPair(gsid, num));
                }

                idx1 = idx2 + 1;
            }

            if (list.Count <= 0)
            {
                throw new DataCorruptException("no sid num pair found: " + toParseText);
            }

            return list;
        }

        /// <summary>
        /// 会抛出 DataCorruptException
        /// </summary>
        public static List<FishGroupWeight> FishGroupWeightOrThrow(string toParseText)
        {
            //10001:10;10002:5;10003:2
            List<FishGroupWeight> list = new List<FishGroupWeight>();
            HashSet<int> sidSet = new HashSet<int>();

            int idx1 = 0;
            while (true)
            {
                bool onlyOnePair = false;
                int idx2 = toParseText.IndexOf(';', idx1);
                if (-1 == idx2)
                {
                    string pairStr = toParseText.Substring(idx1);

                    int gsid;
                    int num;
                    parsePair(sidSet, pairStr, out gsid, out num);
                    list.Add(new FishGroupWeight(gsid, num));
                    break;
                }

                {
                    int count = idx2 - idx1;
                    string pairStr = toParseText.Substring(idx1, count);

                    int gsid;
                    int num;
                    parsePair(sidSet, pairStr, out gsid, out num);
                    list.Add(new FishGroupWeight(gsid, num));
                }

                idx1 = idx2 + 1;
            }

            if (list.Count <= 0)
            {
                throw new DataCorruptException("no sid num pair found: " + toParseText);
            }

            return list;
        }
		#endif

        static void parsePair(HashSet<int> checkRepeatSidOrNull, string pairStr, out int key, out int val)
        {
            int idx3 = pairStr.IndexOf(':');
            if (-1 != idx3)
            {
                string gsidStr = pairStr.Substring(0, idx3);
                string numStr = pairStr.Substring(idx3 + 1, pairStr.Length - (idx3 + 1));

                try
                {
                    int gsid = int.Parse(gsidStr);
                    int num = int.Parse(numStr);

                    if (null != checkRepeatSidOrNull)
                    {
                        if (checkRepeatSidOrNull.Contains(gsid))
                        {
                            //id不允许重复;
                            throw new DataCorruptException("sid repeated: " + gsidStr);
                        }
                        else
                        {
                            checkRepeatSidOrNull.Add(gsid);
                        }
                    }

                    key = gsid;
                    val = num;
                }
                catch (FormatException ex)
                {
                    throw new DataCorruptException(pairStr + " can't parse int");
                }
            }
            else
            {
                //数据格式不对;
                throw new DataCorruptException(pairStr + " contains no pair");
            }
        }

        private ValUtils()
        {
        }

    }
}

