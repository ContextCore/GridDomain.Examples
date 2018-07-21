using GridDomain.Configuration;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    class BinaryOptionDomainConfiguration : IDomainConfiguration
    {
        
        public void Register(IDomainBuilder builder)
        {
            var provider = new BitCoinUsdPriceProvider();    
            builder.RegisterAggregate(
               new DefaultAggregateDependencyFactory<BinaryOptionGame>(() => new BinaryOptionCommandHandler(provider)));
        }

        class BinaryOptionCommandHandler : AggregateCommandsHandler<BinaryOptionGame>
        {
            public BinaryOptionCommandHandler(IPriceProvider provider)
            {
                Map<CreateNewGameCommand>(c => new BinaryOptionGame(c.AggregateId,c.StartAmount,c.WinAmount,provider));
                Map<PlaceBidCommand>((c,a) => a.PlaceBid(c.Direction,c.Amount, provider));
            }
        }
    }
}