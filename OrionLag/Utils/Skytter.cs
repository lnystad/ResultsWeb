using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Utils
{
    using System.Collections.ObjectModel;

    using OrionLag.Input.Data;

    public class Skytter
    {
        public Skytter()
        {
            
        }
        public Skytter(Skytter copy)
        {
            SkytterNr = copy.SkytterNr;
            Name = copy.Name;
            Skytterlag = copy.Skytterlag;
            Klasse = copy.Klasse;
            InputXmlData = copy.InputXmlData;
        }

        //public Skytter(int skytterNr, string name, string skytterlag, string klasse)
        //{
        //    SkytterNr = skytterNr;
        //    Name = name;
        //    Skytterlag = skytterlag;
        //    Klasse = klasse;
        //}

        public int SkytterNr { get; set; }
        public string Name { get; set; }

        public string Skytterlag { get; set; }

        public string Klasse { get; set; }


        public Collection<XmlTags> InputXmlData { get; set; }
    }
}
