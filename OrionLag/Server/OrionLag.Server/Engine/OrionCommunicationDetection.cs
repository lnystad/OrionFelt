using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Engine
{
    using System.IO;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Parsers;

    public class OrionCommunicationDetection
    {
        public const string newComDetected = "KMI.UPD";
        public const string dataFile = "KMINEW.TXT";
        public const string dataFileInit = "KMIINIT.TXT";

        public const string newComOrionDetected = "KMO.UPD";
        public const string dataFileResult = "KMONEW.TXT";
        public const string dataFileResultAll = "KMOALLINIT.TXT";

        public string m_orionOutputPath;

        public string m_orionOutputBkupPath;

        private byte[] m_newfileContent = null;

        OrionInputParser m_parser = new OrionInputParser();

        OrionResultParser m_resultparser = new OrionResultParser();

        public List<Lag> CheckComfiles()
        {
            List<Lag> retColl = new List<Lag>();
            return retColl;
        }

        public void Init()
        {
            m_newfileContent = new byte[] { 0x4B };

            this.m_orionOutputPath = ConfigurationLoader.GetAppSettingsValue("OrionInputDir");

            if (string.IsNullOrEmpty(this.m_orionOutputPath))
            {
                Log.Error("OrionInputDir Path must be specified");
                throw new InvalidOperationException("OrionInputDir must be specified");
            }

            if (!Directory.Exists(this.m_orionOutputPath))
            {
                var errormsg = string.Format("OrionInputDir does not exsits {0}", this.m_orionOutputPath);
                Log.Error(errormsg);
                throw new InvalidOperationException(errormsg);
            }

            this.m_orionOutputBkupPath = Path.Combine(this.m_orionOutputPath,"BkupToOrion");
            if (!Directory.Exists(this.m_orionOutputBkupPath))
            {
                Directory.CreateDirectory(this.m_orionOutputBkupPath);
            }
            try
            {

            }
            catch (Exception e)
            {
                Log.Error(e, "Error init");
                throw;
            }

        }

        public bool SendToOrion(List<Lag> orionLag)
        {
            if (orionLag == null)
            {
                return true;
            }

            if (orionLag.Count == 0)
            {
                return true;
            }

            string updateFileName = Path.Combine(m_orionOutputPath, newComDetected);
            if (File.Exists(updateFileName))
            {
                Log.Warning("Not able to communicate Orion has not read prev file");
                return false;
            }

            var lines = m_parser.ParseToOrionInputFormat(orionLag);


            if (lines.Length > 0)
            {
                string contentFileName = Path.Combine(m_orionOutputPath, dataFile);
                using (System.IO.StreamWriter file =  new System.IO.StreamWriter(contentFileName))
                {
                    int count = 0;
                    foreach (string line in lines)
                    {
                        count ++;
                        // If the line doesn't contain the word 'Second', write the line to the file.
                        if (!string.IsNullOrEmpty(line))
                        {
                            file.WriteLine(line);
                        }
                        else
                        {
                            Log.Error("LIne no={0} Is Empty {1}", count, lines.Length);
                        }
                        
                    }

                    file.Flush();
                    file.Close();
                }

                
                var fileToBackup = Path.GetFileName(contentFileName);

                fileToBackup = string.Format("{0}ToOrion_{1}", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"), fileToBackup);

                File.Copy(contentFileName, Path.Combine(this.m_orionOutputBkupPath, fileToBackup), false);


                using (FileStream file = new FileStream(updateFileName, FileMode.Create, System.IO.FileAccess.Write))
                {
                    file.Write(this.m_newfileContent, 0, this.m_newfileContent.Length);
                    file.Flush(true);
                    file.Close();
                }

                return true;
            }


            return false;
        }
        
        public bool CheckComfiles(out List<SkytterResultat> funnetLag, out bool allresultsComm)
        {
            try
            {
                bool commFound = false;
                allresultsComm = false;
                funnetLag = new List<SkytterResultat>();
                if (File.Exists(Path.Combine(this.m_orionOutputPath, newComOrionDetected)))
                {
                    string filename = string.Empty;
                    if (File.Exists(Path.Combine(this.m_orionOutputPath, dataFileResultAll)))
                    {
                        allresultsComm = true;
                        filename = Path.Combine(this.m_orionOutputPath, dataFileResultAll);
                        Log.Info("New All Result File Detected {0}", filename);
                    }
                    else if (File.Exists(Path.Combine(this.m_orionOutputPath, dataFileResult)))
                    {
                        filename = Path.Combine(this.m_orionOutputPath, dataFileResult);
                        Log.Info("New result File Detected {0}", filename);
                    }

                    if (!string.IsNullOrEmpty(filename))
                    {

                        commFound = true;
                        var inputContent = File.ReadAllLines(filename, new UTF7Encoding());
                        funnetLag = this.m_resultparser.ParseOrionResultOutputFormat(inputContent);
                        if(funnetLag.Count<=0)
                        {
                            commFound = false;
                        }

                        var fileToBackup = Path.GetFileName(filename);

                        fileToBackup = string.Format("{0}FromOrion_{1}", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"), fileToBackup);

                        File.Copy(filename, Path.Combine(m_orionOutputBkupPath, fileToBackup), false);
                        File.Delete(filename);

                    }

                    File.Delete(Path.Combine(this.m_orionOutputPath, newComOrionDetected));

                    return commFound;
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error reading");
                throw;
            }
        }
    }
}
