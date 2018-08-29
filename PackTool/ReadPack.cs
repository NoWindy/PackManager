using System;
using System.Resources;
using System.Collections;

namespace ReadPack
{
    public class ReadPack
    {
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
