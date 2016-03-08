using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SendingResults.Diagnosis;

namespace SendingResults
{
    using System.Threading;

    using FileUploaderService;

    public partial class SendingResults : ServiceBase
    {
        private Thread m_runningThread;
        private FileSnifferEngine m_messageRetrieval;

        public SendingResults()
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
                    m_messageRetrieval = new FileSnifferEngine();
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
