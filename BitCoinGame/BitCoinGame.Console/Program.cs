using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Logging;
using Serilog;
using Serilog.Events;

namespace BitCoinGame.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration().WriteToFile(LogEventLevel.Warning).CreateLogger();

            using (var actorSystem = await new ActorSystemConfigBuilder()
                                           .Cluster("BitCoinGame")
                                           .AutoSeeds(1)
                                           .Build()
                                           .Log(s => logger)
                                           .OnClusterUp(async s =>
                                                        {
                                                            var ext = s.GetExtension<LoggingExtension>();
                                                            var node = new ClusterNodeBuilder()
                                                                       .ActorSystem(() => s)
                                                                       .DomainConfigurations(domainConfig)
                                                                       .Log(ext.Logger)
                                                                       .Build();
                                                            await node.Start();
                                                        })
                                           .CreateCluster())
            {
                using (var node = await new GridNodeBuilder().Log(logger)
                                                             .DomainConfigurations(new
                                                                                       BinaryOptionDomainConfiguration(new
                                                                                                                           RandomPriceProvider(1000,
                                                                                                                                               500)))
                                                             .ActorSystem(() => new ActorSystemConfigBuilder().)
                                                             .Build()
                                                             .Start())
                {

                    try
                    {
                        while (true)
                        {
                            Console.WriteLine("Welcome to Bitcoin binary option game");
                            Console.WriteLine("Press 'n' to start a new gate or 'e' to exit");
                            var command = Console.ReadKey();
                            if (command.KeyChar == 'e') break;
                            if (command.KeyChar == 'n')
                                await NewGameLoop(node);
                        }
                    }
                    finally
                    {
                        Console.ReadKey();
                    }
                }
            }
        }

        private static async Task NewGameLoop(ICommandExecutor executor)
        {
            Console.WriteLine("Awesome! input initial amount as number");
            var initialAmount = Decimal.Parse(Console.ReadLine());
            Console.WriteLine("Excellent! input win amount as number");
            var winAmount = Decimal.Parse(Console.ReadLine());
            Console.WriteLine("Launching your game...");
            
            await executor.Execute(new CreateNewGameCommand(initialAmount, winAmount));
            
            //Console.WriteLine("C your game...");

        }
    }


    class RandomPriceProvider : IPriceProvider
    {
        private readonly decimal _basePrice;
        private readonly decimal _delta;
        private Random _random;

        public RandomPriceProvider(decimal basePrice, decimal delta)
        {
            _delta = delta;
            _basePrice = basePrice;
            _random = new Random();
        }

        public Task<decimal> GetPrice()
        {
            return Task.FromResult(_basePrice + (decimal) (2 * (_random.NextDouble() - 0.5)) * _delta);
        }
    }
}