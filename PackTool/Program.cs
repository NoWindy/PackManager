using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Resources;
using ReadPackTool;

namespace PackTool
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.Write("Path: ");
            string path = Console.ReadLine();
            Console.Write("PackName: ");
            string packName = Console.ReadLine();
            //Console.Write("PackPath: ");
            //string packPath = Console.ReadLine();

            string targetFilePath = "PackFolder/"+packName + ".pck";
            //if (packPath == "")
            //    targetFilePath = packName + ".pck";
            //else
            //    targetFilePath = packPath + "/" + packName + ".pck";

            ResPacker.PackAllFile(path, targetFilePath);

            File.WriteAllBytes("b.jpg", ReadPack.GetFile("a.jpg"));
        }
    }

    public static class ResPacker
    {
        //打包文件夹内的所有文件
        public static void PackAllFile(string path,string targetFilePath)
        {
            //搜索出所有子文件并把路径保存到列表List中。
            List<string> filesList = new List<string>();
            GetFile(path, filesList);
            //根据List把文件的名字和内容(byte[])存入字典
            Dictionary<string, byte[]> dicToPack = new Dictionary<string, byte[]>();
            foreach (string filePath in filesList)
            {
                dicToPack.Add(Path.GetFileName(filePath), File.ReadAllBytes(filePath));
            }
            //将字典打包为pck文件
            Pack(dicToPack,targetFilePath);

        }

        public static void Pack(Dictionary<string, byte[]> objCollection, string targetFilePath = "MyRes.pck")
        {
            //打包：以名字-内容互相绑定的形式写入
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
