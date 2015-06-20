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
            string errs = null;
            var cmm = new SrtCommunicator();
            Task<string> task = null;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    task = cmm.ReadSrtAsync(arg);
                    break;
                }
            }

            Console.WriteLine("Enter time shift in format - +-mm:ss.fff:");
            string input = Console.ReadLine();

            if (task != null)
            {
                errs = task.Result;

                if (!string.IsNullOrEmpty(errs))
                {
                    Console.WriteLine("\r\nErrors : {0}\r\n", errs);
                }
            }

            TimeSpan shift;
            if (TimeSpan.TryParseExact(input, @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None, out shift)
                || TimeSpan.TryParseExact(input, @"\-mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None, out shift))
            {
                cmm.ShiftSubtitles(shift);
            }

            var file = cmm.WriteSrt();
            Console.WriteLine(file);

            Console.WriteLine("\r\nPress Enter key to continue...");
            Console.ReadLine();
        }
    }
}
