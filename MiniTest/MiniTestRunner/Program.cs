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

    }
