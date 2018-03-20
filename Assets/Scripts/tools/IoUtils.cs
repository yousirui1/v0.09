using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace tpgm
{
    public class IoUtils
    {

        //#DirectoryNotFoundException: 磁盘不存在, 父文件夹不存在时;
        public static void EnsureDir(string dirAbsPath)
        {
            if (!Directory.Exists(dirAbsPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(dirAbsPath);
            }
        }

        //#DirectoryNotFoundException: 父文件夹不存在时;
        //#UnauthorizedAccessException: 没有权限时;
        public static void EnsureFile(string fileAbsPath)
        {
            if (!File.Exists(fileAbsPath))
            {
                using (FileStream fs = File.Create(fileAbsPath))
                {
                }
            } 
        }

        //#读取文件中的文本;
        public static string ReadStrFromFile(string fileAbsPath)
        {
            //使用流的形式读取
            using (StreamReader sr = File.OpenText(fileAbsPath))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    char[] buffer = new char[1024];
                    int length;
                    while (-1 != (length = sr.Read(buffer, 0, 1024)))
                    {
                        sb.Append(buffer, 0, length);
                    }

                    return sb.ToString();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            return "";
        }

        public static void WriteStrToFile(string absFilePath, string text)
        {
        }

        //public static long GetAllFilesBytes(string dirAbsPath)
        //{
        //    long bytes = 0;
        //    if (!Directory.Exists(dirAbsPath))
        //    {
        //        return 0;
        //    }

        //    DirectoryInfo rootDirInfo = new DirectoryInfo(dirAbsPath);

        //    FileInfo[] subFiles = rootDirInfo.GetFiles();
        //    if (null != subFiles && subFiles.Length > 0)
        //    {
        //        foreach (FileInfo f in subFiles)
        //        {
        //            bytes += f.Length;
        //        }
        //    }


        //    DirectoryInfo[] subDirs = rootDirInfo.GetDirectories();
        //    if (null != subDirs && subDirs.Length > 0)
        //    {
        //        for (int i = 0; i < subDirs.Length; i++)
        //        {
        //            bytes += GetAllFilesBytes(subDirs[i].FullName);
        //        }
        //    }

        //    return bytes;
        //}

        //public static long GetFileBytes(string absFilePath)
        //{
        //    if (!File.Exists(absFilePath))
        //    {
        //        return 0;
        //    }

        //    FileInfo Files = new FileInfo(absFilePath);
        //    return Files.Length;
        //}

        public static void DeleteFile(string absFilePath)
        {
            File.Delete(absFilePath);
        }

        public static void closeSilent(Stream s)
        {
            if (null != s)
            {
                s.Close();
            }
        }

        //    public static void unzip(string zipFilePath, string dstDirPath)
        //    {
        //        ZipInputStream s = new ZipInputStream(File.OpenRead(args[0]));
        //
        //        ZipEntry theEntry;
        //        while ((theEntry = s.GetNextEntry()) != null)
        //        {
        //
        //            string directoryName = Path.GetDirectoryName(args[1]);
        //            string fileName      = Path.GetFileName(theEntry.Name);
        //
        //            //生成解压目录
        //            Directory.CreateDirectory(directoryName);
        //
        //            if (fileName != String.Empty)
        //            {
        //                //解压文件到指定的目录
        //                FileStream streamWriter = File.Create(args[1]+theEntry.Name);
        //
        //                int size = 2048;
        //                byte[] data = new byte[2048];
        //                while (true)
        //                {
        //                    size = s.Read(data, 0, data.Length);
        //                    if (size > 0)
        //                    {
        //                        streamWriter.Write(data, 0, size);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //
        //                streamWriter.Close();
        //            }
        //        }
        //        s.Close();
        //    }

        //    public static void zip(string srcDirPath, string zipFilePath)
        //    {
        //    }

        //#会抛出的异常: DirectoryNotFoundException;
        public static void unZip(string zipFilePath, string unZipDirPath)
        {
            //Encoding gbk = Encoding.GetEncoding("gbk");  
            ZipConstants.DefaultCodePage = 0; //Encoding.UTF8;

            if (!unZipDirPath.EndsWith("/"))
            {
                unZipDirPath += "/";
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string dirName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (dirName.Length > 0)
                    {
                        IoUtils.EnsureDir(unZipDirPath + dirName); //会抛异常;
                    }
                    if (!dirName.EndsWith("/"))
                    {
                        dirName += "/";
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(unZipDirPath + theEntry.Name))
                        {
                            int size = 2048;  
                            byte[] data = new byte[2048];  
                            while (true)
                            {  
                                size = s.Read(data, 0, data.Length);  
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);  
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private IoUtils()
        {
        }


    }
}
