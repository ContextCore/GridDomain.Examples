using System.Threading.Tasks;
using Info.Blockchain.API.Client;

namespace BitCoinGame
{
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
}