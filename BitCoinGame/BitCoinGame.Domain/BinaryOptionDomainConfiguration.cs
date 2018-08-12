using System;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace BitCoinGame
{
    public class BinaryOptionDomainConfiguration : IDomainConfiguration
    {
        
        public void Register(IDomainBuilder builder)
        {
            var provider = new BitCoinUsdPriceProvider();    
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.ForCommandAggregate<BinaryOptionGame>(new BinaryOptionAggregateFactory(provider)));
        }

    }

    public class BinaryOptionAggregateFactory : IConstructAggregates
    {
        private IPriceProvider _provider;

        public BinaryOptionAggregateFactory(IPriceProvider provider)
        {
            _provider = provider;
        }
        public IAggregate Build(Type type, string id, IMemento snapshot = null)
        {
           if(type == typeof(BinaryOptionGame))
               return new BinaryOptionGame(id,_provider);

            return AggregateFactory.Default.Build(type, id, snapshot);
        }
    }
}