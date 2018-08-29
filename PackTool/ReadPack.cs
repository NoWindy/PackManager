using System;
using System.Resources;
using System.Collections;

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
    }
}
