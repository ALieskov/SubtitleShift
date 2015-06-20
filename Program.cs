using System;
using System.Globalization;
using System.Threading.Tasks;
using SrtShiftApp.Model;

namespace SrtShiftApp
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args == null || args.Length == 0)
            {
                ShowCommandLineInfo();
                Console.ReadLine();
                return;
            }

            var cmm = new SrtProcessor();
            Task<string> task = null;

            TimeSpan shift = TimeSpan.Zero;
            string fileName = null;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.Equals("/h"))
                    {
                        ShowCommandLineInfo();
                        Console.ReadLine();
                        return;
                    }

                    TimeSpan tryShift;
                    if (TimeSpan.TryParseExact(arg, @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None, out tryShift)
                        || TimeSpan.TryParseExact(arg, @"\-mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None, out tryShift))
                    {
                        shift = tryShift;
                        continue;
                    }
                    else
                    {
                        fileName = arg;
                        break;
                    }
                }
            }


            if (shift == TimeSpan.Zero)
            {
                Console.WriteLine("Wrong or empty 'Time shift' option value.");
                Console.ReadLine();
                return;
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Wrong or empty '\"<full_srt_file_name>\"' value.");
                Console.ReadLine();
                return;
            }

            task = cmm.ReadSrtAsync(fileName);

            var errs = task.Result;

            if (!string.IsNullOrEmpty(errs))
            {
                Console.WriteLine("\r\nErrors : {0}\r\n", errs);
            }

            cmm.ShiftSubtitles(shift);

            var file = cmm.WriteSrt();
            Console.WriteLine(file);

            Console.WriteLine("\r\nPress Enter key to continue...");
            Console.ReadLine();
        }

        private static void ShowCommandLineInfo()
        {
            var cmdCommonOutput =
                "\r\nUsage: SrtShiftApp.exe option \"<full_srt_file_name>\"" +
                "\r\n     options:" +
                "\r\n\t/h               Shows help" +
                "\r\n\t+-mm:ss.fff      Time shift";
            Console.WriteLine(cmdCommonOutput);
        }
    }
}
