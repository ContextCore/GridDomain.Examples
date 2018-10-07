﻿using System;
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
        
        internal BinaryOptionGame(string id, IPriceProvider provider) : base(id)
        {
            Apply<GameCreated>(e =>
                               {
                                   TotalAmount = e.InitialAmount;
                                   WinAmount = e.WinAmount;
                                   IsWon = false;
                                   IsEnded = false;
                                   IsLost = false;
                               });
            Apply<BidPlaced>(e =>
                             {
                                 CurrentBid = new Bid(e.Dir, e.Amount, e.BaseLevel, provider);
                                 TotalAmount -= e.Amount;
                             });
            Apply<BidWon>(e => TotalAmount += e.Amount);
            Apply<BidLost>(e => {});
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
            
            Execute<CreateNewGameCommand>(c => Emit(new GameCreated(c.AggregateId, c.StartAmount,c.WinAmount)));
            Execute<PlaceBidCommand>(async c => await PlaceBid(c.Direction,c.Amount,provider));
            Execute<CheckBidCommand>(async c => await CheckBid(provider));
        }

        public BinaryOptionGame(string gameId, decimal initialAmount, decimal winAmount, IPriceProvider provider):this(gameId, provider)
        {
            Emit(new GameCreated(Id,initialAmount, winAmount));
        }

        public async Task PlaceBid(Direction dir, decimal amount,IPriceProvider priceProvider)
        {
            if (IsEnded)
                throw new CannotBidOnEndedGameException();
           
            if (CurrentBid != null)
                throw new BidAlreadyPlacedException();

            if (amount > TotalAmount)
                throw new NotEnoughMoneyException();
            
            Emit(new BidPlaced(Id,dir,amount, await priceProvider.GetPrice()));
        }

        private async Task CheckBid(IPriceProvider priceProvider)
        {
            if (IsEnded)
                throw new CannotCheckBidOnEndedGameException();
            if (CurrentBid == null)
                throw new NoActiveBidException();
            if (CurrentBid.IsWon(await priceProvider.GetPrice()))
                Emit(new BidWon(Id, CurrentBid.Amount * 2));
            else
                Emit(new BidLost(Id, CurrentBid.Amount));
            
            if(TotalAmount <=0)
                Emit(new GameLost(Id));
            if (TotalAmount >= WinAmount)
                Emit(new GameWon(Id));
        }
    }
}