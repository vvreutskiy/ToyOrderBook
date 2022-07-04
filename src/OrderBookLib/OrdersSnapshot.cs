namespace OrderBookLib;

public class OrdersSnapshot
{
    public OrdersSnapshot(
        SortedDictionary<decimal, OrderBookLine> buyBook,
        SortedDictionary<decimal, OrderBookLine> sellBook)
    {
        BuyOrders = new List<Order>(buyBook.Values.SelectMany(v => v));
        SellOrders = new List<Order>(sellBook.Values.SelectMany(v => v));
    }

    public List<Order> BuyOrders { get; }
    public List<Order> SellOrders { get; }
}