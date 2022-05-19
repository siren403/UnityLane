using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VContainer;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Debug = UnityEngine.Debug;

namespace UnityLane.Editor
{
    [Serializable]
    public class BuildConfig
    {
        public Root Root;
    }

    [Serializable]
    public class Root
    {
        public Dictionary<string, int[]> Addresses;
        public string[] Commands;
    }
    
    public static class Sandbox
    {
        [MenuItem("Yaml/Example01")]
        public static void Yaml()
        {
            var asset = Resources.Load<TextAsset>("Example01");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<BuildConfig>(asset.text);
            
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