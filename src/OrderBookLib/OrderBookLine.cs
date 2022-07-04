using System.Collections.ObjectModel;

namespace OrderBookLib;

public class OrderBookLine : KeyedCollection<int, Order>
{
    protected override int GetKeyForItem(Order item)
    {
        return item.Id;
    }
}