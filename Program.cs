using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AfterDownloadFileHandler
{
    class Program
    {
        public static string[] regexesEpisode = {
                "\\.[Ss]\\d{1,2}\\.?[Ee]\\d{1,2}\\.",
                "(\\d{1,2})[xX](\\d{1,2})"
            };

        public static string[] regexesSeasons = {
                "\\.[sS]\\d{1,2}\\."
            };

        public static string regexMovieName = "(?<name>.*)(\\.[sS]\\d{1,2})";

        static void Main(string[] args)
        {
            string configLabelTriggerValue = Properties.Params.Default.LABEL_TRIGGER_VALUE;
            bool configAutoSubDownloadFlag = Properties.Params.Default.AUTO_SUBTITLE_DOWNLOAD;
            string configRemoteMoviesPath = Properties.Params.Default.REMOTE_MOVIES_PATH;
            string configRemoteSeriesPath = Properties.Params.Default.REMOTE_SERIES_PATH;
            string configRemoteUnkownPath = Properties.Params.Default.REMOTE_UNKNOWN_PATH;
            string configSkipSamples = Properties.Params.Default.SKIP_SAMPLES;
            string configSkipFileExtensions = Properties.Params.Default.SKIP_FILE_EXTENSIONS;
            string configSubExtensions = Properties.Params.Default.SUB_EXTENSIONS;
            string configAutoSubDownloaderLanguages = Properties.Params.Default.ASD_LANG;

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Console.WriteLine("##############################################################");
            Console.WriteLine("##    After Download File Handler (v" + version + ")   Made by CRK  ##");
            Console.WriteLine("##############################################################\r\n");
            if (args.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("%D - REQ: Directory where files are saved");
                Console.WriteLine("%L - REQ: Label");
                Console.WriteLine("%F - OPT: Name of downloaded file (if single file)");
                Console.WriteLine();
                Console.WriteLine("Eg.: ADFH.exe C:/downloads Movies");
                Console.WriteLine("Eg.: ADFH.exe C:/downloads TVSeries Episode3.avi");
                Console.WriteLine();
                Console.WriteLine("uTorrent configuration example: ");
                Console.WriteLine("Preferences -> Advance -> Run program :");
                Console.WriteLine("[...] when a torrent finishes:");
                Console.WriteLine("path\\2\\AfterDownloadFileHandler.exe \"%D\" \"%L\" \"%F\"");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Configure also AfterDownloadFileHandler.exe.config");
                Console.ResetColor();
            }

            if(args.Length >= 1)
            {
                string directory = args[0];
                if (!Directory.Exists(directory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("%D - Download directory does not exist!");
                    Console.ResetColor();
                    Console.ReadKey();
                    return;
                }

                string label = args[1];
                if (label.Contains(Properties.Params.Default.LABEL_TRIGGER_VALUE))
                {
                    string singleFilenameDownload = null;
                    if (args.Length == 3 && args[2] != String.Empty)
                    {
                        singleFilenameDownload = args[2];
                    }

                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    //Console.WriteLine("DEBUG : Dir path " + directory);
                    //Console.WriteLine("DEBUG : Dir name" + dirInfo.Name);
                    //Console.WriteLine("DEBUG : Label " + label);


                    //IF DIRECTORY COPY - CASE 1-4
                    if (singleFilenameDownload == null)
                    {
                        //// CASES ORDER IS IMPORTANT ////
                        //CASE 1 - DIR OF TV SERIES Season X, Episode Y - very rare - usually episodes are not in separate folders.
                        if (isNameTVSeriesSeasonEpisode(dirInfo.Name))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("CASE 1 : DIR TVSeries S E - CHECK IF FOLDERS MATCH");
                            Console.ResetColor();

                            //Create or use existing TVSeries Folder.
                            DirectoryInfo tvSeriesDir = new DirectoryInfo(configRemoteSeriesPath + "\\" + getTVSeriesName(dirInfo.Name));
                            if (!tvSeriesDir.Exists)
                            {
                                Directory.CreateDirectory(tvSeriesDir.FullName);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Creating destination directory : " + tvSeriesDir.FullName);
                                Console.ResetColor();
                            }

                            //Create or use existing TVSeries Season Folder.
                            DirectoryInfo tvSeriesSeasonDir = new DirectoryInfo(tvSeriesDir.FullName + "\\" + getTVSeriesSeasonName(dirInfo.Name));
                            if (!tvSeriesSeasonDir.Exists)
                            {
                                Directory.CreateDirectory(tvSeriesSeasonDir.FullName);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Creating destination directory : " + tvSeriesSeasonDir.FullName);
                                Console.ResetColor();
                            }

                            //prepare skippable content in lists
                            List<String> skippableExtensions = configSkipFileExtensions.Split(',').ToList();
                            List<String> skippableSamples = configSkipSamples.Split(',').ToList();

                            //Copy the content of the episode folder in the right season folder (without separated folder)
                            foreach (FileInfo fileInfo in dirInfo.GetFiles())
                            {
                                //skip files that set in skipFileExtensions parameter
                                if (skippableExtensions.Contains(fileInfo.Extension))
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("SKIP FILE : " + fileInfo.Name);
                                    Console.ResetColor();
                                    continue;
                                }

                                //skip sample files
                                bool isSample = false;
                                foreach (String sample in skippableSamples)
                                {
                                    if (fileInfo.Name.Contains(sample))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("SKIP FILE : " + fileInfo.Name);
                                        Console.ResetColor();
                                        isSample = true;
                                        break;
                                    }
                                }
                                if (isSample) continue;

                                //check for existing files - avoid overwrites
                                if(File.Exists(tvSeriesSeasonDir.FullName + "\\" + fileInfo.Name))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("ERROR : File " + fileInfo.Name + " already exists. Avoiding overwrites. EXIT!");
                                    Console.ResetColor();
                                    continue;
                                }

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("COPYING : " + fileInfo.Name + " (please wait)");
                                Console.ResetColor();
                                using (var progress = new ProgressBar())
                                {
                                    fileInfo.CopyTo(tvSeriesSeasonDir.FullName + "\\" + fileInfo.Name, false);
                                    progress.Report((double)100 / 100);
                                }
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Done.");
                                Console.ResetColor();

                                //Call ASD to download subs.
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Call ASD...");
                                Console.ResetColor();
                                var output = AutoSubtitleDownloader.ASD.Start(new string[] { tvSeriesSeasonDir.FullName + '/', configAutoSubDownloaderLanguages, "", "", "/s" });
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine(output);
                                Console.ResetColor();
                            }
                        }
                        
                        //// CASES ORDER IS IMPORTANT ////
                        //CASE 2 - DIR OF TV SERIES Season X - very common
                        else if (isNameTVSeriesSeason(dirInfo.Name))
                        {
                            //TODO : check for existing Series folder
                            //TODO : copy the content of the season folder in the right Series folder

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("CASE 2 : DIR TVSeries S");
                            Console.ResetColor();

                            //Create or use existing TVSeries Folder.
                            DirectoryInfo tvSeriesDir = new DirectoryInfo(configRemoteSeriesPath + "\\" + getTVSeriesName(dirInfo.Name));
                            if (!tvSeriesDir.Exists)
                            {
                                Directory.CreateDirectory(tvSeriesDir.FullName);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Creating destination directory : " + tvSeriesDir.FullName);
                                Console.ResetColor();
                            }

                            //Try to copy new TV Series Season into the TV Series folder
                            if (DirectoryCopy.Copy(dirInfo.FullName, tvSeriesDir.FullName, true, configSkipSamples, configSkipFileExtensions))
                            {
                                //Check for subtitles.
                                string destDirName = tvSeriesDir.FullName + '/' + dirInfo.Name + '/';
                                if (!hasExistingFolderSubs(destDirName, configSubExtensions))
                                {
                                    //Call ASD to download subs if not present.
                                    var output = AutoSubtitleDownloader.ASD.Start(new string[] { destDirName, configAutoSubDownloaderLanguages, "", "", "/s" });
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine(output);
                                    Console.ResetColor();
                                }
                            }
                        }
                        
                        //// CASES ORDER IS IMPORTANT ////
                        //CASE 3 - DIR of MOVIE movie.1080p.xvid
                        //CASE 4 - DIR OF ENTIRE TV SERIES - very rare
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("CASE 3/4 : DIR Movie OR Entire TV Series which will not copy to correct location - MANUALLY MOVE TO CORRECT FOLDER");
                            Console.ResetColor();

                            //Copy Folder
                            if (DirectoryCopy.Copy(dirInfo.FullName, configRemoteMoviesPath, true, configSkipSamples, configSkipFileExtensions))
                            {
                                //TODO : Check for subtitles.
                                string destDirName = configRemoteMoviesPath.TrimEnd(new char[] { '/', '\\' }) + '/' + dirInfo.Name + '/';
                                if (!hasExistingFolderSubs(destDirName, configSubExtensions))
                                {
                                    //TODO : Call ASD to download subs if not present.
                                    var output = AutoSubtitleDownloader.ASD.Start(new string[] { destDirName, configAutoSubDownloaderLanguages, "", "", "/s" });
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine(output);
                                    Console.ResetColor();
                                }
                            }
                        }

                    }
                    else //IF SINGLE FILE COPY - CASE 5-6
                    {
                        FileInfo fileInfo = new FileInfo(directory + "\\" + singleFilenameDownload);

                        //Decide file type (file can be movie, or TV Series episode (not season only)
                        
                        //CASE 5 - FILE IS TV SERIES EPISODE
                        if (isNameTVSeriesSeasonEpisode(fileInfo.Name))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("CASE 5 : SINGLE FILE TVSeries S or S E");
                            Console.ResetColor();
                            //Console.WriteLine(getTVSeriesName(fileInfo.Name));

                            //Create or use existing TVSeries Folder.
                            DirectoryInfo tvSeriesDir = new DirectoryInfo(configRemoteSeriesPath + "\\" + getTVSeriesName(fileInfo.Name));
                            if (!tvSeriesDir.Exists)
                            {
                                Directory.CreateDirectory(tvSeriesDir.FullName);
                            }

                            //Create or use existing TVSeries Season Folder.
                            DirectoryInfo tvSeriesSeasonDir = new DirectoryInfo(tvSeriesDir.FullName + "\\" + getTVSeriesSeasonName(fileInfo.Name));
                            if (!tvSeriesSeasonDir.Exists)
                            {
                                Directory.CreateDirectory(tvSeriesSeasonDir.FullName);
                            }

                            if (!File.Exists(tvSeriesSeasonDir.FullName + "\\" + fileInfo.Name))
                            {
                                //Copy File
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("COPYING : " + fileInfo.Name + " (please wait)");
                                Console.ResetColor();
                                using (var progress = new ProgressBar())
                                {
                                    fileInfo.CopyTo(tvSeriesSeasonDir.FullName + "\\" + fileInfo.Name, false);
                                    progress.Report((double)100 / 100);
                                }
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Done.");
                                Console.ResetColor();

                                //Call ASD to download subs if not present.
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Call ASD...");
                                Console.ResetColor();
                                var output = AutoSubtitleDownloader.ASD.Start(new string[] { tvSeriesSeasonDir.FullName + '/', configAutoSubDownloaderLanguages, "", "", "/s" });
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine(output);
                                Console.ResetColor();
                            } else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERROR : File " + fileInfo.Name + " already exists. Avoiding overwrites. EXIT!");
                                Console.ResetColor();
                            }
                        }
                        
                        //CASE 6 - FILE IS MOVIE
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("CASE 6 : SINGLE FILE Movie");
                            Console.ResetColor();
                            string destDirName = configRemoteMoviesPath + Path.GetFileNameWithoutExtension(fileInfo.FullName);

                            //Create Destination folder
                            if (!Directory.Exists(destDirName))
                            {
                                Directory.CreateDirectory(destDirName);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Creating destination directory : " + destDirName);
                                Console.ResetColor();

                                //Copy File
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("COPYING : " + fileInfo.Name + " (please wait)");
                                Console.ResetColor();
                                using (var progress = new ProgressBar())
                                {
                                    fileInfo.CopyTo(destDirName + "\\" + fileInfo.Name, false);
                                    progress.Report((double)100 / 100);
                                }
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Done.");
                                Console.ResetColor();

                                //Call ASD to download subs if not present.
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("INFO : Call ASD...");
                                Console.ResetColor();
                                var output = AutoSubtitleDownloader.ASD.Start(new string[] { destDirName + '/', configAutoSubDownloaderLanguages, "", "", "/s" });
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine(output);
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERROR : Destination directory " + destDirName + " already exists! Avoid overwriting - EXIT!");
                                Console.ResetColor();
                            }
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR : Label " + label + " does not match target label. EXIT!");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static bool isNameTVSeriesSeasonEpisode (string name)
        {
            foreach (string regex in regexesEpisode) {
                return System.Text.RegularExpressions.Regex.Match(name, regex).Success;
            }
            return false;
        }

        public static bool isNameTVSeriesSeason(string name)
        {
            foreach (string regex in regexesSeasons)
            {
                return System.Text.RegularExpressions.Regex.Match(name, regex).Success;
            }
            return false;
        }

        public static string getTVSeriesName(string name)
        {
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(name, regexMovieName);
            if (match.Groups.Count > 0)
            {
                return match.Groups["name"].Value.ToString();
            }
            return "";
        }

        public static string getTVSeriesSeasonName(string name)
        {
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(name, regexMovieName);
            if (match.Groups.Count > 0)
            {
                return match.Groups[0].Value.ToString();
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFolder"></param>
        /// <param name="subExtensions"></param>
        /// <returns></returns>
        public static bool hasExistingFolderSubs(string targetFolder, String subExtensions)
        {
            //prepare sub extensions in list
            List<String> subExtensionList = subExtensions.Split(',').ToList();

            if (Directory.Exists(targetFolder))
            {
                DirectoryInfo dir = new DirectoryInfo(targetFolder);

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    //skip files that set in skipFileExtensions parameter
                    if (subExtensionList.Contains(file.Extension))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("INFO : Sub found : " + file.Name + ". No need to ASD!");
                        Console.ResetColor();
                        return true;
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("INFO : Sub not found! Call ASD...");
                Console.ResetColor();
                return false;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR : Target folder " + targetFolder + " does not exist.");
            Console.ResetColor();
            return false;
        }
    }
}
