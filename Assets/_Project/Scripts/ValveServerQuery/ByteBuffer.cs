﻿using System.IO;
using System.Text;
using System;

namespace ValveServerQuery
{
    /// <summary>
    /// Abstraction of byte buffer used by server-client system
    /// </summary>
    public class ByteBuffer
    {
        MemoryStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;

        public ByteBuffer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public ByteBuffer(byte[] data)
        {
            if (data != null)
            {
                stream = new MemoryStream(data);
                reader = new BinaryReader(stream);
            }
            else
            {
                stream = new MemoryStream();
                writer = new BinaryWriter(stream);
            }
        }

        public void Close()
        {
            if (writer != null) writer.Close();
            if (reader != null) reader.Close();

            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        public void WriteByte(byte v)
        {
            writer.Write(v);
        }

        public void WriteInt(int v)
        {
            writer.Write((int)v);
        }

        public void WriteShort(ushort v)
        {
            writer.Write((ushort)v);
        }

        public void WriteLong(long v)
        {
            writer.Write((long)v);
        }

        public void WriteFloat(float v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToSingle(temp, 0));
        }

        public void WriteDouble(double v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToDouble(temp, 0));
        }

        public void WriteString(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            writer.Write(bytes);
            writer.Write((byte)0x00);
        }
        public void WriteString(string v, bool length)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            if (length) writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public void WriteBytes(byte[] v)
        {
            writer.Write((int)v.Length);
            writer.Write(v);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public int ReadInt()
        {
            return (int)reader.ReadInt32();
        }

        public short ReadShort()
        {
            return reader.ReadInt16();
        }

        public long ReadLong()
        {
            return (long)reader.ReadInt64();
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        public string ReadString()
        {
            byte[] buffer = new byte[1024];
            int i = 0;
            while (true)
            {
                var c = reader.ReadByte();
                buffer[i++] = c;
                if (c == 0x00)
                {
                    break;
                }
            }

            byte[] ret = new byte[i];
            for (int j = 0; j < i; j++)
            {
                ret[j] = buffer[j];
            }
            return Encoding.UTF8.GetString(ret);
        }

        public string ReadString(int len)
        {
            byte[] buffer = new byte[len];
            buffer = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }

        public byte[] ReadBytes(int len)
        {
            return reader.ReadBytes(len);
        }

        public byte[] ToBytes()
        {
            return stream.ToArray();
        }

        public void Flush()
        {
            writer.Flush();
        }

        public string VisualizeData()
        {
            string s = "";
            foreach (var c in ToBytes())
            {
                switch (c)
                {
                    case 0xff: s += " 0xff "; break;
                    case 0x00: s += " 0x00 "; break;
                    default: s += (char)c; break;
                }
            }

            return s;
        }
    }
}