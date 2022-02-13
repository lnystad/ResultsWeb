using System.Xml.Serialization;

namespace WebResultsClient.Definisjoner
{
    public class Ovelse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("klasse")]
        public string Klasse { get; set; }
        [XmlAttribute("premie")]
        public string Premie { get; set; }
        [XmlElement("serie")]
        public Serie[] Serier { get; set; }
    }
}
