using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityLane.Editor.ConfigSandbox
{
    public class WorkflowRunner
    {
        private static readonly Dictionary<string, (BuildTarget, BuildTargetGroup)> PlatformNameToTargets = new()
        {
            {"android", (BuildTarget.Android, BuildTargetGroup.Android)},
            {"ios", (BuildTarget.iOS, BuildTargetGroup.iOS)},
        };

        private readonly WorkflowArgumentView _argumentView;
        private readonly Workflow _workflow;
        private readonly WorkflowContext _context;

        public WorkflowRunner(Workflow workflow, WorkflowArgumentView argumentView)
        {
            _argumentView = argumentView;
            _workflow = workflow;
            _context = new WorkflowContext(argumentView);
        }

        public void Run()
        {
            var jobName = _argumentView.JobName;
            if (!_workflow.jobs.TryGetValue(jobName, out var job))
            {
                throw new Exception($"Not Found Job: {jobName}");
            }

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            if (!Application.isBatchMode)
            {
                //switch platform
                if (PlatformNameToTargets.TryGetValue(job.platform, out var targets))
                {
                    var isSuccess = EditorUserBuildSettings.SwitchActiveBuildTarget(targets.Item2, targets.Item1);
                    if (!isSuccess) throw new Exception("[UnityLane] SwitchPlatform Failed!");

                    buildTarget = targets.Item1;
                    buildTargetGroup = targets.Item2;
                }
            }

            //TODO: reset player settings
            if (job.playerSettings != null)
            {
                ApplyPlayerSettings(buildTargetGroup, job.playerSettings);
            }

            var executor = new WorkflowActionExecutor();

            foreach (var step in job.steps)
            {
                executor.Execute();
            }
        }

        private void ApplyPlayerSettings(BuildTargetGroup targetGroup, PlayerSettingsData settingsData)
        {
            //TODO: convert Actions
            if (!string.IsNullOrEmpty(settingsData.companyName))
            {
                PlayerSettings.companyName = settingsData.companyName;
            }

            if (!string.IsNullOrEmpty(settingsData.productName))
            {
                PlayerSettings.productName = settingsData.productName;
            }

            if (!string.IsNullOrEmpty(settingsData.version))
            {
                PlayerSettings.bundleVersion = settingsData.version;
            }

            // target platforms
            if (!string.IsNullOrEmpty(settingsData.packageName))
            {
                PlayerSettings.SetApplicationIdentifier(targetGroup, settingsData.packageName);
            }

            switch (targetGroup)
            {
                case BuildTargetGroup.Android:
                    PlayerSettings.Android.targetArchitectures =
                        settingsData.architectures.Aggregate((acc, current) => acc | current);

                    var keystoreData = settingsData.keystore;
                    if (keystoreData != null)
                    {
                        PlayerSettings.Android.useCustomKeystore = true;
                        PlayerSettings.Android.keystoreName = _argumentView.Format(keystoreData.path);
                        PlayerSettings.Android.keystorePass = _argumentView.Format(keystoreData.passwd);
                        PlayerSettings.Android.keyaliasName = _argumentView.Format(keystoreData.alias);
                        PlayerSettings.Android.keyaliasPass = _argumentView.Format(keystoreData.aliasPasswd);
                    }

                    break;
            }
        }
    }
}