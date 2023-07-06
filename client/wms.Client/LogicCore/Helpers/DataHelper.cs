using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace wms.Client.LogicCore.Helpers
{
    public static class DataHelper
    {
        public static T ToDynamic<T>(this T source)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bFormatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)bFormatter.Deserialize(stream);
        }
    }
}
