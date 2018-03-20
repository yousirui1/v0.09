using System;
using System.Collections;
using System.Collections.Generic;
using tpgm;

/**************************************
*FileName: Task.cs
*User: ysr 
*Data: 2017/12/12
*Describe: 主线程泛型
**************************************/

public class Task
{
	#if false
	public enum ArgNum
	{
		AN_NONE,
		AN_ONE,
		AN_TWO,
		AN_THREE,
	}
	public ArgNum CurArgNum;
	private Action mTaskFuncVoid;
	private Action<string> mTaskFuncX;
    
    private Action<string, string> mTaskFuncXX;
	private Action<string, string, string> mTaskFuncXXX;
	private List<string> mArgs = new List<string>();

    private Action<FrameBuf> mTaskFuncX_buf;
    private List<FrameBuf> mArgs_buf = new List<FrameBuf>();

    public Task (Action taskfunc)
	{
		mTaskFuncVoid = taskfunc;
		CurArgNum = ArgNum.AN_NONE;
	}

	/*public Task(Action<string> taskfunc, string arg0)
	{
		mTaskFuncX = taskfunc;
		mArgs.Clear ();
		mArgs.Add (arg0);
		CurArgNum = ArgNum.AN_ONE;
	}*/

    public Task(Action<FrameBuf> taskfunc, FrameBuf arg0)
    {
        mTaskFuncX_buf = taskfunc;
        mArgs_buf.Clear();
        mArgs_buf.Add(arg0);
        CurArgNum = ArgNum.AN_ONE;
    }


    public Task(Action<string, string> taskfunc, string arg0, string arg1)
	{
		mTaskFuncXX = taskfunc;
		mArgs.Clear ();
		mArgs.Add (arg0);
		mArgs.Add (arg1);
		CurArgNum = ArgNum.AN_TWO;
	}
	public Task(Action<string, string, string> taskfunc, string arg0, string arg1, string arg2)
	{
		mTaskFuncXXX = taskfunc;
		mArgs.Clear ();
		mArgs.Add (arg0);
		mArgs.Add (arg1);
		mArgs.Add (arg2);
		CurArgNum = ArgNum.AN_THREE;
	}

	public void Execute()
	{
		switch(CurArgNum)
		{
		case ArgNum.AN_NONE:
			mTaskFuncVoid();
			break;
		case ArgNum.AN_ONE:
            mTaskFuncX_buf(mArgs_buf[0]);
			break;
		case ArgNum.AN_TWO:
			mTaskFuncXX(mArgs[0], mArgs[1]);
			break;
		case ArgNum.AN_THREE:
			mTaskFuncXXX(mArgs[0], mArgs[1], mArgs[2]);
			break;
		}
	}
	#endif
}


