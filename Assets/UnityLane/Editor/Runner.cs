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
            #region Remote Request

            var builder = new ContainerBuilder();

            var options = builder.RegisterMessagePipe();

            var services = builder.AsServiceCollection();
            var tcpOptions = services.AddMessagePipeTcpInterprocess("127.0.0.1", 3216);
            services.RegisterTcpInterprocessMessageBroker<string, string>(tcpOptions);

            var namedOptions = services.AddMessagePipeNamedPipeInterprocess("unitylane");
            // services.RegisterTcpRemoteRequestHandler<string, string>(interprocessOptions);

            using var resolver = builder.Build();

            try
            {
                Debug.Log("connect");

                // var sub = await resolver.Resolve<IDistributedSubscriber<string, string>>()
                //     .SubscribeAsync("project", _ => { Debug.Log(_); });
                // await sub.DisposeAsync();

                using var sub = resolver.Resolve<ISubscriber<string, string>>()
                    .Subscribe("project", _ => { Debug.Log(_); });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                Debug.Log("disconnect");
            }

            #endregion
        }
    }
}