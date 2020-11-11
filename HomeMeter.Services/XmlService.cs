namespace HomeMeter.Services
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlService
    {
        public static void Serialize<T>(string fileName, T obj, bool clear = false)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var xmlnsEmpty = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName(string.Empty, string.Empty),
            });
            var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, obj, xmlnsEmpty);
            File.WriteAllText(fileName, stringWriter.ToString());
        }

        public static T Deserialize<T>(string fileName)
        {
            var content = File.ReadAllText(fileName);
            if (content.Length == 0) return default(T);
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(content);
            return (T)xmlSerializer.Deserialize(stringReader);
        }
    }

}
