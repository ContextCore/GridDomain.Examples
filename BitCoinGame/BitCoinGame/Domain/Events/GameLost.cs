using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameLost : DomainEvent
    {
        public GameLost(Guid id):base(id)
        {
        }
    }
}