using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    class Program
    {
        private static string _eventHubConnectionString = "[YOUR EVENT HUB READER CONNECTIONTRING]";
        private static string _eventHubPath = "[NAME OF YOUR EVENTHUB]";
        private static string _storageConnectionString = "[YOUR AZURE STORAGE CONNECTION STRING]";
        private static string _consumerGroupName = "[NAME OF THE CONSUMER GROUP]";

        private static EventProcessorHost _host;

        static void Main(string[] args)
        {
            InitializeEventProcessorHost();

            _host.RegisterEventProcessorAsync<MyOwnEventProcessor>().Wait();

            Console.WriteLine("Press any key to end the program");
            Console.ReadKey();
        }

        private static void InitializeEventProcessorHost()
        {
            Console.WriteLine("Initializing EventProcessor Host");

            string hostName = CreateUniqueHostname();

            try
            {
                _host = new EventProcessorHost(hostName, _eventHubPath, _consumerGroupName, _eventHubConnectionString, _storageConnectionString);
                Console.WriteLine($"{hostName} created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Host creation failed: {ex.Message}\n\n{ex.StackTrace.ToString()}");
                throw;
            }
        }

        private static string CreateUniqueHostname()
        {
            return "HOST_" + Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
