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
        public TimeSpan Duration { get; private set; }
        public string Text { get; set; }

        public void SetDuration(TimeSpan timeEnd)
        {
            Duration = timeEnd - TimeStart;
        }

        public override string ToString()
        {
            StringBuilder cnt = new StringBuilder();
            cnt.AppendLine(Index.ToString());
            cnt.AppendFormat("{0} --> {1}", TimeStart.ToString(@"hh\:mm\:ss\,fff"), (TimeStart + Duration).ToString(@"hh\:mm\:ss\,fff"));
            cnt.Append(Environment.NewLine);
            cnt.AppendLine(Text);
            cnt.Append(Environment.NewLine);
            
            return cnt.ToString();
        }
    }
}
