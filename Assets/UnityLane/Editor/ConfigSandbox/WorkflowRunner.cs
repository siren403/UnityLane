using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityLane.Editor.ConfigSandbox
{
    [Flags]
    public enum TargetPlatform
    {
        None = 0,
        Android = 1 << 0,
        iOS = 1 << 1,
        All = Int32.MaxValue,
    }

    public class WorkflowRunner
    {
        private static readonly Dictionary<string, (BuildTarget, BuildTargetGroup, TargetPlatform)>
            PlatformNameToTargets = new()
            {
                {"android", (BuildTarget.Android, BuildTargetGroup.Android, TargetPlatform.Android)},
                {"ios", (BuildTarget.iOS, BuildTargetGroup.iOS, TargetPlatform.iOS)},
            };

        private readonly WorkflowArgumentView _argumentView;
        private readonly WorkflowActionRunner _actionRunner;
        private readonly Workflow _workflow;
        private WorkflowContext _context;

        public WorkflowRunner(Workflow workflow, WorkflowArgumentView argumentView,
            WorkflowActionRunner actionRunner)
        {
            _argumentView = argumentView;
            _actionRunner = actionRunner;
            _workflow = workflow;
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

            if (!PlatformNameToTargets.TryGetValue(job.platform, out var targets))
            {
                throw new Exception($"not found platform: {job.platform}");
            }

            if (!Application.isBatchMode)
            {
                //switch platform
                var isSuccess = EditorUserBuildSettings.SwitchActiveBuildTarget(targets.Item2, targets.Item1);
                if (!isSuccess) throw new Exception("[UnityLane] SwitchPlatform Failed!");

                buildTarget = targets.Item1;
                buildTargetGroup = targets.Item2;
            }

            _context = new WorkflowContext(_argumentView, targets.Item3);

            foreach (var step in job.steps)
            {
                _actionRunner.Run(_context, step.uses, step.with);
            }
        }
    }
}