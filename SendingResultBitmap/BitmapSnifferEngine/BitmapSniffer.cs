using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine
{
    using System.Configuration;
    using System.IO;
    using System.Threading;
    using System.Xml.Serialization;

    using BitmapSnifferEngine.Common;
    using BitmapSnifferEngine.Logging;
    using BitmapSnifferEngine.Orion;

    public class BitmapSniffer
    {
        #region Fields

        /// <summary>
        /// The bk up bitmap.
        /// </summary>
        private bool BkUpBitmap;

    
      
        /// <summary>
        /// The m_install dir.
        /// </summary>
        private string m_installDir;

        /// <summary>
        /// The m_my ftp util.
        /// </summary>
       
        /// <summary>
        /// The m_my orion file loader bkup.
        /// </summary>
        private OrionFileLoader m_myOrionFileLoaderBkup;

        /// <summary>
        /// The m_remote bit map dir 100 m.
        /// </summary>
     
        /// <summary>
        /// The m_stop me.
        /// </summary>
        private bool m_stopMe;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the exit code.
        /// </summary>
        public int ExitCode { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            try
            {
                this.InitApplication();
                Log.Info("Starting");
                while (!this.m_stopMe)
                {


                   
                        bool upload = this.m_myOrionFileLoaderBkup.CheckForNewBitmapFiles();
                        if (upload)
                        {
                            var bitmapFiles = this.m_myOrionFileLoaderBkup.BackUpBitMaps();
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

        #endregion

        #region Methods

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


            
            string bitMapConfigFile = ConfigurationManager.AppSettings["ConfigFile"];
            if (string.IsNullOrEmpty(bitMapConfigFile))
            {
                Log.Error("ConfigFile not set");
                m_stopMe = true;
                return;
            }
            SetupConfiguration inputConfig;

            if (!File.Exists(bitMapConfigFile))
            {
                Log.Error("ConfigFile not found {0}", bitMapConfigFile);
                m_stopMe = true;
                return;
            }
            else
            {
                XmlSerializer reader = new XmlSerializer(typeof(SetupConfiguration));
                System.IO.StreamReader file =  new System.IO.StreamReader(bitMapConfigFile);

                // Deserialize the content of the file into a Book object.
                inputConfig  = (SetupConfiguration)reader.Deserialize(file);
            }

            //string bitMapFeltstr = ConfigurationManager.AppSettings["BitMapStevneType"];
            //BaneType bitmapfelt = BaneType.Undefined;
            //if (!string.IsNullOrEmpty(bitMapFeltstr))
            //{
            //    BaneType testVal;
            //    if (Enum.TryParse(bitMapFeltstr, out testVal))
            //    {
            //        // We now have an enum type.
            //        bitmapfelt = testVal;
            //    }
            //}

            //string bitMapSkiveriLagetstr = ConfigurationManager.AppSettings["BitMapSkiveriLaget"];
            //int bitMapSkiveriLaget = 0;
            //if (!string.IsNullOrEmpty(bitMapSkiveriLagetstr))
            //{
            //    int testValint;
            //    if (int.TryParse(bitMapSkiveriLagetstr, out testValint))
            //    {
            //        bitMapSkiveriLaget = testValint;
            //    }
            //}

            //string bitMapStartHoldstr = ConfigurationManager.AppSettings["BitMapStartHold"];
            //int bitMapStartHold = 1;
            //if (!string.IsNullOrEmpty(bitMapStartHoldstr))
            //{
            //    int testValint;
            //    if (int.TryParse(bitMapStartHoldstr, out testValint))
            //    {
            //        bitMapStartHold = testValint;
            //    }
            //}


            //string bitMapDir = ConfigurationManager.AppSettings["BitMapDir"];
            //string bitMapBackupDir = ConfigurationManager.AppSettings["BitMapBackupDir"];
            //string bitMapFetchTimeOutstr = ConfigurationManager.AppSettings["BitMapFetchTimeOut"];




            //if (!string.IsNullOrEmpty(bitMapDir) && !string.IsNullOrEmpty(bitMapBackupDir))
            //{
            //    if (!Directory.Exists(bitMapDir))
            //    {
            //        Log.Error("Bitmap dir not exsist {0} ", bitMapDir);
            //        this.BkUpBitmap = false;
            //    }
            //    else
            //    {
            //        if (!Directory.Exists(bitMapBackupDir))
            //        {
            //            Log.Error("bitMapBackupDir dir not exsist {0} ", bitMapBackupDir);
            //            this.BkUpBitmap = false;
            //        }
            //        else
            //        {
            //            this.BkUpBitmap = true;
            //            Log.Error("Bitmaps will be backedup from {0} to {1} ", bitMapDir, bitMapBackupDir);
            //        }
            //    }
            //}

            this.m_myOrionFileLoaderBkup = new OrionFileLoader();


            if (this.m_myOrionFileLoaderBkup.Init(inputConfig))
            {
                
                Log.Info("Bitmap Bakup from Orion Initiated with sucess");
            }
            else
            {
                Log.Info("Bitmap Bakup from Orion not Initiated");
                this.m_stopMe = true;
            }

            

            

            
        }


        #endregion
    }
}
