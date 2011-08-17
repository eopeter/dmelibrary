using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DME.Base.Cache.MemoryCached
{
    /// <summary>
    /// Utility class for serializing and deserializing objects to and from byte streams
    /// </summary>
    public static class DME_SerializationUtility
    {
        /// <summary>
        /// Converts an object into an array of bytes. Object must be serializable.
        /// </summary>
        /// <param name="value">Object to serialize. May be null.</param>
        /// <returns>Serialized object, or null if input was null.</returns>
        public static byte[] ToBytes(object value)
        {
            if (value == null)
            {
                return null;
            }

            byte[] inMemoryBytes;
            using (MemoryStream inMemoryData = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Binder = new DME_Binder();
                binaryFormatter.Serialize(inMemoryData, value);
                inMemoryBytes = inMemoryData.ToArray();
            }

            return inMemoryBytes;
        }

        /// <summary>
        /// Converts a byte array into an object. 
        /// </summary>
        /// <param name="serializedObject">Object to deserialize. May be null.</param>
        /// <returns>Deserialized object, or null if input was null.</returns>
        public static object ToObject(byte[] serializedObject)
        {
            if (serializedObject == null)
            {
                return null;
            }

            using (MemoryStream dataInMemory = new MemoryStream(serializedObject))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Binder = new DME_Binder();
                return binaryFormatter.Deserialize(dataInMemory);
            }
        }
    }
}
