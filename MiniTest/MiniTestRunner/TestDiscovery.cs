using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MiniTest;
using System.Runtime.InteropServices;
using System.Data;

namespace MiniTestRunner;

public static class TestDiscovery
{
    public static IEnumerable<(Type TestClass, MethodInfo? BeforeEach, MethodInfo? AfterEach,
        IEnumerable<(MethodInfo Method, object[]? Data)> TestMethods)> DiscoverTests(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() is not null))
        {
            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                // to sie nie wypisze imo i tak ale warto sprobowac xD
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Warning] Skipping {type.FullName}: No parameterless constructor.");
                Console.ResetColor();
                continue;
            }
            var beforeEach = FindMethodWithAttribute<BeforeEachAttribute>(type);
            var afterEach = FindMethodWithAttribute<AfterEachAttribute>(type);
            var testMethods = new List<(MethodInfo Method, object[]? Data)>();

            foreach(var method in type.GetMethods()
                .Where(m => m.GetCustomAttribute<TestMethodAttribute>() is not null))
            {
                var dataRows = method.GetCustomAttributes<DataRowAttribute>();
                if (dataRows.Any())
                {
                    foreach(var row in dataRows)
                    {
                        if(method.GetParameters().Length != row.Data.Length)
                        {
                            Console.WriteLine(" i tak sie nie wypisze xd");
                            continue;
                        }
                        testMethods.Add((method,row.Data)!);
                    }
                }
                else if (!method.GetParameters().Any())
                {
                    testMethods.Add((method, null));
                }
                else
                {
                    Console.WriteLine("i tak sie nie wypisze");
                }
            }
            yield return (type,beforeEach,afterEach,testMethods);
        }
    }
    private static MethodInfo? FindMethodWithAttribute<T>(Type type)
     where T : Attribute => type.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<T>() is not null);
}



