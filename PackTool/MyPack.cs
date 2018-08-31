/* 打包文件的实现：
 * 
 * 1.文件包格式：文件头+文件数据+文件头+文件数据.....
 * 2.头文件以结构体转byte[]形式存入文件包，固定分配10kb空间
 * 3.提供两个外部方：1.添加文件。2.输出文件包
 * 
 * 优化：
 * 1.取消文件头固定空间。在文件头之前使用特定字符串如“10]”，保存文件头的长度
 * 
 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MyPack
{
    public class MyPackTool:IDisposable 
    {
        private byte[] packByte;        //文件包
        private int positionCount;      //用于计算当前写入文件包的位置

        //public const int headLength = 1024*10;      //头文件的固定长度

        //头文件保存：1.文件名。2.文件数据起始位置。3.文件数据长度。
        //当前保存的变量主要为了实现需求：输入文件名获取文件数据。
        public struct HeadInfo
        {
            public string name;
            public int start;
            public int length;
            public HeadInfo(string n, int s, int l) { name = n; start = s; length = l; }
        }

        //外部方法：添加一个文件到包
        public void AddFile(string name,byte[] file)
        {
            //记录头文件信息并转为byte[]
            HeadInfo currentInfo = new HeadInfo(name,positionCount,file.Length);
            byte[] structByte = StructToBytes(currentInfo);

            //将文件信息和文件数据写入包中
            CopyToPackByte(structByte, file);
        }

        //将文件头和文件数据写入PackByte
        private void CopyToPackByte(byte[] head, byte[]file)
        {

            //把文件头的长度写入，以“]”结束
            int headLength = head.Length;
            string headInfo = headLength.ToString() + "]";
            byte[] hhByte = System.Text.Encoding.Default.GetBytes(headInfo);

            //文件内容写入
            byte[] temp;
            if (packByte != null)
            {
                //如果不是第一次写入，则把之前的文件包数据写入
                temp = new byte[packByte.Length + headLength + file.Length+ hhByte.Length];
                Array.Copy(packByte, 0, temp, 0, packByte.Length);
            }
            else
            {
                temp = new byte[headLength + file.Length+ hhByte.Length];
            }
            Array.Copy(hhByte, 0, temp, positionCount, headInfo.Length);
            Array.Copy(head, 0, temp, positionCount+ hhByte.Length, head.Length);
            Array.Copy(file, 0, temp, positionCount + headLength+ hhByte.Length, file.Length);
            packByte = temp;

            //文件包写入位置移动一个文件的空间
            positionCount += (headLength + file.Length+ hhByte.Length);
        }

        //输出文件包
        public void WritePack(string path)
        {
            File.WriteAllBytes(path, packByte);
        }

        //结构体转化为数组
        public static Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, buffer, false);
            Byte[] bytes = new Byte[size];
            Marshal.Copy(buffer, bytes, 0, size);
            return bytes;

        }

        public void Dispose()
        {
            packByte = null;
        }
    }

}
