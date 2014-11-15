namespace SideBySideDiffs
{
    public class DiffLineViewModel
    {
        public string Text { get; set; }
        public DiffContext Context { get; set; }
        public int RowNumber { get; set; }
    }
}