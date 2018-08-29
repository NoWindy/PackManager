using System;
using System.Resources;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ReadPackTool
{
    public static class ReadPack
    {
        //默认的资源包文件夹
        public const string PackPath = "PackFolder";
        //资源包列表
        private static List<string> PackList = new List<string>();

        //通过文件名获取文件内容
        public static byte[] GetFile(string resName)
        {
            byte[] result = null;
            UpdatePackList();
            foreach (var pack in PackList)
            {
                using (ResourceReader rr = new ResourceReader(pack))
                {
                    foreach (DictionaryEntry entry in rr)
                    {
                        if ((string)entry.Key == resName)
                        {
                            result = entry.Value as byte[];
                            break;
                        }
                    }
                }
            }
            return result;
        }

        //从目录中获取资源包
        private static void UpdatePackList()
        {
            DirectoryInfo dir = new DirectoryInfo(PackPath);
            FileInfo[] files = dir.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".pck"&&!PackList.Contains(file.FullName))
                    PackList.Add(file.FullName);
            }
        }

        ////根据名字查找对应的文件，以byte[]形式输出
        //public static byte[] GetFile(string resName, string packPath)
        //{
        //    byte[] result = null;
        //    using (ResourceReader rr = new ResourceReader(packPath))
        //    {
        //        foreach (DictionaryEntry entry in rr)
        //        {
        //            if ((string)entry.Key == resName)
        //            {
        //                result = entry.Value as byte[];
        //                break;
        //            }
        //        }
        //    }
        //    return result;
        //}

        ////批量加载文件
        //public static List<byte[]> GetFiles(string packPath, params string[] names)
        //{
        //    List<byte[]> result = new List<byte[]>();
        //    using (ResourceReader rr = new ResourceReader(packPath))
        //    {
        //        for (int i = 0; i < names.Length; i++)
        //        {
        //            foreach (DictionaryEntry entry in rr)
        //            {
        //                if ((string)entry.Key == names[i])
        //                {
        //                    result.Add(entry.Value as byte[]);
        //                    break;
        //                }
        //            }
        //        }

        //    }
        //    return result;
        //}
    }
}
