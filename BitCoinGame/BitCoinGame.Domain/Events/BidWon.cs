using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class BidWon : DomainEvent<BinaryOptionGame>
    {
        public decimal Amount { get; }

        public BidWon(string id, decimal amount):base(id)
        {
            Amount = amount;
        }
    }
}