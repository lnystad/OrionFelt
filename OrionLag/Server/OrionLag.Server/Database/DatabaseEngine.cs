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

        private List<LagDefinition> m_LagDefinitionList;

        private XmlSerializer m_serLagDef = new XmlSerializer(typeof(List<LagDefinition>));

        

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
            this.m_LagDefinitionList = new List<LagDefinition>();
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
                    this.m_Resultater.Clear();
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
                        flush = true;
                    }
                    else
                    {
                        currentLag = InsertNewTeam(inputel);
                        flush = true;
                    }

                    foreach (var skive in inputel.SkiverILaget)
                    {
                        UpdateSkytterByOrionId(skive.Skytter);
                        UpdateSkytterTotalSum(skive.Skytter);
                    }

                    if (flush)
                    {
                        FlushDatabase();
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
                    if (string.IsNullOrEmpty(resultat.SkytterNr))
                    {
                        Log.Error("ResultFile with skytter no Number {0}", resultat.LagNummer, resultat.SkiveNr);
                        continue;
                    }

                    if (resultat.Skytter==null)
                    {
                        Log.Error("ResultFile with empty skytter no Number {0}", resultat.LagNummer, resultat.SkiveNr);
                        continue;
                    }



                    var foundSkytterRes = this.m_Resultater.Find(x => x.SkytterNr == resultat.SkytterNr);

                    var foundSkytter= this.m_Skyttere.Find(x => x.SkytterNr == resultat.SkytterNr);
                    if (foundSkytter != null)
                    {
                        resultat.SkytterId = foundSkytter.Id;
                    }
                    else
                    {
                        this.m_Skyttere.Add(new Skytter(resultat.Skytter));
                    }


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

                    resultat.OrionTotalSum = foundSkytterRes.TotalSum();
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
                    if (filenameonly.ToUpper() != "LAGDEF.XML")
                    {
                        File.Move(file, Path.Combine(backupdir, filenameonly));
                    }
                    
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

                currentFile = Path.Combine(m_DatabaseDir, "LagDef.xml");
                if (File.Exists(currentFile))
                {
                    MemoryStream stream = ReadStream(currentFile);

                    using (StreamReader mread = new StreamReader(stream, new UTF8Encoding(false), true))
                    {
                        this.m_LagDefinitionList = this.m_serLagDef.Deserialize(mread) as List<LagDefinition>;
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
            if (skytter == null)
            {
                return;
            }

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
            if (inputel == null)
            {
                return null;
            }

            var insert = new Lag(inputel);
            List<Skiver> feilvalidertSkive=new List<Skiver>();
            foreach (var skive in insert.SkiverILaget)
            {
                if (skive.SkiveNummer <= 0)
                {
                    feilvalidertSkive.Add(skive);
                    Log.Warning("Fant Lag med tomt skivenr ved insett av nytt lag lag={0}", insert.LagNummer);
                    continue;
                }
                
                if (skive.Skytter != null)
                {
                    if (skive.Skytter.SkytterNr == "0" && string.IsNullOrEmpty(skive.Skytter.Name))
                    {
                        skive.Free = true;
                        skive.Skytter = null;
                        skive.SkytterGuid = null;
                    }
                    else
                    {
                        var foundSkytter = m_Skyttere.Find(y => y.SkytterNr == skive.Skytter.SkytterNr);
                        if (foundSkytter != null)
                        {
                            UpdateSkytter(foundSkytter, skive.Skytter);
                            skive.Skytter = null;
                            skive.SkytterGuid = foundSkytter.Id;
                        }
                        else
                        {
                            m_Skyttere.Add(skive.Skytter);
                            skive.SkytterGuid = skive.Skytter.Id;
                            skive.Skytter = null;
                            
                        }
                    }
                }
            }

            foreach (var delSKive in feilvalidertSkive)
            {
                insert.SkiverILaget.Remove(delSKive);
            }

            m_LagList.Add(insert);
            return insert;
        }

        private Lag UpdateTeam(Lag foundTeam, Lag inputlag)
        {
            foundTeam.LagTid = inputlag.LagTid;
            foundTeam.MaxSkiveNummer = inputlag.MaxSkiveNummer;
            foundTeam.OrionHoldId = inputlag.OrionHoldId;
            foreach (var skive in inputlag.SkiverILaget)
            {
                if (skive.SkiveNummer <= 0)
                {
                    Log.Warning("Fant Lag med tomt skivenr");
                    continue;
                }

                var foundSKive = foundTeam.SkiverILaget.Find(x => x.SkiveNummer == skive.SkiveNummer);
                if (foundSKive == null)
                {   Log.Info("Fant ikke skive nr {0} lag til lag nr {1}", skive.SkiveNummer, foundTeam.LagNummer);
                    foundSKive = new Skiver(skive);
                    foundTeam.SkiverILaget.Add(foundSKive);
                }

                if (skive.Skytter != null)
                {
                    if (skive.Skytter.SkytterNr == "0" && string.IsNullOrEmpty(skive.Skytter.Name))
                    {
                        foundSKive.Free = true;
                        foundSKive.Skytter = null;
                        foundSKive.SkytterGuid = null;
                    }
                    else
                    {
                        var foundSkytter = m_Skyttere.Find(y => y.SkytterNr == skive.Skytter.SkytterNr);
                        if (foundSkytter != null)
                        {
                            UpdateSkytter(foundSkytter, skive.Skytter);
                            foundSKive.Skytter = null;
                            foundSKive.SkytterGuid = foundSkytter.Id;
                        }
                        else
                        {
                            var nySkytter = new Skytter(skive.Skytter);
                            foundSKive.Skytter = null;
                            foundSKive.SkytterGuid = nySkytter.Id;
                            m_Skyttere.Add(nySkytter);
                            Log.Info("La til ny skytter Lag={0} skive{1} navn={2}", foundTeam.LagNummer, foundSKive.SkiveNummer, nySkytter.Name);
                        }
                    }
                    
                }
                else
                {
                    if (foundSKive.SkytterGuid != null)
                    {
                        var foundSkytter = m_Skyttere.Find(y => y.Id == foundSKive.SkytterGuid);
                        if (foundSkytter != null)
                        {
                            Log.Error(
                             "Remove skytter = {0} nr={1} from Lag={2} skive={3}",
                             foundSkytter.Name,
                             foundSkytter.SkytterNr,
                             foundTeam.LagNummer,
                             foundSKive.SkiveNummer);
                        }
                        
                        foundSKive.Skytter = null;
                    }

                    foundSKive.SkytterGuid = null;
                }

                //var newSKive = new Skiver(skive);

                //foundTeam.SkiverILaget.Add(newSKive);
            }

            return foundTeam;
        }

        private void UpdateSkytter(Skytter foundSkytter, Skytter skytter)
        {
            Log.Info("Updating skytter with number={0}", foundSkytter.SkytterNr);
            foundSkytter.Name = UpdateString(skytter.Name);
            foundSkytter.Klasse = UpdateString(skytter.Klasse);
            foundSkytter.Skytterlag = UpdateString(skytter.Skytterlag);
            foundSkytter.SkytterlagNr = skytter.SkytterlagNr;
        }

        public Lag GetLeonLagWithSum(int lag)
        {
            var funnetLag = m_LagList.Find(x => x.LagNummer == lag);
            Lag nyttLag = null;
            if (funnetLag != null)
            {
                nyttLag = new Lag(funnetLag);

                foreach (var lagskive in funnetLag.SkiverILaget)
                {
                    var nyskive = nyttLag.SkiverILaget.Find(x => x.SkiveNummer == lagskive.SkiveNummer);
                    if (nyskive == null)
                    {
                        continue;
                    }

                    if (lagskive.SkytterGuid != null)
                    {
                        var skytter=m_Skyttere.Find(y => y.Id == lagskive.SkytterGuid);
                        if (skytter == null)
                        {
                            continue;
                        }

                        nyskive.Skytter = new Skytter(skytter);
                        nyskive.SkytterGuid = skytter.Id;
                        nyskive.Skytter.TotalSum = 0;
                        var res = m_Resultater.Find(z => z.SkytterId == lagskive.SkytterGuid);
                        if (res != null)
                        {
                            nyskive.Skytter.TotalSum = res.TotalSum();
                            nyskive.Skytter.TotalFeltSum = res.FeltSum();
                        }
                    }
                }
                


            }


            return nyttLag;
        }

        public Lag GetLag(int lagNr)
        {
            Lag funnetLag = null;
            try
            {
                IniDatabase();
                var searchLag = m_LagList.Find(x => x.LagNummer == lagNr);
                if (searchLag == null)
                {
                    return null;
                }
                funnetLag = new Lag(searchLag);
                var lagdef = m_LagDefinitionList.Find(x => x.LagNummer == lagNr);
                if (lagdef != null)
                {
                    funnetLag.LagTid = lagdef.LagTid;
                }
                foreach (var skive in funnetLag.SkiverILaget)
                {
                    if (skive.SkytterGuid != null)
                    {
                        var skytt = m_Skyttere.Find(y => y.Id == skive.SkytterGuid);
                        if (skytt != null)
                        {
                            var skiveFunnet = searchLag.SkiverILaget.Find(ak => ak.SkiveNummer == skive.SkiveNummer);
                            if (skiveFunnet != null)
                            {
                                skiveFunnet.Skytter = new Skytter(skytt);
                                skiveFunnet.SkytterGuid = skytt.Id;
                                skiveFunnet.Skytter.Id = skytt.Id;
                            }
                        }
                    }
                }
                return funnetLag;
            }
            catch (Exception e)
            {
                Log.Error(e, "GetLag");
            }

            return funnetLag;
        }

        public List<Lag> GetLag()
        {
            List<Lag> lagRes= new List<Lag>();
            try
            {
                IniDatabase();

                foreach (var lag in m_LagList)
                {
                    var lagdef=m_LagDefinitionList.Find(x => x.LagNummer == lag.LagNummer);
                    Lag cpy = new Lag(lag);
                    if (lagdef != null)
                    {
                        cpy.LagTid = lagdef.LagTid;
                    }

                    foreach (var skive in lag.SkiverILaget)
                    {
                        if (skive.SkytterGuid != null)
                        {
                            var skytt = m_Skyttere.Find(y => y.Id == skive.SkytterGuid);
                            if (skytt != null)
                            {
                                var skiveFunnet = cpy.SkiverILaget.Find(ak => ak.SkiveNummer == skive.SkiveNummer);
                                if (skiveFunnet != null)
                                {
                                    skiveFunnet.Skytter = new Skytter(skytt);
                                    skiveFunnet.SkytterGuid = skytt.Id;
                                    skiveFunnet.Skytter.Id= skytt.Id;
                                }
                            }
                        }
                    }


                    lagRes.Add(cpy);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "GetLag");
            }

            return lagRes;
        }

        public List<Resultat> GetResultsForSkytter(Guid skytterId)
        {
            List<Resultat> retColl= new List<Resultat>();
            try
            {
                IniDatabase();

                var foundRes=m_Resultater.Find(res => res.SkytterId == skytterId);
                if (foundRes != null)
                {
                    retColl.Add(foundRes);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "GetLag");
            }

            return retColl;
        }
    }
}