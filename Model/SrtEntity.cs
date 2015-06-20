using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace SrtShiftApp.Model
{
    public class SrtEntity
    {
        public IList<SrtEntry> Subtitles { get; set; }

        public IList<SrtError> Validate()
        {
            var erros = new List<SrtError>();

            return erros;
        }

        public void DoShift(TimeSpan shift)
        {
            foreach (var subtitle in Subtitles)
            {
                subtitle.TimeStart = subtitle.TimeStart + shift;
            }
        }
    }
}
