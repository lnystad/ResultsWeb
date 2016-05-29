using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestSendingBitmap
{
    using System.IO;
    using System.Linq;
    using System.Runtime.Remoting;

    using BitmapSnifferEngine.Common;
    using BitmapSnifferEngine.Orion;

    [TestClass]
    public class OrionFileLoaderTest200m
    {
        [TestMethod]
        public void Lag1PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files=Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag1\TR-1-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag1\TR-1-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag1\TR-1-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag1\TR-1-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-4.PNG"));
            var success=orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            Assert.AreEqual(1, orion.LagInfo.Count);
            var lag = orion.LagInfo[0];
            Assert.AreEqual(1, lag.LagNr);
            

            var now = DateTime.Now;
            var expDate= new DateTime(now.Year,now.Month,now.Day,0,0,0);
            Assert.AreEqual(expDate.ToString("s"), lag.ArrangeDate.ToString("s"));
            Assert.AreEqual(4, lag.Skiver.Count);
            int count = 1;
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();
           
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir,"TR-1-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-4-0.png")));
        }

        [TestMethod]
        public void Lag2PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag2\TR-2-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag2\TR-2-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag2\TR-2-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag2\TR-2-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-4.PNG"));
            
            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

           
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-4-0.png")));
        }

        [TestMethod]
        public void Lag3PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-4.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-7.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-7.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-8.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-8.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-9.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-9.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag3\TR-3-10.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-10.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-4-0.png")));
           

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-3-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-4-1.png")));

        }

        [TestMethod]
        public void Lag4PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-4.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-7.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-7.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-8.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-8.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-9.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-9.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag4\TR-4-10.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-4-10.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-4-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-4-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-4-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-4-4-0.png")));


            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-2-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-3-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-4-1.png")));

        }

        [TestMethod]
        public void Lag5PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-4.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-7.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-7.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-8.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-8.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-9.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-9.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-10.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-10.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-17.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-17.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-18.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-18.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-19.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-19.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag5\TR-5-20.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-20.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-4-0.png")));


            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-2-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-3-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-4-1.png")));

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-3-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-4-2.png")));

        }

        [TestMethod]
        public void Lag7PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-4.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-7.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-7.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-8.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-8.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-9.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-9.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-10.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-10.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-17.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-17.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-18.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-18.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-19.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-19.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-7-20.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-7-20.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-4-0.png")));


            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-2-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-3-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-4-1.png")));

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-1-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-2-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-3-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-4-2.png")));
        }

        [TestMethod]
        public void Lag7PC2()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC2(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-1-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-1-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-1-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag7\TR-1-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-4.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-3.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-3.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-3-3.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-4-3.png")));

        }


        [TestMethod]
        public void Lag11PC1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC1(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-4.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-7.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-7.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-8.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-8.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-9.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-9.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-10.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-10.PNG"));

            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-17.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-17.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-18.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-18.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-19.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-19.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-11-20.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-11-20.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-11-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-11-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-11-3-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-11-4-0.png")));


            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-9-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-9-2-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-9-3-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-9-4-1.png")));

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-1-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-2-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-3-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-7-4-2.png")));
        }

        [TestMethod]
        public void Lag11PC2()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC2(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-5-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-5-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-5-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-5-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-5-4.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-1-3.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-2-3.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-3-3.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-5-4-3.png")));

        }
        [TestMethod]
        public void Lag11PC3()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC3(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-3-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-3-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-3-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-3-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-4.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-1-4.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-2-4.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-3-4.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-4-4.png")));

        }

        [TestMethod]
        public void Lag11PC4()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig200PC4(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-1-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-1-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-1-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"200m\Lag11\TR-1-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-4.PNG"));

            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-5.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-5.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-3-5.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-4-5.png")));

        }


        private static void InitConfig200PC1(SetupConfiguration setupConfig)
        {
            setupConfig.BitMapDir = @"C:\Users\lan\Source\Repos\ResultsWeb\SendingResultBitmap\UnitTestSendingBitmap\TestData";
            setupConfig.BitMapBackupDir = @"C:\Orion2\Test200m";
            setupConfig.BaneType = BaneType.GrovFelt;
            setupConfig.PausedBetweenHold = false;
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr=false ,HoldNr = 0, HoldType = HoldType.Minne, SluttSkive = 4, StartSkive = 1 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 1, FixLagnr = true, HoldNr = 1, HoldType = HoldType.Felt, SluttSkive = 10, StartSkive = 7 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 2, FixLagnr = true, HoldNr = 2, HoldType = HoldType.Felt, SluttSkive = 20, StartSkive = 17 });
            //setupConfig.HoldConfig.Add(
            //    new HoldTypeConfiguration() { VenteBenk = 0, HoldNr = 3, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 1 });
            //setupConfig.HoldConfig.Add(
            //    new HoldTypeConfiguration() { VenteBenk = 0, HoldNr = 4, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 1 });
            //setupConfig.HoldConfig.Add(
            //    new HoldTypeConfiguration() { VenteBenk = 0, HoldNr = 5, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 1 });
        }

        private static void InitConfig200PC2(SetupConfiguration setupConfig)
        {
            setupConfig.BitMapDir = @"C:\Users\lan\Source\Repos\ResultsWeb\SendingResultBitmap\UnitTestSendingBitmap\TestData";
            setupConfig.BitMapBackupDir = @"C:\Orion2\Test200m";
            setupConfig.BaneType = BaneType.GrovFelt;
            setupConfig.PausedBetweenHold = false;
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = false, HoldNr = 3, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 1 });
        }

        private static void InitConfig200PC3(SetupConfiguration setupConfig)
        {
            setupConfig.BitMapDir = @"C:\Users\lan\Source\Repos\ResultsWeb\SendingResultBitmap\UnitTestSendingBitmap\TestData";
            setupConfig.BitMapBackupDir = @"C:\Orion2\Test200m";
            setupConfig.BaneType = BaneType.GrovFelt;
            setupConfig.PausedBetweenHold = false;
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = false, HoldNr = 4, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 1 });
        }

        private static void InitConfig200PC4(SetupConfiguration setupConfig)
        {
            setupConfig.BitMapDir = @"C:\Users\lan\Source\Repos\ResultsWeb\SendingResultBitmap\UnitTestSendingBitmap\TestData";
            setupConfig.BitMapBackupDir = @"C:\Orion2\Test200m";
            setupConfig.BaneType = BaneType.GrovFelt;
            setupConfig.PausedBetweenHold = false;
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = false, HoldNr = 5, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 1 });
        }
    }
}
