using System.Reflection;
using System.Runtime.Loader;
using System;

namespace MiniTestRunner;


internal class Program
{
    static void Main(string[] args)
    {
        args = new string[]
{
    @"D:\P3Z_24Z_Project1\MiniTest\AuthenticationService.Tests\obj\Debug\net8.0\AuthenticationService.Tests.dll"
};

        if (args.Length == 0)
        {
            throw new ArgumentException("At least one DLL is required");
        }
        foreach(var dllPath in args)
        {
            var context = new AssemblyLoadContext(null, isCollectible: true);
            try
            {

                var assembly = context.LoadFromAssemblyPath(dllPath);
                var testDiscovery = TestDiscovery.DiscoveredTests(assembly);
                TestExecution.ExecuteTests(testDiscovery);
                
            }
            finally
            {
                context.Unload();

            }
        }

    }

}
