using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace IRCKickBot
{
    class Plugin
    {
        private List<MethodInfo> pluginMethods = new List<MethodInfo>();
        private Assembly loadedAssembly;

        public Plugin(string assemblyPath)
        {
            loadedAssembly = Assembly.LoadFile(assemblyPath);
        }

        public void LoadMethod(string typeName, string methodName)
        {
            MethodInfo pluginMethod = loadedAssembly.GetType(typeName).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (pluginMethod == null)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Plugin method loading: method {0} in type {1} not found.", methodName, typeName));
            }
            ParameterInfo[] parameters = pluginMethod.GetParameters();
            if (parameters.Length != 1)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Plugin method loading: method {0} in type {1} does not take exactly one argument.", methodName, typeName));
            }
            if (parameters[0].ParameterType != typeof(string))
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Plugin method loading: method {0} in type {1} does not take a String as argument.", methodName, typeName));
            }
            if (pluginMethod.ReturnType != typeof(string))
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Plugin method loading: method {0} in type {1} does not return a String.", methodName, typeName));
            }
            pluginMethods.Add(pluginMethod);
        }

        public string ShouldKick(string input)
        {
            foreach (MethodInfo method in pluginMethods)
            {
                string returnValue = (string)method.Invoke(null, new object[] { input });
                if (returnValue != null)
                {
                    return returnValue;
                }
            }
            return null;
        }
    }
}
