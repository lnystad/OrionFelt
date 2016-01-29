using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Services
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Common.DataModel;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Common.Services;

    public class LagOppsettDataService : ILagOppsettDataService
    {
        private XmlSerializer m_ser = new XmlSerializer(typeof(Lag));
        public string m_inputFinfeltPath;
        public string m_inputGrovfeltPath;

        public LagOppsettDataService()
        {
            
        }

        public Collection<Lag> GetAllFromDatabase(string baseName)
        {
            Collection < Lag > retColl = new Collection<Lag>();
            if (string.IsNullOrEmpty(baseName))
            {
                Log.Error("GetAllFromDatabase Path must be specified");
                throw new InvalidOperationException("Path must be specified");
            }

            if (!Directory.Exists(baseName))
            {
                var errormsg = string.Format("GetAllFromDatabase Path does not exsits {0}", baseName);
                Log.Error(errormsg);
                throw new InvalidOperationException(errormsg);
            }
            try
            {
                var allfiles = Directory.GetFiles(baseName, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in allfiles)
                {
                    try
                    {
                        using (FileStream fileRead = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[fileRead.Length];
                            int numBytesToRead = (int)fileRead.Length;
                            int numBytesRead = 0;
                            while (numBytesToRead > 0)
                            {
                                // Read may return anything from 0 to numBytesToRead.
                                int n = fileRead.Read(bytes, numBytesRead, numBytesToRead);

                                // Break when the end of the file is reached.
                                if (n == 0)
                                    break;

                                numBytesRead += n;
                                numBytesToRead -= n;
                            }
                            numBytesToRead = bytes.Length;

                            var inputStream = new MemoryStream(bytes);

                            using (StreamReader mread = new StreamReader(inputStream, new UTF8Encoding(false),true))
                            {
                                var lagRead = this.m_ser.Deserialize(mread) as Lag;
                                if (lagRead != null)
                                {
                                    retColl.Add(lagRead);
                                }
                                else
                                {
                                    Log.Error(string.Format("Could not deserialize file {0}", file));
                                }
                            }
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, string.Format("error with file {0}", file));
                    }
                    
                }

                return retColl;
            }
            catch (Exception e )
            {
                Log.Error(e, "GetAllFromDatabase");
                throw;
            }
        }

        public void StoreToDatabase(Collection<Lag> allinfo, string baseName)
        {
            if (string.IsNullOrEmpty(baseName))
            {
               Log.Error("StoreToDatabase Path must be specified");
               throw new InvalidOperationException("Path must be specified");   
            }

            if (!Directory.Exists(baseName))
            {
                var errormsg = string.Format("Path does not exsits {0}", baseName);
                Log.Error(errormsg);
                throw new InvalidOperationException(errormsg);
            }
            try
            {

                BackupDir(baseName);            
            
                foreach (var utskriftsLag in allinfo)
                {
                    string filename = Path.Combine(baseName, string.Format("Hold_{0}_Lag_{1}.xml", "1", utskriftsLag.LagNummer));
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = false;
                    settings.Encoding = new UTF8Encoding(false);

                    using (XmlWriter Write = XmlWriter.Create(filename, settings))
                    {
                        m_ser.Serialize(Write, utskriftsLag);
                    }
                }
            }
            catch (Exception e )
            {
                Log.Error(e, "Error StoreToDatabase");
                throw;
            }
        }

        private void BackupDir(string baseName)
        {
            try
            {
                var backupdir = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                backupdir = Path.Combine(baseName, backupdir);
                Directory.CreateDirectory(backupdir);
                var allfilesToBackup = Directory.GetFiles(baseName, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in allfilesToBackup)
                {
                    var filenameonly = Path.GetFileName(file);
                    File.Move(file, Path.Combine(backupdir, filenameonly));
                }
            }
            catch (Exception e )
            {
                Log.Error(e, "Error BackupDir");
                throw;
            }
            
        }

        public LagOppsettConfig GetOppsettConfig()
        {
           
            m_inputFinfeltPath = ConfigurationLoader.GetAppSettingsValue("FinFeltDatabase");
            m_inputGrovfeltPath = ConfigurationLoader.GetAppSettingsValue("GrovFeltDatabase");

            return new LagOppsettConfig { PathFinfelt = this.m_inputFinfeltPath, PathGrovfelt = this.m_inputGrovfeltPath };
        }
    }
}
