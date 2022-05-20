using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnityLane.Editor.ConfigSandbox
{
    public class WorkflowRunnerBuilder
    {
        private static IDeserializer CreateDeserializer()
        {
            return new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
        }

        private static Workflow LoadWorkflow(string filePath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            using var reader = new StreamReader(path);
            return CreateDeserializer().Deserialize<Workflow>(reader.ReadToEnd());
        }

        private string _filePath = "workflow.yaml";
        private bool _enableLoadEnv;
        private string _jobName;

        public WorkflowRunnerBuilder LoadEnvironmentVariables()
        {
            _enableLoadEnv = true;
            return this;
        }

        public WorkflowRunnerBuilder SetWorkflowFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new NullReferenceException(nameof(filePath));
            }

            _filePath = filePath;
            return this;
        }

        public WorkflowRunnerBuilder SetJobName(string jobName)
        {
            _jobName = jobName;
            return this;
        }

        public WorkflowRunner Build()
        {
            Dictionary<string, string> envs = null;
            if (_enableLoadEnv)
            {
                envs = DotEnv.Fluent().Copy();
            }

            var argumentView = new WorkflowArgumentView(envs);
            if (!string.IsNullOrEmpty(_jobName))
            {
                argumentView.JobName = _jobName;
            }

            var workflow = LoadWorkflow(_filePath);

            return new WorkflowRunner(workflow, argumentView);
        }
    }
}