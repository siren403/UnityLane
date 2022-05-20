using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace UnityLane.Editor.ConfigSandbox.Actions
{
    public class PlayerSettingsAndroid : IAction
    {
        public class Registration : IRegistration
        {
            public void Register(DeserializerBuilder builder)
            {
                builder.WithTagMapping(new TagName("!keystore"), typeof(Keystore));
                builder.WithTagMapping(new TagName("!architectures"), typeof(AndroidArchitecture[]));
            }
        }

        [Serializable]
        public class Keystore
        {
            public string path;
            public string passwd;
            public string alias;
            public string aliasPasswd;
        }

        public TargetPlatform Targets => TargetPlatform.Android;

        private readonly Dictionary<string, object> _with;

        public PlayerSettingsAndroid(Dictionary<string, object> with)
        {
            _with = with;
        }


        public void Execute(WorkflowContext context)
        {
            if (_with.TryGetIsValue("architectures", out AndroidArchitecture[] architectures))
            {
                PlayerSettings.Android.targetArchitectures = architectures
                    .Aggregate((acc, current) => acc | current);
            }

            if (_with.TryGetIsValue("keystore", out Keystore keystore))
            {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = context.Format(keystore.path);
                PlayerSettings.Android.keystorePass = context.Format(keystore.passwd);
                PlayerSettings.Android.keyaliasName = context.Format(keystore.alias);
                PlayerSettings.Android.keyaliasPass = context.Format(keystore.aliasPasswd);
            }
            else
            {
                PlayerSettings.Android.useCustomKeystore = false;
            }
        }
    }
}