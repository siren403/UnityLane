// See https://aka.ms/new-console-template for more information

using MessagePipe;

Console.WriteLine("Hello, World!");

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
{
    services.AddMessagePipe();
    // services.AddMessagePipeTcpInterprocess("127.0.0.1", 3333);
    services.AddMessagePipeNamedPipeInterprocess("unitylane");
});

var app = builder.Build();
app.AddRootCommand(async (IDistributedSubscriber<string, string> subscriber) =>
{
    var source = new TaskCompletionSource();

    await subscriber.SubscribeAsync("project", _ =>
    {
        Console.WriteLine(_);
        source.TrySetResult();
    });

    await source.Task;
});
app.Run();