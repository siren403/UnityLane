using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityLane.Editor.ConfigSandbox
{
    public static class ConfigSandbox
    {
        [Serializable]
        public class Step
        {
            public string name;
            public string run;
            public Dictionary<string, object> args;
        }

        [Serializable]
        public class PlayerSettings
        {
            public string companyName;
            public string productName;
            public string packageName;
            public string version;
            public string[] architectures;
            public Dictionary<string, string> keystore;
        }

        [Serializable]
        public class Job
        {
            public string platform;
            public Dictionary<string, string> envs;
            public PlayerSettings playerSettings;
            public Step[] steps;
        }

        [Serializable]
        public class Workflow
        {
            public Dictionary<string, Job> jobs;
        }

        private static IDeserializer CreateDeserializer()
        {
            return new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
        }

        private static Workflow LoadWorkflow()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "workflow.yaml");
            using var reader = new StreamReader(path);
            return CreateDeserializer().Deserialize<Workflow>(reader.ReadToEnd());
        }


        [MenuItem("UnityLane/ConfigSandbox/Example1 - Load")]
        public static void Load()
        {
            var workflow = LoadWorkflow();
        }

     
        [MenuItem("UnityLane/Run")]
        public static void Run()
        {
            Env.Load();
            var envs = Env.Read();
            
        }

        public class AutoIncrementVersionCode
        {
            public AutoIncrementVersionCode(string a, int b)
            {
            }

            public void Execute()
            {
            }
        }
    }
}