using System.Xml.Serialization;

namespace WebResultsClient.Definisjoner
{
    public class Resultat
    {
        [XmlAttribute("medlemsid")]
        public string Medlemsid { get; set; }
        [XmlAttribute("fornavn")]
        public string Fornavn { get; set; }
        [XmlAttribute("etternavn")]
        public string Etternavn { get; set; }
        [XmlAttribute("sklagnr")]
        public string SkytterlagsNummer { get; set; }
        [XmlAttribute("kat-mf")]
        public string KatMf { get; set; }
        [XmlAttribute("kat-mb")]
        public string KatMb { get; set; }
        [XmlAttribute("kat-k")]
        public string KatK { get; set; }
        [XmlAttribute("kat-l")]
        public string KatL { get; set; }
        [XmlAttribute("kat-a")]
        public string KatA { get; set; }
        [XmlAttribute("kat-n")]
        public string KatN { get; set; }
        [XmlAttribute("kat-ft")]
        public string KatFt { get; set; }
        [XmlAttribute("kat-22")]
        public string Kat22 { get; set; }
        [XmlAttribute("kat-luft")]
        public string KatLuft { get; set; }
        [XmlAttribute("epost")]
        public string Epost { get; set; }
        [XmlAttribute("postnr-hvis-ikke-ID")]
        public string PostNr { get; set; }

        [XmlElement("ovelse")]
        public Ovelse Ovelse { get; set; }
    }
}
