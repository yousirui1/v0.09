using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//#备注:
//#正在isPlaying, 再调用play, 会从头开始播放;
//#



//#需要测试的:
//(#)有音乐在播放, 放入后台暂停, 恢复时播放;
//(#)没音乐在播放, 放入后台nothing, 恢复时nothing;
//(#)有音效在播放, 放入后台暂停, 恢复时不再播放;
//(#)没音效在播放, 放入后台nothing, 恢复时nothing;
//(#)一开始在播放音乐1, 放入后台暂停, 3s后尝试播放音乐2(不会被播放), 恢复时音乐2继续播放;
//(#)一开始不在播放音乐, 放入后台暂停, 3s后尝试播放音乐1(不会被播放), 恢复时音乐1继续播放;
//
//(#)播放音乐1时, 关闭音乐, 再启用音乐, 音乐1从头开始播放; 再关闭音乐, 尝试播放音乐2(不会被播放), 启用音乐, 音乐2从头开始播放;
//(#)播放音乐1时, 关闭音乐, 放入后台, 在恢复时音乐不会播放, 再启用音乐, 音乐1从头开始播放;
//(#)播放音乐1时, 关闭音乐, 放入后台, 在恢复时音乐不会播放; 尝试播放音乐2(不会播放), 启用音乐, 音乐2从头开始播放;
//

namespace tpgm
{
    public class Audio2D : MonoBehaviour
    {
        private static readonly string TAG = typeof(Audio2D).FullName;

        MainLooper m_initedLooper;
        AudioCache m_initedAudioCache;

        //#在对象释放时调用一下他的OnObjectDestroy;
        MessageHandlerProxy m_msgHandlerProxy;

        //#背景音乐只会有一个;
        AudioSource m_bgMusicPlayer;
        //#音效可以同时播放多个;
        AudioSource m_sfxPlayer;

        //#当前音乐;
        string m_curBgMusicKey = "";
        //#当前音效列表;
        List<string> m_sfxList = new List<string>();

        private bool m_appPutBackground;

        private bool m_musicEnable = true;
        private bool m_soundEnable = true;

        public static Audio2D create(MainLooper looper, AudioCache audioCache)
        {
            GameObject go = new GameObject("audio");
            MonoBehaviour.DontDestroyOnLoad(go);
            Audio2D a2d = go.AddComponent<Audio2D>();
            a2d.setup(looper, audioCache);

            return a2d;
        }


        class MsgData
        {
            public string m_audioPath = "";
            public OnSoundEnd m_callback;
        }

        public delegate void OnSoundEnd(Audio2D sender, string key);

		void handleMsg(HandlerMessage msg)
        {
            switch (msg.m_what)
            {
            case 1:
            {
                MsgData dataObj = (MsgData)msg.m_dataObj;
                dataObj.m_callback(this, dataObj.m_audioPath);
            }
            break;

            }
        }


        Audio2D()
        {
        }

        //初始化
        void setup(MainLooper looper, AudioCache audioCache)
        {
            if (null == looper)
            {
                throw new ArgumentException("MainLooper not setup, it's null");
            }

            m_initedLooper = looper;
            m_initedAudioCache = audioCache;

            initMusicPlayer();
            initSfxPlayer();

            m_msgHandlerProxy = new MessageHandlerProxy(handleMsg);
        }
        //初始化背景音乐
        void initMusicPlayer()
        {
            GameObject go = new GameObject("musicPlayer");
            go.transform.SetParent(transform, false);
            AudioSource player = go.AddComponent<AudioSource>();
            m_bgMusicPlayer = player;

            player.loop = true;
            player.mute = false;
            player.volume = 1.0f;
            player.pitch = 1.0f;
            player.playOnAwake = false;
        }

        //初始化音效 
        void initSfxPlayer()
        {
            GameObject go = new GameObject("soundPlayer");
            go.transform.SetParent(transform, false);
            AudioSource player = go.AddComponent<AudioSource>();
            m_sfxPlayer = player;

            player.loop = false;
            player.mute = false;
            player.volume = 1.0f;
            player.pitch = 1.0f;
            player.playOnAwake = false;
        }

        //************************************************** MonoBehaviour回调 begin;

//        void Awake()
//        {
//        }

        void OnDestroy()
        {
            //m_msgHandlerProxy.onObjectDestroy();
        }

        //#当程序获得或失去焦点;
        //#比如: 游戏过程中, 有电话过来了;
//        void OnApplicationFocus(bool isFocus)
//        {
//            if (isFocus)
//            {
//                appPutForeground();
//            }
//            else
//            {
//                appPutBackground();
//            }
//        }

        //#当程序暂停;
        //#游戏过程中按home键放入后台;
        void OnApplicationPause(bool isPause)  
        {
            if(isPause)  
            {
                appPutBackground();
            }
            else
            {
                appPutForeground();
            }
        }

        //************************************************** MonoBehaviour回调 end;
        //设置背景音乐是否循环
        public void setBgMusicLoop(bool b)
        {
            m_bgMusicPlayer.loop = b;
        }
        //设置背景音乐的音量
        public void setBgMusicVol(float f)
        {
            m_bgMusicPlayer.volume = f;
        }
        //获取背景音乐的音量
        public float getBgMusicVol()
        {
            return m_bgMusicPlayer.volume;
        }
        //设置音效的音量
        public void setSoundVol(float f)
        {
            m_sfxPlayer.volume = f;
        }

        //获得音效的音量
        public float getSoundVol()
        {
            return m_sfxPlayer.volume;
        }

        //#程序被放到了后台;
        public void appPutBackground()
        {
            Log.d<Audio2D>("AppPutBackground");
            if (m_appPutBackground)
            {
                return;
            }

            m_appPutBackground = true;

            if (m_soundEnable)
            {
                m_sfxPlayer.Pause();
            }

            pauseMusic();
        }

        //#程序返回前台;
        public void appPutForeground()
        {
            Log.d<Audio2D>("AppPutForground");
            if (!m_appPutBackground)
            {
                return;
            }

            m_appPutBackground = false;

            if (m_soundEnable)
            {
                m_sfxPlayer.Play();
            }

            resumeMusic();
        }
        //设置是否播放音乐
        public void setMusicEnable(bool b)
        {
            m_musicEnable = b;

            if (!b)
            {
                if (m_curBgMusicKey.Length > 0)
                {
                    if (m_bgMusicPlayer.isPlaying)
                    {
                        m_bgMusicPlayer.Stop();
                    }
                }
            }
            else
            {
                if (m_curBgMusicKey.Length > 0)
                {
                    //if (!m_bgMusicPlayer.isPlaying)
                    {
                        m_bgMusicPlayer.Play();
                    }
                }
            }

            m_sfxList.Clear();
        }
        //音乐开关
        public bool isMusicEnable()
        {
            return m_musicEnable;
        }
        //设置音效
        public void setSoundEnable(bool b)
        {
            m_soundEnable = b;

            if (!b)
            {
                if (m_sfxPlayer.isPlaying)
                {
                    m_sfxPlayer.Stop();
                }
            }

            m_sfxList.Clear();
        }

        //音效开关
        public bool isSoundEnable()
        {
            return m_soundEnable;
        }

        //************************************************** music begin;
        //播放音乐
        public void playMusic(string audioPath)
        {
            if (m_curBgMusicKey.Equals(audioPath))
            {
                Log.i<Audio2D>("play same music: " + audioPath);
                return;
            }

            if (m_curBgMusicKey.Length > 0)
            {
                m_initedAudioCache.unmarkPageUse("music", audioPath);
            }

            m_curBgMusicKey = audioPath;
            m_initedAudioCache.markPageUse("music", audioPath);
            if (m_musicEnable)
            {
                AudioClip ac = m_initedAudioCache.usePageScopeAudio("music", audioPath);

                m_bgMusicPlayer.clip = ac;
                m_bgMusicPlayer.Play();
            }
        }
         //不播放音乐
        public void playNoMusic()
        {
            if (m_curBgMusicKey.Length > 0)
            {
                m_initedAudioCache.unmarkPageUse("music", m_curBgMusicKey);
                m_curBgMusicKey = "";
                m_bgMusicPlayer.clip = null;

                if (m_bgMusicPlayer.isPlaying)
                {
                    m_bgMusicPlayer.Stop();
                }
            }
        }

        //暂停播放
        public void pauseMusic()
        {
            if (m_musicEnable)
            {
                if (m_curBgMusicKey.Length > 0)
                {
                    m_bgMusicPlayer.Pause();
                }
            }
        }

        //恢复播放
        public void resumeMusic()
        {
            if (m_musicEnable)
            {
                if (m_curBgMusicKey.Length > 0)
                {
                    m_bgMusicPlayer.Play();
                }
            }
        }

        //************************************************** music end;

        //************************************************** sound begin;
    
        public void playSoundGlobal(string audioPath)
        {
            m_sfxList.Add(audioPath);

            if (m_soundEnable)
            {
                AudioClip ac = m_initedAudioCache.useGlobalSound(audioPath);
                m_sfxPlayer.PlayOneShot(ac);
            }
        }

        public void playSoundPageScope(string instanceID, string audioPath)
        {
            m_sfxList.Add(audioPath);

            if (m_soundEnable)
            {
                AudioClip ac = m_initedAudioCache.usePageScopeAudio(instanceID, audioPath);
                m_sfxPlayer.PlayOneShot(ac);
            }
        }

//        public void playSound(string instanceID, string audioPath, OnSoundEnd callback)
//        {
//            m_sfxList.Add(audioPath);
//
//            if (m_soundEnable)
//            {
//                AudioClip ac = m_initedAudioCache.useGlobalSound(audioPath);
//                m_sfxPlayer.PlayOneShot(ac);
//
//                long ms = (long) ac.length * 1000;
//                Message msg = MainLooper.obtainMessage(m_msgHandlerProxy.handleMessage, 1);
//
//                MsgData dataObj = new MsgData();
//                dataObj.m_audioPath = audioPath;
//                dataObj.m_callback = callback;
//                msg.m_dataObj = dataObj;
//                m_initedLooper.postMessageDelay(msg, ms);
//            }
//        }

      
        //************************************************** sound end;

      
    }
}
