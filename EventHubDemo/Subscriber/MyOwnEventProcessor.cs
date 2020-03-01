using Domain;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Subscriber
{
    public class MyOwnEventProcessor : IEventProcessor
    {
        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var stopwatch = Stopwatch.StartNew();
            foreach (var message in messages)
            {
                var deserializedMessage = RandomMessage.FromByteArray(message.Body.Array);

                // Do something with the measure here, like storing it in a table somewhere...
                if (deserializedMessage.Participant.email == "pedro@digitaldias.com")
                {
                    Console.WriteLine($"[Partition {context.Lease.PartitionId}] {DateTime.Now.ToString("HH:mm:ss")}: --- {deserializedMessage.Participant._id} from {deserializedMessage.Participant.email.ToUpper()} ---");
                }
                else
                {
                    Console.WriteLine($"[Partition {context.Lease.PartitionId}] {DateTime.Now.ToString("HH:mm:ss")}: {deserializedMessage.Participant._id} from {deserializedMessage.Participant.email}");
                }
            }
            Console.WriteLine($">>>> {messages.Count()} processed in {stopwatch.ElapsedMilliseconds}ms <<<<");
            await context.CheckpointAsync();
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("Lease Opened on Partition " + context.Lease.PartitionId + ", owned by " + context.Lease.Owner);
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"ERROR: {error.Message} - EventHubPath: '{context.EventHubPath}', Partition: {context.Lease.PartitionId}");
            return Task.CompletedTask;
        }

    }
}