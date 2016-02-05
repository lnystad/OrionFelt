using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Engine
{
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using System.Windows.Media;

    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Database;

    public class ComEngine
    {
        private bool m_stopMe;
        public int ExitCode { get; set; }

        private LeonCommunicationDetection m_leonCommDetection;

        private OrionCommunicationDetection m_OrionCommDetection;
        private DatabaseEngine m_DatabaseEngine;

        private ConvertOrionLeon m_convertLeonToOrion;
        public void Start()
        {
            try
            {
                this.InitApplication();
                Log.Info("Starting");
                while (!this.m_stopMe)
                {
                    List<Lag> inputFomLeon= null;
                    bool initCom = false;
                    // Her sjekkes oppropslister fra Leon
                    if(this.m_leonCommDetection.CheckComfiles(out inputFomLeon,out initCom))
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
                        else
                        {
                            Log.Info("FileDetected no update");
                        }
                    }

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
            Lag retLag = null;

            if (inputFomOrion == null && !allresultsComm)
            {
                return null;
            }

            foreach (var result in inputFomOrion)
            {
                int LagNo = result.LagNummer + 1;
                if (retLag == null)
                {
                    retLag = new Lag(LagNo, result.OrionHoldId, 0);
                }
                else
                {
                    if (LagNo != retLag.LagNummer)
                    {
                        Log.Error(" Feil i lagnummer ved oppdatering av totresultat {0} forvantet={1}", LagNo, retLag.LagNummer);
                        continue;
                    }
                }


                if (result.Skytter != null)
                {
                    //var skive = new Skiver();
                    //skive.Skytter = result.Skytter;
                    //foreach (var skive in retLag.SkiverILaget)
                    //{
                    //    if (skive.Skytter != null)
                    //    {
                    //        if(skive.Skytter)
                    //    }
                    //}
                }
            }
            if (retLag != null)
            {
                var input = new List<Lag>() { retLag };
                var LeonLag = m_convertLeonToOrion.ConvertToLeonLag(input);
                var updatedLag = m_DatabaseEngine.GetOrionLagWithSum(LeonLag);
                var OrionLag = m_convertLeonToOrion.ConvertToOrionLag(updatedLag);
                return OrionLag;
            }


            return null;

        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.m_stopMe = true;
        }



        /// <summary>
        /// The init application.
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
