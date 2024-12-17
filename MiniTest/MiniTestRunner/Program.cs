using System.Reflection;
using System.Runtime.Loader;
using System;

namespace MiniTestRunner;


internal class Program
{
    static void Main(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            Console.Error.WriteLine("Error: No DLL paths provided.\nUsage: MiniTestRunner <path-to-test-assembly1.dll> <path-to-test-assembly2.dll> ...");
            Environment.Exit(1);
        }

        ProcessAssemblies(args);
    }
    private static void ProcessAssemblies(string[] assemblyPaths)
    {
        foreach (var assemblyPath in assemblyPaths)
        {
            if (!File.Exists(assemblyPath))
            {
                Console.Error.WriteLine($"Error: Assembly file not found at '{assemblyPath}'");
                continue;
            }
            var context = new AssemblyLoadContext(null, isCollectible: true);

            try
            {
                var assembly = context.LoadFromAssemblyPath(assemblyPath);
                Console.WriteLine($"\nRunning tests in '{assembly.FullName}'...");
                TestExecution.RunTests(assembly);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error running tests in '{assemblyPath}': {ex.Message}");
            }
            finally
            {
                context.Unload();
            }
        }
    }
        /*
         DO TESTOWANIA 
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
                    TestExecution.RunTests(assembly);

                }
                finally
                {
                    context.Unload();

                }
            }

        } */

    }
