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
        private static string _eventHubConnectionString = "Endpoint=sb://bigdatahub.servicebus.windows.net/;SharedAccessKeyName=processor;SharedAccessKey=DkNixCV7QUfwAsL1STbUnov6LBEEOj5zGQmJJ+MLuwE=";
        private static string _eventHubPath = "bigdataeventhub";
        private static string _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=lotsofdata;AccountKey=QoKJgHZGujg4t9hU1QOsV1OyYRT9mlqpvdkm48dCECQ/EHKUsrXpof/g4DLdCLJLMgyrYcHkMCjE/QoNiuq+lw==";
        private static string _consumerGroupName = "bigdataconsumergroup";

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
