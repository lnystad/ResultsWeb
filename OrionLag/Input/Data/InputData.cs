using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.Data
{
    using System.Collections.ObjectModel;

    public class InputData
    {
        public InputData()
        {
            this.InputXmlData = new Collection<XmlTags>();
        }
        public int SkytterNr { get; set; }
        public string Skive { get; set; }
        public string Name { get; set; }

        public int Lagnr { get; set; }

        public string Skytterlag { get; set; }
        
        public string Klasse { get; set; }

        public bool Links { get; set; }

        public Collection<XmlTags> InputXmlData { get; set; }
    }

    public class XmlTags
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
