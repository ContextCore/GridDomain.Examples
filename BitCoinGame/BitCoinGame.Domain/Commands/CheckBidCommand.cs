using GridDomain.CQRS;

namespace BitCoinGame
{
    public class CheckBidCommand : Command<BinaryOptionGame>
    {
        public CheckBidCommand(string gameId):base(gameId)
        {
        }
    }
}