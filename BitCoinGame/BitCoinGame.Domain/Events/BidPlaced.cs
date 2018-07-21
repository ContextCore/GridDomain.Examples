using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class BidPlaced : DomainEvent
    {
        public Direction Dir { get; }
        public decimal Amount { get; }
        public decimal BaseLevel { get; }

        public BidPlaced(string id, Direction dir, decimal amount, decimal baseLevel):base(id)
        {
            Dir = dir;
            Amount = amount;
            BaseLevel = baseLevel;
        }
    }
}