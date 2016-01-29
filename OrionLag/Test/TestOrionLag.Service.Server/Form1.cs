using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestOrionLag.Service.Server
{
    using System.Threading;

    using OrionLag.Common.Diagnosis;
    using OrionLag.Server.Engine;

    public partial class Form1 : Form
    {
        private Thread m_runningThread;
        private ComEngine m_messageRetrieval;
        public Form1()
        {
            InitializeComponent();

            m_runningThread = new Thread(RunningThread) { Name = "RunningThread" };
            m_runningThread.Start();
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
