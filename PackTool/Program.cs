﻿using System;
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

            Console.Write("Path: ");
            string path = Console.ReadLine();
            Console.Write("PackName: ");
            string packName = Console.ReadLine();
            Console.Write("PackPath: ");
            string packPath = Console.ReadLine();

            string targetFilePath = "PackFolder/"+packName + ".pck";

            //string path = "1";
            //string targetFilePath = "PackFolder/MyPack.pck";
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

            MyPackTool myPack = new MyPackTool();
            foreach (string filePath in filesList)
            {
                myPack.AddFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath));
            }
            myPack.WritePack(targetFilePath);
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
