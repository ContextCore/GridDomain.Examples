using System;
using GridDomain.CQRS;

namespace BitCoinGame
{
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
}