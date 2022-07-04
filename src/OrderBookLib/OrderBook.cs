namespace OrderBookLib;

public class OrderBook
{
    private readonly Func<int> _orderIdGenerator;
    private readonly SortedDictionary<decimal, OrderBookLine> _buyBook = new(new DescendingDecimalComparer());
    private readonly SortedDictionary<decimal, OrderBookLine> _sellBook = new(Comparer<decimal>.Default);

    public OrderBook(Func<int>? orderIdGenerator = null)
    {
        _orderIdGenerator = orderIdGenerator ?? Random.Shared.Next;
    }

    public PlacementResult Add(Order order)
    {
        order.Id = _orderIdGenerator();

        switch (order.Direction)
        {
            case Direction.Buy when TryMatch(_sellBook, order):
                return new PlacementResult(Saved: order.Value != 0, Matched: true, order);
            case Direction.Buy:
                SaveOrder(_buyBook, order);
                return new PlacementResult(Saved: true, Matched: false, order);
            case Direction.Sell when TryMatch(_buyBook, order):
                return new PlacementResult(Saved: order.Value != 0, Matched: true, order);
            case Direction.Sell:
                SaveOrder(_sellBook, order);
                return new PlacementResult(Saved: true, Matched: false, order);
            default:
                throw new InvalidOperationException("How did you get there?");
        }
    }

    private bool TryMatch(IDictionary<decimal, OrderBookLine> book, Order order)
    {
        var canBeMatched = CanBeMatched(book, order);

        if (!canBeMatched)
        {
            return false;
        }

        do
        {
            if (!book.Values.Any())
            {
                break;
            }

            MatchOrderLine(book, book.Values.First(), order);
        } while (CanBeMatched(book, order));

        return true;
    }

    private void MatchOrderLine(IDictionary<decimal, OrderBookLine> book, OrderBookLine line, Order order)
    {
        while (order.Value > 0 && line.Any())
        {
            var nearestLineOrder = line.First();
            if (order.Value >= nearestLineOrder.Value)
            {
                order.Value -= nearestLineOrder.Value;
                line.Remove(nearestLineOrder.Id);
                if (!line.Any())
                {
                    book.Remove(nearestLineOrder.Price);
                }
            }
            else
            {
                nearestLineOrder.Value -= order.Value;
                order.Value = 0;
            }
        }
    }

    private static bool CanBeMatched(IDictionary<decimal, OrderBookLine> book, Order order)
    {
        return order.Direction switch
        {
            Direction.Buy => book.Keys.Any() && book.Keys.First() <= order.Price,
            Direction.Sell => book.Keys.Any() && book.Keys.First() >= order.Price,
            _ => throw new InvalidOperationException("There is no order directions between Buy and Sell")
        };
    }

    private void SaveOrder(IDictionary<decimal, OrderBookLine> book, Order order)
    {
        if (book.TryGetValue(order.Price, out var list))
        {
            list.Add(order);
        }
        else
        {
            book[order.Price] = new OrderBookLine { order };
        }
    }

    public CancellationResult Cancel(Order order)
    {
        if (order.Direction == Direction.Buy)
        {
            return CancelInternal(_buyBook, order);
        }

        return CancelInternal(_sellBook, order);
    }

    private CancellationResult CancelInternal(IDictionary<decimal, OrderBookLine> book, Order order)
    {
        if (book.TryGetValue(order.Price, out var ordersLine))
        {
            if (ordersLine.TryGetValue(order.Id, out var orderInLine))
            {
                ordersLine.Remove(order.Id);
                if (!ordersLine.Any())
                {
                    book.Remove(order.Price);
                }

                return new CancellationResult(true, orderInLine);
            }
        }

        return new CancellationResult(false, order);
    }

    public OrdersSnapshot GetSnapshot()
    {
        return new OrdersSnapshot(_buyBook, _sellBook);
    }
}