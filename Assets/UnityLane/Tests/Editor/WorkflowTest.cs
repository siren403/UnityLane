using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityLane.Editor.ConfigSandbox;
using UnityLane.Editor.ConfigSandbox.Actions;

public class WorkflowTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void EmptyJobNameException()
    {
        var kebab = nameof(EmptyJobNameException).PascalToKebabCase();

        var factory = new WorkflowActionRunner();
    }

    [Test]
    public void Registrations()
    {
        var result = AppDomain.CurrentDomain.GetAssemblies()
            .Where(_ => _.GetName().Name == "UnityLane.Editor")
            .SelectMany(_ => { return _.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IRegistration))); })
            .ToArray();
    }

    [Test]
    public void ActionConstructorArgs()
    {
        var type = typeof(AutoIncrementVersionCode);
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .First();
        var instance = constructor.Invoke(new[] {1, "2", Type.Missing});
    }

    [Test]
    public void DictionaryFormatting()
    {
        var dic = new Dictionary<string, string>()
        {
            {"ENV1", "VALUE1"},
            {"ENV2", "VALUE2"},
            {"ENV3", "VALUE3"},
        };

        var str = "{ENV1} - {ENV2} - {ENV3}";
        var result = str.Format(dic);
    }
}