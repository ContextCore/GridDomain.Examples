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
            builder.RegisterAggregate(
               new DefaultAggregateDependencyFactory<BinaryOptionGame>(() => new BinaryOptionCommandHandler(provider)));
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

    public class BinaryOptionCommandHandler : AggregateCommandsHandler<BinaryOptionGame>
    {
        public BinaryOptionCommandHandler(IPriceProvider provider)
        {
            Map<CreateNewGameCommand>(c => new BinaryOptionGame(c.AggregateId,c.StartAmount,c.WinAmount,provider));
            Map<PlaceBidCommand>((c,a) => a.PlaceBid(c.Direction,c.Amount, provider));
        }
    }
}