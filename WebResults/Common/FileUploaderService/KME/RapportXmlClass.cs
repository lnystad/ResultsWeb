﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.Xml;

    public class RapportXmlClass
    {

        public XmlDocument BitMapInfo { get; set; }

        public XmlDocument Rapport { get; set; }

        public string Filnavn { get; set; }

        //public StevneType StevneType { get; set; }

        public BaneType BaneType { get; set; }

        public XmlDocument ToppListInfoWithRef { get; set; }

        public XmlDocument ToppListInfoLagWithRef { get; set; }

        public ProgramType ProgramType { get; set; }

        public List<string> Klasse { get; set; }
    }
}
