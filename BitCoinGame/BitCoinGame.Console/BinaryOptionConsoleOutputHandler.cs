using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace BitCoinGame
{
    public class BinaryOptionConsoleOutputHandler: IHandler<GameCreated>, 
                                                   IHandler<GameWon>,
                                                   IHandler<GameLost>,
                                                   IHandler<BidPlaced>,
                                                   IHandler<BidWon>,
                                                   IHandler<BidLost>
                                                   
                                                   
    {
        public Task Handle(GameCreated message, IMessageMetadata metadata = null)
        {
            Console.WriteLine($"Started new binary option game {message.SourceId}");
            Console.WriteLine($"Initial amount is {message.InitialAmount}");
            Console.WriteLine($"You need to reach {message.WinAmount} to win");
            return Task.CompletedTask;
        }
        
        public Task Handle(GameWon message, IMessageMetadata metadata = null)
        {
            Console.WriteLine($"Congratulation! You won the game {message.SourceId}!");
            return Task.CompletedTask;
        }
        
        public Task Handle(GameLost message, IMessageMetadata metadata = null)
        {
            Console.WriteLine($"Ouch! Seems you lost the game {message.SourceId} 8( Try one more time!");
            return Task.CompletedTask;
        }
        
        public Task Handle(BidPlaced message, IMessageMetadata metadata = null)
        {
            Console.WriteLine($"You have placed a new bid: {message.Dir} with amount {message.Amount}. Bid was placed against price {message.BaseLevel}");
            return Task.CompletedTask;
        }

        public Task Handle(BidWon message, IMessageMetadata metadata = null)
        {
            Console.WriteLine($"You have won the bid for {message.Amount}!");
            return Task.CompletedTask;
        }

        public Task Handle(BidLost message, IMessageMetadata metadata = null)
        {
            Console.WriteLine($"You have lost the bid for {message.Amount} 8( ");
            return Task.CompletedTask;
        }
    }
}