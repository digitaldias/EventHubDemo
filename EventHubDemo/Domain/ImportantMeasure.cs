using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Domain
{
    [Serializable]
    public class ImportantMeasure
    {
        public double ImportantValue { get; set; }

        public byte[] AsByteArray()
        {
            var formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static ImportantMeasure FromByteArray(byte[] array)
        {
            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(array, 0, array.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return binaryFormatter.Deserialize(memoryStream) as ImportantMeasure;
            }
        }
    }
}
