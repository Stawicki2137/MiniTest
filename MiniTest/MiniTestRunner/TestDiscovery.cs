using MiniTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniTestRunner;

public static class TestDiscovery
{
    public static List<string> Warnings { get; private set; } = new();

    public static IEnumerable<(Type TestClass, Action<object>? BeforeEach, Action<object>? AfterEach,
        IEnumerable<(MethodInfo Method, object[]? Data)> TestMethods)> DiscoverTests(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() is not null))
        {
            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                Warnings.Add($"[Warning] Skipping {type.FullName}: No parameterless constructor.");
                continue;
            }

            Action<object>? beforeEach = CreateInstanceActionForAttribute<BeforeEachAttribute>(type);
            Action<object>? afterEach = CreateInstanceActionForAttribute<AfterEachAttribute>(type);

            var testMethods = new List<(MethodInfo Method, object[]? Data)>();

            foreach (var method in type.GetMethods()
                .Where(m => m.GetCustomAttribute<TestMethodAttribute>() is not null))
            {
                var dataRows = method.GetCustomAttributes<DataRowAttribute>();
                if (dataRows.Any())
                {
                    foreach (var row in dataRows)
                    {
                        if (method.GetParameters().Length != row.Data.Length)
                        {
                            Warnings.Add($"[Warning] Method {method.Name} has invalid DataRow parameters.");
                            continue;
                        }
                        testMethods.Add((method, row.Data)!);
                    }
                }
                else if (!method.GetParameters().Any())
                {
                    testMethods.Add((method, null));
                }
                else
                {
                    Warnings.Add($"[Warning] Method {method.Name} requires parameters but has no DataRow.");
                }
            }
            yield return (type, beforeEach, afterEach, testMethods);
        }
    }

    private static Action<object>? CreateInstanceActionForAttribute<T>(Type type) where T : Attribute
    {
        var method = type.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<T>() is not null);
        if (method != null)
        {
            return instance => method.Invoke(instance, null);
        }
        return null;
    }
}
