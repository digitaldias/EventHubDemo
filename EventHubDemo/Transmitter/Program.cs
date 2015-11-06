using Domain;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Diagnostics;

namespace Transmitter
{
    class Program
    {
        private static string _eventHubConnectionstring = "[YOUR EVENTHUB SENDER CONNECTION STRING HERE]";
        private static string _eventHubPath = "[NAME OF YOUR EVENTHUB]";
        private static int MAX_VALUE = 5000;

        private static EventHubClient _eventHubClient;

        static void Main(string[] args)
        {

            InitializeEventHubClient();
            var degree = 0;            

            while (true)
            {
                ImportantMeasure measure = CreateImportantMeasure(degree);
                SendImportantMeasureToEventHub(measure);

                Trace.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}: Transmitted [{measure.ImportantValue.ToString("0.00")}]");

                if (++degree >= 360)
                    degree = 0;
            }
        }

        private static void InitializeEventHubClient()
        {
            Trace.WriteLine("Initializing EventHub Client");
            try
            {
                _eventHubClient = EventHubClient.CreateFromConnectionString(_eventHubConnectionstring, _eventHubPath);
                Trace.WriteLine("Initialization complete");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Initialization Failed: {ex.Message}\n\n{ex.StackTrace.ToString()}");
                throw;
            }

        }

        private static void SendImportantMeasureToEventHub(ImportantMeasure measure)
        {
            // Convert to a byte[] for the transmission
            var eventData = new EventData(measure.AsByteArray());

            // Send it
            try
            {
                _eventHubClient.Send(eventData);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"ERROR SENDING: {ex.Message}\n\n{ex.StackTrace.ToString()}");
                throw;
            }
        }

        private static ImportantMeasure CreateImportantMeasure(int degree)
        {
            var radian = Math.PI / 180 * degree;
            var newValue = MAX_VALUE * Math.Sin(radian);
            var measure = new ImportantMeasure { ImportantValue = newValue };

            return measure;
        }
    }
}
