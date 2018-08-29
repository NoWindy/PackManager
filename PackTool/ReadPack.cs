using System;
using System.Resources;
using System.Collections;
using System.Collections.Generic;

namespace ReadPackTool
{
    public static class ReadPack
    {
        //根据名字查找对应的文件，以byte[]形式输出
        public static byte[] GetFile(string resName, string packPath)
        {
            byte[] result = null;
            using (ResourceReader rr = new ResourceReader(packPath))
            {
                foreach (DictionaryEntry entry in rr)
                    if ((string)entry.Key == resName)
                    {
                        result = entry.Value as byte[];
                        break;
                    }
            }
            return result;
        }

        //批量加载文件
        public static List<byte[]> GetFiles(string packPath, params string[] names)
        {
            List<byte[]> result = new List<byte[]>();
            using (ResourceReader rr = new ResourceReader(packPath))
            {
                for (int i = 0; i < names.Length; i++)
                {
                    foreach (DictionaryEntry entry in rr)
                    {
                        if ((string)entry.Key == names[i])
                        {
                            result.Add(entry.Value as byte[]);
                            break;
                        }
                    }
                }

            }
            return result;
        }
    }
}
