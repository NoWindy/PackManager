using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Resources;

namespace PackTool
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    public static class ResPacker
    {

        public static void PackAllFile(string path,string targetFilePath)
        {
            List<string> filesList = new List<string>();
            GetFile(path, filesList);
            Dictionary<string, byte[]> dicToPack = new Dictionary<string, byte[]();
            foreach (string filePath in filesList)
            {
                dicToPack.Add(Path.GetFileName(filePath), File.ReadAllBytes(filePath));
            }
            Pack(dicToPack);

        }

        public static void Pack(Dictionary<string, byte[]> objCollection, string targetFilePath = "MyRes.pck")
        {

            using (ResourceWriter rw = new ResourceWriter(targetFilePath))
            {
                foreach (KeyValuePair<string, byte[]> pair in objCollection)
                    rw.AddResource(pair.Key, pair.Value);
                rw.Generate();
                rw.Close();
            }
        }

        //根据路径获取所有子文件
        public static List<string> GetFile(string path, List<string> FileList)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();

            //获取子文件
            foreach (FileInfo f in fil)
            {
                long size = f.Length;
                FileList.Add(f.FullName);
            }

            //获取子文件夹内的文件列表，递归遍历
            foreach (DirectoryInfo d in dii)
            {
                GetFile(d.FullName, FileList);
            }
            return FileList;
        }

    }
}
