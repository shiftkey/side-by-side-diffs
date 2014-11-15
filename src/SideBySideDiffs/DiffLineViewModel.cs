using System;
using System.Diagnostics;

namespace SideBySideDiffs
{
    public class DiffLineViewModel
    {
        public string Text { get; set; }
        public DiffContext Style { get; set; }
        public int RowNumber { get; set; }
        public string PrefixForStyle { get; set; }

        public override string ToString()
        {
            return String.Format("{0}{1}", PrefixForStyle, Text);
        }
    }
}