using System.Threading.Tasks;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Builders;
using GridDomain.Scenarios.Runners;
using GridDomain.Tests.Common;
using Moq;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace BitCoinGame.Tests.Unit
{
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
        private IAggregateScenarioBuilder<BinaryOptionGame> Scenario { get; }
        protected Mock<IPriceProvider> PriceProviderMock { get; }

        protected BitCoinGameTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration().WriteTo.XunitTestOutput(output).CreateLogger();
            PriceProviderMock = new Mock<IPriceProvider>();
            var factory = new BinaryOptionAggregateFactory(PriceProviderMock.Object);
            Scenario = AggregateScenario.New<BinaryOptionGame>().With(factory);
        }

        protected abstract Task<IAggregateScenarioRun<BinaryOptionGame>> Run(
            IAggregateScenario<BinaryOptionGame> scenario);

        protected async Task Check(IAggregateScenario<BinaryOptionGame> scenario)
        {
            await Run(scenario).Check();
        }


        [Fact]
        public async Task Given_Game_When_bid_is_placed_Then_it_amount_is_substracted_from_funds()
        {
            var scenario = Scenario
                           .Given(new GameCreated("a", 10, 15))
                           .When(new PlaceBidCommand("a", Direction.Up, 5))
                           .Build();
            var run = await Run(scenario);

            Assert.Equal(5, run.Aggregate.TotalAmount);
        }


        [Fact]
        public async Task Given_Game_When_bid_is_placed_Then_bid_event_occurs()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 15))
                                            .When(new PlaceBidCommand("a", Direction.Up, 5))
                                            .Then(new BidPlaced("a", Direction.Up, 5, 0))
                                            .Build();

            await Check(scenario);
        }

        [Fact]
        public async Task Given_Game_When_placing_bid_bigger_then_amount_Then_bid_is_not_accepted()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 15))
                                            .When(new PlaceBidCommand("a", Direction.Up, 20))
                                            .Build();

            await Run(scenario).ShouldThrow<NotEnoughMoneyException>();
        }

        [Fact]
        public async Task Given_Game_When_dib_won_And_total_funds_greater_target_Then_game_is_won()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 15),
                                                   new BidPlaced("a", Direction.Down, 10, 1000))
                                            .When(new CheckBidCommand("a"))
                                            .Then(new BidWon("a", 20),
                                                  new GameWon("a"))
                                            .Build();

            await Run(scenario).Check();
        }

        [Fact]
        public async Task Given_Game_When_bid_lost_And_total_funds_are_zero_Then_game_is_lost()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 15),
                                                   new BidPlaced("a", Direction.Up, 10, 1000))
                                            .When(new CheckBidCommand("a"))
                                            .Then(new BidLost("a", 10),
                                                  new GameLost("a"))
                                            .Build();

            await Run(scenario).Check();
        }

        [Fact]
        public async Task
            Given_game_with_active_bid_for_raise_When_bid_is_checked_And_won_Then_double_bid_returned_to_funds()
        {
            PriceProviderMock.Setup(m => m.GetPrice()).Returns(Task.FromResult(2000M));

            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new BidPlaced("a", Direction.Up, 10, 1000))
                                            .When(new CheckBidCommand("a"))
                                            .Then(new BidWon("a", 20))
                                            .Build();

            var run = await Run(scenario);
            Assert.Equal(20, run.Aggregate.TotalAmount);
        }

        [Fact]
        public async Task
            Given_game_with_active_bid_for_lower_When_bid_is_checked_And_won_Then_double_bid_returned_to_funds()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new BidPlaced("a", Direction.Down, 10, 1000))
                                            .When(new CheckBidCommand("a"))
                                            .Then(new BidWon("a", 20))
                                            .Build();

            var run = await Run(scenario);
            Assert.Equal(20, run.Aggregate.TotalAmount);
        }

        [Fact]
        public async Task
            Given_game_with_active_bid_for_lower_When_bid_is_checked_And_lost_Then_it_is_not_returned_to_funds()
        {
            PriceProviderMock.Setup(m => m.GetPrice()).Returns(Task.FromResult(1500M));
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new BidPlaced("a", Direction.Down, 4, 1000))
                                            .When(new CheckBidCommand("a"))
                                            .Then(new BidLost("a", 10))
                                            .Build();

            var run = await Run(scenario);
            Assert.Equal(6, run.Aggregate.TotalAmount);
        }

        [Fact]
        public async Task
            Given_game_with_active_bid_for_raise_When_bid_is_checked_And_lost_Then_it_is_not_returned_to_funds()
        {
            PriceProviderMock.Setup(m => m.GetPrice()).Returns(Task.FromResult(500M));
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new BidPlaced("a", Direction.Up, 4, 1000))
                                            .When(new CheckBidCommand("a"))
                                            .Then(new BidLost("a", 10))
                                            .Build();

            var run = await Run(scenario);
            Assert.Equal(6, run.Aggregate.TotalAmount);
        }

        [Fact]
        public async Task Given_lost_game_When_placing_new_bid_Then_error_occures()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new GameLost("a"))
                                            .When(new PlaceBidCommand("a", Direction.Down, 5))
                                            .Build();

            await Run(scenario).ShouldThrow<CannotBidOnEndedGameException>();
        }

        [Fact]
        public async Task Given_lost_game_When_checking_a_bid_Then_error_occur()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new GameLost("a"))
                                            .When(new CheckBidCommand("a"))
                                            .Build();

            await Run(scenario).ShouldThrow<CannotCheckBidOnEndedGameException>();
        }

        [Fact]
        public async Task Given_won_game_When_checking_a_bid_Then_error_occur()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new GameWon("a"))
                                            .When(new CheckBidCommand("a"))
                                            .Build();

            await Run(scenario).ShouldThrow<CannotCheckBidOnEndedGameException>();
        }

        [Fact]
        public async Task Given_active_game_without_a_bid_When_checking_a_bid_Then_error_occur()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35))
                                            .When(new CheckBidCommand("a"))
                                            .Build();

            await Run(scenario).ShouldThrow<NoActiveBidException>();
        }

        [Fact]
        public async Task Given_active_game_with_a_bid_When_placing_a_new_bid_Then_error_occur()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new BidPlaced("a", Direction.Down, 10, 1000))
                                            .When(new PlaceBidCommand("a", Direction.Up, 12))
                                            .Build();

            await Run(scenario).ShouldThrow<BidAlreadyPlacedException>();
        }


        [Fact]
        public async Task Given_won_game_When_placing_new_bid_Then_error_occur()
        {
            var scenario = Scenario
                                            .Given(new GameCreated("a", 10, 35),
                                                   new GameWon("a"))
                                            .When(new PlaceBidCommand("a", Direction.Down, 5))
                                            .Build();

            await Run(scenario).ShouldThrow<CannotBidOnEndedGameException>();
        }
    }
}