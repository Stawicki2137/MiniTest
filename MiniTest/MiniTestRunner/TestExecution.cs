using MiniTest;
using System.Reflection;

namespace MiniTestRunner;
public static class TestExecution
{
    public static void RunTests(Assembly assembly)
    {
        var testClasses = TestDiscovery.DiscoverTests(assembly)
            .OrderBy(t => t.TestClass.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0)
            .ThenBy(t => t.TestClass.Name)
            .ToList();

        if (TestDiscovery.Warnings.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warnings detected:");
            foreach (var warning in TestDiscovery.Warnings)
            {
                Console.WriteLine(warning);
            }
            Console.ResetColor();
        }

        Counter totalCounter = new Counter();
        foreach (var (testClass, beforeEach, afterEach, testMethods) in testClasses)
        {
            var classDescription = testClass.GetCustomAttribute<DescriptionAttribute>()?.Description;
            Console.WriteLine($"Running test from class {testClass.FullName}...");
            if (!string.IsNullOrEmpty(classDescription))
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"Description: {classDescription}");
                Console.ResetColor();
            }

            Counter classCounter = new Counter();
            var testInstance = Activator.CreateInstance(testClass);
            var groupMethods = testMethods
                .GroupBy(m => m.Method)
                .OrderBy(m => m.Key.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0)
                .ThenBy(m => m.Key.Name);

            foreach (var group in groupMethods)
            {
                var method = group.Key;
                var methodDescription = method.GetCustomAttribute<DescriptionAttribute>()?.Description;

                Console.WriteLine(method.Name);
                foreach (var (testMethod, data) in group)
                {
                    try
                    {
                        beforeEach?.Invoke(testInstance!); 
                        if (data != null)
                        {
                            RunParameterizedTest(testMethod, testInstance, data, ref classCounter);
                        }
                        else
                        {
                            RunSimpleTest(testMethod, testInstance, ref classCounter);
                        }
                    }
                    catch (Exception ex)
                    {
                        PrintTestResult($"{string.Join(", ", data ?? new object[] { })}", false, ex.InnerException?.Message ?? ex.Message);
                        classCounter.failed++;
                    }
                    finally
                    {
                        afterEach?.Invoke(testInstance!); 
                    }
                }

                if (!string.IsNullOrEmpty(methodDescription))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(methodDescription);
                    Console.ResetColor();
                }
            }

            totalCounter.passed += classCounter.passed;
            totalCounter.failed += classCounter.failed;
            PrintClassSummary(classCounter);
        }

        PrintGlobalSummary(totalCounter, assembly);
    }

    private static void PrintClassSummary(Counter counter)
    {
        Console.WriteLine(new string('*', 30));
        Console.WriteLine($"* Test passed: {counter.passed,6} / {counter.total,-4} *");
        Console.WriteLine($"* Failed: {counter.failed,11} {"*",30 - 11 - 11}");
        Console.WriteLine(new string('*', 30));
        Console.WriteLine(new string('#', 80));
    }

    private static void PrintGlobalSummary(Counter counter, Assembly assembly)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"Summary of running tests from {assembly.GetName().Name}:");
        Console.WriteLine(new string('*', 30));
        Console.WriteLine($"* Test passed: {counter.passed,6} / {counter.total,-4} *");
        Console.WriteLine($"* Failed: {counter.failed,11} {"*",30 - 11 - 11}");
        Console.WriteLine(new string('*', 30));
        Console.ResetColor();
    }

    private static void RunSimpleTest(MethodInfo method, object? instance, ref Counter counter)
    {
        try
        {
            method.Invoke(instance, null);
            PrintTestResult("No data", true);
            counter.passed++;
        }
        catch (Exception ex)
        {
            PrintTestResult("No data", false, ex.InnerException?.Message ?? ex.Message);
            counter.failed++;
        }
    }

    private static void RunParameterizedTest(MethodInfo method, object? instance, object[] data, ref Counter counter)
    {
        try
        {
            method.Invoke(instance, data);
            PrintTestResult($"{string.Join(", ", data)}", true);
            counter.passed++;
        }
        catch (Exception ex)
        {
            PrintTestResult($"{string.Join(", ", data)}", false, ex.InnerException?.Message ?? ex.Message);
            counter.failed++;
        }
    }

    private static void PrintTestResult(string testData, bool isPassed, string? errorMessage = null)
    {
        Console.ForegroundColor = isPassed ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($" - {testData.PadRight(60)}: {(isPassed ? "PASSED" : "FAILED")}");
        if (!string.IsNullOrEmpty(errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   {errorMessage}");
        }
        Console.ResetColor();
    }
}

public struct Counter
{
    public int passed;
    public int failed;
    public int total => passed + failed;
}
