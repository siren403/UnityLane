using UnityEditor;
using UnityEngine;

namespace UnityLane.Editor.ConfigSandbox
{
    public static class ConfigSandbox
    {
        [MenuItem("UnityLane/Run")]
        public static void Run()
        {
            Bootstrap.Run("build-apk");
        }

        [MenuItem("UnityLane/Run - Command")]
        public static void RunCommand()
        {
            Bootstrap.Run("run-command");
        }
    }
}