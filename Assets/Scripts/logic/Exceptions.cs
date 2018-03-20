using System;
/**************************************
*FileName: DataInvalidException.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 异常处理程序
**************************************/
namespace tpgm
{
    public class DataInvalidException : ApplicationException
    {
        public DataInvalidException()
        {
        }

        public DataInvalidException(string message)
            : base(message)
        {
        }
    }


    //#游戏数据异常;
    public class DataCorruptException : ApplicationException
    {
        public DataCorruptException()
        {
        }

        public DataCorruptException(string message)
            : base(message)
        {
        }
    }

    //#游戏数据损坏;
    public class DataDamageException : ApplicationException
    {
        public DataDamageException()
        {
        }

        public DataDamageException(string message)
            : base(message)
        {
        }
    }

}

