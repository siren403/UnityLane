using System;
using System.Collections.Generic;

namespace UnityLane.Editor.ConfigSandbox
{
    public class WorkflowArgumentView
    {
        private const string KeySelectedJob = "UL_JOB";

        public WorkflowArgumentView(Dictionary<string, string> args)
        {
            _args = args;
        }

        private readonly Dictionary<string, string> _args;

        public string JobName
        {
            get
            {
                if (_args.TryGetValue(KeySelectedJob, out var value))
                {
                    return value;
                }

                throw new Exception($"Not Found Argument: {KeySelectedJob}");
            }
            set => _args[KeySelectedJob] = value;
        }

        public string Format(string format)
        {
            return format.Format(_args);
        }
    }
}