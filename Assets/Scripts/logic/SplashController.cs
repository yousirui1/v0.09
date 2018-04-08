using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.Threading;
using System.IO;

using tpgm;
using tpgm.update;

/**************************************
*FileName: SplashController.cs
*User: ysr 
*Data: 2018/2/1
*Describe: 启动时版本检查
**************************************/


namespace tpgm
{
	public class SplashController : BaseController<SplashUIPage> 
	{
		
		private const string TAG = "SplashController";
		
		public ApkHash m_todownApkHash;

		//Code版本号
		int m_apkVerCode;
		
		//保存当前状态
		public int m_errCode;
		
		//累计下载的大小
		public long m_totalBytes;
		//当前接受的数据大小
		public long m_nowBytes;  
		public int m_prg = 0;
		public int m_bytesPerSec = 0;

		public SplashController(SplashUIPage iview) : base(iview)
		{

		}

		//检查是否有新的apk更新
		public void checkHasNewApk()
		{
			int verCode = -1;
			
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass unityCallJava = new AndroidJavaClass("com.tpgm.fsdzz.UnityCallJava");
			verCode = unityCallJava.CallStatic<int>("getVerCode");
			#endif
		
			//版本不匹配则异步请求服务端
			if(-1 != verCode)
			{
				m_apkVerCode = verCode;
				ThreadPool.QueueUserWorkItem(checkHasNewApkAsync);				
			}
		
			ThreadPool.QueueUserWorkItem(checkHasNewApkAsync);		
		}

		//异步向服务请求检查版本
		void checkHasNewApkAsync(object state)
		{
			try
			{
				//更新前先备份
				m_errCode = ErrCode.CHECK_APK_UPDATE_BAK_DIR;
				checkApkUpdateBakDir();
				
				
				HttpWebRequest req = (HttpWebRequest)FileWebRequest.Create("http://gdown.baidu.com/data/wisegame/e2ec5ae15b93fdaa/anzhuoshichang_16793302.apk");
				
				HttpWebResponse resp = null;
				resp = (HttpWebResponse)req.GetResponse();

				switch(resp.StatusCode)
				{
					case HttpStatusCode.OK:
					{
						try
						{
							m_errCode = ErrCode.PARSE_SERVER_APK_HASH;
							Stream stream = resp.GetResponseStream();

							ApkHash serverApkHash;
							using(StreamReader sr = new StreamReader(stream))
							{
								//下载的文件为txt需要解析其中的hash值
								serverApkHash = parseVersionApkHash(sr);

							}			
							
							Debug.Log(TAG + ":" + "apkVerCode: "+ m_apkVerCode + ", " +serverApkHash.m_verCode);
					
							if(m_apkVerCode != serverApkHash.m_verCode)
							{
								m_todownApkHash = serverApkHash;
							
								//通知页面需要更新apk
								MainLooper.instance().sendMessage(handleMessage, MSG_FIND_NEW_APK);

							}
							else
							{
								//不需要更新,继续下面的流程
								MainLooper.instance().sendMessage(handleMessage, MSG_ENTER_GAME);
							}
							m_errCode = ErrCode.INIT;

						}
						catch(Exception e)
						{
							Debug.Log(TAG + ":" + e.Message);
						}
						
						switch(m_errCode)
						{
							case ErrCode.INIT:
							{
							}
							break;
							
							default:
							{
								MainLooper.instance().sendMessage(handleMessage, MSG_ERR);
							}
							break;
						}

					}
					break;
					
					default:
					{

					}
					break;

				}

			}
			catch(WebException ex)
			{
				Debug.Log(TAG + ":" +ex.Message);
				
				m_errCode = ErrCode.CHECK_NEW_APK_NET_ERR;
				MainLooper.instance().sendMessage(handleMessage, MSG_ERR);
			}

		}

		
		//更新之前备份检查备份的路径
		void checkApkUpdateBakDir()
		{
			if(false) //模拟异常	
			{
				throw new IOException("fcheckApkUpdateBakDir fail!");
			}
			
			string apkUpdateBakDirPath = SavedContext.s_externalPath + "apk_update_bak";
			if(Directory.Exists(apkUpdateBakDirPath))
			{
				//先尝试备份成功后替换
				string apkUpdateTmpDirPath = SavedContext.s_externalPath + "apk_update_tmp";
				if(Directory.Exists(apkUpdateTmpDirPath))
				{
				
					Directory.Delete(apkUpdateBakDirPath, true);
					
					string apkUpdateDirPath = SavedContext.s_externalPath + "apk_update";
					Directory.Move(apkUpdateTmpDirPath, apkUpdateDirPath);
				}
				else
				{
					Directory.Delete(apkUpdateBakDirPath, true);
				}
					
			}
			
		}

		//解析收到的下载流
		static ApkHash parseVersionApkHash(TextReader tr)
		{
			ApkHash apkHash = new ApkHash();
			
			int lineNum = 0;
			string line;

			while(null != (line = tr.ReadLine()))
			{
				switch(lineNum)
				{
					case 0:
					{
						try
						{
							apkHash.m_rVal = Convert.ToInt32(line);
						}
						catch(FormatException ex)
						{
							throw new IOException("invalid line 1 :" + line);

						}

					}
					break;

				case 1:
					{
						try {
							apkHash.m_verCode = Convert.ToInt32 (line);
						} catch (FormatException ex) {
							throw new IOException ("invalid line 2: " + line);
						}

					}
					break;
			
					case 2:
					{
						int index1 = line.IndexOf(" ; ");
						if(-1 == index1)
						{
							throw new IOException("invalid line " + (lineNum + 1) + ": " + line);
						}
						String filename = line.Substring(0, index1);

						int index2 = line.IndexOf(" ; ", index1 + 3);
						if(-1 == index2)
						{
							throw new IOException("invalid line " + (lineNum + 1) + ": " + line);
						}
						String hash = line.Substring(index1 + 3 , index2 - (index1 + 3));


						index1 = index2;
						index2 = line.IndexOf(" ; ", index1 + 3);
						if(-1 == index2)
						{
							throw new IOException("invalid line " + (lineNum + 1) + ": " + line);
						}
						String bytesStr = line.Substring(index1 + 3, index2 - (index1 + 3 ));
						long bytes;

						try
						{
							bytes = Convert.ToInt64(bytesStr);
						}
						catch(FormatException ex)
						{
							throw new IOException("invalid line " + (lineNum + 1) + ": " +line);
						}

						index1 = index2;
						String url = line.Substring(index1 + 3 );

						apkHash.m_filename = filename;
						apkHash.m_hash = hash;
						apkHash.m_bytes = bytes;
						apkHash.m_url = url;

					}
					break;
					
					default :
					{
						//错误打印log
						apkHash.m_log += line + "\n";
					}
					break;

				}
				lineNum++;

			}
			return apkHash;
		}

		public void downApk()
		{
			ThreadPool.QueueUserWorkItem(downApkAsync);
		}

		
		//异步线程池下载通过主线程loop 发现消息通知页面
		void downApkAsync(object state)
		{
			ApkHash apkHash = m_todownApkHash;
			
			int errCode = ErrCode.INIT;
			do
			{
				try
				{
					errCode = ErrCode.DEL_OLD_UPDATE_APK_TMP;
					string apkUpdateTmpDirPath = SavedContext.s_externalPath + "apk_update_tmp/";
					if(Directory.Exists(apkUpdateTmpDirPath))
					{
						Directory.Delete(apkUpdateTmpDirPath, true);
					}	

					HttpWebRequest req = (HttpWebRequest)FileWebRequest.Create(apkHash.m_url);

					if(false) //模拟异常
					{
						throw new WebException("mock : ");
					}

					HttpWebResponse response = (HttpWebResponse)req.GetResponse();

					FileInfo apkFile = new FileInfo(SavedContext.s_externalPath + "apk_update_tmp/" + m_todownApkHash.m_filename);
					errCode = ErrCode.DOWN_DIR_CREATE;

					if(false) //模拟异常
					{
						throw new IOException("mock : ");
					}

					IoUtils.EnsureDir(apkFile.DirectoryName);
					
					errCode = ErrCode.DO_DOWN;
					doDownloadOrThrow(response);

					errCode = ErrCode.MD5_VERIFY;
					//验证下载文件的md5判断文件的完整性
					string hash = HashUtils.getFileHashOrThrow(apkFile);
					if(false) //模拟异常
					{
						hash = "1";
					}	
					if(!hash.Equals(m_todownApkHash.m_hash))
					{
						break;
					}
					
					errCode = ErrCode.WRITE_OUT_SERVER_APK_HASH;
					//写入新的hash用作更新判断的标志
					writeOutServerApkHashOrThrow();
				
					errCode = ErrCode.FINISH_UPDATE;
					finishAllOrThrow();

					MainLooper.instance().sendMessage(handleMessage, MSG_INSTALL_APK);
					
					errCode = ErrCode.INIT;
				}
				catch(IOException ex)
				{
					//下载失败
					Debug.Log(ex.Message);
				}	
				catch(WebException ex)
				{
					Debug.Log(ex.Message);
					
					errCode = ErrCode.DOWN_APK_NET_ERR;
				}
			}while(false);				

			switch(errCode)
			{
				case ErrCode.INIT:
				{

				}
				break;

				default:
				{
					m_errCode = errCode;
					
					MainLooper.instance().sendMessage(handleMessage, MSG_ERR);
				}
				break;
			}
		}

		//可能会抛出IOException 异常
		void doDownloadOrThrow(HttpWebResponse response)
		{
			long totalBytes = response.ContentLength;
			m_totalBytes = totalBytes;

			FileInfo apkFile = new FileInfo(SavedContext.s_externalPath + "apk_update_tmp/" + m_todownApkHash.m_filename);
			MainLooper looper = MainLooper.instance();
			using(Stream s = response.GetResponseStream())
			using(FileStream fs = apkFile.OpenWrite())
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
					nowBytes += len;
					m_nowBytes = nowBytes;

					long t2 = TimeUtils.utcNowMs();	
					long diff = t2 - t1;
					if(diff > 0)
					{
						m_bytesPerSec = (int)(nowBytes * 1000 /diff);
					}
					fs.Write(buffer, 0, len);

					if(totalBytes > 0)
					{
						int prg2 = (int)(nowBytes * 100 /totalBytes);
						int diffPrg = prg2 - m_prg;
						if(diffPrg >= 1)
						{
							m_prg = prg2;
							looper.sendMessage(handleMessage, MSG_DOWN_PRG);
							
							if(false)
							{
								if(prg2 >= 30)
								{
									throw new IOException("mock down err ");
								}
							}

						}
		
					}
					else
					{
						//获取文件大小失败,总大小未知,只能显示出已下载多少
					}

				}

				fs.Flush();
			}
			

		}

		//从服务器获取的hash写入本地，用于下次验证
		public void retryWriteOutServerApkHash()
		{
			ThreadPool.QueueUserWorkItem(writeOutServerApkHashAsync);
		}
		

		void writeOutServerApkHashAsync(object state)
		{
			int errCode = ErrCode.INIT;
			try
			{
				errCode = ErrCode.WRITE_OUT_SERVER_APK_HASH;
				writeOutServerApkHashOrThrow();
				
				errCode = ErrCode.FINISH_UPDATE;
				finishAllOrThrow();
				
				MainLooper.instance().sendMessage(handleMessage, MSG_INSTALL_APK);
			}
			catch(IOException ex)
			{
				Debug.Log(ex.Message);
			}
			
			switch(errCode)
			{
				case ErrCode.INIT:
				{
					
				}
				break;
				default:
				{
					m_errCode =errCode;
					MainLooper.instance().sendMessage(handleMessage, MSG_ERR);
				}
				break;
			}	
				

		}			
		
		//可以会抛出IOException 异常
		void writeOutServerApkHashOrThrow()
		{

			if(false) //mock
			{
				throw new IOException("mock : writeOutServerApkHash fail");
			}
			
			string apkHashFilePath = SavedContext.s_externalPath + "apk_update_tmp/" + SavedContext.APK_HASH_NAME + ".txt";
			using (FileStream fs = File.OpenWrite(apkHashFilePath))
			{
				StreamWriter sw = new StreamWriter(fs);

				ApkHash apkHash = m_todownApkHash;

				sw.Write("" + apkHash.m_rVal);
				sw.WriteLine();

				sw.Write(apkHash.m_verCode);
				sw.WriteLine();

				sw.Write(apkHash.m_filename);

				sw.Write(" ; ");
				sw.Write(apkHash.m_hash);

				sw.Write(" ; ");
				sw.Write("" + apkHash.m_bytes);

				sw.Write(" ; ");
				sw.Write(apkHash.m_url);

				sw.Flush();
			}
		
		}


		//可以会抛出IOException 异常
		void finishAllOrThrow()
		{
			if(false) // mock;
			{
				throw new IOException("mock : finishAll fail");
			}
			
			string apkUpdateBakDirPath = SavedContext.s_externalPath + "apk_update_bak";
			
			string apkUpdateDirPath = SavedContext.s_externalPath + "apk_update";
			bool apkUpdateDirExists = Directory.Exists(apkUpdateDirPath);

			if(apkUpdateDirExists)
			{
				Directory.Move(apkUpdateDirPath, apkUpdateBakDirPath);
			}	

			string apkUpdateTmpDirPath = SavedContext.s_externalPath + "apk_update_tmp";
			Directory.Move(apkUpdateTmpDirPath, apkUpdateDirPath);

			if(apkUpdateDirExists)
			{
				Directory.Delete(apkUpdateBakDirPath ,true);
			}
				
		}

		public void installApk()
		{
			Debug.Log("installApk begin");

			#if UNITY_ANDROID && !UNITY_EDITOR
			FileInfo apkFile = new FileInfo(SavedContext.s_externalPath + "apk_update/" + m_todownApkHash.m_filename);
			AndroidJavaClass unityCallJava = new AndroidJavaClass("com.tpad.ttd.UnityCallJava");
			
			bool ok = unityCallJava.CallStatic<bool>("installApk", apkFile.FullName);

			if(!ok)
			{
				m_errCode = ErrCode.INSTALL_APK;
			}

			#endif

			m_todownApkHash = null;
			Debug.Log("installApk end");
		}


		private const int MSG_ERR = 1;

		private const int MSG_FIND_NEW_APK = 2;

		private const int MSG_ENTER_GAME = 3;

		private const int MSG_DOWN_BEGIN = 4;

		private const int MSG_DOWN_PRG = 5;

		private const int MSG_INSTALL_APK = 6;

		public void handleMessage(HandlerMessage msg)
		{
			Log.d<SplashController>("what: " + msg.m_what);

			switch (msg.m_what)
			{
			case MSG_FIND_NEW_APK:
				{
					m_iview.findNewApk();
				}
				break;

			case MSG_ENTER_GAME:
				{
					//m_iview.enterGame();
				}
				break;

			case MSG_DOWN_BEGIN:
				{
					m_iview.apkDownBegin();
				}
				break;

			case MSG_DOWN_PRG:
				{
					m_iview.downPrg();
				}
				break;

			case MSG_INSTALL_APK:
				{
					installApk();

					m_iview.installApk();
				}
				break;

			case MSG_ERR:
				{
					Log.w<SplashController>("err: " + m_errCode);

					switch (m_errCode)
					{
					case ErrCode.CHECK_NEW_APK_NET_ERR:
						{
							m_iview.checkNewApkNetErr();
						}
						break;

					case ErrCode.CHECK_APK_UPDATE_BAK_DIR:
						{
							m_iview.checkApkUpdateBakDirErr();
						}
						break;

					case ErrCode.GET_APK_VER_CODE:
						{
							m_iview.parseLocalApkHashErr();
						}
						break;

					case ErrCode.PARSE_SERVER_APK_HASH:
						{
							m_iview.parseServerApkHashErr();
						}
						break;

					case ErrCode.DOWN_APK_NET_ERR:
						{
							m_iview.downApkNetErr();
						}
						break;

					case ErrCode.DEL_OLD_UPDATE_APK_TMP:
						{
							m_iview.delOldUpdateApkTmp();
						}
						break;

					case ErrCode.DOWN_DIR_CREATE:
						{
							m_iview.downDirCreateErr();
						}
						break;

					case ErrCode.DO_DOWN:
						{
							m_iview.doDownErr();
						}
						break;

					case ErrCode.MD5_VERIFY:
						{
							m_iview.md5VerifyErr();
						}
						break;

					case ErrCode.WRITE_OUT_SERVER_APK_HASH:
						{
							m_iview.writeOutServerApkHashErr();
						}
						break;

					case ErrCode.FINISH_UPDATE:
						{
							m_iview.finishUpdateErr();
						}
						break;

					case ErrCode.INSTALL_APK:
						{
							m_iview.installApkErr();
						}
						break;

					}
				}
				break;

			}
		}


	}

}
