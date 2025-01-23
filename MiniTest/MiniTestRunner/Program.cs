using System;
using System.IO;
using System.Reflection;

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

        // Obsługa zdarzenia AssemblyResolve
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

        foreach (var assemblyPath in args)
        {
            if (!File.Exists(assemblyPath))
            {
                Console.Error.WriteLine($"Error: Assembly file not found at '{assemblyPath}'");
                continue;
            }

            try
            {
                // Ładujemy główny zestaw
                var assembly = Assembly.LoadFrom(assemblyPath);
                Console.WriteLine($"\nRunning tests in '{assembly.FullName}'...");

                // Uruchamiamy testy
                TestExecution.RunTests(assembly);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error running tests in '{assemblyPath}': {ex.Message}");
            }
        }
    }

    private static Assembly? ResolveAssembly(object? sender, ResolveEventArgs args)
    {
        // Wyciągamy nazwę zestawu
        var assemblyName = new AssemblyName(args.Name);
        Console.WriteLine($"[AssemblyResolve] Resolving: {assemblyName.Name}");

        // Lokalizujemy ścieżkę do zestawu
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var potentialPath = Path.Combine(baseDirectory, assemblyName.Name + ".dll");

        // Jeśli plik istnieje, ładowanie zestawu
        if (File.Exists(potentialPath))
        {
            Console.WriteLine($"[AssemblyResolve] Found and loading: {potentialPath}");
            return Assembly.LoadFrom(potentialPath);
        }

        // Zwracamy null, jeśli nie znaleziono zestawu
        Console.WriteLine($"[AssemblyResolve] Not found: {assemblyName.Name}");
        return null;
    }
}
