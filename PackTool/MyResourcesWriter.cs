/* 基本思路：
 * 1.每个文件使用byte[]保存
 * 2.文件的名字和在数据包中的位置存储在字典中
 * 2.在把数据包写入磁盘时，会把字典加入到数据包尾端
 * 
 * 待优化：
 * 1.AddResources与WritePack整合，
 * 
 * 难点：
 * 1.找到数据包中字典所在的位置。
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyResources
{
    public class MyResourcesWriter
    {
        //资源包的路径
        private string _Path;
        public string Path{ get { return _Path; }}
        //资源包的数据
        private byte[] myBytes;

        //private int fileCount;
        //用于查询的字典，将文件的名字和文件在资源包中的数据空间绑定保存
        private Dictionary<string, FileSpace> FilesDir;

        //每个文件的数据空间
        public class FileSpace
        {
            public int start;
            public int end;
            public FileSpace(int s, int e) { start = s; end = e; }
            public int GetLength() { return end - start; }
        }

        public MyResourcesWriter (string path)
        {
            _Path = path;
        }

        //将文件打包
        public void AddResources(string name,byte[] file)
        {
            //把当前文件的信息存入字典
            FileSpace currentFile = new FileSpace(myBytes.Length, myBytes.Length + file.Length);
            FilesDir.Add(name, currentFile);

            //根据要合并的两个数组元素总数新建一个数组
            byte[] newArray = new byte[myBytes.Length + file.Length];
            Array.Copy(myBytes, 0, newArray, 0, myBytes.Length);
            Array.Copy(file, 0, newArray, file.Length, file.Length);

            myBytes = newArray;

            //File.WriteAllBytes(Path, myBytes);
        }

        //将数据包写入磁盘
        public void WritePack()
        {
            //将字典数据化并存入资源包最后
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, FilesDir);
            byte[] dicByte = ms.GetBuffer();

            byte[] newArray = new byte[myBytes.Length + dicByte.Length];
            Array.Copy(myBytes, 0, newArray, 0, myBytes.Length);
            Array.Copy(dicByte, 0, newArray, dicByte.Length, dicByte.Length);

            myBytes = newArray;
            File.WriteAllBytes(Path, myBytes);
        }

        /*
        //根据名字获取相应资源
        public byte[] GetFile(string name)
        {
            FileSpace targetSpace = FilesDir[name];
            byte[] targetFile = new byte[targetSpace.GetLength()];
            Array.Copy(myBytes, targetSpace.start, targetFile, 0, targetSpace.GetLength());
            return targetFile;
        }
        */

    }
}
