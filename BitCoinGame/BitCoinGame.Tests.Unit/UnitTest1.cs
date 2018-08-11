using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using GridDomain.Tests.Scenarios;
using GridDomain.Tests.Scenarios.Runners;
using Moq;
using Serilog;
using Serilog.Core;
using Should.Fluent;
using Xunit;
using Xunit.Abstractions;

namespace BitCoinGame.Tests.Unit
{
    public class LocalBitcoinGateTests : BitCoinGameTests
    {
        private IPriceProvider _priceProvider;
        private BinaryOptionCommandHandler _handler;
        private BinaryOptionAggregateFactory _factory;

        public LocalBitcoinGateTests(ITestOutputHelper output) : base(output)
        {
            _priceProvider = Mock.Of<IPriceProvider>();
            _handler = new BinaryOptionCommandHandler(_priceProvider);
            _factory = new BinaryOptionAggregateFactory(_priceProvider);
        }

        protected override async Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario scenario)
        {
            return await scenario.Run()
                                 .Local(_factory, _handler, Logger);
        }
    }

    public class NodeBitcoinGateTests : BitCoinGameTests
    {
        private IPriceProvider _priceProvider;
        private BinaryOptionCommandHandler _handler;
        private BinaryOptionAggregateFactory _factory;

        public NodeBitcoinGateTests(ITestOutputHelper output) : base(output)
        {
            _priceProvider = Mock.Of<IPriceProvider>();
            _handler = new BinaryOptionCommandHandler(_priceProvider);
            _factory = new BinaryOptionAggregateFactory(_priceProvider);
        }

        protected override async Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario scenario)
        {
            return await scenario.Run()
                                 .Node<BinaryOptionGame>(new BinaryOptionDomainConfiguration(), Logger);
        }
    }

    public class ClusterBitcoinGateTests : BitCoinGameTests
    {
        private IPriceProvider _priceProvider;
        private BinaryOptionCommandHandler _handler;
        private BinaryOptionAggregateFactory _factory;
        private ITestOutputHelper _output;

        public ClusterBitcoinGateTests(ITestOutputHelper output) : base(output)
        {
            _output = output;
            _priceProvider = Mock.Of<IPriceProvider>();
            _handler = new BinaryOptionCommandHandler(_priceProvider);
            _factory = new BinaryOptionAggregateFactory(_priceProvider);
        }

        protected override async Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario scenario)
        {
            return await scenario.Run()
                                 .Cluster<BinaryOptionGame>(new BinaryOptionDomainConfiguration(),
                                                            () => new LoggerConfiguration()
                                                                  .WriteTo.XunitTestOutput(_output),
                                                            2,
                                                            2);
        }
    }

    /// <summary>
    /// Bitcoin game is a stake-based game when player has initial funds 
    /// and should increase it to given amount
    /// 
    /// Player can bid on bitcoin price raise or low with amount of money 
    /// Amount is reduced from player funds
    /// 
    /// After a time player can check his bid 
    /// 
    /// If he won, double bid is returned to player balance 
    /// 
    /// If he lost, bid is not returned 
    /// 
    /// If player has zero funds, he loose 
    /// If player increase his funds to target amount, he won
    /// Player can not place a bid for more money that he has
    /// Player can have only one active bid 
    /// </summary>
    public abstract class BitCoinGameTests
    {
        protected readonly Logger Logger;

        protected BitCoinGameTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration().WriteTo.XunitTestOutput(output).CreateLogger();
        }

        protected abstract Task<IAggregateScenarioRun<BinaryOptionGame>> Run(IAggregateScenario scenario);

        protected async Task Check(IAggregateScenario scenario)
        {
            await Run(scenario).Check();
        }


        [Fact]
        public async Task Given_Game_When_bid_is_placed_Then_it_amount_is_substracted_from_funds()
        {
            var scenario = AggregateScenarioBuilder.New()
                                                   .Given(new GameCreated("a", 10, 15))
                                                   .When(new PlaceBidCommand("a", Direction.Up, 5))
                                                   .Build();
            var run = await Run(scenario);

            Assert.Equal(5, run.Aggregate.TotalAmount);
        }


        [Fact]
        public async Task Given_Game_When_bid_is_placed_Then_bid_event_occurs()
        {
            var scenario = AggregateScenarioBuilder.New()
                                                  .Given(new GameCreated("a", 10, 15))
                                                  .When(new PlaceBidCommand("a", Direction.Up, 5))
                                                  .Then(new BidPlaced("a", Direction.Up, 5, 0))
                                                  .Build();

            await Check(scenario);
        }

        [Fact]
        public async Task Given_Game_When_placing_bid_bigger_then_amount_Then_bid_is_not_accepted()
        {
            var scenario = AggregateScenarioBuilder.New()
                                                   .Given(new GameCreated("a", 10, 15))
                                                   .When(new PlaceBidCommand("a", Direction.Up, 20))
                                                   .Build();

            await Check(scenario).ShouldThrow<NotEnoughMoneyException>();
        }

        [Fact]
        public void Given_Game_When_dib_won_And_total_funds_greater_target_Then_game_is_won()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Given_Game_When_dib_lost_And_total_funds_are_zero_Then_game_is_lost()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Given_game_with_active_bid_for_raise_When_bid_is_checked_And_won_Then_double_bid_returned_to_funds()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Given_game_with_active_bid_for_lower_When_bid_is_checked_And_won_Then_double_bid_returned_to_funds()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Given_game_with_active_bid_for_lower_When_bid_is_checked_And_lst_Then_funds_not_changed()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Given_game_with_active_bid_for_raise_When_bid_is_checked_And_lst_Then_funds_not_changed()
        {
            throw new NotImplementedException();
        }
    }
}