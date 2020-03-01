using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Domain
{
    [Serializable]
    public class RandomMessage
    {        
        public Participant Participant { get; set; }

        public byte[] AsByteArray()
        {
            var formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static RandomMessage FromByteArray(byte[] array)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(array, 0, array.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return binaryFormatter.Deserialize(memoryStream) as RandomMessage;
            }
        }
    }
}
