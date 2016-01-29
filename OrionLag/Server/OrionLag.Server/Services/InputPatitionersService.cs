using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Services
{
    using System.IO;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Common.Services;
    using OrionLag.Common.Utils;

    public class InputPatitionersService : IInputPatitionerService
    {

        private string m_inputPath;
        public InputPatitionersService()
        {
            System.Configuration.Configuration configuration =
                 ConfigurationLoader.GetInstance().GetConfigurationForRunningProcess();
            m_inputPath = ConfigurationLoader.GetAppSettingsValue("InputDataPath");
        }

        public List<InputData> GetAllComptitiors(string path, string filename)
        {
            try
            {
                string pathToUse = path;
                if (string.IsNullOrEmpty(path))
                {
                    pathToUse = m_inputPath;
                }

                string filenameToUse = filename;
                if (string.IsNullOrEmpty(filenameToUse))
                {
                    filenameToUse = "INPUT.TXT";
                }


                var input = InputFileParser.ParseFile(pathToUse, filenameToUse);
                return input;
            }
            catch (Exception e)
            {
                Log.Error(e, "GetAllComptitiors");
                throw;
            }

           
        }
    }
}
