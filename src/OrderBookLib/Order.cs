namespace OrderBookLib;

public class Order
{
    public int Id { get; set; }
    public Direction Direction { get; private init; }
    public decimal Price { get; private init; }
    public int Value { get; set; }

    public static Order NewBuyOrder(decimal price, int value) => new()
    {
        Direction = Direction.Buy,
        Price = price,
        Value = value
    };

    public static Order NewSellOrder(decimal price, int value) => new()
    {
        Direction = Direction.Sell,
        Price = price,
        Value = value
    };
}