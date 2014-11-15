using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;

namespace SideBySideDiffs
{
    public class DiffInfoMargin : AbstractMargin
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        public List<DiffLineViewModel> Lines { get; set; }
    }
}
