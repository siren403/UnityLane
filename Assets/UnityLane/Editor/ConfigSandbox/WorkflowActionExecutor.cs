using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityLane.Editor.ConfigSandbox
{
    public class WorkflowActionExecutor
    {
        private readonly Dictionary<string, Type> _actionTypes;

        public WorkflowActionExecutor()
        {
            _actionTypes = AppDomain.CurrentDomain.GetAssemblies()
                // .Where(_ => _.GetReferencedAssemblies().Any(assembly => assembly.Name == "UnityLane.Editor"))
                .Where(_ => _.GetName().Name == "UnityLane.Editor")
                .SelectMany(_ => { return _.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAction))); })
                .ToDictionary(_ => _.Name.PascalToKebabCase());
        }

        public void Execute(string name, Dictionary<string, string> with)
        {
            if (!_actionTypes.TryGetValue(name, out var type)) return;
            
        }
    }
}