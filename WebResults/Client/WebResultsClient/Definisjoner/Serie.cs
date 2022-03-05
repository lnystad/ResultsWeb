using System.Xml.Serialization;

namespace WebResultsClient.Definisjoner
{
    public class Serie
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("sum")]
        public string Sum { get; set; }
        [XmlAttribute("it")]
        public string InnerTiere { get; set; }
        [XmlAttribute("h")]
        public string Treff { get; set; }
        [XmlAttribute("ih")]
        public string Innertreff { get; set; }
        [XmlAttribute("premie")]
        public string Premie { get; set; }
        [XmlAttribute("rank-klasse")]
        public string RankKlasse { get; set; }
        [XmlAttribute("rank-total")]
        public string RankTotal { get; set; }
    }
}
