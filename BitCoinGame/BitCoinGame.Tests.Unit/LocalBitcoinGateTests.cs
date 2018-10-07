using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Runners;
using Xunit.Abstractions;

namespace BitCoinGame.Tests.Unit
{
    public class LocalBitcoinGateTests : BitCoinGameTests
    {
        public LocalBitcoinGateTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override async Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario<BinaryOptionGame> scenario)
        {
            return await scenario.Run()
                                 .Local(Logger);
        }
    }
}