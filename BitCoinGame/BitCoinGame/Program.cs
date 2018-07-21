using System;
using System.Threading;
using Akka.Actor;
using Akka.Remote.Transport;
using GridDomain.Node;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace BitCoinGame
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var node = new GridDomainNode(() => ActorSystem.Create("test"));
            
            
            Console.ReadKey();
        }
    }
}