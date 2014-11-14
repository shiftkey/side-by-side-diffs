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
            // TODO: hack out the patch for a diff
            // TODO: split this into left and right pieces
            // TODO: bind this to the page
            // TODO: style this mofo
            // TODO: parse the strings into proper domain objects
            // TODO: introduce line highlighting
            // TODO: introduce highlighting specific sections

            var left = new TextEditor();
            Grid.SetColumn(left, 0);
            rootGrid.Children.Add(left);

            var right = new TextEditor();
            Grid.SetColumn(right, 1);
            rootGrid.Children.Add(right);
        }
    }
}
