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
        /// <summary>
        /// 批量打包任意对象到资源文件
        /// </summary>
        /// <param name="objCollection">被打包对象的列表。键值对中键为其在资源文件中的唯一标示名。</param>
        /// <param name="targetFilePath">目标资源文件。默认参数为当前目录下的"MyRes.pck"文件。</param>
        /// <param name="overwrite">是否覆盖已存在的目标文件。默认=True</param>
        public static void Pack(IDictionary<string, object> objCollection, string targetFilePath = "MyRes.pck", bool overwrite = true)
        {
            if (overwrite)
                File.Delete(targetFilePath);
            using (ResourceWriter rw = new ResourceWriter(targetFilePath))
            {
                foreach (KeyValuePair<string, object> pair in objCollection)
                    //为了防传进来的资源名有数字开头，资源名都加了前缀_
                    rw.AddResource("_" + pair.Key, pair.Value);
                rw.Generate();
                rw.Close();
            }
        }
        /// <summary>
        /// 解包资源文件，返回所有资源及其资源名
        /// </summary>
        /// <param name="targetFilePath">要解包的资源文件。默认为当前目录下的"MyRes.pck"</param>
        /// <returns>资源字典，键值为资源唯一标示名。若无资源返回空集合。</returns>
        public static Dictionary<string, object> GetAllResources(string targetFilePath = "MyRes.pck")
        {
            Dictionary<string, object> rtn = new Dictionary<string, object>();
            using (ResourceReader rr = new ResourceReader(targetFilePath))
            {
                foreach (DictionaryEntry entry in rr)
                    rtn.Add(((string)entry.Key).Substring(1), entry.Value);
            }
            return rtn;
        }
        /// <summary>
        /// 根据资源名在指定的资源文件中检索资源
        /// </summary>
        /// <param name="resName">资源名</param>
        /// <param name="targetFilePath">要在其中检索的资源文件名，默认为"MyRes.pck"</param>
        /// <returns>资源名对应的资源</returns>
        public static object SearchResource(string resName, string targetFilePath = "MyRes.pck")
        {
            object rtn = null;
            using (ResourceReader rr = new ResourceReader(targetFilePath))
            {
                foreach (DictionaryEntry entry in rr)
                    if ((string)entry.Key == '_' + resName)
                    {
                        rtn = entry.Value;
                        break;
                    }
            }
            return rtn;
        }
    }
}
