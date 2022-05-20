using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityLane.Editor.ConfigSandbox
{
    public class WorkflowActionExecutor
    {
        private readonly Dictionary<string, Type> _actionTypes;

        public WorkflowActionExecutor()
        {
            _actionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => _.GetName().Name == "UnityLane.Editor")
                .SelectMany(_ => { return _.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAction))); })
                .ToDictionary(_ => _.Name.PascalToKebabCase());
        }

        public void Execute(WorkflowContext context, string name, Dictionary<string, object> with)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (!_actionTypes.TryGetValue(name, out var type)) return;

            try
            {
                var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                var withKeys = with.Keys.ToArray();
                var withTypes = with.Select(_ => _.Value.GetType()).ToArray();

                var matchedConstructor = constructors.First(_ =>
                {
                    var param = _.GetParameters();
                    
                    var requireParamCount = param.Aggregate(0, (acc, info) => !info.HasDefaultValue ? acc + 1 : acc);
                    
                    if (withTypes.Length != requireParamCount) return false;

                    for (int i = 0; i < withTypes.Length; i++)
                    {
                        if (withTypes[i] != param[i].ParameterType) return false;
                        if (withKeys[i] != param[i].Name) return false;
                    }

                    return true;
                });

                var instance = matchedConstructor.Invoke(with.Values.ToArray()) as IAction;
                instance?.Execute(context);
            }
            catch (Exception e)
            {
                Debug.Log("failed! find constructor");
                Debug.Log(e);
            }
        }
    }
}