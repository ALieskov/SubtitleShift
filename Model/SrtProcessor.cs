using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtShiftApp.Model
{
    public class SrtProcessor
    {
        private string _filePath;
        private SrtEntity _srtSet = new SrtEntity();
        private const string TimeSeparator = "-->";
        private const string TimeSeparatorShort = ">";

        public async Task<string> ReadSrtAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (File.Exists(fileName))
            {
                _filePath = fileName;
            }
            else
            {
                throw new Exception("File does not exist.");
            }

            string[] lines = await Task.Run(() => File.ReadAllLines(_filePath));
            string errors = string.Empty;

            _srtSet.Subtitles = ParseSrtContent(lines, out errors);
            return errors;
        }

        private IList<SrtEntry> ParseSrtContent(string[] lines, out string errors)
        {
            int tmpInt = 0;
            int blockCounter = 0;
            var srts = new List<SrtEntry>();
            SrtEntry entry = null;
            StringBuilder errs = new StringBuilder();
            foreach (var line in lines)
            {
                if (Int32.TryParse(line, out tmpInt))
                {
                    entry = new SrtEntry {Index = blockCounter};
                    srts.Add(entry);
                    blockCounter++;
                    continue;
                }

                if (line.Contains(TimeSeparator))
                {
                    if (entry == null)
                        continue;

                    var parts =
                        line.Replace(TimeSeparator, TimeSeparatorShort).Replace(",", ".").Split(TimeSeparatorShort[0]);
                    
                    TimeSpan time;
                    if (TimeSpan.TryParse(parts[0], out time))
                    {
                        entry.TimeStart = time;
                    }
                    else
                    {
                        errs.AppendFormat("[{0}] Wrong TimeStart", blockCounter);
                    }

                    if (TimeSpan.TryParse(parts[1], out time))
                    {
                        entry.SetDuration(time);
                    }
                    else
                    {
                        errs.AppendFormat("[{0}] Wrong TimeEnd", blockCounter);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line) && !line.Equals(Environment.NewLine))
                {
                    entry.Text = string.Format("{0}{1}{2}"
                        , entry.Text
                        , string.IsNullOrWhiteSpace(entry.Text) ? "" : Environment.NewLine
                        , line);
                }

            }
            errors = errs.ToString();
            return srts;
        }

        /// <summary>
        /// Writes corrected subtitles to file
        /// </summary>
        /// <param name="newFileNamePath">Optional</param>
        /// <returns>New file with path</returns>
        public string WriteSrt(string newFileNamePath = null)
        {
            StringBuilder content = new StringBuilder();
            foreach (var srtEntry in _srtSet.Subtitles)
            {
                content.Append(srtEntry);
            }
            var path = Path.GetDirectoryName(_filePath);
            var file = Path.GetFileName(_filePath);
            //var newFileName = string.Format("{0}_{1}", file, Path.GetRandomFileName());
            var newFileName = string.Format("{0}_{1:yyyy_MM_dd_HH_mm_ss}{2}",
                                                Path.GetFileNameWithoutExtension(file),
                                                DateTime.Now,
                                                Path.GetExtension(file));
            if (string.IsNullOrWhiteSpace(newFileNamePath))
            {
                newFileNamePath = Path.Combine(path, newFileName);
            }
            File.WriteAllText(newFileNamePath, content.ToString());

            return newFileNamePath;
        }

        /// <summary>
        /// Shifts subtitles
        /// </summary>
        /// <param name="shift"></param>
        public void ShiftSubtitles(TimeSpan shift)
        {
            if (shift == TimeSpan.Zero)
            {
                throw new ArgumentNullException("shift");
            }

            Parallel.ForEach(_srtSet.Subtitles, srt => srt.TimeStart = srt.TimeStart + shift);

            //foreach (var subtitle in _srtSet.Subtitles)
            //{
            //    subtitle.TimeStart = subtitle.TimeStart + shift;
            //}
        }
    }
}
