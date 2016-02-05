namespace OrionLag.Server.Database
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;

    public class DatabaseEngine
    {
        private string m_DatabaseDir;

        private List<Lag> m_LagList;

        private List<Resultat> m_Resultater;

        private XmlSerializer m_serLag = new XmlSerializer(typeof(List<Lag>));

        private XmlSerializer m_serResultat = new XmlSerializer(typeof(List<Resultat>));

        private XmlSerializer m_serSkytter = new XmlSerializer(typeof(List<Skytter>));

        private List<Skytter> m_Skyttere;

        public void Init()
        {
            this.m_DatabaseDir = ConfigurationManager.AppSettings["Database"];
            if (string.IsNullOrEmpty(this.m_DatabaseDir))
            {
                throw new ArgumentNullException("Database", "Databasedir must have value");
            }

            if (!Directory.Exists(this.m_DatabaseDir))
            {
                throw new ArgumentNullException("Database", string.Format("Databasedir must exsist {0}", this.m_DatabaseDir));
            }

            this.m_LagList = new List<Lag>();
            this.m_Skyttere = new List<Skytter>();
            this.m_Resultater = new List<Resultat>();
            this.IniDatabase();
        }

        public bool UpdateDataBaseFromLeon(List<Lag> inputFomLeon, bool initCom)
        {
            try
            {
                bool flush = false;

                if (initCom)
                {
                    this.m_LagList.Clear();
                    this.m_Skyttere.Clear();
                    if (inputFomLeon.Count > 0)
                    {
                        foreach (var inputel in inputFomLeon)
                        {
                            Lag currentLag = null;
                            var foundTeam = this.m_LagList.Find(x => x.LagNummer == inputel.LagNummer);
                            if (foundTeam != null)
                            {
                                currentLag = UpdateTeam(foundTeam, inputel);
                                flush = true;
                            }
                            else
                            {
                                currentLag = InsertNewTeam(inputel);
                                flush = true;
                            }

                            foreach (var skive in inputel.SkiverILaget)
                            {
                                if (UpdateSkytterByOrionId(skive.Skytter))
                                {
                                    flush = true;
                                }

                                UpdateSkytterTotalSum(skive.Skytter);
                            }
                        }

                        if (flush)
                        {
                            FlushDatabase();
                        }

                        return true;
                    }

                    return false;
                }

                if (inputFomLeon.Count < 10)
                {
                    var inputel = inputFomLeon[0];
                    var foundTeam = this.m_LagList.Find(x => x.LagNummer == inputel.LagNummer);
                    Lag currentLag = null;
                    if (foundTeam != null)
                    {
                        currentLag = UpdateTeam(foundTeam, inputel);
                    }
                    else
                    {
                        currentLag = InsertNewTeam(inputel);
                    }

                    foreach (var skive in inputel.SkiverILaget)
                    {
                        UpdateSkytterByOrionId(skive.Skytter);
                        UpdateSkytterTotalSum(skive.Skytter);
                    }
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        internal bool UpdateDataBaseFromOrion(List<SkytterResultat> inputFomOrion, bool allresultsComm)
        {
            if (inputFomOrion == null)
            {
                return true;
            }
            try
            {
                bool flush = false;
                if (allresultsComm)
                {
                    this.m_Resultater.Clear();
                }

                foreach (var resultat in inputFomOrion)
                {
                    var foundSkytterRes = this.m_Resultater.Find(x => x.SkytterNr == resultat.SkytterNr);
                    if (foundSkytterRes != null)
                    {
                        flush = true;
                        foundSkytterRes.Update(resultat);
                    }
                    else
                    {
                        flush = true;
                        foundSkytterRes = new Resultat(resultat.SkytterNr, resultat.SkytterId, resultat.Serier);
                        this.m_Resultater.Add(foundSkytterRes);
                    }
                }

                if (flush)
                {
                    this.FlushDatabase();
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        private void FlushDatabase()
        {
            CheckIndexes();
            BackupDir();

            string filename = Path.Combine(m_DatabaseDir, "Lag.xml");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.Encoding = new UTF8Encoding(false);

            using (XmlWriter Write = XmlWriter.Create(filename, settings))
            {
                m_serLag.Serialize(Write, this.m_LagList);
            }

            filename = Path.Combine(m_DatabaseDir, "Skytter.xml");
            settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.Encoding = new UTF8Encoding(false);

            using (XmlWriter Write = XmlWriter.Create(filename, settings))
            {
                m_serSkytter.Serialize(Write, this.m_Skyttere);
            }

            filename = Path.Combine(m_DatabaseDir, "Resultat.xml");
            settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.Encoding = new UTF8Encoding(false);

            using (XmlWriter Write = XmlWriter.Create(filename, settings))
            {
                m_serResultat.Serialize(Write, this.m_Resultater);
            }
        }

        private void CheckIndexes()
        {
            foreach (var lag in this.m_LagList)
            {
                foreach (var skive in lag.SkiverILaget)
                {
                    if (skive.Skytter != null)
                    {
                        skive.SkytterGuid = skive.Skytter.Id;
                        skive.Skytter = null;
                    }
                }
            }
        }

        private void BackupDir()
        {
            try
            {
                var backupdir = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                backupdir = Path.Combine(m_DatabaseDir, backupdir);
                Directory.CreateDirectory(backupdir);
                var allfilesToBackup = Directory.GetFiles(m_DatabaseDir, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var file in allfilesToBackup)
                {
                    var filenameonly = Path.GetFileName(file);
                    File.Move(file, Path.Combine(backupdir, filenameonly));
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error BackupDir");
                throw;
            }
        }

        private void IniDatabase()
        {
            string currentFile = string.Empty;
            try
            {
                currentFile = Path.Combine(m_DatabaseDir, "Lag.xml");
                if (File.Exists(currentFile))
                {
                    MemoryStream stream = ReadStream(currentFile);

                    using (StreamReader mread = new StreamReader(stream, new UTF8Encoding(false), true))
                    {
                        this.m_LagList = this.m_serLag.Deserialize(mread) as List<Lag>;
                    }
                }
                currentFile = Path.Combine(m_DatabaseDir, "Skytter.xml");
                if (File.Exists(currentFile))
                {
                    MemoryStream stream = ReadStream(currentFile);

                    using (StreamReader mread = new StreamReader(stream, new UTF8Encoding(false), true))
                    {
                        this.m_Skyttere = this.m_serSkytter.Deserialize(mread) as List<Skytter>;
                    }
                }

                currentFile = Path.Combine(m_DatabaseDir, "Resultat.xml");
                if (File.Exists(currentFile))
                {
                    MemoryStream stream = ReadStream(currentFile);

                    using (StreamReader mread = new StreamReader(stream, new UTF8Encoding(false), true))
                    {
                        this.m_Resultater = this.m_serResultat.Deserialize(mread) as List<Resultat>;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("error with file {0}", currentFile));
            }
        }

        private MemoryStream ReadStream(string filename)
        {
            using (FileStream fileRead = new FileStream(filename, FileMode.Open, FileAccess.Read))
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
                    {
                        break;
                    }

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;

                return new MemoryStream(bytes);
            }
        }

        private void UpdateSkytterTotalSum(Skytter skytter)
        {
            var res = m_Resultater.FirstOrDefault(x => x.SkytterNr == skytter.SkytterNr);
            if (res != null)
            {
                skytter.TotalSum = res.TotalSum();
            }
        }

        private bool UpdateSkytterByOrionId(Skytter skytter)
        {
            if (skytter == null)
            {
                return false;
            }

            Skytter found = null;
            if (!string.IsNullOrEmpty(skytter.SkytterNr))
            {
                found = m_Skyttere.FirstOrDefault(x => x.SkytterNr == skytter.SkytterNr);
            }
            else
            {
                Log.Warning("Skytter named {0} missing SkytterId {0} {1} {2}", skytter.Name, skytter.Skytterlag, skytter.SkytterNr);
                found = m_Skyttere.FirstOrDefault(x => x.Name == skytter.Name && x.Skytterlag == skytter.Skytterlag);
                if (found != null)
                {
                    Log.Warning("Skytter named {0} missing SkytterId {0} {1} {2} found By Name", skytter.Name, skytter.Skytterlag, skytter.SkytterNr);
                }
            }

            if (found == null)
            {
                found = new Skytter(skytter);
                found.Klasse = UpdateString(skytter.Klasse);
                found.Name = UpdateString(skytter.Name);
                found.Skytterlag = UpdateString(skytter.Skytterlag);
                m_Skyttere.Add(found);
                return true;
            }
            else
            {
                found.Klasse = UpdateString(skytter.Klasse);
                found.Name = UpdateString(skytter.Name);
                found.Skytterlag = UpdateString(skytter.Skytterlag);
                found.SkytterNr = UpdateString(skytter.SkytterNr);
                found.SkytterlagNr = skytter.SkytterlagNr;
                return true;
            }
        }

        private string UpdateString(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value.Trim();
        }

        private Lag InsertNewTeam(Lag inputel)
        {
            var insert = new Lag(inputel);
            m_LagList.Add(insert);
            return insert;
        }

        private Lag UpdateTeam(Lag foundTeam, Lag inputlag)
        {
            foundTeam.LagTid = inputlag.LagTid;
            foundTeam.MaxSkiveNummer = inputlag.MaxSkiveNummer;
            foundTeam.OrionHoldId = inputlag.OrionHoldId;
            foundTeam.SkiverILaget.Clear();
            foreach (var skive in inputlag.SkiverILaget)
            {
                var newSKive = new Skiver(skive);

                foundTeam.SkiverILaget.Add(newSKive);
            }

            return foundTeam;
        }

        public List<Lag> GetOrionLagWithSum(List<Lag> updatedLagList)
        {
            List<Lag> retColl = new List<Lag>();
            if (updatedLagList == null)
            {
                return retColl;
            }

            foreach (var lag in updatedLagList)
            {
                
            }


            return retColl;
        }
    }
}