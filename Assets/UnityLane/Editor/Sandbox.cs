using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using MessagePipe;
using MessagePipe.Interprocess;
using UnityEditor;
using VContainer;
using Debug = UnityEngine.Debug;

namespace UnityLane.Editor
{
    public static class Sandbox
    {
        private static IObjectResolver BuildResolver(string host, int port,
            System.Action<IServiceCollection, MessagePipeInterprocessOptions> configuration)
        {
            var builder = new ContainerBuilder();

            var options = builder.RegisterMessagePipe();

            var services = builder.AsServiceCollection();
            var interprocessOptions =
                services.AddMessagePipeTcpInterprocess(host, port, _ => { _.HostAsServer = false; });
            configuration(services, interprocessOptions);
            return builder.Build();
        }

        [MenuItem("UnityLane/PubSub")]
        public static async void PubSub()
        {
            var builder = new ContainerBuilder();

            var options = builder.RegisterMessagePipe();

            var services = builder.AsServiceCollection();
            var interprocessOptions = services.AddMessagePipeNamedPipeInterprocess("unitylane");
            services.RegisterNamedPipeInterprocessMessageBroker<string, string>(interprocessOptions);
            var resolver = builder.Build();

            var source = new UniTaskCompletionSource();
            try
            {
                Debug.Log("connect");
                var sub = await resolver.Resolve<IDistributedSubscriber<string, string>>()
                    .SubscribeAsync("project", _ =>
                    {
                        Debug.Log(_);
                        source.TrySetResult();
                    });


                await source.Task.Timeout(TimeSpan.FromSeconds(5));
                await sub.DisposeAsync();
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

        [MenuItem("UnityLane/Remote")]
        public static async void Remote()
        {
            using var resolver = BuildResolver("127.0.0.1", 3333,
                (services, options) =>
                {
                    services.RegisterTcpRemoteRequestHandler<string, string>(options);
                    services.RegisterTcpRemoteRequestHandler<string, int>(options);
                    services.RegisterTcpInterprocessMessageBroker<string, string>(options);
                });

            var handler = resolver.Resolve<IRemoteRequestHandler<string, string>>();
            Debug.Log(await handler.InvokeAsync("GooglePlay"));

            var ehandler = resolver.Resolve<IRemoteRequestHandler<string, int>>();

            var e = await ehandler.InvokeAsync("GooglePlay2222");
            Debug.Log(e);
        }

        [MenuItem("UnityLane/Shell")]
        public static void Shell()
        {
            var info = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "chsk",
                WindowStyle = ProcessWindowStyle.Hidden,
                // Arguments = "--version",
                Arguments = "--list-runtimes",
                RedirectStandardOutput = true
            };
            try
            {
                using var process = Process.Start(info);
                if (process == null) throw new Exception("process is null");

                // while (!process.StandardOutput.EndOfStream)
                // {
                //     Debug.Log(process.StandardOutput.ReadLine() + " ,");
                // }
                process.WaitForExit();

                Debug.Log(process.StandardOutput.ReadToEnd());
            }
            catch (Win32Exception e)
            {
                Debug.Log($"Execute Failed, {e}");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}