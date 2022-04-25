using System.Reflection;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace UnityLaneRunner;

public static class Runner
{
    public static void Run<TLane>(string[] args) where TLane : LaneBase
    {
        var builder = ConsoleApp.CreateBuilder(args);
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(typeof(TLane));
            services.AddMessagePipe();
            services.AddMessagePipeTcpInterprocess("127.0.0.1", 3215, options => options.HostAsServer = true);
            services.AddAsyncRequestHandler<MyAsyncHandler<TLane>>();
        });

        var app = builder.Build();
        app.Services.GetService<IRemoteRequestHandler<string, string>>(); //Listen
        app.AddCommands<RunnerCommand<TLane>>();
        app.Run();
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class LaneAttribute : Attribute
{
}

public class RunnerCommand<TLane> : ConsoleAppBase where TLane : LaneBase
{
    public void List(TLane lane)
    {
        foreach (var laneName in lane.LaneNames)
        {
            Console.WriteLine(laneName);
        }
    }

    [RootCommand]
    public async Task Run(TLane lane, [Option("l")] string? laneName = null)
    {
        lane.SelectLane(laneName);
        // you can write infinite-loop while stop request(Ctrl+C or docker terminate).
        try
        {
            while (!this.Context.CancellationToken.IsCancellationRequested && !lane.IsExited)
            {
                try
                {
                    Console.Write(".");
                }
                catch (Exception ex)
                {
                    // error occured but continue to run(or terminate).
                    Console.WriteLine(ex);
                }

                // wait for next time
                await Task.Delay(TimeSpan.FromSeconds(1), this.Context.CancellationToken);
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            // you can write finally exception handling(without cancellation)
        }
        finally
        {
            // you can write cleanup code here.
        }
    }
}

public abstract class LaneBase
{
    public string SelectedName { get; private set; }
    public bool IsExited { get; private set; } = false;

    private readonly MethodInfo[] _laneMethods;

    public IEnumerable<string> LaneNames => _laneMethods.Select(method => method.Name);

    public LaneBase()
    {
        _laneMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(info => info.GetCustomAttribute<LaneAttribute>() != null)
            .ToArray();
    }

    public void SelectLane(string? laneName)
    {
        SelectedName = laneName ?? string.Empty;
    }

    public void Run(string? laneName = null)
    {
        var name = string.IsNullOrEmpty(laneName) ? SelectedName : laneName;
        try
        {
            var method = _laneMethods.First(m => m.Name.Equals(name));
            method.Invoke(this, null);
        }
        catch (Exception e)
        {
            Console.WriteLine($"not found lane: {name}");
            throw;
        }
    }

    public void Exit()
    {
        IsExited = true;
    }
}

public class MyAsyncHandler<TLane> : IAsyncRequestHandler<string, string> where TLane : LaneBase
{
    private readonly TLane _lane;

    public MyAsyncHandler(TLane lane)
    {
        _lane = lane;
    }

    public async ValueTask<string> InvokeAsync(string request, CancellationToken cancellationToken = new())
    {
        string status;
        try
        {
            _lane.Run(request);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            status = "success";
        }
        catch (Exception e)
        {
            status = "false";
        }

        _lane.Exit();
        return status;
    }
}