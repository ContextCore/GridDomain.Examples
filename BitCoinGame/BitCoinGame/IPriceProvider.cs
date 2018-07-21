using System.Threading.Tasks;

namespace BitCoinGame
{
    public interface IPriceProvider
    {
        Task<decimal> GetPrice();
    }
}