using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Services
{
    using System.IO;
    using System.Xml.Serialization;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Common.Services;
    public class OppropDataService : IOppropDataService
    {
        XmlSerializer m_lagser = new XmlSerializer(typeof(Lag));
        private string m_inputPath;
        public OppropDataService()
        {
            System.Configuration.Configuration configuration =
                 ConfigurationLoader.GetInstance().GetConfigurationForRunningProcess();
            m_inputPath = ConfigurationLoader.GetAppSettingsValue("OppRopDataPath");
        }

        public List<Lag> FetchAllLag()
        {
            List<Lag> retColl = new List<Lag>();
            try
            {
            var filesInDir = Directory.GetFiles(m_inputPath, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in filesInDir)
            {
                using (var fileReader = new System.IO.StreamReader(file, new UTF8Encoding()))
                {
                    var funnetLag = m_lagser.Deserialize(fileReader) as Lag;
                        retColl.Add(funnetLag);
                    
                }

            }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }

            return retColl;

        }
    }
}
