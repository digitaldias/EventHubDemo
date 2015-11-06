using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Domain;

namespace Subscriber
{
    public class MyOwnEventProcessor : IEventProcessor
    {
        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
           if(reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("Context Lease Opened on Partition " + context.Lease.PartitionId + " owned by " + context.Lease.Owner);
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach(var message in messages)
            {                
                var importantMeasure = ImportantMeasure.FromByteArray(message.GetBytes());
                Console.WriteLine($"[Partition {context.Lease.PartitionId}] {DateTime.Now.ToString("HH:mm:ss")}: {importantMeasure.ImportantValue.ToString("0.00")}");
            }            
            await context.CheckpointAsync();
        }
    }
}