using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Diagnostics;

namespace Subscriber
{
    class Program
    {
        private static string _eventHubConnectionString = "<Reader Connection String>";
        private static string _eventHubPath             = "<Name of your eventhub>";
        private static string _storageConnectionString  = "<Your  storage connection string>";
        private static string _containerName            = "<Container name for your evenhub>";
        private static string _consumerGroupName        = "$Default"; // Essentially a "cursor" in database terms

        private static EventProcessorHost _eventProcessorHost;

        static void Main(string[] args)
        {
            InitializeEventProcessorHost();

            _eventProcessorHost.RegisterEventProcessorAsync<MyOwnEventProcessor>().Wait();
#if DEBUG
            Trace.WriteLine("Press any key to end the program");
            Console.ReadKey();
#endif
        }

        private static void InitializeEventProcessorHost()
        {
            Trace.WriteLine("Initializing EventProcessor Host");
            string hostName = CreateUniqueHostname();

            try
            {
                _eventProcessorHost = new EventProcessorHost(_eventHubPath, _consumerGroupName,_eventHubConnectionString, _storageConnectionString, _containerName);
                Trace.WriteLine($"{hostName} created successfully.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Host creation failed: {ex.Message}\n\n{ex.StackTrace.ToString()}");
                throw;
            }
        }

        private static string CreateUniqueHostname()
        {
            return "HOST_" + Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
