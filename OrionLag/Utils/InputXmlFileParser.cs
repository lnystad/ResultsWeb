using OrionLag.Input.Data;
using SendingResults.Diagnosis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrionLag.Utils
{
    public static class InputXmlFileParser
    {


        
           public static StevneInfo ParseStevneInfoFile(string path, string fileName)
        {
            var restunVal = new StevneInfo();
            string fullPath = Path.Combine(path, fileName);
            XDocument xmlDoc = XDocument.Load(fullPath);
            int elementno = 0;
            foreach (XElement desc in xmlDoc.Descendants())
            {
                elementno++;
                if (desc.Name.LocalName == "paamelding")
                {
                    InputData data = new InputData();
                    foreach (var attr in desc.Attributes())
                    {
                        var xmlTags = new XmlTags();
                        xmlTags.Name = attr.Name.LocalName;
                        xmlTags.Value = attr.Value;
                        data.InputXmlData.Add(xmlTags);
                    }
                    //   stevnenavn = "Landsdelskretsstevne Nord-Norge 2018" stevnenummer = "180210" arrangor = "10584" stevnestart = "27.06.2018" stevneslutt = "30.06.2018" >

                    restunVal.stevnenavn = data.InputXmlData.FirstOrDefault(x => x.Name == "stevnenavn").Value;
                    restunVal.stevnenummer = data.InputXmlData.FirstOrDefault(x => x.Name == "stevnenummer").Value;
                    restunVal.arrangor = data.InputXmlData.FirstOrDefault(x => x.Name == "arrangor").Value;
                    restunVal.stevnestart = data.InputXmlData.FirstOrDefault(x => x.Name == "stevnestart").Value;
                    restunVal.stevneslutt = data.InputXmlData.FirstOrDefault(x => x.Name == "stevneslutt").Value;
                }
            }
            return restunVal;
        }
    

    public static List<InputData> ParseXmlFile(string path, string fileName)
        {
            var restunVal = new List<InputData>();
            string fullPath = Path.Combine(path, fileName);
            XDocument xmlDoc = XDocument.Load(fullPath);
            int elementno = 0;
            foreach (XElement desc in xmlDoc.Descendants())
            {
                elementno++;
                if (desc.Name.LocalName == "paamelding-skytter")
                {
                    InputData data = new InputData();

                    foreach (var attr in desc.Attributes())
                    {
                        var xmlTags = new XmlTags();
                        xmlTags.Name = attr.Name.LocalName;
                        xmlTags.Value = attr.Value;
                        data.InputXmlData.Add(xmlTags);
                    }


                    data.Name = data.InputXmlData.FirstOrDefault(x => x.Name == "fornavn").Value + " "
                + data.InputXmlData.FirstOrDefault(x => x.Name == "etternavn").Value;

                    data.Klasse = data.InputXmlData.FirstOrDefault(x => x.Name == "klasse").Value;

                    string id = data.InputXmlData.FirstOrDefault(x => x.Name == "medlemsid").Value;
                    if (string.IsNullOrEmpty(id))
                    {
                        Log.Error("Missing skytterid line={0}", elementno);
                    }
                    else
                    {
                        int skytterid = 0;
                        if (int.TryParse(id, out skytterid))
                        {
                            data.SkytterNr = skytterid;
                        }
                    }

                    string lag = data.InputXmlData.FirstOrDefault(x => x.Name == "lag").Value;
                    if (!string.IsNullOrEmpty(lag))
                    {
                        int lagno = 0;
                        if (int.TryParse(lag, out lagno))
                        {
                            data.Lagnr = lagno;
                        }
                    }

                    string skive = data.InputXmlData.FirstOrDefault(x => x.Name == "skive").Value;
                    if (!string.IsNullOrEmpty(skive))
                    {
                        int skiveno = 0;
                        if (int.TryParse(skive, out skiveno))
                        {
                            data.Skive = skiveno;
                        }
                    }

                    string Links = data.InputXmlData.FirstOrDefault(x => x.Name == "kat-l").Value;
                    if (!string.IsNullOrEmpty(Links))
                    {
                        if (Links == "1")
                        {
                            data.Links = true;
                        }
                    }

                    data.Skytterlag = data.InputXmlData.FirstOrDefault(x => x.Name == "sklag-nr").Value;
                    restunVal.Add(data);
                }
            }
            return restunVal;
        }

        public static List<Lag> ParseXmlFileLagData(string path, string fileName)
        {
            var restunVal = new List<Lag>();
            string fullPath = Path.Combine(path, fileName);
            XDocument xmlDoc = XDocument.Load(fullPath);
            int elementno = 0;
            foreach (XElement desc in xmlDoc.Descendants())
            {
                elementno++;
                if (desc.Name.LocalName == "lag")
                {
                    Lag data = new Lag();

                    var InputXmlData = new List<XmlTags>();
                    foreach (var attr in desc.Attributes())
                    {
                        var xmlTags = new XmlTags();
                        xmlTags.Name = attr.Name.LocalName;
                        xmlTags.Value = attr.Value;
                        InputXmlData.Add(xmlTags);
                    }


                    var lagno = InputXmlData.FirstOrDefault(x => x.Name == "lagnr")?.Value;
                    if(!string.IsNullOrEmpty(lagno))
                    {
                        data.LagNummer = int.Parse(lagno);
                    }

                    var dato = InputXmlData.FirstOrDefault(x => x.Name == "dato")?.Value;

                    var opproptid = InputXmlData.FirstOrDefault(x => x.Name == "kl-opprop")?.Value;

                    var skytetid = InputXmlData.FirstOrDefault(x => x.Name == "kl-skytetid")?.Value;

                    if (!string.IsNullOrEmpty(dato)&& !string.IsNullOrEmpty(opproptid))
                    {
                        data.OppropsTid = DateTime.ParseExact(dato + "T" + opproptid, "dd.MM.yyyyTHH:mm", null);
                    }
                    if (!string.IsNullOrEmpty(dato) && !string.IsNullOrEmpty(skytetid))
                    {
                        data.LagTid = DateTime.ParseExact(dato + "T" + skytetid, "dd.MM.yyyyTHH:mm", null);
                    }

                    restunVal.Add(data);
                }
            }
            return restunVal;
        }
    }
}
