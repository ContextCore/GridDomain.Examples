using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class BidWon : DomainEvent
    {
        public decimal Amount { get; }

        public BidWon(Guid id, decimal amount):base(id)
        {
            Amount = amount;
        }
    }
}