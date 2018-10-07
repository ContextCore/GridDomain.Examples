using System;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class GameLost : DomainEvent<BinaryOptionGame>
    {
        public GameLost(string id):base(id)
        {
        }
    }
}