using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using tpgm.val;

/**************************************
*FileName: ValUpdateController.cs
*User: ysr 
*Data: 2018/2/1
*Describe: 数值表更新
**************************************/

namespace tpgm
{
	public class ValUpdateController : BaseController<SplashUIPage>
	{
		private const string TAG = "ValUpdateController";
		MainLooper m_initedLooper;
		
		//模拟测试开关
		bool m_mockSwitch = true;

		//本地存储标志用来判断是否需要更新
		string m_lastModified = "";
		string m_etag = "";
		
		//下载总大小
		public long m_totalBytes;
		//下载速度
		public long m_nowBytes;
		public int m_prg = 0;
		public int m_bytesPerSec = 0;
	
		//保存当前状态	
		public int m_errCode;
		

		public ValUpdateController(SplashUIPage iview) : base(iview)
		{
			m_initedLooper = MainLooper.instance();
		}

		//检查本地持久化数据tag用来判断是否存在数值表
		public void checkFileModified()
		{
			m_lastModified = PrefValUpdate.getIfModifiedSince();
			m_etag = PrefValUpdate.getETag();
			ThreadPool.QueueUserWorkItem(checkFileModifiedAsync);
			
		}
	
		//异步远程下载
		void checkFileModifiedAsync(object state)
		{
			m_errCode = ErrCode.INIT;
			string fileUrl = SavedContext.s_valUrl;
			HttpWebRequest req = (HttpWebRequest)FileWebRequest.Create(fileUrl);
			req.Method = "GET";

			//如果文件夹不存在,则总更新
			string dataDirPath = SavedContext.getExternalPath("data/");

			
			if(Directory.Exists(dataDirPath))
			{
			//	Debug.Log (TAG + ":" + "dataDirPath null");
				if(m_lastModified != string.Empty)
				{
					
					req.IfModifiedSince = Convert.ToDateTime(m_lastModified);

					WebHeaderCollection headers =req.Headers;
					headers.Add("ETag: "+ m_etag);
					Debug.Log("ETag: "+ m_etag);
				}
			}

			int errCode = ErrCode.INIT;
			do
			{
				try
				{
					Debug.Log (TAG + ":" + "http begin");
					//模拟
					if(false && m_mockSwitch)
					{
						m_mockSwitch = false;
						
						//线程睡眠1s
						Thread.Sleep(1000);
						Debug.Log("mock net err");
						throw new WebException("mock net err");
					}
						
					HttpWebResponse resp = null;
					resp = (HttpWebResponse)req.GetResponse();
					
					string lastModified = resp.GetResponseHeader("Last-Modified");
					m_lastModified = Utils.ensureValue<string>(lastModified, "");
					
					//以太网帧头
					string etag = resp.GetResponseHeader("ETag");
					m_etag = Utils.ensureValue<string>(etag, "");
					Debug.Log (TAG + ":" + "http "+resp.StatusCode);
					switch(resp.StatusCode)
					{

						case HttpStatusCode.OK:
						{
							Debug.Log("HttpStatusCode.OK");
							string dataTmpBinFilePath = SavedContext.getExternalPath("data_tmp.bin");
							string dataTmpDirPath = SavedContext.getExternalPath("data_tmp/");
							
							errCode = ErrCode.CHECK_DATA_TMP_BIN;
							bool lastDownFail = File.Exists(dataTmpBinFilePath);
							if(lastDownFail)
							{
								//下载失败
								if(Directory.Exists(dataTmpDirPath))
								{
									Directory.Delete(dataTmpDirPath, true);
								}
								
								if(Directory.Exists(dataDirPath))
								{
									Directory.Delete(dataDirPath, true);
								}
							}
							else
							{
								if(false && m_mockSwitch)
								{
								
								}
								//
								IoUtils.EnsureDir(SavedContext.s_externalPath);
								IoUtils.EnsureFile(dataTmpBinFilePath);
							}
							
							errCode = ErrCode.DEL_OLD_DATA_TMP;
							if(Directory.Exists(dataTmpDirPath))
							{
								Directory.Delete(dataTmpDirPath);
							}

							errCode = ErrCode.DOWN_DIR_CREATE;
							if(!Directory.Exists(dataTmpDirPath))
							{
								IoUtils.EnsureDir(dataTmpDirPath);
							}

							//下载到data_tmp中
							errCode = ErrCode.DO_DOWN;
							string valFilePath = SavedContext.getExternalPath("data_tmp/val.zip");
							Debug.Log(valFilePath);
							FileInfo dstFile = new FileInfo(valFilePath);
							doDownload(resp, dstFile);				
				
							//验证md5
							errCode = ErrCode.MD5_VERIFY;
							string localHash = HashUtils.getFileHashOrThrow(valFilePath);
							string serverHash = Utils.ensureValue<string>(resp.GetResponseHeader("Content-MD5"),	"");
							
							if(serverHash.Length > 0 && !serverHash.Equals(localHash))
							{
								break;
							}
							
							errCode = ErrCode.UNZIP;
							unzip();
						
							//finishAll;
							errCode = ErrCode.FINISH_ALL;
							finishAll();
							
							//验证所以数值表(尝试加载)
							errCode = ErrCode.LOAD_VAL;
							m_initedLooper.sendMessage(handleMessage, MSG_LOAD_VAL);
							loadVal();							

							m_initedLooper.sendMessage(handleMessage, MSG_END);
							errCode = ErrCode.INIT;
						}
						break;

						case HttpStatusCode.NotModified:
						{
							Debug.Log("HttpStatusCode.OK"+HttpStatusCode.NotModified);
							errCode = ErrCode.Do_Nothing;
							notModified();
						}
						break;

					}
				}
				catch(WebException ex)
				{
					bool netErr = true;
					if(null != ex.Response)
					{
						HttpWebResponse resp = (HttpWebResponse) ex.Response;
						if(null != ex.Response)
						{
							netErr = false;
							
							errCode = ErrCode.Do_Nothing;
							notModified();
						}
					
					}

					if(netErr)
					{
						Debug.Log(ex.Message);
						errCode = ErrCode.DOWN_FILE_NET_ERR;
					}

				}
				catch(Exception ex)
				{
					Debug.Log(TAG + ":" + ex.Message);
				}

			}while(false);

			switch(errCode)
			{
				case ErrCode.Do_Nothing:
				{

				}
				break;
				
				case ErrCode.INIT:
				{
					//成功
				}
				break;

				default:
				{
					m_errCode = errCode;
					m_initedLooper.sendMessage(handleMessage, MSG_ERR);
				}
				break;
			}

		}

		//验证数值表,加载常驻的一些数值表
		void loadVal()
		{

		}


		//没有索引
		void notModified()
		{
			int errCode = ErrCode.INIT;
			try
			{
				errCode = ErrCode.LOAD_VAL;
				m_initedLooper.sendMessage(handleMessage, MSG_LOAD_VAL);
				
				loadVal();
				m_initedLooper.sendMessage(handleMessage, MSG_NOT_MODIFIED);
				
				errCode = ErrCode.INIT;
			}
			catch(Exception ex)
			{

			}
			
			switch(errCode)	
			{
				case ErrCode.INIT:
				{

				}
				break;
				
				default:
				{
					m_errCode = errCode;
					m_initedLooper.sendMessage(handleMessage, MSG_ERR);
				}
				break;
			}

		}


		void doDownload(HttpWebResponse response, FileInfo dstFile)
		{
			Debug.Log("doDownload begin");
			
			//文件总大小
			long totalBytes = response.ContentLength;
			m_totalBytes = totalBytes;
			
			MainLooper looper = m_initedLooper;
			using(Stream s = response.GetResponseStream())
			using(FileStream fs = dstFile.OpenWrite())
			{
				long nowBytes = 0;
				m_nowBytes = 0;
				m_prg = 0;
				m_bytesPerSec = 0;
				byte[] buffer = new byte[1024];

				int len;
				long t1 = TimeUtils.utcNowMs();
				looper.sendMessage(handleMessage, MSG_DOWN_BEGIN);
				while(true)
				{
					if((len = s.Read(buffer, 0, buffer.Length)) <= 0)
					{
						break;
					}
					//当前时间收到文件大小
					nowBytes += len;
					m_nowBytes = nowBytes;

					//计算下载速度
					long t2 = TimeUtils.utcNowMs();
					long diff = t2 - t1;
					if(diff > 0)
					{
						m_bytesPerSec = (int)(nowBytes * 1000 /diff);
					}	
					
					fs.Write (buffer, 0, len);
						
					if(totalBytes > 0)
					{
						int prg2 = (int)(nowBytes * 100 / totalBytes);
						int diffPrg = prg2 - m_prg;
						if(diffPrg >= 1)
						{
							m_prg = prg2;
							//Debug.Log(TAG + ":" + prg2 + ", speed:" + m_bytesPerSec);
							
							looper.sendMessage(handleMessage, MSG_DOWN_PRG);
							
							if(false && m_mockSwitch)
							{
								if(prg2 >= 30)
								{

								}

							}	

						}
					}
					else
					{
						//总大小未知,只能显示已下载多少;
					}

					if(true)//mock;
					{
						Thread.Sleep(100);
					}
				}
				
				fs.Flush();

			}
			Debug.Log(TAG + ":" + "doDownload end");
			
		}

		//解压缩
		void unzip()
		{
			Debug.Log(TAG + ":" +"unzip begin");
			
			m_initedLooper.sendMessage(handleMessage, MSG_UNZIP);
			
			if(true)
			{
				Thread.Sleep(2000);
			}
			if(false && m_mockSwitch)
			{

			}

			string zipFilePath = SavedContext.getExternalPath("data_tmp/val.zip");
			string unzipDirPath = SavedContext.getExternalPath("data_tmp/");

			IoUtils.unZip(zipFilePath, unzipDirPath);
		
			Debug.Log(TAG + ":" +"unzip end");
		}

		//所有完成后删除缓存
		void finishAll()
		{
			Debug.Log(TAG + ":" +"finish begin");
	
			string dataTmpDirPath = SavedContext.getExternalPath("data_tmp/");
			string dataDirPath = SavedContext.getExternalPath("data/");

			if(Directory.Exists(dataDirPath))
			{
				Directory.Delete(dataDirPath, true);
			}
			
			Directory.Move(dataTmpDirPath, dataDirPath);
			
			string dataTmpBinFilePath = SavedContext.getExternalPath("data_tmp.bin");
			File.Delete(dataTmpBinFilePath);
			
			Debug.Log(TAG + ":" +"finish end");

		}

		const int MSG_ERR = 1;

		const int MSG_DOWN_BEGIN = 2;
		const int MSG_DOWN_PRG = 3;
		const int MSG_UNZIP = 4;
		const int MSG_LOAD_VAL = 5;
		const int MSG_END = 6;
		const int MSG_NOT_MODIFIED = 7;
		
		void handleMessage(HandlerMessage msg)
		{
			switch(msg.m_what)
			{
				case MSG_ERR:
				{
					if(null != m_iview)
					{
						m_iview.valErr();
					}
				}
				break;

				case MSG_DOWN_BEGIN:
				{
					m_iview.valDownBegin();
				}
				break;

				case MSG_DOWN_PRG:
				{
					m_iview.valDownPrg();
				}
				break;

				case MSG_UNZIP:
				{
					m_iview.valUnzip();
				}
				break;

				case MSG_LOAD_VAL:
				{
					m_iview.valLoadVal();
				}
				break;

				case MSG_END:
				{
					PrefValUpdate.saveFileModifyData(m_lastModified, m_etag);
					m_lastModified = "";
					m_etag = "";
					
					m_iview.valEnd();
				}
				break;

				case MSG_NOT_MODIFIED:
				{
					PrefValUpdate.saveFileModifyData(m_lastModified, m_etag);
					
					m_lastModified = "";
					m_etag = "";
					
					m_iview.valNotModified();
				}
				break;
			}

		}
	}


}
