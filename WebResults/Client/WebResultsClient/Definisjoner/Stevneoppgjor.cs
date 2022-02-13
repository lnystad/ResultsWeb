using System.Xml.Serialization;

namespace WebResultsClient.Definisjoner
{
    [XmlRoot("resultat-stevne")]
    public class Stevneoppgjor
    {
        [XmlAttribute("stevnenavn")]
        public string StevneNavn { get; set; }

        [XmlAttribute("stevnenummer")]
        public string StevneId { get; set; }

        [XmlElement("resultat")]
        public Resultat[] Resultater { get; set; }
    }
}
