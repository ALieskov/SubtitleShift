using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtShiftApp.Model
{
    public class SrtCommunicator
    {
        private string _filePath;
        private SrtEntity _srtSet = new SrtEntity();

        public async Task<string> ReadSrtAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", @"File name can't be empty or null.");
            }

            if (File.Exists(fileName))
            {
                _filePath = fileName;
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
                    if(entry != null)
                        srts.Add(entry);
                    entry = new SrtEntry();
                    entry.Index = blockCounter;
                    blockCounter++;
                }
                else if (line.Contains("-->"))
                {
                    var parts = line.Replace("-->", ">").Replace(",", ".").Split('>');
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
                        entry.CalculateShift(time);
                    }
                    else
                    {
                        errs.AppendFormat("[{0}] Wrong TimeEnd", blockCounter);
                    }
                }
                else if (!line.Equals(Environment.NewLine))
                {
                    entry.Text = string.Format("{0}{1}{2}"
                                                        , entry.Text
                                                        , string.IsNullOrEmpty(entry.Text)? "" : Environment.NewLine
                                                        , line);
                }
            }
            errors = errs.ToString();
            return srts;
        }

        public string WriteSrt()
        {
            StringBuilder content = new StringBuilder();
            foreach (var srtEntry in _srtSet.Subtitles)
            {
                content.Append(srtEntry);
            }
            var path = Path.GetDirectoryName(_filePath);
            var file = Path.GetFileName(_filePath);
            var newFileName = string.Format("{0}_{1}", Path.GetRandomFileName(), file);
            var newFilePath = Path.Combine(path, newFileName);
            File.WriteAllText(newFilePath, content.ToString());

            return newFilePath;
        }

        public void ShiftSubtitles(TimeSpan shift)
        {
            _srtSet.DoShift(shift);
        }
    }
}
