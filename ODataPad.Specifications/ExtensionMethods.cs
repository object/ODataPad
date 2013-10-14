using System;
using TechTalk.SpecFlow;

namespace ODataPad.Specifications
{
    public static class ExtensionMethods
    {
        public static T GetOrAdd<T>(this ScenarioContext context, string key, Func<T> create)
        {
            T value;
            if (ScenarioContext.Current.ContainsKey(key))
            {
                value = (T)ScenarioContext.Current[key];
            }
            else
            {
                value = create();
                ScenarioContext.Current.Add(key, value);
            }
            return value;
        }
    }
}