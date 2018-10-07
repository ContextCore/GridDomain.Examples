using System.Threading.Tasks;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Runners;
using Serilog;
using Xunit.Abstractions;

namespace BitCoinGame.Tests.Unit
{
    public class ClusterBitcoinGameTests : BitCoinGameTests
    {
        private ITestOutputHelper _output;

        public ClusterBitcoinGameTests(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        protected override async Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario<BinaryOptionGame> scenario)
        {
            return await scenario.Run()
                                 .Cluster<BinaryOptionGame>(new BinaryOptionDomainConfiguration(PriceProviderMock.Object),
                                                            () => new LoggerConfiguration()
                                                                  .WriteTo.XunitTestOutput(_output),
                                                            2,
                                                            2);
        }
    }
}