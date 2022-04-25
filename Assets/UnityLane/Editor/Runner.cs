using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEditor;
using UnityEngine;
using VContainer;

namespace UnityLane.Editor
{
    public static class Runner
    {
        [MenuItem("UnityLane/Run")]
        public static async void Run()
        {
            var builder = new ContainerBuilder();

            var options = builder.RegisterMessagePipe();

            var services = builder.AsServiceCollection();
            var interprocessOptions = services.AddMessagePipeTcpInterprocess("127.0.0.1", 3215);

            // services.RegisterTcpInterprocessMessageBroker<int, string>(interprocessOptions);
            services.RegisterTcpRemoteRequestHandler<string, string>(interprocessOptions);

            using var resolver = builder.Build();

            try
            {
                Debug.Log("connect");
                
                // var publisher = resolver.Resolve<IDistributedPublisher<int, string>>();
                // await publisher.PublishAsync(0, $"{nameof(IDistributedPublisher<int, string>)} / from unity");

                var request = resolver.Resolve<IRemoteRequestHandler<string, string>>();
                // var result = await request.InvokeAsync($"{nameof(IRemoteRequestHandler<string, string>)} / From Unity").Timeout(TimeSpan.FromSeconds(10));
                var result = await request.InvokeAsync($"GooglePlay").Timeout(TimeSpan.FromSeconds(10));
                Debug.Log($"result: {result}");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                Debug.Log("disconnect");
            }
        }
    }
}