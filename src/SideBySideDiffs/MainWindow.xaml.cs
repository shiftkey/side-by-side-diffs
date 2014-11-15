﻿using System;
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

            var leftPanelContents = allLines.Where(x => x.StartsWith(" ") || x.StartsWith("- ")).ToArray();
            var rightPanelContents = allLines.Where(x => x.StartsWith(" ") || x.StartsWith("+ ")).ToArray();

            var leftPanel = leftPanelContents.Select(x => new { Item = x, Index = Array.IndexOf(leftPanelContents, x) })
                             .Select(x => CreateRowModel(x.Index, x.Item))
                             .ToList();

            var rightPanel = rightPanelContents.Select(x => new { Item = x, Index = Array.IndexOf(leftPanelContents, x) })
                         .Select(x => CreateRowModel(x.Index, x.Item))
                         .ToList();


            var leftMargin = new DiffInfoMargin { Lines = leftPanel };
            left.TextArea.LeftMargins.Add(leftMargin);

            var rightMargin = new DiffInfoMargin { Lines = rightPanel };
            right.TextArea.LeftMargins.Add(rightMargin);


            var leftText = String.Join("\r\n", leftPanelContents);
            var rightText = String.Join("\r\n", rightPanelContents);




            left.Text = leftText;
            right.Text = rightText;

            // TODO: bind this to the page
            // TODO: style this mofo
            // TODO: parse the strings into proper domain objects
            // TODO: introduce line highlighting
            // TODO: introduce highlighting specific sections
        }

        static DiffLineViewModel CreateRowModel(int index, string s)
        {
            var viewModel = new DiffLineViewModel();
            viewModel.RowNumber = index;
            viewModel.Text = s;
            viewModel.Context = s.StartsWith("+ ")
                ? DiffContext.Add
                : s.StartsWith("- ")
                    ? DiffContext.Remove
                    : DiffContext.Context;

            return viewModel;
        }
    }
}
