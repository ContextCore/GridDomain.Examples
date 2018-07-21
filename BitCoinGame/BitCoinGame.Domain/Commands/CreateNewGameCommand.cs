using System;
using GridDomain.CQRS;

namespace BitCoinGame
{
    public class CreateNewGameCommand : Command<BinaryOptionGame>
    {
        public decimal StartAmount { get; }
        public decimal WinAmount { get; }

        public CreateNewGameCommand(decimal startAmount, decimal winAmount):base("bopg_"+Guid.NewGuid())
        {
            StartAmount = startAmount;
            WinAmount = winAmount;
        }
    }
}