using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcatVids
{
    class Program
    {
        public static string SourceVideoPath = "";
        public static List<string> VidsList = new List<string>();
        public static string ffmpegInputFile = "";
        public static string ffmpegInputFileName = "ffmpegFileList.txt";
        public static string CommandText = "";
        public static string OutputFileName = "";

        static void Main(string[] args)
        {
            string usageString = GetUsageString();

            if (args.Length > 0)
            {
                if (args[0] == "-help" || args[0] == "-h")
                {
                    Console.WriteLine(usageString);
                    return;
                }
            }

            if (args.Length > 1)
                ProcessOptionalArgs(args[0], args[1]);

            if (args.Length == 4)
                ProcessOptionalArgs(args[1], args[2]);

            if (args.Length > 4)
                Console.WriteLine("Only the first 4 args were parsed, all else was ignored.");

            if (!SetUpVidsList())
            {
                Console.WriteLine("");
                Console.WriteLine("Error getting list from file system directory listing: ");
                Console.WriteLine(SourceVideoPath);
                Console.WriteLine("");
                return;
            }

            // ffmpeg -y -f concat -safe 0 -i dir.txt -c copy 2018-04-28-Team1VsTeam2.mp4
            CommandText += " -y";
            CommandText += " -f concat -safe 0";
            CommandText += " -i " + ffmpegInputFileName;
            CommandText += " -c copy " + OutputFileName;

            Console.WriteLine(" # Files: " + VidsList.Count.ToString());
            Console.WriteLine(" Do you want to generate a concatenation of " + VidsList.Count.ToString() + " video clips as " + OutputFileName + "?");
            if (File.Exists(OutputFileName))
                Console.WriteLine(" Note that " + OutputFileName + " already exists and will be overwritten if you proceed.");

            string KeyPress = Console.ReadLine();

            if (KeyPress != "Y" && KeyPress != "y")
            {
                Console.WriteLine();
                Console.WriteLine("ok, nothing generated. Command would be: ");
                Console.WriteLine();
                Console.WriteLine("ffmpeg.exe" + CommandText + Environment.NewLine);
                Console.WriteLine("Contents of ffmpeg Input File:" + Environment.NewLine);
                Console.WriteLine(ffmpegInputFile);

                return;
            }

            Console.WriteLine(" ffmpeg.exe" + CommandText);
            Console.WriteLine();

            if (File.Exists(ffmpegInputFileName))
            {
                try
                {
                    File.Delete(ffmpegInputFileName);
                }
                catch
                {
                    Console.WriteLine(" Unable to delete previous file list: " + ffmpegInputFileName);
                    return;
                }
            }

            File.WriteAllText(ffmpegInputFileName, ffmpegInputFile);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "ffmpeg.exe";
                process.StartInfo.Arguments = CommandText;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }


        }

        static bool SetUpVidsList()
        {
            // If not specified on commandline, set default value
            if (SourceVideoPath == "")
                SourceVideoPath = Directory.GetCurrentDirectory();

            // If not specified on commandline, set default value
            if (OutputFileName == "")
                OutputFileName = Path.GetFileName(Environment.CurrentDirectory) + ".mp4";

            DirectoryInfo d = new DirectoryInfo(SourceVideoPath);
            FileInfo[] Files = d.GetFiles("*.mp4");

            if (Files.Count() < 2)
            {
                Console.WriteLine("More than 1 file required, only found: " + Files.Count().ToString() + " files.");
                return false;
            }

            StringBuilder sb = new StringBuilder();

            foreach (FileInfo file in Files)
            {
                // ensure that a prevuisly generated concatenated video is not included in the list
                if (file.Name.ToLower() != OutputFileName.ToLower())  
                {
                    VidsList.Add(file.Name);
                    sb.Append("file '" + file.Name + "'" + Environment.NewLine);
                }
            }

            VidsList.Sort();
            ffmpegInputFile = sb.ToString();
            sb.Clear();

            return true;
        }



        static void ProcessOptionalArgs(string argName, string argValue)
        {
            if (argName == "-vpath") // specifies source video path
            {
                if (Directory.Exists(argValue))
                    SourceVideoPath = argValue;
                else
                    Console.WriteLine("Source video path not found: " + argValue);
            }

            else if (argName == "-ofn") // specifies clip destination path
            {
                // by default, the output file name is the name of the current directory, 
                // however this can be overridden using the -ofn argument

                string temp = argValue.Substring(Math.Max(0, argValue.Length - 4));
                if (temp.ToLower() != ".mp4")
                    OutputFileName = argValue + ".mp4";
                else
                    OutputFileName = argValue;

            }

            else
            {
                Console.WriteLine("Unrecognized parameter was ignored: " + argName + " " + argValue);
            }

        }

        static string GetUsageString()
        {
            StringBuilder sbUsage = new StringBuilder();
            sbUsage.Append(Environment.NewLine);
            sbUsage.Append(" ConcatVids.exe " + Environment.NewLine);
            sbUsage.Append(Environment.NewLine);
            sbUsage.Append(" Optional Parameters:" + Environment.NewLine);
            sbUsage.Append("   -vpath PATH    set path to the source video files; will be current directory when not specified" + Environment.NewLine);
            sbUsage.Append("   -ofn STRING    set name for the output file. If not specified, it will take the name of the current directory, e.g.: dirName.mp4" + Environment.NewLine);
            sbUsage.Append(Environment.NewLine);
            sbUsage.Append(" NOTE: all of the source videos must be of the same type: resolution, frame rate, time rate, etc." + Environment.NewLine);
            sbUsage.Append(" This program uses the ffmpeg concat feature and copies existing video/audio codecs, requiring this similarity between all clips." + Environment.NewLine);
            sbUsage.Append(" Video is Not re-encoded." + Environment.NewLine);
            sbUsage.Append(Environment.NewLine);

            return sbUsage.ToString();
        }


    }
}
