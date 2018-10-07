using System.Threading.Tasks;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Runners;
using Xunit.Abstractions;

namespace BitCoinGame.Tests.Unit
{
    public class NodeBitcoinGameTests : BitCoinGameTests
    {
        public NodeBitcoinGameTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override async Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario<BinaryOptionGame> scenario)
        {
            var node = scenario.Run()
                               .Node(new BinaryOptionDomainConfiguration(PriceProviderMock.Object), Logger);
            
            return await node;
        }
    }
}