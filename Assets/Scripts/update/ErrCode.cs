using UnityEngine;
using System.Collections;

namespace tpgm.update
{

    public class ErrCode
    {
        public const int INIT = 0;

        public const int CHECK_NEW_APK_NET_ERR = 1;

        public const int CHECK_APK_UPDATE_BAK_DIR = 12;

        public const int GET_APK_VER_CODE = 2;

        public const int PARSE_SERVER_APK_HASH = 3;

        public const int DOWN_APK_NET_ERR = 4;

        public const int DEL_OLD_UPDATE_APK_TMP = 5;

        public const int DOWN_DIR_CREATE = 6;

        public const int DO_DOWN = 7;

        public const int MD5_VERIFY = 8;

        public const int WRITE_OUT_SERVER_APK_HASH =98;

        public const int FINISH_UPDATE = 10;

        public const int INSTALL_APK = 11;

        private ErrCode()
        {
        }
    }

}
