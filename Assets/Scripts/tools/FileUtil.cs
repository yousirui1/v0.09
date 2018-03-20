using System.Collections;
using System.IO;
using UnityEngine;
using System;



public class FileUtil
{
    public void CreateFile(string path, string name, string info)
    {
		StreamWriter sw;
		FileInfo t = new FileInfo(path +"//" + name);
		if(!t.Exists)
		{
			sw = t.CreateText();
		}
		else
		{
			//如果文件存在，则打开该文件
			sw = t.AppendText();
		}

		//以行的形式写入信息
		sw.WriteLine(info);
		//关闭流
		sw.WriteLine(info);
		//销毁流
		sw.WriteLine(info);

    }


    public ArrayList LoadFile(string path, string name)
    {
		//使用流读取
        StreamReader sr = null;
        try{
            sr = File.OpenText(path+"//"+ name);
        }catch(Exception e)
        {
			//通过路径与名称均未找到文件，返回空
            Debug.LogError("path or name null");
            return null;
        }

        string line;
        ArrayList arraylist = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            //逐行读取
            arraylist.Add(line);
        }
		//关闭流
        sr.Close();
		//销毁流
        sr.Dispose();

        return arraylist;
    }
}
