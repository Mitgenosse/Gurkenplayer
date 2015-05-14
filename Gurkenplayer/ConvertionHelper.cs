using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Gurkenplayer
{
    /// <summary>
    /// Provides static methods for convertions.
    /// </summary>
    public static class ConvertionHelper
    {
        /// <summary>
        /// Converts a byte array into a ushort array.
        /// </summary>
        /// <param name="byteArr">The byte array to convert.</param>
        /// <returns>The ushort array equivalent of the byte array.</returns>
        public static ushort[] ConvertToUInt16Array(byte[] byteArr)
        {
            if (byteArr == null)
                return null;

            ushort[] ushortArr = new ushort[byteArr.Length / 2];
            int byteOffset = 0;
            for (int i = 0; i < ushortArr.Length; i++)
            {
                ushortArr[i] = BitConverter.ToUInt16(byteArr, byteOffset);
                byteOffset += 2;
            }
            return ushortArr;
        }
        /// <summary>
        /// Converts a ushort array into a byte array.
        /// </summary>
        /// <param name="shortArr">The ushort array to convert.</param>
        /// <returns>The byte array equivalent of the ushort array.</returns>
        public static byte[] ConvertToByteArray(ushort[] shortArr)
        {
            if (shortArr == null)
                return null;

            bool isLittleEndian = true;
            byte[] data = new byte[shortArr.Length * 2];
            int offset = 0;
            foreach (ushort value in shortArr)
            {
                byte[] buffer = BitConverter.GetBytes(value);
                if (BitConverter.IsLittleEndian != isLittleEndian)
                {
                    Array.Reverse(buffer);
                }
                buffer.CopyTo(data, offset);
                offset += 2;
            }
            return data;
        }
        /// <summary>
        /// Converts an serializable object to a byte array.
        /// </summary>
        /// <param name="obj">Object to convert/serialize.</param>
        /// <returns>A byte array of the object.</returns>
        public static byte[] ConvertToByteArray(object obj)
        {
            if (obj == null)
                return null;

            // If object is not serializable, return null.
            if (!obj.GetType().IsSerializable)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Converts a byte array to a object.
        /// </summary>
        /// <param name="byteObjectArr">Bytearray of object.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObject(byte[] byteObjectArr)
        {
            if (byteObjectArr == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream(byteObjectArr))
            {
                return bf.Deserialize(ms);
            }
        }
    }
}