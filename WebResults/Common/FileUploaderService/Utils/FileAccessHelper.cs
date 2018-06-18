using FileUploaderService.Diagnosis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Utils
{
    public static class FileAccessHelper
    {
        public static string ReadText(string inputFileName)
        {
            string outText = File.ReadAllText(inputFileName);
             return outText;
        }

        private static FileInfo MoveBitmapFile(FileInfo rawBitmapFile, string destDir)
        {
           

            return null;
        }

        public static void MoveFiles(List<FileInfo> fileList, string toDir, bool backupRemote=false)
        {
            foreach (var bitmapFile in fileList)
            {
                try
                {
                    if (File.Exists(bitmapFile.FullName))
                    {
                        var destFileName = Path.Combine(toDir, bitmapFile.Name);
                            FileInfo inf = new FileInfo(destFileName);

                            if (inf.Exists)
                            {
                                var filenameonly = Path.GetFileNameWithoutExtension(inf.Name);
                                var path = Path.GetDirectoryName(inf.FullName);
                                var destDirDublettDir = Path.Combine(path, "Copies");
                                if (!Directory.Exists(destDirDublettDir))
                                {
                                    Directory.CreateDirectory(destDirDublettDir);
                                }

                                Log.Warning("Bitmap destination file already exstst {0}", inf.FullName);
                                DateTime time = DateTime.Now;

                                string backup = time.ToString("yyyyMMddHHmmss");
                                string oldFileName = string.Format("{0}old{1}.PNG", filenameonly, backup);
                                string totfileName = Path.Combine(destDirDublettDir, oldFileName);
                                inf.MoveTo(totfileName);
                            }

                            Log.Info("Backing up Raw bitmaps to stevner From={0} To={1}", bitmapFile.FullName, destFileName);
                            File.Copy(bitmapFile.FullName, destFileName);
                            if(backupRemote)
                            { 
                            var sourceFileName = Path.GetFileName(bitmapFile.FullName);
                            var sourceDirName = Path.GetDirectoryName(bitmapFile.FullName);
                            var newSourceFileName = "MOV" + sourceFileName;
                            var newSourceFileNameWithPath = Path.Combine(sourceDirName, newSourceFileName);
                            if (File.Exists(newSourceFileNameWithPath))
                            {
                                inf = new FileInfo(newSourceFileNameWithPath);
                                var filenameonly = Path.GetFileNameWithoutExtension(newSourceFileNameWithPath);
                                var path = Path.GetDirectoryName(newSourceFileNameWithPath);
                                var destDirDublettDir = Path.Combine(path, "Copies");
                                if (!Directory.Exists(destDirDublettDir))
                                {
                                    Directory.CreateDirectory(destDirDublettDir);
                                }

                                Log.Warning("Bitmap Backup destination file already exstst {0}", newSourceFileNameWithPath);
                                DateTime time = DateTime.Now;

                                string backup = time.ToString("yyyyMMddHHmmss");
                                string oldFileName = string.Format("{0}old{1}.PNG", filenameonly, backup);
                                string totfileName = Path.Combine(destDirDublettDir, oldFileName);

                                inf.MoveTo(totfileName);
                            }

                            File.Copy(bitmapFile.FullName, newSourceFileNameWithPath);
                        }

                        File.Delete(bitmapFile.FullName);
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e, "Error");
                }
            }
        }
    }
}
