using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Domain;

namespace Subscriber;

class Program
{
    private static readonly string _eventHubConnectionString = "<Reader Connection String>";
    private static readonly string _storageConnectionString  = "<Your  storage connection string>";
    private static readonly string _containerName            = "<Container name for your EvenHub>";
    private static readonly string _consumerGroupName        = "$Default"; // Essentially a "cursor" in database terms

    private static EventProcessorClient _eventProcessorClient = default!;

    static void Main(string[] args)
    {
        InitializeEventProcessorHost();

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
            var storageClient = new BlobContainerClient(_storageConnectionString, _containerName);
            _eventProcessorClient = new EventProcessorClient(storageClient, _consumerGroupName, _eventHubConnectionString);
            _eventProcessorClient.PartitionInitializingAsync += HandlePartitionInitializingAsync;
            _eventProcessorClient.PartitionClosingAsync += HandlePartitionClosingAsync;
            _eventProcessorClient.ProcessEventAsync += HandleProcessEventAsync;
            _eventProcessorClient.ProcessErrorAsync += HandleProcessErrorAsync;

            Trace.WriteLine($"{hostName} created successfully.");
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Host creation failed: {ex.Message}\n\n{ex.StackTrace}");
            throw;
        }
    }

    private static async Task HandlePartitionClosingAsync(PartitionClosingEventArgs arg)
    {
        Console.WriteLine($"Lease closing on partition {arg.PartitionId}: {arg.Reason}");
        await Task.CompletedTask;
    }

    private static async Task HandlePartitionInitializingAsync(PartitionInitializingEventArgs arg)
    {
        Console.WriteLine($"Lease opened on partition {arg.PartitionId}");
        await Task.CompletedTask;
    }

    private static async Task HandleProcessErrorAsync(ProcessErrorEventArgs eventArgs)
    {
        Console.WriteLine($"Partition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(eventArgs.Exception.Message);
        await Task.CompletedTask;
    }

    private static async Task HandleProcessEventAsync(ProcessEventArgs eventArgs)
    {
        var msgBody = eventArgs.Data.EventBody.ToString();
        var randomMessage = JsonSerializer.Deserialize<RandomMessage>(msgBody);
        await Console.Out.WriteLineAsync($"""
                [Partition {eventArgs.Partition}] from {randomMessage?.Participant?.Email}
                {msgBody}
                """);
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    private static string CreateUniqueHostname()
    {
        return string.Concat("HOST_", Guid.NewGuid().ToString().AsSpan(0, 8));
    }
}
