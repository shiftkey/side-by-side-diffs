using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace SideBySideDiffs
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // this is the format you can use to generate a "good enough" raw diff
            // $ git difftool HEAD~1 -y -x "diff --old-line-format=\"- %L\" --new-line-format=\"+ %L\" --unchanged-line-format=\"  %L\"" > output.txt

            string diffContents = "";

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var outputResourceKey = names.First(x => x.EndsWith("output.txt"));

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(outputResourceKey))
            using (var streamReader = new StreamReader(stream))
            {
                diffContents = streamReader.ReadToEnd();
            }

            diffContents = diffContents.Replace("\r\r\n", "\r\n");

            var allLines = diffContents.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var leftPanelContents = allLines.Select(StripNewValues).ToArray();
            var rightPanelContents = allLines.Select(StripOldValues).ToArray();

            var rowNumbers = Enumerable.Range(1, allLines.Length).ToArray();

            var leftPanel = leftPanelContents.Zip(rowNumbers, (s, i) => new {Item = s, Index = i})
                             .Select(x => CreateRowModel(x.Index, x.Item))
                             .ToList();

            var rightPanel = rightPanelContents.Zip(rowNumbers, (s, i) => new {Item = s, Index = i})
                         .Select(x => CreateRowModel(x.Index, x.Item))
                         .ToList();

            var leftMargin = new DiffInfoMargin { Lines = leftPanel };
            left.TextArea.LeftMargins.Add(leftMargin);
            var leftBackgroundRenderer = new DiffLineBackgroundRenderer { Lines = leftPanel };
            left.TextArea.TextView.BackgroundRenderers.Add(leftBackgroundRenderer);

            var rightMargin = new DiffInfoMargin { Lines = rightPanel };
            right.TextArea.LeftMargins.Add(rightMargin);

            var rightBackgroundRenderer = new DiffLineBackgroundRenderer { Lines = rightPanel };
            right.TextArea.TextView.BackgroundRenderers.Add(rightBackgroundRenderer);

            var leftText = String.Join("\r\n", leftPanel.Select(x => x.Text));
            var rightText = String.Join("\r\n", rightPanel.Select(x => x.Text));

            left.Text = leftText;
            right.Text = rightText;

            // TODO: bind this to the page
            // TODO: style this mofo
            // TODO: parse the strings into proper domain objects
            // TODO: introduce line highlighting
            // TODO: introduce highlighting specific sections
        }

        static string StripOldValues(string s)
        {
            if (s.StartsWith("- "))
                return "";

            return s;
        }

        static string StripNewValues(string s)
        {
            if (s.StartsWith("+ "))
                return "";

            return s;
        }

        static DiffLineViewModel CreateRowModel(int index, string s)
        {
            var viewModel = new DiffLineViewModel();
            viewModel.RowNumber = index;

            if (s.StartsWith("+ "))
            {
                viewModel.Style = DiffContext.Added;
                viewModel.PrefixForStyle = "+ ";
                viewModel.Text = s.Substring(2);
            }
            else if (s.StartsWith("- "))
            {
                viewModel.Style = DiffContext.Deleted;
                viewModel.PrefixForStyle = "- ";
                viewModel.Text = s.Substring(2);
            }
            else
            {
                viewModel.Style = DiffContext.Context;
                viewModel.PrefixForStyle = "  ";
                viewModel.Text = s.Length > 1 ? s.Substring(1) : s; // lol hax
            }

            return viewModel;
        }
    }
}
