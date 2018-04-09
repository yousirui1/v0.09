using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System;
using System.Threading;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using SimpleJson;
using tpgm;
//#也可以叫RequestState
using ProtoBuf;

namespace tpgm
{




    public class DataNeedOnResponse
    {
        public string m_url = "";

        //#哪个接口请求的;
        public int m_reqTag;

        //#接口响应时, 通过该tag来查询关联数据的;
        public string m_userDataTag = "";

        //#用于唯一标识一次请求, 防止表单重复提交;
        public string m_checkID = "";

        public int m_dataType = 0;
    }

    public class ResponseData
    {
        //#0时返回protobuf数据; 1时返回json数据;
		public int m_dataType = 0;

        public byte[] m_protobufBytes;

        public string m_json = "";
    }


    //#在WebRequest上做一层简单封装, 使得一般情况下更易于使用;
    public class NetHttp
    {
		private const string TAG = "NetHttp";

		private const int MSG_HTTP_OK = 1;
		private const int MSG_HTTP_ERR = 2;
		private const int MSG_NET_ERR = 3;

		const string ENCRYPT_KEY = "0123456789abcd0123456789";
		const bool ENCRTYP_ENABLE = true;

		private INetCallback m_callbackOrNull;

		private MainLooper m_initedLooper;


        public NetHttp()
        {
            m_initedLooper = MainLooper.instance();
            if (null == m_initedLooper)
            {
                throw new NullReferenceException("MainLooper not inited");
            }
        }

        public NetHttp(MainLooper looper)
        {
            if (null == looper)
            {
                throw new NullReferenceException("looper is null");
            }

            m_initedLooper = looper;
        }

        class MsgData
        {
            public DataNeedOnResponse m_dataNeeded;
            public string m_base64BodyContent = "";
            public int m_statusCode;
        }

        public interface INetCallback
        {
            void onHttpOk(DataNeedOnResponse data, ResponseData respData);

            void onHttpErr(DataNeedOnResponse data, int statusCode, string errMsg);

            void onOtherErr(DataNeedOnResponse data, int type);
        }

        //#reqTag: 用于标识这是哪个接口的;
        public void getAsync(string url, int reqTag, string checkID)
        {
            DataNeedOnResponse data = new DataNeedOnResponse();
            data.m_url = url;
            data.m_reqTag = reqTag;
            data.m_checkID = checkID;
            ThreadPool.QueueUserWorkItem(doGetAsync, data);
        }

        void doGetAsync(object state)
        {
            DataNeedOnResponse data = (DataNeedOnResponse)state;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(data.m_url);
            //req.Method = "GET";
            //req.IfModifiedSince = PrefUpdate.getIfModifiedSince();
        
            //WebHeaderCollection headers = req.Headers;
            //headers.Add("ETag: " + PrefUpdate.getETag());

            resp(data, req);
        }


        //    public void postJsonObjectAsync(string url, JsonObject jsonObject, int reqTag)
        //    {
        //        string reqID = "aaa";
        //        jsonObject["reqID"] = reqID;
        //
        //        m_requestStack.Add(reqID, jsonObject);
        //
        //        DataNeedOnResponse data = new DataNeedOnResponse();
        //        data.m_url = url;
        //        data.m_reqTag = reqTag;
        //        data.m_reqID = reqID;
        //
        //        AsyncState state = new AsyncState();
        //        state.m_data = data;
        //        state.m_jsonBody = jsonBody;
        //
        //        ThreadPool.QueueUserWorkItem(doPostJsonBodyAsync, state);
        //    }

        //#或叫AsyncParamData;
        class AsyncState
        {
            public DataNeedOnResponse m_data;
            public Dictionary<string, string> m_pairs;
            public string m_paramsVal;
        }

		//json数据传输
        public void postParamsJsonAsync(string url, JsonObject jsonObj, int reqTag, string checkID)
        {

            DataNeedOnResponse data = new DataNeedOnResponse();
            data.m_url = url;
            data.m_reqTag = reqTag;
            data.m_checkID = checkID;
            data.m_dataType = 1;

            //#这个可以做成Strategy模式;
            string json = jsonObj.ToString();
            string encryptedParamsVal;
            if (ENCRTYP_ENABLE)
            {
                encryptedParamsVal = CryptUtils.Encrypt3DesStr(ENCRYPT_KEY, json);
            }
            else
            {
                encryptedParamsVal = json;
            }

            AsyncState state = new AsyncState();
            state.m_data = data;
            state.m_paramsVal = encryptedParamsVal;

            ThreadPool.QueueUserWorkItem(doPostParamsValAsync, state);
        }

		//buffer数据传输
        public void postParamsValAsync<T>(string url, T paramsVal, int reqTag, string checkID)
        {
            DataNeedOnResponse data = new DataNeedOnResponse();
            data.m_url = url;
            data.m_reqTag = reqTag;
            data.m_checkID = checkID;
            data.m_dataType = 0;

            //#这个可以做成Strategy模式;
            string encryptedParamsVal;
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, paramsVal);

                byte[] bytes = ms.ToArray();
				//Debug.Log ("" + BitConverter.ToString (bytes));

                if (ENCRTYP_ENABLE)
                {
					encryptedParamsVal =  CryptUtils.Encrypt3DesStr(ENCRYPT_KEY, bytes);
                }
                else
                {
                    encryptedParamsVal = Convert.ToBase64String(bytes);
                }
            }

            AsyncState state = new AsyncState();
            state.m_data = data;
            state.m_paramsVal = encryptedParamsVal;

			//数据保存在字典里面
            m_requestDict[checkID] = paramsVal;

            ThreadPool.QueueUserWorkItem(doPostParamsValAsync, state);
        }

        void doPostParamsValAsync(object objState)
        {
            AsyncState state = (AsyncState)objState;
            DataNeedOnResponse data = state.m_data;
            bool doReq = true;
            switch (m_mockType)
            {
            case 1:
            {
                if (m_mockErr)
                {
                    doReq = false;

                    Thread.Sleep(2000);

					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_HTTP_OK);

                    MsgData dataObj = new MsgData();
                    dataObj.m_base64BodyContent = "";
                    dataObj.m_dataNeeded = data;
                    msg.m_dataObj = dataObj;
                    msg.m_handler = handleMessage;
                    m_initedLooper.sendMessage(msg);
                }

                m_mockErr = !m_mockErr;
            }
            break;

            case 2:
            {
                if (m_mockErr)
                {
                    doReq = false;

                    Thread.Sleep(2000);

					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_NET_ERR);

                    MsgData dataObj = new MsgData();
                    dataObj.m_dataNeeded = data;
                    msg.m_dataObj = dataObj;
                    m_initedLooper.sendMessage(msg);
                }

                m_mockErr = !m_mockErr;
            }
            break;

            }

            if (doReq)
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(data.m_url);

                StringBuilder sb = new StringBuilder();
                //sb.Append("params='");
				sb.Append("params=");
                sb.Append(WWW.EscapeURL(state.m_paramsVal));
                //sb.Append("'");
                string body = sb.ToString();

                byte[] postBodyBytes = Encoding.UTF8.GetBytes(body);


                req.Method = "POST"; 
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postBodyBytes.Length; //如果ContentLength不需要提前计算好, 那可以在获取到GetRequestStream后直接写到这个流中;

                //req.AllowAutoRedirect = false;
                //req.ContentType = "application/x-www-form-urlencoded;charset=gbk";
                //req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11";

                try
                {
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(postBodyBytes, 0, postBodyBytes.Length);

                        reqStream.Flush();
                    }

                    resp(data, req);
                }
                catch (WebException ex)
                {
                    Log.w<NetHttp>(ex.Message);

                    //#网络连接错误;
					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_NET_ERR);

                    MsgData dataObj = new MsgData();
                    dataObj.m_dataNeeded = data;
                    msg.m_dataObj = dataObj;
                    //msg.m_handler = handleMessage;
                    m_initedLooper.sendMessage(msg);
                }
            }
        }

        //    void doPostFormDataAsync(object objState)
        //    {
        //        AsyncState state = (AsyncState)objState;
        //        DataNeedOnResponse data = state.m_data;
        //        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(data.m_url);
        //
        //        string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
        //
        //        byte[] postBytes = createMultipartPostData(boundary, state.m_pairs);
        //
        //        req.Method = "POST";
        //        req.ContentType = "multipart/form-data; boundary=" + boundary;
        //        req.ContentLength = postBytes.Length; //如果ContentLength不需要提前计算好, 那可以在获取到GetRequestStream后直接写到这个流中;
        //
        //        try
        //        {
        //            using (Stream newStream = req.GetRequestStream())
        //            {
        //                newStream.Write(postBytes, 0, postBytes.Length);
        //            }
        //
        //            resp(data, req);
        //        }
        //        catch (WebException ex)
        //        {
        //            //#网络连接错误;
        //            Message msg = MainLooper.obtainMessage(MSG_NET_ERR);
        //
        //            MsgData dataObj = new MsgData();
        //            dataObj.m_dataNeeded = data;
        //            msg.m_dataObj = dataObj;
        //            m_initedLooper.sendMessage(msg);
        //        }
        //    }

        // body中写入这些:
        //    Content-Type: multipart/form-data; boundary=AaB03x
        //
        //        --AaB03x
        //        Content-Disposition: form-data; name="submit-name"
        //
        //        Larry
        //        --AaB03x
        //        Content-Disposition: form-data; name="file"; filename="file1.dat"
        //        Content-Type: application/octet-stream
        //
        //        ... contents of file1.dat ...
        //        --AaB03x--

        //    static byte[] createMultipartPostData(string boundary, Dictionary<string, string> pairs)
        //    {
        //        StringBuilder postBodyData = new StringBuilder();
        //
        //        //append access token
        //        postBodyData.AppendLine("--" + boundary);
        //        postBodyData.Append(Environment.NewLine);
        //
        //        //append form part
        //        if (null != pairs && pairs.Count > 0)
        //        {
        //            foreach (KeyValuePair<string, string> pair in pairs)
        //            {
        //                postBodyData.AppendLine("--" + boundary);
        //                postBodyData.AppendLine(string.Format("Content-Disposition: form-data;name=\"{0}\"", pair.Key));
        //                postBodyData.Append(Environment.NewLine);
        //                postBodyData.AppendLine(pair.Value);
        //            }
        //        }
        //
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            BinaryWriter bw = new BinaryWriter(ms);
        //            bw.Write(Encoding.UTF8.GetBytes(postBodyData.ToString()));
        //
        //            bw.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
        //            bw.Write(Encoding.UTF8.GetBytes("--" + boundary)); //结束符;
        //            ms.Flush();
        //            ms.Position = 0;
        //
        //            byte[] result = ms.ToArray();
        //            return result;
        //        }
        //    }

        //    static byte[] createXWWWFormUrlencoded(Dictionary<string, string> pairs)
        //    {
        //        StringBuilder postBodyData = new StringBuilder();
        //
        //        if (null != pairs && pairs.Count > 0)
        //        {
        //            foreach (KeyValuePair<string, string> pair in pairs)
        //            {
        //                postBodyData.Append(pair.Key).Append('=').Append(pair.Value);
        //                postBodyData.Append('&');
        //            }
        //
        //            postBodyData.Length = postBodyData.Length - 1; //最后面多一个&的;
        //            return Encoding.UTF8.GetBytes(postBodyData.ToString());
        //        }
        //
        //        return new byte[0];
        //    }

        static void dumpReqHeaders(WebRequest req)
        {
            Log.d<NetHttp>("req headers ==== begin");
            WebHeaderCollection headers = req.Headers;
//        for (int i = 0; i < headers.Count; i++)
//        {
//            Log.d<NetHttp>(headers.GetKey(i) + " : " + headers.Get(i));
//        }

            foreach (string name in headers)
            {
                Log.d<NetHttp>(name + ": " + headers.Get(name));
            }

            Log.d<NetHttp>("req headers ==== end");
        }

        static void dumpRespHeaders(WebResponse resp)
        {
            Log.d<NetHttp>("resp headers ==== begin");

            WebHeaderCollection headers = resp.Headers;

            foreach (string name in headers)
            {
                Log.d<NetHttp>(name + ": " + headers.Get(name));
            }

            Log.d<NetHttp>("resp headers ==== end");
        }

        void resp(DataNeedOnResponse data, HttpWebRequest req)
        {
            //dumpReqHeaders(req);

            Log.d<NetHttp>("resp: " + req.RequestUri.ToString() + ", " + data.m_checkID);
            try
            {
                HttpWebResponse resp = null;
                resp = (HttpWebResponse)req.GetResponse();
                //dumpRespHeaders(resp);

                switch (resp.StatusCode)
                {
                case HttpStatusCode.OK:
                {
                    Stream stream = resp.GetResponseStream();

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        //#将流读取为string;
                        string base64BodyContent = sr.ReadToEnd();
                        base64BodyContent = WWW.UnEscapeURL(base64BodyContent);
                        base64BodyContent = processBase64BodyContent(base64BodyContent);

                        //#解析json;

                        //#通知ok;
						HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_HTTP_OK);

                        MsgData dataObj = new MsgData();
                        dataObj.m_base64BodyContent = base64BodyContent;
                        dataObj.m_dataNeeded = data;
                        msg.m_dataObj = dataObj;
                        //msg.m_handler = handleMessage;
                        m_initedLooper.sendMessage(msg);
                    }
                }
                break;

                default:
                {
					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_HTTP_ERR);

                    MsgData dataObj = new MsgData();
                    dataObj.m_dataNeeded = data;
                    dataObj.m_statusCode = Convert.ToInt32(resp.StatusCode);
                    msg.m_dataObj = dataObj;
                    //msg.m_handler = handleMessage;
                    m_initedLooper.sendMessage(msg);
                }
                break;

                }
            }
            catch (WebException ex)
            {
                Log.w<NetHttp>(ex.Message);

                //#网络连接错误;
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_NET_ERR);

                MsgData dataObj = new MsgData();
                dataObj.m_dataNeeded = data;
                msg.m_dataObj = dataObj;
                //msg.m_handler = handleMessage;
                m_initedLooper.sendMessage(msg);
            }
            catch (Exception ex)
            {
                Log.w<NetHttp>(ex.GetType().FullName + ": " + ex.Message);
                //will not he here;
            }
        }

        static string processBase64BodyContent(string base64BodyContent)
        {
            if (base64BodyContent.StartsWith("\"") && base64BodyContent.EndsWith("\""))
            {
                base64BodyContent = base64BodyContent.Substring(1, base64BodyContent.Length - 2);
            }

            return base64BodyContent;
        }

		public void handleMessage(HandlerMessage msg)
        {
            Log.d<NetHttp>("handleMessage");
            switch (msg.m_what)
            {
            case MSG_HTTP_OK:
            {
                MsgData dataObj = (MsgData)msg.m_dataObj;
                DataNeedOnResponse dataNeedOnResp = dataObj.m_dataNeeded;

                //这个也可以写成Strategy模式;
                ResponseData resp = new ResponseData();
                resp.m_dataType = dataNeedOnResp.m_dataType;

					Log.d<NetHttp>("base64Body: " + dataObj.m_base64BodyContent+"type"+dataNeedOnResp.m_dataType);

                try
                {
 					//处理不同数据返回类型分json 和buffer
                    switch (dataNeedOnResp.m_dataType)
                    {

                    case 0:
                    {
                        byte[] bytes;
                        if (ENCRTYP_ENABLE)
                        {
                            bytes = CryptUtils.Decrypt3DesBytes(ENCRYPT_KEY, dataObj.m_base64BodyContent);
                        }
                        else
                        {
                            bytes = Convert.FromBase64String(dataObj.m_base64BodyContent);
                        }
                        resp.m_protobufBytes = bytes;
                    }
                    break;

                    case 1:
                    {
                        string json;
                        if (ENCRTYP_ENABLE)
                        {
                            json = CryptUtils.Decrypt3DesStr(ENCRYPT_KEY, dataObj.m_base64BodyContent);
                        }
                        else
                        {
							//将指定的字符串（它将二进制数据编码为 Base64 数字）转换为等效的 8 位无符号整数数组。 失败!!
                            //json = Encoding.UTF8.GetString(Convert.FromBase64String(dataObj.m_base64BodyContent));
									json = dataObj.m_base64BodyContent.ToString();
                        }
                        resp.m_json = json;
                    }
                    break;
                    }

                    m_requestDict.Remove(dataNeedOnResp.m_checkID);
                    tellHttpOk(dataObj.m_dataNeeded, resp);
                }
                catch (FormatException ex)
                {
                    //#服务端返回的数据格式异常;
					Debug.Log (TAG + ":" + "format ex: " + ex.Message);
                    m_failRequestStack.Push(dataNeedOnResp.m_checkID);

                    tellOtherErr(dataObj.m_dataNeeded, 1);
                }
            }
            break;

            case MSG_HTTP_ERR:
            {

				Debug.Log (TAG + ":" + "err");
                MsgData dataObj = (MsgData)msg.m_dataObj;
                DataNeedOnResponse dataNeedOnResp = dataObj.m_dataNeeded;

                m_failRequestStack.Push(dataNeedOnResp.m_checkID);

                tellHttpErr(dataObj.m_dataNeeded, dataObj.m_statusCode);
            }
            break;

            case MSG_NET_ERR:
            {
				Debug.Log (TAG + ":" + "MSG_NET_ERR");
                MsgData dataObj = (MsgData)msg.m_dataObj;
                DataNeedOnResponse dataNeedOnResp = dataObj.m_dataNeeded;

                m_failRequestStack.Push(dataNeedOnResp.m_checkID);

                tellOtherErr(dataObj.m_dataNeeded, 0);
            }
            break;

            }
        }

        void tellHttpOk(DataNeedOnResponse data, ResponseData resp)
        {
        
            if (null != m_callbackOrNull)
            {
                m_callbackOrNull.onHttpOk(data, resp);
            }
        }

        void tellHttpErr(DataNeedOnResponse data, int statusCode)
        {
          
            if (null != m_callbackOrNull)
            {
                m_callbackOrNull.onHttpErr(data, statusCode, "");
            }
        }

        void tellOtherErr(DataNeedOnResponse data, int type)
        {
            
            if (null != m_callbackOrNull)
            {
                m_callbackOrNull.onOtherErr(data, type);
            }
        }

        //#每个页面都使用自己的NetHttp;
        public void setPageNetCallback(INetCallback cb)
        {
            m_callbackOrNull = cb;
        }

      

        //    private Dictionary<string, JsonObject> m_requestStack = new Dictionary<string, JsonObject>();

        //#获得失败堆栈上, 顶部checkID所对应的请求对象;
		//保存上次失败的数据存入堆栈,根据checkID再次请求
        public T peekTopReqParamsValObj<T>()
        {
            Log.d<NetHttp>("fail req: " + m_failRequestStack.Count + ", req dict: " + m_requestDict.Count);

            string checkID = m_failRequestStack.Pop();
            object obj = m_requestDict[checkID];
            return (T)obj;
        }

		//失败的数据请求成功后出堆栈
        public void popTopReqPramsValObj()
        {
            Log.d<NetHttp>("fail req: " + m_failRequestStack.Count + ", req dict: " + m_requestDict.Count);

            string checkID = m_failRequestStack.Pop();
            m_requestDict.Remove(checkID);
        }

        Dictionary<string, object> m_requestDict = new Dictionary<string, object>();
        Stack<string> m_failRequestStack = new Stack<string>();

        //#1模拟网络超时; 2: 模拟逻辑错误;
        int m_mockType = 0;
        bool m_mockErr = true;

        //public static string s_baseUrl = "http://192.168.52.2:4014";
    }
}
