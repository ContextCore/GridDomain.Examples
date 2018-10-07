using GridDomain.Configuration;

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
            var dependencyFactory = new AggregateDependencies<BinaryOptionGame>()
                                    {
                                        AggregateFactory = new BinaryOptionAggregateFactory(_priceProvider)
                                    };

            builder.RegisterAggregate(dependencyFactory);

            var handler = new BinaryOptionConsoleOutputHandler(); 
            
            builder.RegisterHandler<GameCreated,BinaryOptionConsoleOutputHandler>(c =>handler).AsSync();
            builder.RegisterHandler<GameWon,BinaryOptionConsoleOutputHandler>(c =>handler).AsSync();
            builder.RegisterHandler<BidPlaced,BinaryOptionConsoleOutputHandler>(c =>handler).AsSync();
            builder.RegisterHandler<BidWon,BinaryOptionConsoleOutputHandler>(c =>handler).AsSync();
            builder.RegisterHandler<BidLost,BinaryOptionConsoleOutputHandler>(c =>handler).AsSync();    
        }

    }
}