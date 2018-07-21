using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameWon : DomainEvent
    {
        public GameWon(string id):base(id)
        {
        }
    }
}