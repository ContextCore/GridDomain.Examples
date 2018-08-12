using System;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace BitCoinGame
{
    public class BinaryOptionDomainConfiguration : IDomainConfiguration
    {
        private readonly IPriceProvider _priceProvider;

        public BinaryOptionDomainConfiguration(IPriceProvider priceProvider)
        {
            _priceProvider = priceProvider;
        }
        public void Register(IDomainBuilder builder)
        {
            var aggregateFactory = new BinaryOptionAggregateFactory(_priceProvider);
            var handler = CommandAggregateHandler.New<BinaryOptionGame>(aggregateFactory);

            var dependencyFactory = new DefaultAggregateDependencyFactory<BinaryOptionGame>(() => handler)
                                    {
                                        AggregateFactoryCreator = () => aggregateFactory
                                    };

            builder.RegisterAggregate(dependencyFactory);
        }

    }

    public class BinaryOptionAggregateFactory : IConstructAggregates
    {
        private readonly IPriceProvider _provider;

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