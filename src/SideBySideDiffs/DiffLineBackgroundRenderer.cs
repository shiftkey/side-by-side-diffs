using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace SideBySideDiffs
{
    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            
        }

        public KnownLayer Layer { get{ return KnownLayer.Background; } }
    }
}