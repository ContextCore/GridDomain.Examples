using System;
using GridDomain.CQRS;

namespace BitCoinGame
{
    public class PlaceBidCommand : Command
    {
        public Direction Direction { get; }
        public decimal Amount { get; }

        public PlaceBidCommand(Guid gameId, Direction direction, decimal amount):base(gameId)
        {
            Direction = direction;
            Amount = amount;
        }
    }
}