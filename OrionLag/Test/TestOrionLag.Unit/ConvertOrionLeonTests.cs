using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrionLag.Server.Parsers;
using System.Collections.Generic;
using OrionLag.Common.DataModel;
using OrionLag.Server.Engine;

namespace TestOrionLag.Unit
{
    using System.Linq;

    using OrionLag.Server.Services;

    [TestClass]
    public class ConvertOrionLeonTests
    {
        private OrionTestHelper m_testHelper;
        private ConvertOrionLeon m_convert ;
        private OrionResultParser m_orionResultparser = new OrionResultParser();
        public ConvertOrionLeonTests()
        {

        }

        [TestInitialize]
       public void Inint()
        {
            m_testHelper = new OrionTestHelper(@"OrionFelt\OrionLag\Test\TestOrionLag.Unit\TestData\ConvertOrionLeonTests\Test1");
            m_convert = new ConvertOrionLeon();
            m_convert.InitConverter();
        }


        [TestMethod]
        public void ParseResultToLeonInputFormat()
        {
            var res=InitiateOrionResults(m_testHelper, "KMOALLINIT.txt");
            var leonResults= m_convert.ConvertToLeonLag(res);
            var LeonFile = m_orionResultparser.ParseResultToLeonInputFormat(leonResults);
            AssertResult(LeonFile, m_testHelper, "KMOALLINIT_Leon.txt");

        }

        [TestMethod]
        public void OppropFraOrionTilLeon()
        {
            var res = ReadAllOrionData(m_testHelper);

            var lagOppsett = m_convert.ConvertToLeonLag(res);

        }

        private List<Lag> ReadAllOrionData(OrionTestHelper mTestHelper)
        {
            LagOppsettDataService dataOrionXml = new LagOppsettDataService();
            var orionLag = dataOrionXml.GetAllFromDatabase(mTestHelper.GetTestPath());

            return orionLag.ToList();
        }

        private void AssertResult(string[] leonFile, OrionTestHelper mTestHelper, string kmoallinitLeonTxt)
        {
            var lines = mTestHelper.ReadResultFile(kmoallinitLeonTxt);

            Assert.AreEqual(lines.Length, leonFile.Length);
            int idx=0;
            while (idx < lines.Length)
            {
                var excpected = lines[idx].Split(new string[] { ";" }, StringSplitOptions.None);
                var actual = leonFile[idx].Split(new string[] { ";" }, StringSplitOptions.None);
                Assert.AreEqual(excpected.Length, actual.Length,string.Format("Differnet len {0} {1} {2} {3}", excpected.Length, actual.Length, lines[idx], leonFile[idx]));
                int step = 0;
                while (step < excpected.Length)
                {
                    Assert.AreEqual(excpected[step], actual[step], string.Format("Element {0}", step));
                    step++;
                }

                idx++;
            }

        }

        private List<SkytterResultat> InitiateOrionResults(OrionTestHelper help,string filename)
        {
            var lines = help.ReadResultFile(filename);
           
            return m_orionResultparser.ParseOrionResultOutputFormat(lines);
        }
    }
}
