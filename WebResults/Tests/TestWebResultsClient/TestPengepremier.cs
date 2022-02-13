using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using WebResultsClient.Definisjoner;
using WebResultsClient.Premieberegning;

namespace TestWebResultsClient
{
    [TestClass]
    public class TestPengepremier
    {
        [TestMethod]
        public void BeregnPengepremier()
        {
            var stevneOppgjor = TestUtils.DeserializeXml<Stevneoppgjor>("Stevneoppgjør-tom.xml");

            Pengepremier pengepremier = new Pengepremier(stevneOppgjor);

            pengepremier.BeregnPengepremier(70, new List<string> {"V55"});

            var ovelse = stevneOppgjor.Resultater.Single(r => r.Fornavn == "Roy Jøran").Ovelse;

            Assert.AreEqual("126", ovelse.Premie);
        }
    }
}