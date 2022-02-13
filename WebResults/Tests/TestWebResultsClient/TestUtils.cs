using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace TestWebResultsClient
{
    public static class TestUtils
    {
        public static T? DeserializeXml<T>(string resourceName) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();

            var resource = resources.FirstOrDefault(r => r.EndsWith(resourceName));
            if (!string.IsNullOrEmpty(resource))
            {
                using (Stream? stream = assembly.GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return serializer.Deserialize(reader) as T;
                        }
                    }
                }
            }

            return null;
        }

        public static void SerializeXml<T>(T xml) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            var encoding = Encoding.GetEncoding("UTF-8");
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            using (StreamWriter writer = new StreamWriter("test.xml", false, encoding))
            {
                serializer.Serialize(writer, xml, xmlns);
            }
        }
    }
}
