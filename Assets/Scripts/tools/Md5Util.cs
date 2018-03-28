using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Md5Util : MonoBehaviour
{

    private string fileName = "1.txt";//streamingAssets 文件夹下面的文件名称


    public static string GetMd5FromStr(string passwd)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(passwd);//将字符串转换为字节数组
        return GetMd5FromBytes(data);
    }


    /// <summary>
    /// 将一个文件转换为md5字符串，并保存
    /// </summary>
    /// <param name="fileName">File name.</param>
    private void PaseFile(string fileName)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);//文件路径
        string fileMd5Path = System.IO.Path.Combine(Application.streamingAssetsPath, "md5_" + fileName);//md5 存储路径
        if (System.IO.File.Exists(filePath))
        {
            using (System.IO.FileStream stream = System.IO.File.Open(filePath, System.IO.FileMode.Open))
            {
                stream.Position = 0;//从文件首部开始
                string md5 = GetMd5FromStream(stream);//获取文件对应的md5数据
                System.IO.FileStream fs = new System.IO.FileStream(fileMd5Path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Write);
                byte[] buff = System.Text.Encoding.UTF8.GetBytes(md5);
                fs.Write(buff, 0, buff.Length);//保存生成的md5信息
                fs.Close();
                Debug.Log("<color=#ff0000> change over</color>");
            }

        }


    }
    /// <summary>
    /// 
    /// 通过数据流获取对应的md5文件
    /// </summary>
    /// <returns>The md5.</returns>
    /// <param name="stream">Stream.</param>
    private static string GetMd5FromStream(System.IO.FileStream stream)
    {
        byte[] buff;
        using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
        {
            buff = md5.ComputeHash(stream);

        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (var item in buff)
        {
            builder.Append(item.ToString("x2").ToLower());//把二进制的字节，转换为16进制的数字形式的字符串
        }
        string res = builder.ToString();
        Debug.Log(res);
        return res;
    }


    /// <summary>
    /// 通过字节数组获取md5字符串
    /// </summary>
    /// <returns>The md5 from bytes.</returns>
    /// <param name="data">Data.</param>
    private static string GetMd5FromBytes(byte[] data)
    {
        byte[] buff;
        using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
        {
            buff = md5.ComputeHash(data);
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (var item in buff)
        {
            builder.Append(item.ToString("x2").ToLower());//把二进制的字节，转换为16进制的数字形式的字符串
        }
        string res = builder.ToString();
        //Debug.Log(res);
        return res;

    }


}