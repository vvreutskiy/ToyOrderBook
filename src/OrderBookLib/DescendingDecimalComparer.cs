namespace OrderBookLib;

public class DescendingDecimalComparer : IComparer<decimal>
{
    public int Compare(decimal x, decimal y) => y.CompareTo(x);
}