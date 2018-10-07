using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class BidLost : DomainEvent<BinaryOptionGame>
    {
        public decimal Amount { get; }

        public BidLost(string id, decimal amount):base(id)
        {
            Amount = amount;
        }
    }
}