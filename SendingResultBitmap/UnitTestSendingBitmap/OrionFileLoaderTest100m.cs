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
    public class OrionFileLoaderTest100m
    {
        [TestMethod]
        public void MinneLag1()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig(setupConfig);
            orion.Init(setupConfig);

            var files=Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag1\TR-1-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag1\TR-1-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-2.PNG"));
            var success=orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            Assert.AreEqual(1, orion.LagInfo.Count);
            var lag = orion.LagInfo[0];
            Assert.AreEqual(1, lag.LagNr);
            

            var now = DateTime.Now;
            var expDate= new DateTime(now.Year,now.Month,now.Day,0,0,0);
            Assert.AreEqual(expDate.ToString("s"), lag.ArrangeDate.ToString("s"));
            Assert.AreEqual(2, lag.Skiver.Count);
            int count = 1;
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();
           
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir,"TR-1-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-0.png")));
        }

        [TestMethod]
        public void Felt2Hold()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag3\TR-3-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag3\TR-3-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag3\TR-3-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag3\TR-3-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-4.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag3\TR-3-5.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-5.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag3\TR-3-6.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-3-6.PNG"));
            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            Assert.AreEqual(3, orion.LagInfo.Count);
            var lagSorted = orion.LagInfo.OrderBy(o => o.LagNr).ToList();
            Assert.AreEqual(1, lagSorted[0].LagNr);
            Assert.AreEqual(2, lagSorted[1].LagNr);
            Assert.AreEqual(3, lagSorted[2].LagNr);
            //Assert.AreEqual(HoldType.Felt, lagSorted[0].HoldType);
            //Assert.AreEqual(HoldType.Felt, lagSorted[1].HoldType);
            //Assert.AreEqual(HoldType.Minne, lagSorted[2].HoldType);

            var now = DateTime.Now;
            var expDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            Assert.AreEqual(expDate.ToString("s"), orion.LagInfo[0].ArrangeDate.ToString("s"));
            Assert.AreEqual(expDate.ToString("s"), orion.LagInfo[1].ArrangeDate.ToString("s"));
            Assert.AreEqual(expDate.ToString("s"), orion.LagInfo[2].ArrangeDate.ToString("s"));
            //Assert.AreEqual(2, lag.Skiver.Count);
            //int count = 1;
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-2.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-2-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-3-2-0.png")));
        }

        [TestMethod]
        public void Felt2Lag()
        {
            SetupConfiguration setupConfig = new SetupConfiguration();
            OrionFileLoader orion = new OrionFileLoader();
            InitConfig(setupConfig);
            orion.Init(setupConfig);

            var files = Directory.GetFiles(setupConfig.BitMapDir);
            if (files != null)
            {
                foreach (var filename in files)
                {
                    File.Delete(filename);
                }
            }
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag1\TR-1-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag1\TR-1-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-1-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag2\TR-2-1.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-1.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag2\TR-2-2.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-2.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag2\TR-2-3.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-3.PNG"));
            File.Copy(Path.Combine(setupConfig.BitMapDir, @"100m\Lag2\TR-2-4.PNG"), Path.Combine(setupConfig.BitMapDir, @"TR-2-4.PNG"));
            var success = orion.CheckForNewBitmapFiles(false);
            Assert.AreEqual(true, success);

            Assert.AreEqual(2, orion.LagInfo.Count);
            var lagSorted = orion.LagInfo.OrderBy(o => o.LagNr).ToList();
            Assert.AreEqual(1, lagSorted[0].LagNr);
            Assert.AreEqual(2, lagSorted[1].LagNr);
            
            //Assert.AreEqual(HoldType.Felt, lagSorted[0].HoldType);
            //Assert.AreEqual(HoldType.Minne, lagSorted[1].HoldType);
          
            var now = DateTime.Now;
            var expDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            Assert.AreEqual(expDate.ToString("s"), orion.LagInfo[0].ArrangeDate.ToString("s"));
            Assert.AreEqual(expDate.ToString("s"), orion.LagInfo[1].ArrangeDate.ToString("s"));
            
            //Assert.AreEqual(2, lag.Skiver.Count);
            //int count = 1;
            var filesInDir = Directory.GetFiles(setupConfig.BitMapDir).ToList();

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-1-1.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-1-2-1.png")));
           

            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-1-0.png")));
            Assert.IsTrue(filesInDir.Contains(Path.Combine(setupConfig.BitMapDir, "TR-2-2-0.png")));
           
           
        }

        private static void InitConfig(SetupConfiguration setupConfig)
        {
            setupConfig.BitMapDir = @"C:\Users\lan\Source\Repos\ResultsWeb\SendingResultBitmap\UnitTestSendingBitmap\TestData";
            setupConfig.BitMapBackupDir = @"C:\Orion2\Test100m";
            setupConfig.BaneType = BaneType.FinFelt;
            setupConfig.PausedBetweenHold = false;
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = false, HoldNr = 0, HoldType = HoldType.Minne, SluttSkive = 2, StartSkive = 1 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = true, HoldNr = 1, HoldType = HoldType.Felt, SluttSkive = 4, StartSkive = 3 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = true, HoldNr = 2, HoldType = HoldType.Felt, SluttSkive = 6, StartSkive = 5 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = true, HoldNr = 3, HoldType = HoldType.Felt, SluttSkive = 8, StartSkive = 7 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = true, HoldNr = 4, HoldType = HoldType.Felt, SluttSkive = 9, StartSkive = 10 });
            setupConfig.HoldConfig.Add(
                new HoldTypeConfiguration() { VenteBenk = 0, FixLagnr = true, HoldNr = 5, HoldType = HoldType.Felt, SluttSkive = 12, StartSkive = 11 });

        }

    }
}
