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

            var context = new PluginLoadContext(assemblyPath);

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

public class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
