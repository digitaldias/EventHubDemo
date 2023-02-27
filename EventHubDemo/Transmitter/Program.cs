using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Domain;
using Domain.Utilities;
using FizzWare.NBuilder;

namespace Transmitter
{
    class Program
    {
        private static string         _eventHubConnectionstring = "<Transmitter EventHub Connection String>";
        private static EventHubProducerClient _producer = default!;

        /// <summary>
        /// Transmits a value between 0 and 5000 to the Azure EventHub.
        /// The values are made by following a Sinus-curve for good looks, in case you want to connect them to a live PowerBI dashboard
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            var messages = CreateMessages(100);
            InitializeEventHubClient();
            var stopWatch = Stopwatch.StartNew();

            while (true)
            {

                stopWatch.Restart();
                SendAsBatch(messages);
                Console.WriteLine($"Transmitted 100 events as batch in {stopWatch.ElapsedMilliseconds}ms" );

                stopWatch.Restart();
                foreach (var message in messages)
                    SendAsIndividualMessages(message);

                Console.WriteLine($"Transmitted 100 events serially in {stopWatch.ElapsedMilliseconds}ms");
            }
        }

        private static void SendAsBatch(IEnumerable<RandomMessage> messages)
        {
            try
            {
                // Convert all messages to EventData objects
                var sendableMessages = messages.Select(m => new EventData(m.AsByteArray()));
                AsyncUtil.RunSync(() => _producer.SendAsync(sendableMessages));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"SEND AS BATCH FAILED: {ex.Message}");
                throw;
            }
        }

        private static void InitializeEventHubClient()
        {
            Trace.WriteLine("Initializing EventHub Client");
            try
            {
                _producer = new(_eventHubConnectionstring, "<EventHub name>");

                Trace.WriteLine("Initialization complete");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Initialization Failed: {ex.Message}\n\n{ex.StackTrace}");
                throw;
            }

        }

        private static void SendAsIndividualMessages(RandomMessage message)
        {
            var messageBytes = System.Text.Json.JsonSerializer.Serialize(message);
            var eventData = new EventData(new BinaryData(messageBytes));

            try
            {

                AsyncUtil.RunSync(() => _producer.SendAsync(new[] { eventData }));

            }
            catch (Exception ex)
            {
                Trace.WriteLine($"ERROR SENDING: {ex.Message}\n\n{ex.StackTrace}");
                throw;
            }
        }

        private static IEnumerable<RandomMessage> CreateMessages(int numberOfMessages)
        {
            if (numberOfMessages < 5)
                throw new ArgumentException("The minimum amount of messages is 5");

            var random = new RandomGenerator((int)DateTime.Now.Ticks);

            var participants = Builder<Participant>
                .CreateListOfSize(numberOfMessages)
                .All()
                    .With(o => o._id = Guid.NewGuid().ToString())
                    .With(o => o.IsActive = random.Boolean())
                    .With(o => o.Age = random.Next(18, 67))
                    .With(o => o.Company = Faker.Company.Name())
                    .With(o => o.About = Faker.Lorem.Sentence(15))
                    .With(o => o.Registered = random.Next(DateTime.Now.AddYears(-60), DateTime.Now.AddMonths(-3)).ToString())
                    .With(o => o.Tags = Faker.Lorem.Words(random.Next(1, 5)).ToArray())
                    .With(o => o.Greeting = Faker.Company.CatchPhrase())
                    .With(o => o.Email = Faker.Internet.Email())
                    .With(o => o.Phone = Faker.Phone.Number())
                    .With(o => o.Address = $"{Faker.Address.StreetAddress()}, {Faker.Address.ZipCode()} {Faker.Address.City()}")
                    .With(o => o.Friends = Builder<Friend>.CreateListOfSize(random.Next(1, 15))
                        .All()
                            .With(f => f.Id = random.Next(10000, 1000000))
                            .With(f => f.Name = Faker.Name.FullName())
                        .Build()
                        .ToArray())
                    .With(o => o.Name = Builder<Name>.CreateNew()
                        .With(n => n.First = Faker.Name.First())
                        .With(n => n.Last = Faker.Name.Last())
                        .Build())
                .Random(numberOfMessages/5)
                    .With(o => o.Email = "pedro@digitaldias.com")
                .Build();

            return participants.Select(participant => new RandomMessage { Participant = participant });
        }
    }
}
