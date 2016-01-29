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
                    //List<Lag> inputFomOrion = null;
                    //bool initCom = false;
                    //m_OrionCommDetection.CheckComfiles();
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
