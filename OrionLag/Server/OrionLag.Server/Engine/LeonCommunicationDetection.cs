namespace OrionLag.Server.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Parsers;

    public class LeonCommunicationDetection
    {
        public const string newComDetected = "KMI.UPD";

        public const string dataFile = "KMINEW.TXT";

        public const string dataFileInit = "KMIINIT.TXT";

        public const string newComOrionResultDetected = "KMO.UPD";

        public const string dataFileResult = "KMONEW.TXT";

        public const string dataFileResultAll = "KMOALL.TXT";

        public string m_comBkupDirectory;

        public string m_comDirectory;

        public OrionInputParser m_parser;

        public OrionResultParser m_resultParser;

        private byte[] m_newfileContent = null;

        public bool CheckComfiles(out List<Lag> funnetLag, out bool initComm)
        {
            try
            {
                bool commFound = false;
                initComm = false;
                funnetLag = new List<Lag>();
                if (File.Exists(Path.Combine(this.m_comDirectory, newComDetected)))
                {
                    string filename = string.Empty;
                    if (File.Exists(Path.Combine(this.m_comDirectory, dataFileInit)))
                    {
                        initComm = true;
                        filename = Path.Combine(this.m_comDirectory, dataFileInit);
                        Log.Info("New Init File Detected {0}", filename);
                    }
                    else if (File.Exists(Path.Combine(this.m_comDirectory, dataFile)))
                    {
                        filename = Path.Combine(this.m_comDirectory, dataFile);
                        Log.Info("New File Detected {0}", filename);
                    }

                    if (!string.IsNullOrEmpty(filename))
                    {

                        commFound = true;
                        var inputContent = File.ReadAllLines(filename, new UTF7Encoding());
                        funnetLag = this.m_parser.ParseLeonOutputFormat(inputContent);

                        var fileToBackup = Path.GetFileName(filename);

                        fileToBackup = string.Format("{0}FromLeon_{1}", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"), fileToBackup);

                        File.Copy(filename, Path.Combine(m_comBkupDirectory, fileToBackup), false);


                        File.Delete(filename);

                    }

                    File.Delete(Path.Combine(this.m_comDirectory, newComDetected));

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

        public void Init()
        {
            try
            {
                this.m_newfileContent = new byte[] { 0x4B };
                this.m_resultParser = new OrionResultParser();
                this.m_parser = new OrionInputParser();
                this.m_comDirectory = ConfigurationLoader.GetAppSettingsValue("LeonInputDir");

                if (string.IsNullOrEmpty(this.m_comDirectory))
                {
                    Log.Error("LeonInputDir Path must be specified");
                    throw new InvalidOperationException("LeonInputDir must be specified");
                }

                if (!Directory.Exists(this.m_comDirectory))
                {
                    var errormsg = string.Format("LeonInputDir does not exsits {0}", this.m_comDirectory);
                    Log.Error(errormsg);
                    throw new InvalidOperationException(errormsg);
                }

                m_comBkupDirectory = Path.Combine(this.m_comDirectory, "backup");
                if (!Directory.Exists(m_comBkupDirectory))
                {
                    Directory.CreateDirectory(m_comBkupDirectory);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error init");
                throw;
            }
        }

        internal bool SendToLeon(List<SkytterResultat> leonLag,bool allResults)
        {
            if (leonLag == null)
            {
                return true;
            }

            if (leonLag.Count == 0)
            {
                return true;
            }

            var LeonFile = this.m_resultParser.ParseResultToLeonInputFormat(leonLag);

            string updateFileName = Path.Combine(this.m_comDirectory, newComOrionResultDetected);
            if (File.Exists(updateFileName))
            {
                Log.Warning("Not able to communicate Orion has not read prev file");
                return false;
            }

            string resultFile;
            if (allResults)
            {
                 resultFile = Path.Combine(this.m_comDirectory, dataFileResultAll);
            }
            else
            {
                 resultFile = Path.Combine(this.m_comDirectory, dataFileResult);
            }
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultFile))
            {
                int count = 0;
                foreach (string line in LeonFile)
                {
                    count++;
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    if (!string.IsNullOrEmpty(line))
                    {
                        file.WriteLine(line);
                    }
                    else
                    {
                        Log.Error("LIne no={0} Is Empty {1}", count, LeonFile.Length);
                    }

                }

                file.Flush();
                file.Close();
            }


            var fileToBackup = Path.GetFileName(resultFile);

            fileToBackup = string.Format("{0}ToLeon_{1}", DateTime.Now.ToString("yyyy_MM_dd_HHmmss"), fileToBackup);

            File.Copy(resultFile, Path.Combine(this.m_comBkupDirectory, fileToBackup), false);

            using (FileStream file = new FileStream(updateFileName, FileMode.Create, System.IO.FileAccess.Write))
            {
                file.Write(this.m_newfileContent, 0, this.m_newfileContent.Length);
                file.Flush(true);
                file.Close();
            }

            return true;
        }
    }
}