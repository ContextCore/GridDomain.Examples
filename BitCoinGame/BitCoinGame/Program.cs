using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Remote.Transport;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using Info.Blockchain.API.Client;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace BitCoinGame
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var node = new GridDomainNode(() => ActorSystem.Create());
            
            
            Console.ReadKey();
        }
    }

    class BinaryOptionDomainConfiguration : IDomainConfiguration
    {
        
        public void Register(IDomainBuilder builder)
        {
            var provider = new BitCoinUsdPriceProvider();    
            var aggregateFactory = DefaultAggregateDependencyFactory.ForCommandAggregate<BinaryOptionGame>();
            aggregateFactory.AggregateFactoryCreator = () => new BinaryOptionsAggregateConstructor(provider);
            builder.RegisterAggregate<BinaryOptionGame>();
        }

        class BinaryOptionsAggregateConstructor : AggregateFactory
        {
            public BinaryOptionsAggregateConstructor(IPriceProvider provider)
            {
                
            }
            
            override 
        }
    }

    class BitCoinUsdPriceProvider : IPriceProvider
    {
        private readonly BlockchainHttpClient _blockchainHttpClient;

        class HistoryPrice
        {
            public decimal last { get; set; } 
        }
        class BitcoinPrice
        {
            public HistoryPrice USD { get; set; }
        }
        
        public BitCoinUsdPriceProvider()
        {
            _blockchainHttpClient = new BlockchainHttpClient();
        }
        public async Task<decimal> GetPrice()
        {
            var response = await _blockchainHttpClient.GetAsync<BitcoinPrice>(@"https://blockchain.info/ru/ticker");
            return response.USD.last;
        }
    }

    public class CreateNewGameCommand : Command
    {
        public decimal StartAmount { get; }
        public decimal WinAmount { get; }

        public CreateNewGameCommand(decimal startAmount, decimal winAmount):base(Guid.NewGuid())
        {
            StartAmount = startAmount;
            WinAmount = winAmount;
        }
    }

    public class PlaceBidCommand : Command
    {
        public Direction Direction { get; }
        public decimal Amount { get; }

        public PlaceBidCommand(Guid gameId, Direction direction, decimal amount):base(gameId)
        {
            Direction = direction;
            Amount = amount;
        }
    }
    
    public class BinaryOptionGame : ConventionAggregate
    {
        
        //Persisted state
        public decimal TotalAmount { get; private set; }
        public decimal WinAmount { get; private set; }

        public Bid CurrentBid { get; private set; }
        public bool IsLost { get; private set; }
        public bool IsWon { get; private set; }
        public bool IsEnded { get; private set; }
        
        
        public class Bid
        {
            public Bid(Direction direction, decimal amount, decimal baseLevel)
            {
                Direction = direction;
                Amount = amount;
                BaseLevel = baseLevel;
            }

            public Direction Direction { get; }
            public decimal Amount { get; }
            public decimal BaseLevel { get; }

            public bool IsWon(decimal currentAmount)
            {
                return Direction == Direction.Down && currentAmount < BaseLevel ||
                       Direction == Direction.Up && currentAmount > BaseLevel;
            }
        }
        
        private BinaryOptionGame(Guid id, IPriceProvider provider) : base(id)
        {
            Apply<GameCreated>(e =>
            {
                TotalAmount = e.InitialAmount;
                WinAmount = WinAmount;
                IsWon = false;
                IsEnded = false;
                IsLost = false;
            });
            Apply<BidPlaced>(e =>
            {
                CurrentBid = new Bid(e.Dir, e.Amount, e.BaseLevel);
            });
            Apply<BidWon>(e => TotalAmount += e.Amount);
            Apply<BidLost>(e => TotalAmount -= e.Amount);
            Apply<GameLost>(e =>
            {
                IsEnded = true;
                IsLost = true;
            });
            Apply<GameWon>(e =>
            {
                IsEnded = true;
                IsWon = true;
            });
            
            Execute<CreateNewGameCommand>(c => new BinaryOptionGame(c.AggregateId, c.StartAmount,c.WinAmount, provider));
            Execute<PlaceBidCommand>(c => PlaceBid(c.Direction,c.Amount,provider));
        }

        public BinaryOptionGame(Guid gameId, decimal initialAmount, decimal winAmount, IPriceProvider provider):this(gameId, provider)
        {
            Produce(new GameCreated(Id,initialAmount, winAmount));
        }

        public async Task PlaceBid(Direction dir, decimal amount,IPriceProvider priceProvider)
        {
            if (IsEnded)
                throw new CannotBidOnEndedGameException();
            if (amount > TotalAmount)
                throw new NotEnoughMoneyException();
            if (CurrentBid != null)
                throw new BidAlreadyPlacedException();
            
            Produce(new BidPlaced(Id,dir,amount, await priceProvider.GetPrice()));
        }

        public async Task CheckBid(IPriceProvider priceProvider)
        {
            if (IsEnded)
                throw new CannotBidOnEndedGameException();
            if (CurrentBid == null)
                throw new NoActiveBidException();
            if (CurrentBid.IsWon(await priceProvider.GetPrice()))
                Produce(new BidWon(Id, CurrentBid.Amount));
            else
                await Emit(new BidLost(Id, CurrentBid.Amount));
            
            if(TotalAmount <=0)
                Produce(new GameLost(Id));
            if (TotalAmount >= WinAmount)
                Produce(new GameWon(Id));
        }

    }

    public class BidAlreadyPlacedException : Exception
    {
    }

    public class CannotBidOnEndedGameException : Exception
    {
    }

    public class GameWon : DomainEvent
    {
        public GameWon(Guid id):base(id)
        {
        }
    }

    public class GameLost : DomainEvent
    {
        public GameLost(Guid id):base(id)
        {
        }
    }

    public class BidLost : DomainEvent
    {
        public decimal Amount { get; }

        public BidLost(Guid id, decimal amount):base(id)
        {
            Amount = amount;
        }
    }

    public class BidWon : DomainEvent
    {
        public decimal Amount { get; }

        public BidWon(Guid id, decimal amount):base(id)
        {
            Amount = amount;
        }
    }

    public class NotEnoughMoneyException : Exception
    {
    }

    public class NoActiveBidException : Exception
    {
    }

    public class BidPlaced : DomainEvent
    {
        public Direction Dir { get; }
        public decimal Amount { get; }
        public decimal BaseLevel { get; }

        public BidPlaced(Guid id, Direction dir, decimal amount, decimal baseLevel):base(id)
        {
            Dir = dir;
            Amount = amount;
            BaseLevel = baseLevel;
        }
    }

    public enum Direction
    {
        Up,
        Down
    }

    public interface IPriceProvider
    {
        Task<decimal> GetPrice();
    }

    public class GameCreated : DomainEvent
    {
        public decimal InitialAmount { get; }

        public GameCreated(Guid sourceId, decimal initialAmount, decimal winAmount) : base(sourceId)
        {
            InitialAmount = initialAmount;
        }
    }

    public class ConsoleUI
    {
        
    }
}