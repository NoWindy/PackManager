/* 解包流程：
 * 1.找“]”字符，获取文件头长度
 * 2.读取文件头，将信息存入字典
 * 3.空间位置移动到下一个文件的开头，重复第一步
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PackReader
{
    public class MyPackReader : IDisposable
    {
        //要解析的文件包
        private byte[] packByte;

        //文件头结构体
        public struct HeadInfo
        {
            public string name;
            public int start;
            public int length;
            public HeadInfo(string n, int s, int l) { name = n; start = s; length = l; }
        }

        //文件名和头文件信息绑定
        private Dictionary<string, HeadInfo> filesDic = new Dictionary<string, HeadInfo>();

        //构造函数：资源包解析。初始化主要目的是把资源包中所有文件头信息保存到字典中。
        public MyPackReader(string packPath)
        {
            int readPosition = 0;
            packByte = File.ReadAllBytes(packPath);

            //读取整个资源包，将所有头文件记录到字典中。
            while (readPosition<packByte.Length)
            {
                //获取文件头的长度
                int hh = 4;
                byte[] hhByte = new byte[4];
                Array.Copy(packByte, readPosition, hhByte, 0, 4);
                int headLength = hhByte[0] + hhByte[1] * 256 + hhByte[2] * 256 * 256 + hhByte[3] * 256 * 256 * 256;

                //获取文件头信息，存入字典
                byte[] headByte = new byte[headLength];
                Array.Copy(packByte, readPosition + hh, headByte, 0, headLength);
                HeadInfo currentHead = (HeadInfo)BytesToStruct(headByte,typeof(HeadInfo));
                currentHead.start += hh + headLength;
                filesDic.Add(currentHead.name, currentHead);
                //移动到下一个文件空间
                readPosition = readPosition + headLength+ currentHead.length+hh;
            }
        }

        //外部方法：根据文件名获取文件数据
        public Byte[] GetFile(string name)
        {
            //先从缓存区查找文件
            if (MyCache.GetInstance().GetCacheFile(name)!=null)
                return MyCache.instance.GetCacheFile(name);

            //根据文件头信息读出文件
            HeadInfo targetHead = filesDic[name];
            byte[] result = new byte[targetHead.length];
            Array.Copy(packByte, targetHead.start, result, 0, targetHead.length);

            //当前文件进行一次规律判断
            MyCache.GetInstance().RuleDetection(name, result);

            return result;
        }

        //将byte[]转化为struct
        private static Object BytesToStruct(Byte[] bytes, Type strcutType)
        {
            Int32 size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, buffer, size);
            return Marshal.PtrToStructure(buffer, strcutType);
        }

        //清理缓存
        public void Dispose()
        {
            packByte = null;
        }
    }
}
