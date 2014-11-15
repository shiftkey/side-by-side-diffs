using System;

namespace SideBySideDiffs
{
    public class DiffLineViewModel
    {
        public string Text { get; set; }
        public DiffContext Style { get; set; }
        public int LineNumber { get; set; }
        public string PrefixForStyle { get; set; }

        public static DiffLineViewModel Create(int lineNumber, string s)
        {
            var viewModel = new DiffLineViewModel();
            viewModel.LineNumber = lineNumber;

            if (s.StartsWith("+"))
            {
                viewModel.Style = DiffContext.Added;
                viewModel.PrefixForStyle = "+";
                viewModel.Text = s.Substring(1);
            }
            else if (s.StartsWith("-"))
            {
                viewModel.Style = DiffContext.Deleted;
                viewModel.PrefixForStyle = "-";
                viewModel.Text = s.Substring(1);
            }
            else
            {
                viewModel.Style = DiffContext.Context;
                viewModel.PrefixForStyle = "";
                viewModel.Text = s.Length > 1 ? s.Substring(1) : s; // lol hax
            }

            return viewModel;
        }


        public override string ToString()
        {
            return String.Format("{0}{1}", PrefixForStyle, Text);
        }
    }
}