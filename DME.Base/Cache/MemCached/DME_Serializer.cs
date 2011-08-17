using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace DME.Base.Cache.MemCached
{
    internal enum DME_SerializedType : ushort
    {
        ByteArray = 0,
        Object = 1,
        String = 2,
        Datetime = 3,
        Bool = 4,
        //SByte		= 5, //Makes no sense.
        Byte = 6,
        Short = 7,
        UShort = 8,
        Int = 9,
        UInt = 10,
        Long = 11,
        ULong = 12,
        Float = 13,
        Double = 14,

        CompressedByteArray = 255,
        CompressedObject = 256,
        CompressedString = 257,
    }
    internal class DME_Serializer
    {
        public static byte[] Serialize(object value, out DME_SerializedType type, uint compressionThreshold)
        {
            byte[] bytes;
            if (value is byte[])
            {
                bytes = (byte[])value;
                type = DME_SerializedType.ByteArray;
                if (bytes.Length > compressionThreshold)
                {
                    bytes = compress(bytes);
                    type = DME_SerializedType.CompressedByteArray;
                }
            }
            else if (value is string)
            {
                bytes = Encoding.UTF8.GetBytes((string)value);
                type = DME_SerializedType.String;
                if (bytes.Length > compressionThreshold)
                {
                    bytes = compress(bytes);
                    type = DME_SerializedType.CompressedString;
                }
            }
            else if (value is DateTime)
            {
                bytes = BitConverter.GetBytes(((DateTime)value).Ticks);
                type = DME_SerializedType.Datetime;
            }
            else if (value is bool)
            {
                bytes = new byte[] { (byte)((bool)value ? 1 : 0) };
                type = DME_SerializedType.Bool;
            }
            else if (value is byte)
            {
                bytes = new byte[] { (byte)value };
                type = DME_SerializedType.Byte;
            }
            else if (value is short)
            {
                bytes = BitConverter.GetBytes((short)value);
                type = DME_SerializedType.Short;
            }
            else if (value is ushort)
            {
                bytes = BitConverter.GetBytes((ushort)value);
                type = DME_SerializedType.UShort;
            }
            else if (value is int)
            {
                bytes = BitConverter.GetBytes((int)value);
                type = DME_SerializedType.Int;
            }
            else if (value is uint)
            {
                bytes = BitConverter.GetBytes((uint)value);
                type = DME_SerializedType.UInt;
            }
            else if (value is long)
            {
                bytes = BitConverter.GetBytes((long)value);
                type = DME_SerializedType.Long;
            }
            else if (value is ulong)
            {
                bytes = BitConverter.GetBytes((ulong)value);
                type = DME_SerializedType.ULong;
            }
            else if (value is float)
            {
                bytes = BitConverter.GetBytes((float)value);
                type = DME_SerializedType.Float;
            }
            else if (value is double)
            {
                bytes = BitConverter.GetBytes((double)value);
                type = DME_SerializedType.Double;
            }
            else
            {
                //Object
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, value);
                    bytes = ms.ToArray();
                    type = DME_SerializedType.Object;
                    if (bytes.Length > compressionThreshold)
                    {
                        bytes = compress(bytes);
                        type = DME_SerializedType.CompressedObject;
                    }
                }
            }
            return bytes;
        }

        private static byte[] compress(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream gzs = new DeflateStream(ms, CompressionMode.Compress, false))
                {
                    gzs.Write(bytes, 0, bytes.Length);
                }
                ms.Close();
                return ms.ToArray();
            }
        }

        private static byte[] decompress(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes, false))
            {
                using (DeflateStream gzs = new DeflateStream(ms, CompressionMode.Decompress, false))
                {
                    using (MemoryStream dest = new MemoryStream())
                    {
                        byte[] tmp = new byte[bytes.Length];
                        int read;
                        while ((read = gzs.Read(tmp, 0, tmp.Length)) != 0)
                        {
                            dest.Write(tmp, 0, read);
                        }
                        dest.Close();
                        return dest.ToArray();
                    }
                }
            }
        }

        public static object DeSerialize(byte[] bytes, DME_SerializedType type)
        {
            switch (type)
            {
                case DME_SerializedType.String:
                    return Encoding.UTF8.GetString(bytes);
                case DME_SerializedType.Datetime:
                    return new DateTime(BitConverter.ToInt64(bytes, 0));
                case DME_SerializedType.Bool:
                    return bytes[0] == 1;
                case DME_SerializedType.Byte:
                    return bytes[0];
                case DME_SerializedType.Short:
                    return BitConverter.ToInt16(bytes, 0);
                case DME_SerializedType.UShort:
                    return BitConverter.ToUInt16(bytes, 0);
                case DME_SerializedType.Int:
                    return BitConverter.ToInt32(bytes, 0);
                case DME_SerializedType.UInt:
                    return BitConverter.ToUInt32(bytes, 0);
                case DME_SerializedType.Long:
                    return BitConverter.ToInt64(bytes, 0);
                case DME_SerializedType.ULong:
                    return BitConverter.ToUInt64(bytes, 0);
                case DME_SerializedType.Float:
                    return BitConverter.ToSingle(bytes, 0);
                case DME_SerializedType.Double:
                    return BitConverter.ToDouble(bytes, 0);
                case DME_SerializedType.Object:
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        return new BinaryFormatter().Deserialize(ms);
                    }
                case DME_SerializedType.CompressedByteArray:
                    return DeSerialize(decompress(bytes), DME_SerializedType.ByteArray);
                case DME_SerializedType.CompressedString:
                    return DeSerialize(decompress(bytes), DME_SerializedType.String);
                case DME_SerializedType.CompressedObject:
                    return DeSerialize(decompress(bytes), DME_SerializedType.Object);
                case DME_SerializedType.ByteArray:
                default:
                    return bytes;
            }
        }
    }
}
