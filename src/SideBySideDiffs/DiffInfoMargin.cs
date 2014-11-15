using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;

namespace SideBySideDiffs
{
    public class DiffInfoMargin : AbstractMargin
    {
        static readonly SolidColorBrush BackBrush;
        const double TextHorizontalMargin = 4.0;

        FormattedText _lineFt, _plusMinusFt;
  
        static DiffInfoMargin()
        {
            BackBrush = new SolidColorBrush(Color.FromRgb(255, 0, 255));
            BackBrush.Freeze();
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
                (_lineFt.Width * 2.0) + _plusMinusFt.WidthIncludingTrailingWhitespace + (TextHorizontalMargin * 4.0),
                0.0);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
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
