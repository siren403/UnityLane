using UnityEditor;
using UnityEngine;

namespace UnityLane.Editor.ConfigSandbox
{
    public static class ConfigSandbox
    {
        [MenuItem("UnityLane/Run")]
        public static void Run()
        {
            var runner = new WorkflowRunnerBuilder()
                .LoadEnvironmentVariables()
                .SetJobName("build-apk")
                .Build();
            runner.Run();
            Debug.Log("Success");
        }
    }
}