using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Service.Servers
{
    using System.Threading;

    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Engine;

    public partial class OrionLeonService : ServiceBase
    {
        private Thread m_runningThread;
        private ComEngine m_messageRetrieval;
        public OrionLeonService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_runningThread = new Thread(RunningThread) { Name = "RunningThread" };
            m_runningThread.Start();
        }

        protected override void OnStop()
        {
            if (m_messageRetrieval != null)
            {
                m_messageRetrieval.Stop();
            }
        }

        public void RunningThread()
        {
            try
            {
                if (m_messageRetrieval == null)
                {
                    m_messageRetrieval = new ComEngine();
                }

                m_messageRetrieval.Start();

                if (m_messageRetrieval.ExitCode == 0)
                {
                    Log.Info("SendingResults running thread stopped");
                }
                else
                {
                    Log.Info("SendingResults running thread stopped with exit code {0}", m_messageRetrieval.ExitCode);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");

                throw;
            }
            finally
            {

            }


        }
    }
}
