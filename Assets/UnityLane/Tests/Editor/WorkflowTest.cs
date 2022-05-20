using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityLane.Editor.ConfigSandbox;

public class WorkflowTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void EmptyJobNameException()
    {
        var kebab = nameof(EmptyJobNameException).PascalToKebabCase();

        var factory = new WorkflowActionExecutor();
        
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