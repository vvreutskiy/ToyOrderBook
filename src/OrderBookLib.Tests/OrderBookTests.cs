namespace OrderBookLib.Tests;

[TestFixture]
public class OrderBookTests
{
    private OrderBook _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new OrderBook();
    }

    [Test]
    public void Should_add_orders()
    {
        var someOrder = Order.NewBuyOrder(10, 10);
        var result = _sut.Add(someOrder);
        Assert.That(result.Saved && !result.Matched);
    }

    [Test]
    public void Should_return_snapshot_with_sorted_buy_orders()
    {
        var someOrder10 = Order.NewBuyOrder(10, 10);
        var someOrder20 = Order.NewBuyOrder(20, 10);
        var someOrder30 = Order.NewBuyOrder(30, 10);
        _ = _sut.Add(someOrder10);
        _ = _sut.Add(someOrder20);
        _ = _sut.Add(someOrder30);

        var snapshot = _sut.GetSnapshot();

        Assert.That(snapshot.BuyOrders, Is.Ordered.By(nameof(Order.Price)).Descending);
    }

    [Test]
    public void Should_return_snapshot_with_sorted_sell_orders()
    {
        var someOrder10 = Order.NewSellOrder(10, 10);
        var someOrder20 = Order.NewSellOrder(20, 10);
        var someOrder30 = Order.NewSellOrder(30, 10);
        _ = _sut.Add(someOrder10);
        _ = _sut.Add(someOrder20);
        _ = _sut.Add(someOrder30);

        var snapshot = _sut.GetSnapshot();

        Assert.That(snapshot.SellOrders, Is.Ordered.By(nameof(Order.Price)).Ascending);
    }

    [Test]
    public void Should_cancel_orders()
    {
        var someOrder1 = Order.NewBuyOrder(10, 10);
        var someOrder2 = Order.NewBuyOrder(10, 20);

        _sut.Add(someOrder1);
        _sut.Add(someOrder2);

        var cancellationResult = _sut.Cancel(someOrder1);

        Assert.That(cancellationResult.Success);
        Assert.That(_sut.GetSnapshot().BuyOrders.Single(), Is.EqualTo(someOrder2));
    }

    [Test]
    public void Should_match_buy_orders_with_sell_order_book()
    {
        _sut.Add(Order.NewSellOrder(10, 10));
        _sut.Add(Order.NewSellOrder(11, 10));
        _sut.Add(Order.NewSellOrder(12, 10));
        _sut.Add(Order.NewSellOrder(13, 10));

        var result = _sut.Add(Order.NewBuyOrder(12, 30));

        Assert.That(result.Matched);
        Assert.That(!result.Saved);
        Assert.That(_sut.GetSnapshot().SellOrders.Count, Is.EqualTo(1));
    }

    [Test]
    public void Should_match_sell_orders_with_buy_order_book()
    {
        _sut.Add(Order.NewBuyOrder(10, 10));
        _sut.Add(Order.NewBuyOrder(11, 10));
        _sut.Add(Order.NewBuyOrder(12, 10));
        _sut.Add(Order.NewBuyOrder(13, 10));

        var result = _sut.Add(Order.NewSellOrder(11, 30));

        Assert.That(result.Matched);
        Assert.That(!result.Saved);
        Assert.That(_sut.GetSnapshot().BuyOrders.Count, Is.EqualTo(1));
    }
}