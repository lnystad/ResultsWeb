using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SendingResultBitmap
{
    using System.Threading;

    using BitmapSnifferEngine;
    using BitmapSnifferEngine.Logging;

    public partial class SendingResultBitmap : ServiceBase
    {
        private Thread m_runningThread;
        private BitmapSniffer m_messageRetrieval;

        public SendingResultBitmap()
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
                    m_messageRetrieval = new BitmapSniffer();
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
