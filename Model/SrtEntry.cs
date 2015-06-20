using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtShiftApp
{
    public class SrtEntry
    {
        public int Index { get; set; }
        public TimeSpan TimeStart { get; set; }
        private TimeSpan TimeShift { get; set; }
        public string Text { get; set; }

        public void CalculateShift(TimeSpan timeEnd)
        {
            TimeShift = timeEnd - TimeStart;
        }

        public override string ToString()
        {
            StringBuilder cnt = new StringBuilder();
            cnt.AppendLine(Index.ToString());
            cnt.AppendFormat("{0} --> {1}", TimeStart.ToString(@"hh\:mm\:ss\,fff"), (TimeStart + TimeShift).ToString(@"hh\:mm\:ss\,fff"));
            cnt.Append(Environment.NewLine);
            cnt.AppendLine(Text);
            //cnt.Append(Environment.NewLine);

            return cnt.ToString();
        }
    }
}
