using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;

/**************************************
*FileName: CryptUtils.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 3DES加解密
**************************************/

public class CryptUtils
{

    //================================================== 3des begin;

    //# 参考: http://liuzongan.iteye.com/blog/443069

    //#使用提供的key加密字符串, 并得到base64加密串;
    public static string Encrypt3DesStr(string cryptKey, string input)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(input); //将字符串转为utf8编码的bytes;

        byte[] encryptedBytes = Encrypt3DesBytes(cryptKey, strBytes);

        string output = Convert.ToBase64String(encryptedBytes);
        return output;
    }

    public static string Encrypt3DesStr(string cryptKey, byte[] bytes)
    {
        byte[] encryptedBytes = Encrypt3DesBytes(cryptKey, bytes);

        string output = Convert.ToBase64String(encryptedBytes);
        return output;
    }

    public static string Encrypt3DesStr(string cryptKey, byte[] bytes, int startIdx, int length)
    {
        byte[] encryptedBytes = Encrypt3DesBytes(cryptKey, bytes, startIdx, length);

        string output = Convert.ToBase64String(encryptedBytes);
        return output;
    }

    public static byte[] Encrypt3DesBytes(string cryptKey, string input)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(input); //将字符串转为utf8编码的bytes;

        return Encrypt3DesBytes(cryptKey, strBytes);
    }

    public static byte[] Encrypt3DesBytes(string cryptKey, byte[] bytes)
    {
        return Encrypt3DesBytes(cryptKey, bytes, 0, bytes.Length);
    }

    public static byte[] Encrypt3DesBytes(string cryptKey, byte[] bytes, int startIdx, int length)
    {
        TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
        //DES.Key = ASCIIEncoding.ASCII.GetBytes(cryptKey);
        DES.Key = Encoding.UTF8.GetBytes(cryptKey);
        DES.Mode = CipherMode.ECB;
        DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        ICryptoTransform DESEncrypt = DES.CreateEncryptor();

        //byte[] strBytes = ASCIIEncoding.ASCII.GetBytes(input);
        byte[] encryptedBytes = DESEncrypt.TransformFinalBlock(bytes, startIdx, length);
        return encryptedBytes;
    }

    //==========

    //#使用提供的key解密base64加密串;
    public static string Decrypt3DesStr(string cryptKey, string input)
    {
        byte[] encryptedBytes = Convert.FromBase64String(input);

        byte[] bytes = Decrypt3DesBytes(cryptKey, encryptedBytes);

        string output = Encoding.UTF8.GetString(bytes); //将bytes按照utf8编码转为字符串;
        //string output = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(bytesHasEncrypt, 0, bytesHasEncrypt.Length));
        return output;
    }

    public static string Decrypt3DesStr(string cryptKey, byte[] encryptedBytes)
    {
        byte[] bytes = Decrypt3DesBytes(cryptKey, encryptedBytes);

        string output = Encoding.UTF8.GetString(bytes); //将bytes按照utf8编码转为字符串;
        //string output = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(bytesHasEncrypt, 0, bytesHasEncrypt.Length));
        return output;
    }

    public static string Decrypt3DesStr(string cryptKey, byte[] encryptedBytes, int startIdx, int length)
    {
        byte[] bytes = Decrypt3DesBytes(cryptKey, encryptedBytes, 0, encryptedBytes.Length);

        string output = Encoding.UTF8.GetString(bytes); //将bytes按照utf8编码转为字符串;
        //string output = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(bytesHasEncrypt, 0, bytesHasEncrypt.Length));
        return output;
    }

    public static byte[] Decrypt3DesBytes(string cryptKey, string input)
    {
        byte[] encryptedBytes = Convert.FromBase64String(input);

        return Decrypt3DesBytes(cryptKey, encryptedBytes);
    }

    public static byte[] Decrypt3DesBytes(string cryptKey, byte[] encryptedBytes)
    {
        return Decrypt3DesBytes(cryptKey, encryptedBytes, 0, encryptedBytes.Length);
    }

    public static byte[] Decrypt3DesBytes(string cryptKey, byte[] encryptedBytes, int startIdx, int length)
    {
        TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
        //DES.Key = ASCIIEncoding.ASCII.GetBytes(cryptKey);
        DES.Key = Encoding.UTF8.GetBytes(cryptKey);
        DES.Mode = CipherMode.ECB;
        DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

        ICryptoTransform DESDecrypt = DES.CreateDecryptor();
        byte[] bytesNotEncrypt = DESDecrypt.TransformFinalBlock(encryptedBytes, startIdx, length);
        return bytesNotEncrypt;
    }

    //================================================== 3des end;

    private CryptUtils()
    {

    }
}
