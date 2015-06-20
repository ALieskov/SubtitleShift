using System;
using System.Collections.Generic;

namespace SrtShiftApp.Model
{
    public class SrtEntity
    {
        public IList<SrtEntry> Subtitles { get; set; }

        public IList<SrtError> Validate()
        {
            throw new NotImplementedException();
            //var erros = new List<SrtError>();
            //return erros;
        }

    }
}
