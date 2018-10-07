using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameCreated : DomainEvent<BinaryOptionGame>
    {
        public decimal InitialAmount { get; }
        public decimal WinAmount { get; }

        public GameCreated(string sourceId, decimal initialAmount, decimal winAmount) : base(sourceId)
        {
            InitialAmount = initialAmount;
            WinAmount = winAmount;
        }
    }
}