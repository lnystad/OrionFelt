using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Services
{
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

        public List<InputData> GetAllComptitiors()
        {
            try
            {
                var input = InputFileParser.ParseFile(m_inputPath, "INPUT.TXT");
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
