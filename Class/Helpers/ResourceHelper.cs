using System;
using System.Drawing;
using System.IO;
using System.Reflection;

using BrightnessSimulator.Class;

namespace BrightnessSimulator.Class.Helpers
{
    public static class ResourceHelper
    {
        public static Icon LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = GetResourceName(resourceName);

            using (var stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                    throw new ArgumentException($"{resourceName} not found.");

                return new Icon(stream);
            }
        }

        private static string GetResourceName(string resourceName)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            return $"{assemblyName}.Icons.{resourceName}";
        }
    }
}
