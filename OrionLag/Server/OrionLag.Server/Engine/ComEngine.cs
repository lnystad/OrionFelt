namespace OrionLag.Server.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Database;

    public class ComEngine
    {
        private ConvertOrionLeon m_convertLeonToOrion;

        private DatabaseEngine m_DatabaseEngine;

        private LeonCommunicationDetection m_leonCommDetection;

        private OrionCommunicationDetection m_OrionCommDetection;

        private bool m_sendResultToLeon;

        private bool m_stopMe;

        public int ExitCode { get; set; }

        public void Start()
        {
            try
            {
                this.InitApplication();
                Log.Info("Starting");
                while (!this.m_stopMe)
                {
                    List<Lag> inputFomLeon = null;
                    bool initCom = false;
                    // Her sjekkes oppropslister fra Leon
                    if (this.m_leonCommDetection.CheckComfiles(out inputFomLeon, out initCom))
                    {
                        if (this.m_DatabaseEngine.UpdateDataBaseFromLeon(inputFomLeon, initCom))
                        {
                            Log.Info("FileDetected starting updating Orion");
                            var OrionLag = this.m_convertLeonToOrion.ConvertToOrionLag(inputFomLeon);

                            while (!this.m_OrionCommDetection.SendToOrion(OrionLag))
                            {
                                Thread.Sleep(1000);
                                if (this.m_stopMe)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Log.Info("FileDetected no update");
                        }
                    }

                    List<SkytterResultat> inputFomOrion;
                    bool allresultsComm;

                    // Her sjekkes Resultater fra Orion
                    if (this.m_OrionCommDetection.CheckComfiles(out inputFomOrion, out allresultsComm))
                    {
                        var leonResults = this.m_convertLeonToOrion.ConvertToLeonLag(inputFomOrion);

                        if (this.m_DatabaseEngine.UpdateDataBaseFromOrion(leonResults, allresultsComm))
                        {
                            if (this.m_sendResultToLeon)
                            {
                                Log.Info("FileDetected starting updating Leon");

                                while (!this.m_leonCommDetection.SendToLeon(leonResults, allresultsComm))
                                {
                                    Thread.Sleep(1000);
                                    if (this.m_stopMe)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Log.Info("FileDetected no update");
                        }
                    }
                    if (inputFomOrion != null && inputFomOrion.Count > 0)
                    {
                        List<Lag> updatedLagList = GetUpdatedLag(inputFomOrion, allresultsComm);

                        if (updatedLagList != null && updatedLagList.Count > 0)
                        {
                            Log.Info("Sending Updated Lag to Orion");
                            while (!this.m_OrionCommDetection.SendToOrion(updatedLagList))
                            {
                                Thread.Sleep(1000);
                                if (this.m_stopMe)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    Thread.Sleep(2000);
                }

                Log.Info("Stopping");
                this.ExitCode = 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                this.ExitCode = 10;
                throw;
            }
        }

        private List<Lag> GetUpdatedLag(List<SkytterResultat> inputFomOrion, bool allresultsComm)
        {
            if (inputFomOrion == null && !allresultsComm)
            {
                return null;
            }


            List<int> lagNo = new List<int>();

            foreach (var result in inputFomOrion)
            {
                var nesteLag = result.LagNummer + 1;
                if (!lagNo.Any(x => x == nesteLag))
                {
                    lagNo.Add(nesteLag);
                }
            }

            List<Lag> leonLagList = new List<Lag>();

            foreach (var lag in lagNo)
            {
                var leonLagno = m_convertLeonToOrion.GetLeonLagNumber(lag);

                foreach (var leonLag in leonLagno)
                {
                    var updatedLag = m_DatabaseEngine.GetLeonLagWithSum(leonLag);

                    if (updatedLag != null)
                    {
                        leonLagList.Add(updatedLag);
                    }
                }
            }

            var OrionLag = m_convertLeonToOrion.ConvertToOrionLag(leonLagList);
            List<Lag> selectedOrionLag = new List<Lag>();
            foreach (var lag in lagNo)
            {
                var found = OrionLag.Find(x => x.LagNummer == lag);
                if (found != null)
                {
                    selectedOrionLag.Add(found);
                }
            }

            return selectedOrionLag;
        }

        /// <summary>
        ///     The stop.
        /// </summary>
        public void Stop()
        {
            this.m_stopMe = true;
        }

        /// <summary>
        ///     The init application.
        /// </summary>
        private void InitApplication()
        {
            var logfile = ConfigurationManager.AppSettings["LogFile"];

            var LoggingLevelsString = ConfigurationManager.AppSettings["LoggingLevels"];
            LoggingLevels enumLowestTrace = LoggingLevels.Info;
            if (!string.IsNullOrEmpty(LoggingLevelsString))
            {
                if (Enum.TryParse(LoggingLevelsString, true, out enumLowestTrace))
                {
                    enumLowestTrace = enumLowestTrace;
                }
                else
                {
                    enumLowestTrace = LoggingLevels.Info;
                }
            }

            var resultToLeon = ConfigurationManager.AppSettings["SendResultToLeon"];
            if (!string.IsNullOrEmpty(resultToLeon))
            {
                bool res = false;
                if (bool.TryParse(resultToLeon, out res))
                {
                    this.m_sendResultToLeon = res;
                }
            }
            else
            {
                this.m_sendResultToLeon = false;
            }

            var fileAppsender = new FileAppender(logfile, enumLowestTrace, LoggingLevels.Trace);
            Log.AddAppender(fileAppsender);
            Log.Info("Starting Config");

            this.m_OrionCommDetection = new OrionCommunicationDetection();
            this.m_OrionCommDetection.Init();
            this.m_leonCommDetection = new LeonCommunicationDetection();
            this.m_leonCommDetection.Init();
            this.m_DatabaseEngine = new DatabaseEngine();
            this.m_DatabaseEngine.Init();
            this.m_convertLeonToOrion = new ConvertOrionLeon();
            this.m_convertLeonToOrion.InitConverter();
        }
    }
}