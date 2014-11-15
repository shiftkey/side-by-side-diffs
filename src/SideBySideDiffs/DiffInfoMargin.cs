using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace SideBySideDiffs
{
    public class DiffInfoMargin : AbstractMargin
    {
        static readonly SolidColorBrush BackBrush;
        static SolidColorBrush ForegroundBrush;
        const double TextHorizontalMargin = 4.0;

        FormattedText _lineFt, _plusMinusFt;
 
        static DiffInfoMargin()
        {
            BackBrush = new SolidColorBrush(Color.FromRgb(255, 0, 255));
            BackBrush.Freeze();

            ForegroundBrush = new SolidColorBrush(Colors.DarkGray);
            ForegroundBrush.Freeze();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Lines == null || Lines.Count == 0) return new Size(0.0, 0.0);

            var textToUse = Lines.Last().RowNumber.ToString();

            var tf = CreateTypeface();
            _lineFt = new FormattedText(
                textToUse,
                CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                tf, (double)GetValue(TextBlock.FontSizeProperty),
                BackBrush);
            _plusMinusFt = new FormattedText(
                "+ ",
                CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                tf, (double)GetValue(TextBlock.FontSizeProperty),
                BackBrush);

            // NB: This is a bit tricky. We use the margin control to actually
            // draw the diff "+/-" prefix, so that it's not selectable. So, looking
            // at this from the perspective of a single line, the arrangement is:
            //
            // margin-lineFt-margin-lineFt-margin-margin-plusMinusFt
            return new Size(
                _lineFt.Width + _plusMinusFt.WidthIncludingTrailingWhitespace + (TextHorizontalMargin * 2.0),
                0.0);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (Lines == null || Lines.Count == 0) return;

            var lineNumberWidth = Math.Round(_lineFt.Width + TextHorizontalMargin * 2.0);

            var tf = CreateTypeface();
            var fontSize = (double)GetValue(TextBlock.FontSizeProperty);

            var visualLines = TextView.VisualLinesValid ? TextView.VisualLines : Enumerable.Empty<VisualLine>();
            foreach (var v in visualLines)
            {
                var rcs = BackgroundGeometryBuilder.GetRectsFromVisualSegment(TextView, v, 0, 1000).ToArray();
                var linenum = v.FirstDocumentLine.LineNumber - 1;
                if (linenum >= Lines.Count) continue;

                var diffLine = Lines[linenum];

                FormattedText ft;

                if (diffLine.Text != "")
                {
                    ft = new FormattedText(diffLine.RowNumber.ToString(),
                        CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        tf, fontSize, ForegroundBrush);

                    var left = 0;

                    drawingContext.DrawText(ft, new Point(left, rcs[0].Top));
                }

                if (diffLine.PrefixForStyle != "")
                {
                    var prefix = diffLine.PrefixForStyle;
                    ft = new FormattedText(prefix,
                        CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        tf, fontSize, (Brush)TextView.GetValue(Control.ForegroundProperty));

                    drawingContext.DrawText(ft, new Point(lineNumberWidth + TextHorizontalMargin, rcs[0].Top));
                }
            }
        }

        public List<DiffLineViewModel> Lines { get; set; }

        Typeface CreateTypeface()
        {
            var fe = TextView;
            return new Typeface((FontFamily)fe.GetValue(TextBlock.FontFamilyProperty),
                (FontStyle)fe.GetValue(TextBlock.FontStyleProperty),
                (FontWeight)fe.GetValue(TextBlock.FontWeightProperty),
                (FontStretch)fe.GetValue(TextBlock.FontStretchProperty));
        }
    }
}
