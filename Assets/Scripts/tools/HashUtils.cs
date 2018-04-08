using UnityEngine;
using System.Collections;

//#hash码的生成相关的代码都放在这边;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;


public class HashUtils
{
    /// <summary>
    /// 为字符串生成md5码;
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string getHash(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        string md5 = getHash(bytes);
        return md5;
    }

//    public static string getStrMd52(string str)
//    {
//        byte[] bytes = Encoding.UTF8.GetBytes(str);
//        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
//        byte[] md5Bytes = md5.ComputeHash(bytes);
//        //string result = Encoding.UTF8.GetString(md5Bytes); //这个是不行的;
//        //string result = bytesToHexString(md5Bytes);
//        string result = BitConverter.ToString(md5Bytes).Replace("-", "").ToLower();
//        return result;
//    }

    /// <summary>
    /// 为字节生成md5码;
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string getHash(byte[] bytes)
    {
        MD5 md5 = MD5.Create();
        byte[] md5Bytes = md5.ComputeHash(bytes);
        string result = bytesToHexString(md5Bytes);
        return result;
    }

    //#会抛出IOException;
    public static string getFileHashOrThrow(string filePath)
    {
        //#文件如果不存在则抛出FileNotFoundException;
        using (FileStream fs = File.OpenRead(filePath))
        {
            return getStreamHash(fs);
        }
    }

    public static string getFileHashOrThrow(FileInfo fi)
    {
        using (FileStream fs = fi.OpenRead())
        {
            return getStreamHash(fs);
        }
    }

    //#Stream不会被Close;
    static string getStreamHash(Stream s)
    {
        MD5 md5 = MD5.Create();
        byte[] md5Bytes = md5.ComputeHash(s);
        string result = bytesToHexString(md5Bytes);
        return result;
    }

    public static string bytesToHexString(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2")); //sb.Append(b.ToString("X").PadLeft(2, '0')); 
        }

        return sb.ToString().ToUpper();
    }

    private HashUtils()
    {
    }
}
