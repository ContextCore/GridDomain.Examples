using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameLost : DomainEvent
    {
        public GameLost(string id):base(id)
        {
        }
    }
}