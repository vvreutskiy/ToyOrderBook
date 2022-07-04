namespace OrderBookLib;

public record PlacementResult(bool Saved, bool Matched, Order Order);