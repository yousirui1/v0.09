using System;

namespace tpgm.val
{
    public class ErrCode
    {
        public const int Do_Nothing = -1;

        public const int INIT = 0;

        public const int CHECK_DATA_TMP_BIN = 13;

        public const int CHECK_VAL_MODIFIED_NET_ERR = 1;

        public const int CREATE_TMP_BIN = 2;

        public const int DOWN_FILE_NET_ERR = 3;

        public const int DEL_OLD_DATA_TMP = 4;

        public const int DOWN_DIR_CREATE = 5;

        public const int DO_DOWN = 6;

        public const int MD5_VERIFY = 7;

        public const int UNZIP = 8;

        public const int FINISH_ALL = 10;

        public const int LOAD_VAL = 11;

        private ErrCode()
        {
        }
    }
}

