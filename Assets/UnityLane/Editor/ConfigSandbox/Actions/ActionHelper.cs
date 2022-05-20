using System.Collections.Generic;

namespace UnityLane.Editor.ConfigSandbox.Actions
{
    public static class ActionHelper
    {
        public static bool TryGetIsValue<T>(this Dictionary<string, object> dictionary, string key, out T value)
        {
            value = default;
            if (dictionary.TryGetValue(key, out var obj) &&
                obj is T matched)
            {
                value = matched;
            }

            return value != null;
        }
    }
}