using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebResultsClient.Premieberegning
{
    public class Summary
    {
        private List<SummaryClass> m_Class;
        public Summary()
        {
            m_Class = new List<SummaryClass>();
        }
        public int TotalParticipants { get; set; }
        public int NumberOfPremies { get; set; }
        public int TotalPremies { get; set; }

        public List<SummaryClass> Classes { get { return m_Class; }  }

        public void CompleteCalculation()
        {
            foreach (SummaryClass c in Classes)
            {
                foreach(var premieVerdi in c.Premies)
                {
                    c.TotalPremies += premieVerdi.Value;
                }
                TotalParticipants = TotalParticipants + c.TotalParticipants;
                TotalPremies = TotalPremies + c.TotalParticipants;
            }

            m_Class= m_Class.OrderByDescending(x => x.TotalParticipants).ToList();
        }
    }

    public class SummaryClass
    {
        public SummaryClass() {
            ClassName = string.Empty;
            TotalParticipants = 0;
            TotalPremies = 0;
            Premies = new List<Premie>();
        }
        public string ClassName { get; set; }
        public int  TotalParticipants { get; set; }
        public int TotalPremies { get; set; }
        public List<Premie> Premies { get; set; }
    }

    public class Premie
    {
        public Premie()
        {
            Name = string.Empty;
            Value = 0;
        }
        [JsonIgnore]
        public string Name { get; set; }
        public int Value { get; set; }
        [JsonIgnore]
        public DFSOvelse? MappedName { get; set; }
        public string Ovelse { get {
                if (MappedName != null)
                {
                    return MappedName.ToString();
                }
                else
                { return string.Empty; }

            } }
        
    }

}
