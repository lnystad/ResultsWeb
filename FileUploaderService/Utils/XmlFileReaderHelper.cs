// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlFileReaderHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The xml file reader helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.Utils
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Xml;

    using SendingResults.Diagnosis;

    /// <summary>
    /// The xml file reader helper.
    /// </summary>
    public static class XmlFileReaderHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The read file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="XmlDocument"/>.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        public static XmlDocument ReadFile(string filename)
        {
            bool read = false;
            int countReadTimes = 0;
            while (!read)
            {
                countReadTimes++;
                try
                {
                    using (FileStream xmlFile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (xmlFile.Length > 0)
                        {
                            var byteContent = new byte[xmlFile.Length];
                            xmlFile.Read(byteContent, 0, (int)xmlFile.Length);

                            var inputStream = new MemoryStream(byteContent);
                            string msg;
                            using (StreamReader reader = new StreamReader(inputStream, new UTF8Encoding(false)))
                            {
                                msg = reader.ReadToEnd();
                            }

                            XmlDocument doc = new XmlDocument();

                            doc.LoadXml(msg);
                            return doc;
                        }
                        else
                        {
                            throw new FileNotFoundException("File of zero Length detected " + (int)xmlFile.Length);
                        }
                    }
                }
                catch (IOException ee)
                {
                    Log.Trace("IO ERROR Could not read from file " + filename, ee);
                    if (countReadTimes > 4)
                    {
                        throw;
                    }

                    Thread.Sleep(4000);
                    continue;
                }
                catch (Exception e)
                {
                    Log.Error("Could not read from file " + filename, e);
                    return null;
                }
            }

            return null;
        }

        #endregion
    }
}