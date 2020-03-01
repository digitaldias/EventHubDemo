using Domain;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Subscriber
{
    public class MyOwnEventProcessor : IEventProcessor
    {
        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            Trace.WriteLine("Lease Opened on Partition " + context.Lease.PartitionId + ", owned by " + context.Lease.Owner);
            return Task.FromResult<object>(null);
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            throw new NotImplementedException();
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var message in messages)
            {
                var importantMeasure = ImportantMeasure.FromByteArray(message.Body.Array);

                // Do something with the measure here, like storing it in a table somewhere...
                Trace.WriteLine($"[Partition {context.Lease.PartitionId}] {DateTime.Now.ToString("HH:mm:ss")}: {importantMeasure.ImportantValue.ToString("0.00")}");
            }
            await context.CheckpointAsync();
        }
    }
}