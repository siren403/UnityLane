using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityLane.Editor.ConfigSandbox
{
    [Serializable]
    public class Step
    {
        // github actions style
        public string uses;
        public Dictionary<string, object> with;

        // custom
        public string name;
        public string run;
        public Dictionary<string, object> args;
    }

    [Serializable]
    public class PlayerSettingsData
    {
        public string companyName;
        public string productName;
        public string packageName;
        public string version;

        //for android
        public AndroidArchitecture[] architectures;
        public AndroidKeystoreData keystore;
    }

    [Serializable]
    public class AndroidKeystoreData
    {
        public string path;
        public string passwd;
        public string alias;
        public string aliasPasswd;
    }


    [Serializable]
    public class Job
    {
        public string platform;
        public Dictionary<string, string> env;
        public PlayerSettingsData playerSettings;
        public Step[] steps;
    }

    [Serializable]
    public class Workflow
    {
        public Dictionary<string, Job> jobs;
    }

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
        }
    }

    public interface IAction
    {
        void Execute(WorkflowContext context);
    }

    public class AutoIncrementVersionCode : IAction
    {
        private readonly int _a;
        private readonly string _b;
        private readonly int _c;

        public AutoIncrementVersionCode(int a, string b, int c = 10)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public void Execute(WorkflowContext context)
        {
            Debug.Log($"{_a}, {_b}, {_c}");
        }
    }
}