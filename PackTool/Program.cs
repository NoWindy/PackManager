using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Resources;
using MyPack;
using PackReader;
using System.Text;

namespace PackTool
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*
            Console.Write("Path: ");
            string path = Console.ReadLine();
            Console.Write("PackName: ");
            string packName = Console.ReadLine();
            //Console.Write("PackPath: ");
            //string packPath = Console.ReadLine();

            string targetFilePath = "PackFolder/"+packName + ".pck";
            */

            string path = "1";
            string targetFilePath = "PackFolder/MyPack.pck";
            ResPacker.PackAllFile(path, targetFilePath);

            MyPackReader reader = new MyPackReader(targetFilePath);
            File.WriteAllBytes("b.jpg",reader.GetFile("a.jpg"));
            File.WriteAllBytes("a.mp3", reader.GetFile("Amaranth.mp3"));

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

        //将字典中文件打包成一个
        public static void Pack(Dictionary<string, byte[]> objCollection, string targetFilePath = "MyRes.pck")
        {
            MyPackTool mp = new MyPackTool();
            foreach (KeyValuePair<string, byte[]> pair in objCollection)
            {
                //文件名和文件数据传入
                mp.AddFile(pair.Key, pair.Value);
            }
            //所有文件添加完毕后输出文件包
            mp.WritePack(targetFilePath);
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
