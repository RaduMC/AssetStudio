/*
Copyright (c) 2016 Radu

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

/*
DISCLAIMER
The reposiotory, code and tools provided herein are for educational purposes only.
The information not meant to change or impact the original code, product or service.
Use of this repository, code or tools does not exempt the user from any EULA, ToS or any other legal agreements that have been agreed with other parties.
The user of this repository, code and tools is responsible for his own actions.

Any forks, clones or copies of this repository are the responsability of their respective authors and users.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Unity_Studio
{
    public enum EndianType
    {
        BigEndian,
        LittleEndian
    }

    public class EndianStream : BinaryReader
    {
        public EndianType endian;
        private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];

        public EndianStream(Stream stream, EndianType endian) : base(stream) { }

        ~EndianStream()
        {
            Dispose();
        }

        public long Position { get { return base.BaseStream.Position; } set { base.BaseStream.Position = value; } }

        public new void Dispose()
        {
            base.Dispose();
        }

        public override bool ReadBoolean()
        {
            return base.ReadBoolean();
        }

        public override byte ReadByte()
        {
            try
            {
                return base.ReadByte();
            }
            catch
            {
                return 0;
            }
        }

        public override char ReadChar()
        {
            return base.ReadChar();
        }
 
        public override Int16 ReadInt16()
        {
            if (endian == EndianType.BigEndian)
            {
                a16 = base.ReadBytes(2);
                Array.Reverse(a16);
                return BitConverter.ToInt16(a16, 0);
            }
            else return base.ReadInt16();
        }
 
        public override int ReadInt32()
        {
            if (endian == EndianType.BigEndian)
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToInt32(a32, 0);
            }
            else return base.ReadInt32();
        }
 
        public override Int64 ReadInt64()
        {
            if (endian == EndianType.BigEndian)
            {
                a64 = base.ReadBytes(8);
                Array.Reverse(a64);
                return BitConverter.ToInt64(a64, 0);
            }
            else return base.ReadInt64();
        }
 
        public override UInt16 ReadUInt16()
        {
            if (endian == EndianType.BigEndian)
            {
                a16 = base.ReadBytes(2);
                Array.Reverse(a16);
                return BitConverter.ToUInt16(a16, 0);
            }
            else return base.ReadUInt16();
        }
 
        public override UInt32 ReadUInt32()
        {
            if (endian == EndianType.BigEndian)
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToUInt32(a32, 0);
            }
            else return base.ReadUInt32();
        }

        public override UInt64 ReadUInt64()
        {
            if (endian == EndianType.BigEndian)
            {
                a64 = base.ReadBytes(8);
                Array.Reverse(a64);
                return BitConverter.ToUInt64(a64, 0);
            }
            else return base.ReadUInt64();
        }
 
        public override Single ReadSingle()
        {
            if (endian == EndianType.BigEndian)
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToSingle(a32, 0);
            }
            else return base.ReadSingle();
        }
 
        public override Double ReadDouble()
        {
            if (endian == EndianType.BigEndian)
            {
                a64 = base.ReadBytes(8);
                Array.Reverse(a64);
                return BitConverter.ToUInt64(a64, 0);
            }
            else return base.ReadDouble();
        }

        public override string ReadString()
        {
            return base.ReadString();
        }

        public string ReadASCII(int length)
        {
            return ASCIIEncoding.ASCII.GetString(base.ReadBytes(length));
        }

        public void AlignStream(int alignment)
        {
            long pos = base.BaseStream.Position;
            //long padding = alignment - pos + (pos / alignment) * alignment;
            //if (padding != alignment) { base.BaseStream.Position += padding; }
            if ((pos % alignment) != 0) { base.BaseStream.Position += alignment - (pos % alignment); }
        }

        public string ReadAlignedString(int length)
        {
            if (length > 0 && length < (base.BaseStream.Length - base.BaseStream.Position))//crude failsafe
            {
                byte[] stringData = new byte[length];
                base.Read(stringData, 0, length);
                var result = System.Text.Encoding.UTF8.GetString(stringData); //must verify strange characters in PS3

                /*string result = "";
                char c;
                for (int i = 0; i < length; i++)
                {
                    c = (char)base.ReadByte();
                    result += c.ToString();
                }*/

                AlignStream(4);
                return result;
            }
            else { return ""; }
        }
 
        public string ReadStringToNull()
        {
            string result = "";
            char c;
            for (int i = 0; i < base.BaseStream.Length; i++)
            {
                if ((c = (char)base.ReadByte()) == 0)
                {
                    break;
                }
                result += c.ToString();
            }
            return result;
        }
    }
}
