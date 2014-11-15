using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
            // $ git show HEAD --format=%b | less > output.txt

            string diffContents;
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var outputResourceKey = names.First(x => x.EndsWith("output.txt"));

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(outputResourceKey))
            using (var streamReader = new StreamReader(stream))
            {
                diffContents = streamReader.ReadToEnd();
            }

            var allLines = diffContents.Split(new[] { "\n" }, StringSplitOptions.None).ToList();

            var arrayOfIndexes = Enumerable.Range(0, allLines.Count);

            var diffSectionHeaders = allLines.Zip(arrayOfIndexes,
                    (x, index) => new { Item = x, Index = index })
                .Where(x => x.Item.StartsWith("diff --git a"))
                .ToList();

            foreach (var header in diffSectionHeaders)
            {
                var hunkElements = allLines
                    .Skip(header.Index + 1)
                    .TakeWhile(x => !x.StartsWith("diff --git a"))
                    .ToList();

                var chunks = ResolveDiffSections(hunkElements);

                foreach (var chunk in chunks)
                {
                    var row = chunks.IndexOf(chunk);

                    rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    // draw header
                    var textBlock = new TextBlock
                    {
                        Text = chunk.DiffSectionHeader,
                        Style = (Style)FindResource("ContextHeaderStyle")
                    };
                    Grid.SetRow(textBlock, 2 * row);
                    Grid.SetColumnSpan(textBlock, 2);
                    rootGrid.Children.Add(textBlock);

                    // draw left diff
                    var leftMargin = new DiffInfoMargin { Lines = chunk.LeftDiff };
                    var left = new TextEditor();
                    left.TextArea.LeftMargins.Add(leftMargin);
                    var leftBackgroundRenderer = new DiffLineBackgroundRenderer { Lines = chunk.LeftDiff };
                    left.TextArea.TextView.BackgroundRenderers.Add(leftBackgroundRenderer);
                    left.Text = String.Join("\r\n", chunk.LeftDiff.Select(x => x.Text));

                    Grid.SetRow(left, 2 * row + 1);
                    rootGrid.Children.Add(left);

                    // draw right diff
                    var rightMargin = new DiffInfoMargin { Lines = chunk.RightDiff };
                    var right = new TextEditor();
                    right.TextArea.LeftMargins.Add(rightMargin);
                    var rightBackgroundRenderer = new DiffLineBackgroundRenderer { Lines = chunk.RightDiff };
                    right.TextArea.TextView.BackgroundRenderers.Add(rightBackgroundRenderer);
                    right.Text = String.Join("\r\n", chunk.RightDiff.Select(x => x.Text));

                    Grid.SetRow(right, 2 * row + 1);
                    Grid.SetColumn(right, 1);
                    rootGrid.Children.Add(right);
                }
            }

            // TODO: introduce highlighting specific sections
        }

        static List<DiffSectionViewModel> ResolveDiffSections(IEnumerable<string> hunkElements)
        {
            // TODO: extract file name
            // TODO: track file name changes

            var diffContents = hunkElements.Skip(3).ToList();
            var sectionHeaders = diffContents.Where(x => x.StartsWith("@@ ")).ToList();

            var regex = new Regex(@"\-(?<leftStart>\d{1,})\,(?<leftCount>\d{1,})\s\+(?<rightStart>\d{1,})\,(?<rightCount>\d{1,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var sections = new List<DiffSectionViewModel>();

            foreach (var header in sectionHeaders)
            {
                var lineNumbers = regex.Match(header);
                var startIndex = diffContents.IndexOf(header);
                var innerDiffContents = diffContents.Skip(startIndex + 1).ToList();

                var leftStart = int.Parse(lineNumbers.Groups["leftStart"].Value);
                var leftDiffSize = int.Parse(lineNumbers.Groups["leftCount"].Value);
                var rightStart = int.Parse(lineNumbers.Groups["rightStart"].Value);
                var rightDiffSize = int.Parse(lineNumbers.Groups["rightCount"].Value);

                var leftLineNumbers = Enumerable.Range(leftStart, leftDiffSize)
                    .Select(x => x.ToString(CultureInfo.InvariantCulture));

                var section = new DiffSectionViewModel();
                section.DiffSectionHeader = header;

                // left section - all context + deletes
                section.LeftDiff = innerDiffContents
                    .Where(x => !x.StartsWith("+"))
                    .Zip(leftLineNumbers, (x, line) => new { Item = x, LineNumber = line })
                    .Select(x => DiffLineViewModel.Create(x.LineNumber, x.Item))
                    .ToList();

                // right section - all context + adds
                var rightLineNumbers = Enumerable.Range(rightStart, rightDiffSize)
                    .Select(x => x.ToString(CultureInfo.InvariantCulture));

                section.RightDiff = innerDiffContents
                    .Where(x => !x.StartsWith("-"))
                    .Zip(rightLineNumbers, (x, line) => new { Item = x, LineNumber = line })
                    .Select(x => DiffLineViewModel.Create(x.LineNumber, x.Item))
                    .ToList();

                var missingRowCount = Math.Abs(section.LeftDiff.Count - section.RightDiff.Count);

                if (section.LeftDiff.Count > section.RightDiff.Count)
                {
                    var lastAdd = section.RightDiff.Last(x => x.Style == DiffContext.Added);
                    var lastIndex = section.RightDiff.IndexOf(lastAdd);
                    for (int i = 0; i < missingRowCount; i++)
                    {
                        var missing = new DiffLineViewModel();
                        missing.Style = DiffContext.Blank;
                        missing.Text = "";
                        missing.PrefixForStyle = "";
                        section.RightDiff.Insert(lastIndex + 1, missing);
                    }
                }
                else
                {
                    // TODO: fill in some extra empty rows in the left diff
                }



                sections.Add(section);

            }

            return sections;

            //            @@ -247,24 +247,7 @@ public Task<IResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthentic
            //                 Timeout = timeout
            //             };

            //-            if (!String.IsNullOrEmpty(accepts))
            //-            {
            //-                request.Headers["Accept"] = accepts;
            //-            }
            //-
            //-            if (!String.IsNullOrEmpty(twoFactorAuthenticationCode))
            //-            {
            //-                request.Headers["X-GitHub-OTP"] = twoFactorAuthenticationCode;
            //-            }
            //-
            //-            if (body != null)
            //-            {
            //-                request.Body = body;
            //-                // Default Content Type per: http://developer.github.com/v3/
            //-                request.ContentType = contentType ?? "application/x-www-form-urlencoded";
            //-            }
            //-
            //-            return Run<T>(request, cancellationToken);
            //+            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode, request);
            //         }

            //         Task<IResponse<T>> SendData<T>(
            //@@ -286,6 +269,11 @@ public Task<IResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthentic
            //                 Endpoint = uri,
            //             };

            //+            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode, request);
            //+        }
            //+
            //+        Task SendDataInternal<T>(object body, string accepts, string contentType, CancellationToken cancellationToken, string twoFactorAuthenticationCode, Request request)
            //+        {
            //             if (!String.IsNullOrEmpty(accepts))
            //             {
            //                 request.Headers["Accept"] = accepts;
            //@@ -303,7 +291,7 @@ public Task<IResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthentic
            //                 request.ContentType = contentType ?? "application/x-www-form-urlencoded";
            //             }

            //-            return Run<T>(request,cancellationToken);
            //+            return Run<T>(request, cancellationToken);
            //         }

            //         /// <summary>
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

    }
}
