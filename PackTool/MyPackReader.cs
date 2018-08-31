using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PackReader
{
    public class MyPackReader
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

        //private const int headLength = 1024 * 10;        //头文件固定长度

        //文件名和头文件信息绑定
        private Dictionary<string, HeadInfo> filesDic = new Dictionary<string, HeadInfo>();

        //构造函数：资源包解析
        public MyPackReader(string packPath)
        {
            int readPosition = 0;
            packByte = File.ReadAllBytes(packPath);

            //读取整个资源包，将所有头文件记录到字典中。
            while (readPosition<packByte.Length)
            {
                int hh = 0;
                int headLength = 0;
                for (int i = readPosition; i < packByte.Length; i++)
                {
                    //找第一个“]”,前面的数据即为头文件的长度
                    if(packByte[i]==93)
                    {
                        hh = i - readPosition+1;
                        headLength = int.Parse(Encoding.Default.GetString(packByte, readPosition, hh-1));
                        break;
                    }
                }
                byte[] headByte = new byte[headLength];
                Array.Copy(packByte, readPosition+hh, headByte, 0, headLength);
                HeadInfo currentHead = (HeadInfo)BytesToStruct(headByte,typeof(HeadInfo));
                currentHead.start += hh + headLength;
                filesDic.Add(currentHead.name, currentHead);
                readPosition = readPosition + headLength+ currentHead.length+hh;
            }
        }

        //外部方：根据文件名获取文件数据
        public Byte[] GetFile(string name)
        {
            HeadInfo targetHead = filesDic[name];
            byte[] result = new byte[targetHead.length];
            Array.Copy(packByte, targetHead.start, result, 0, targetHead.length);
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

    }
}
