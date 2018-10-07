using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace BitCoinGame
{
    public class BinaryOptionAggregateFactory : AggregateFactory
    {
        private readonly IPriceProvider _provider;

        public BinaryOptionAggregateFactory(IPriceProvider provider)
        {
            _provider = provider;
        }
        public override IAggregate Build(Type type, string id, ISnapshot snapshot = null)
        {
            if (type == typeof(BinaryOptionGame))
            {
                var binaryOptionGame = new BinaryOptionGame(id, _provider);
                binaryOptionGame.Clear();
                return binaryOptionGame;
            }
            
            return base.Build(type, id, snapshot);
        }
    }
}