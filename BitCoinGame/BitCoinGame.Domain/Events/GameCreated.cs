using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameCreated : DomainEvent
    {
        public decimal InitialAmount { get; }

        public GameCreated(string sourceId, decimal initialAmount, decimal winAmount) : base(sourceId)
        {
            InitialAmount = initialAmount;
        }
    }
}