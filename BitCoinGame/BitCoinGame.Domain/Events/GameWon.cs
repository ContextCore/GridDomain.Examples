using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameWon : DomainEvent<BinaryOptionGame>
    {
        public GameWon(string id):base(id)
        {
        }
    }
}