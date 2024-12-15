using System.Reflection;
using System.Runtime.Loader;
using System;

namespace MiniTestRunner;


internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("At least one DLL is required");
        }
        foreach(var dllPath in args)
        {
            Console.WriteLine($"loading {dllPath}");
            var context = new AssemblyLoadContext(null, isCollectible: true);
            try
            {

                var assembly = context.LoadFromAssemblyPath(dllPath);
                var testDiscovery = TestDiscovery.DiscoveredTests(assembly);
                //
            }
            finally
            {
                context.Unload();

            }
        }

    }

}
