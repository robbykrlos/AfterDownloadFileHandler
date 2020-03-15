using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace AfterDownloadFileHandler
{
    class DirectoryCopy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        /// <param name="skipSamples"></param>
        /// <param name="skipFileExtensions">coma separated values e.g.: dat,nfo</param>
        public static bool Copy(string sourceDirName, string destDirName, bool copySubDirs, String skipSamples, String skipFileExtensions)
        {
            //prepare skippable content in lists
            List<String> skippableExtensions = skipFileExtensions.Split(',').ToList();
            List<String> skippableSamples = skipSamples.Split(',').ToList();

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR : Source directory does not exist or could not be found: " + sourceDirName);
                Console.ResetColor();
                return false;
            }

            //Console.WriteLine(destDirName);

            if (!Directory.Exists(destDirName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR : Destination directory does not exist or could not be found: " + destDirName);
                Console.ResetColor();
                return false;
            }

            //Append movie folder after base destination folder.
            destDirName += "\\" + dir.Name;

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("INFO : Creating destination directory : " + destDirName);
                Console.ResetColor();
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR : Destination directory " + destDirName + " already exists! Avoid overwriting - EXIT!");
                Console.ResetColor();
                return false;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                //Console.WriteLine(file.Name);

                //skip files that set in skipFileExtensions parameter
                if (skippableExtensions.Contains(file.Extension))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("SKIP FILE : " + file.Name);
                    Console.ResetColor();
                    continue;
                }

                bool isSample = false;
                foreach (String sample in skippableSamples)
                {
                    if (file.Name.Contains(sample))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("SKIP FILE : " + file.Name);
                        Console.ResetColor();
                        isSample = true;
                        break;
                    }
                }
                if (isSample) continue;

                string temppath = Path.Combine(destDirName, file.Name);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("COPYING : " + file.Name + " (please wait)");
                Console.ResetColor();

                using (var progress = new ProgressBar())
                {
                    file.CopyTo(temppath, false);
                    progress.Report((double)100 / 100);
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Done.");
                Console.ResetColor();


            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    //skip files that set in skipFileExtensions parameter
                    //Console.WriteLine("DEBUG : " + subdir.Name + skippableSamples[0]);

                    if (skippableSamples.Contains(subdir.Name))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("SKIP FOLDER : " + subdir.Name);
                        Console.ResetColor();
                        continue;
                    }

                    string temppath = Path.Combine(destDirName, subdir.Name);
                    return Copy(subdir.FullName, temppath, copySubDirs, skipSamples, skipFileExtensions);
                }
            }

            return true;
        }
    }
}
