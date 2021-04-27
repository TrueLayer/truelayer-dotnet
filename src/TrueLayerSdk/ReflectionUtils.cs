using System.Linq;
using System.Reflection;

namespace TrueLayerSdk
{
    internal static class ReflectionUtils
    {
        public static string GetAssemblyVersion<T>() 
        {
            Assembly containingAssembly = typeof(T).Assembly;

            return containingAssembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?
                .InformationalVersion;
        }
    }
}
