namespace SideBySideDiffs
{
    public class DiffLineViewModel
    {
        public string Text { get; set; }
        public DiffContext Style { get; set; }
        public int RowNumber { get; set; }
        public string PrefixForStyle { get; set; }
    }
}