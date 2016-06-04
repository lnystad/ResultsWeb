using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Utils
{
    using System.IO;

    using OrionLag.Input.Data;

    using SendingResults.Diagnosis;

    public static class InputFileParser
    {

        public static List<InputData> ParseFile(string path, string filename)
        {
            var restunVal = new List<InputData>();
            string fullPath = Path.Combine(path, filename);
            string[] text = System.IO.File.ReadAllLines(fullPath,Encoding.UTF7);
            var Heading = text[0];

            var Headings = Heading.Split(new char[] { ';' }, StringSplitOptions.None);
            int lineNo = 0;
            foreach (var line in text)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var elements = line.Split(new char[] { ';' },StringSplitOptions.None);
                    if (lineNo > 0)
                    {
                        InputData data = new InputData();
                        int coloum = 0;
                        while (coloum < elements.Length)
                        {
                            var xmlTags = new XmlTags();
                            xmlTags.Name = Headings[coloum];
                            xmlTags.Value = elements[coloum];
                            data.InputXmlData.Add(xmlTags);
                            coloum++;
                        }

                        data.Name = data.InputXmlData.FirstOrDefault(x => x.Name == "fornavn").Value + " "
                                    + data.InputXmlData.FirstOrDefault(x => x.Name == "etternavn").Value;

                        data.Klasse = data.InputXmlData.FirstOrDefault(x => x.Name == "klasse").Value;

                        string id = data.InputXmlData.FirstOrDefault(x => x.Name == "medlemsid").Value;
                        if (string.IsNullOrEmpty(id))
                        {
                            Log.Error("Missing skytterid line={0}", line);
                        }
                        else
                        {
                            int skytterid = 0;
                            if (int.TryParse(id, out skytterid))
                            {
                                data.SkytterNr = skytterid;
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

                lineNo++;
            }


            return restunVal;
        }
    }
}
