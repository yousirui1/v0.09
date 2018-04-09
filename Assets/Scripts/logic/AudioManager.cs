using UnityEngine;
using System.Collections;

namespace tpgm
{
    //数值表的更新信息;
    //public class PrefAudio
    public class AudioManager
    {
        private const string PREF_NAME = "pref_audio_";

        private const string KEY_MUSIC_EN = "music_en";
        private const string KEY_MUSIC_VOL = "music_val";

        private const string KEY_SOUND_EN = "sound_en";
        private const string KEY_SOUND_VOL = "sound_val";

        //************************************************** music begin;

        //保存音乐的设置
        static void saveMusicEnable(bool en)
        {
            PlayerPrefs.SetInt(PREF_NAME + KEY_MUSIC_EN, en ? 1 : 0);
            PlayerPrefs.Save();
        }

        //读取音乐配置
        static bool isMusicEnable()
        {
            int i = PlayerPrefs.GetInt(PREF_NAME + KEY_MUSIC_EN, 1);
            return (i == 1);
        }

        //保存音量大小
        static void saveMusicVol(float f)
        {
            PlayerPrefs.SetFloat(PREF_NAME + KEY_MUSIC_VOL, f);
            PlayerPrefs.Save();
        }

        //读取音量的大小
        static float getMusicPrefVol()
        {
            float f = PlayerPrefs.GetFloat(PREF_NAME + KEY_MUSIC_VOL, 1);
            return f;
        }

        //************************************************** music end;

        //************************************************** sound begin;

        static void saveSoundEnable(bool en)
        {
            PlayerPrefs.SetInt(PREF_NAME + KEY_SOUND_EN, en ? 1 : 0);
            PlayerPrefs.Save();
        }

        static bool isSoundEnable()
        {
            int i = PlayerPrefs.GetInt(PREF_NAME + KEY_SOUND_EN, 1);
            return (i == 1);
        }

        static void saveSoundVol(float f)
        {
            PlayerPrefs.SetFloat(PREF_NAME + KEY_SOUND_VOL, f);
            PlayerPrefs.Save();
        }

        static float getSoundPrefVol()
        {
            float f = PlayerPrefs.GetFloat(PREF_NAME + KEY_SOUND_VOL, 1);
            return f;
        }

        //************************************************** sound end;

        //对外接口
		//初始化音频设置

        public static void setupAudio(Audio2D audio2D)
        {
            //audio2D.setMusicEnable(isMusicEnable());
            //audio2D.setSoundEnable(isSoundEnable());

            audio2D.setBgMusicVol(getMusicPrefVol());
            audio2D.setSoundVol(getSoundPrefVol());
        }

        public static void setMusicVol(float f)
        {
            Audio2D audio2D = SavedContext.s_audio2D;
            audio2D.setBgMusicVol(f);

            saveMusicVol(f);
        }

        public static float getMusicVol()
        {
            return SavedContext.s_audio2D.getBgMusicVol();
        }

        public static void setSoundVol(float f)
        {
            Audio2D audio2D = SavedContext.s_audio2D;
            audio2D.setSoundVol(f);

            saveSoundVol(f);
        }

        public static float getSoundVol()
        {
            return SavedContext.s_audio2D.getSoundVol();
        }

        public static void setMusicEnable(bool b)
        {
            Audio2D audio2D = SavedContext.s_audio2D;
            audio2D.setMusicEnable(b);

            saveMusicEnable(b);
        }

        public static void setSoundEnable(bool b)
        {
            Audio2D audio2D = SavedContext.s_audio2D;
            audio2D.setSoundEnable(b);

            saveSoundEnable(b);
        }

        private AudioManager()
        {
        }
    }

    public class MusicPlay
    {
        //暂停
        public static void pause()
        {
            SavedContext.s_audio2D.pauseMusic();
        }

        //恢复播放
        public static void resume()
        {
            SavedContext.s_audio2D.resumeMusic();
        }

        //#登录音乐;
        public static void login()
        {
			SavedContext.s_audio2D.playMusic("audio/yy_main_zhuJieMian");
        }
       
        //游戏音乐1	
		public static void game_mode0()
        {
			SavedContext.s_audio2D.playMusic("audio/game/yy_fight_zhanDou1");
        }

		//游戏音乐2	
		public static void game_mode1()
		{
			SavedContext.s_audio2D.playMusic("audio/game/yy_fight_zhanDou2");
		}

		//游戏音乐3	
		public static void game_mode2()
		{
			SavedContext.s_audio2D.playMusic("audio/game/yy_fight_zhanDou3");
		}



        private MusicPlay()
        {
        }

    }

    public class SoundPlay
    {
        // 点击按钮
        public static void btnClick()
        {
			SavedContext.s_audio2D.playSoundGlobal("audio/yx_share_dianJiAnNiu");
        }

		// 卡牌旋转按钮
		public static void cardClick()
		{
			SavedContext.s_audio2D.playSoundGlobal("audio/yx_main_kaiPai");
		}

		// 侧边按钮隐藏
		public static void btnHide()
		{
			SavedContext.s_audio2D.playSoundGlobal("audio/yx_main_shouQi");
		}


		// 侧边按钮激活
		public static void btnActive()
		{
			SavedContext.s_audio2D.playSoundGlobal("audio/yx_main_zhanKai");
		}
			

        // 界面弹开
        public static void layerShow()
        {
            SavedContext.s_audio2D.playSoundGlobal("game_res/Audio/");
        }

        // 界面关闭
        public static void layerClose()
        {
            SavedContext.s_audio2D.playSoundGlobal("game_res/Audio/");
        }

        // 游戏胜利
        public static void gameWin()
        {
            SavedContext.s_audio2D.playSoundGlobal("game_res/Audio/");
        }

        // 游戏失败
        public static void gameFail()
        {
            SavedContext.s_audio2D.playSoundGlobal("game_res/Audio/");
        }

       

        private SoundPlay()
        {
        }
    }
}
