using System;
using System.IO;
using System.Reflection;

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

        static void Main(string[] args)
        {

            

            //foreach(string s in args)
            //{
            //    Console.WriteLine(s);
            //}

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Console.WriteLine("==================================================");
            Console.WriteLine("==  ADFH v" + version + "                   made by CRK ==");
            Console.WriteLine("==================================================");
            if (args.Length < 1)
            {
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
                Console.WriteLine("Configure also AfterDownloadFileHandler.exe.config");
            }

            if(args.Length >= 1)
            {
                string directory = args[0];
                if (!Directory.Exists(directory))
                {
                    Console.WriteLine("%D - Download directory does not exist!");
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
                    Console.WriteLine("Dir path " + directory);
                    Console.WriteLine("Dir name" + dirInfo.Name);
                    Console.WriteLine("Label " + label);


                    //IF DIRECTORY COPY
                    if (singleFilenameDownload == null)
                    {
                        //TODO Avoid sample and .DAT & .nfo

                        //CASE 1 - DIR OF TV SERIES Season X, Episode Y
                        if (isNameTVSeriesSeasonEpisode(dirInfo.Name))
                        {
                            Console.WriteLine("DIR TVSeries S E");
                        }
                        //CASE 2 - DIR OF TV SERIES Season X
                        else if (isNameTVSeriesSeason(dirInfo.Name))
                        {
                            Console.WriteLine("DIR TVSeries S");
                        }
                        else
                        {
                            //CASE 3 - DIR of MOVIE movie.1080p.xvid
                            //CASE 4 - DIR OF ENTIRE TV SERIES - very rare
                            Console.WriteLine("DIR Movie");
                        }

                    }
                    else //IF single file COPY
                    {
                        FileInfo fileInfo = new FileInfo(directory + "\\" + singleFilenameDownload);
                        Console.WriteLine("File name " + fileInfo.Name);
                        Console.WriteLine("File Dir name " + fileInfo.DirectoryName);
                        Console.WriteLine("File ext " + fileInfo.Extension);
                        Console.WriteLine("File fullName " + fileInfo.FullName);
                    }

                }
                else
                {
                    Console.WriteLine("Label does not match target label. Exit!");
                }
            }

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
    }
}
