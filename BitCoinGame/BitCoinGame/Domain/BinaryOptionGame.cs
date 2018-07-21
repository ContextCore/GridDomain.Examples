using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace BitCoinGame
{
    public class BinaryOptionGame : ConventionAggregate
    {
        //Persisted state
        public decimal TotalAmount { get; private set; }
        public decimal WinAmount { get; private set; }
        Bid CurrentBid { get; set; }
        public bool IsLost { get; private set; }
        public bool IsWon { get; private set; }
        public bool IsEnded { get; private set; }
        
        private class Bid
        {
            public Bid(Direction direction, decimal amount, decimal baseLevel, IPriceProvider provider)
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
                                 CurrentBid = new Bid(e.Dir, e.Amount, e.BaseLevel, provider);
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

        private async Task PlaceBid(Direction dir, decimal amount,IPriceProvider priceProvider)
        {
            if (IsEnded)
                throw new CannotBidOnEndedGameException();
            if (amount > TotalAmount)
                throw new NotEnoughMoneyException();
            if (CurrentBid != null)
                throw new BidAlreadyPlacedException();
            
            Produce(new BidPlaced(Id,dir,amount, await priceProvider.GetPrice()));
        }

        private async Task CheckBid(IPriceProvider priceProvider)
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
}