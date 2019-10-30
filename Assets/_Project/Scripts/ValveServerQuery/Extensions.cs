using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ValveServerQuery
{
    public static class Extensions
    {
        public static void SendMessage(this Socket socket,int mesType,byte[] data)
        {
            socket.Send(Common.WriteMessage(data,mesType));
        }
        public static void SendMessage(this Socket socket,int mesType,string data)
        {
            socket.Send(Common.WriteMessage(data.ToBytes(), mesType));
        }
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public static string ToStr(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
    public class Common
    {
        //数据格式：类型+长度+字符数组
        public static byte[] WriteMessage(byte[] message, int mesType = 0)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                int msglen = message.Length;
                writer.Write(mesType);
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                return ms.ToArray();
            }
        }
    }
}
