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

            var ovelse = stevneOppgjor.Resultater.Single(r => r.Fornavn == "Roy Jøran").Ovelse;

            Assert.AreEqual("0", ovelse.Premie);
            pengepremier.BeregnPengepremier(70, new List<string> {"V55"});
            Assert.AreEqual("126", ovelse.Premie);
        }

        [TestMethod]
        public void BeregnPengepremier2024()
        {
            var stevneOppgjor = TestUtils.DeserializeXml<Stevneoppgjor>("Stevneoppgjør2024.xml");

            Pengepremier pengepremier = new Pengepremier(stevneOppgjor);

            pengepremier.BeregnPengepremier(50, new List<string> { "R",
                                                        "ER",
                                                        "J",
                                                        "EJ" });

            pengepremier.BeregnPengepremier(70, new List<string> { "SK1",
                                                                    "SK2",
                                                                    "1",
                                                                    "2",
                                                                    "3",
                                                                    "4",
                                                                    "5",
                                                                    "V55",
                                                                    "V65",
                                                                    "V75",
                                                                    "KIK",
                                                                    "AG3",
                                                                    "HK416" });
        }
    }
}