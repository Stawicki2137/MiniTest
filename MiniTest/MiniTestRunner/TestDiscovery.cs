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
//zastanowic sie z ta niekomaptybilna konfiguracja 
public class DiscoveredTestMethod
{
    public MethodInfo TestMethod { get; set; }
    public int Priority { get; set; }
    public string? Description { get; set; }
    public object?[]? DataRow { get; set; } 

    public DiscoveredTestMethod(MethodInfo testMethod, int priority, object?[]? dataRow, string? description)
    {
       
        TestMethod = testMethod;
        Priority = priority;
        DataRow = dataRow;
        Description = description;
    }
}
public class DiscoveredTestClass
{
    public Type TestClass { get; set; }
    public Action? BeforeEach { get; set; }
    public Action? AfterEach { get; set; }
    public int Priority { get; set; }
    public string? Description { get; set; }
    public DiscoveredTestClass (Type testClass, Action? beforeEach, Action? afterEach, int priority, string? description)
    {
        TestClass = testClass;
        BeforeEach = beforeEach;
        AfterEach = afterEach;
        Priority = priority;
        Description = description;
    }
}
public static class TestDiscovery
{

    public static Dictionary<DiscoveredTestClass, List<DiscoveredTestMethod>> DiscoveredTests(Assembly assembly)
    {
        var discoveredTests = new Dictionary<DiscoveredTestClass,List<DiscoveredTestMethod>>();
        foreach (var type in assembly.GetTypes())
        {
            if(type.GetCustomAttributes<TestClassAttribute>() is null)
                continue;
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor is null)
            {
                Console.WriteLine($"Skipping {type.FullName}: no parameterless constructor");
                continue;
            }
            var testClassInstance = constructor.Invoke(null);
            var beforeEachMethod = type.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<BeforeEachAttribute>() is not null);
            Action? beforeEach = beforeEachMethod != null ? () => beforeEachMethod.Invoke(testClassInstance, null) : null;

            var afterEachMethod = type.GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<AfterEachAttribute>() is not null);
            Action? afterEach = afterEachMethod != null ? () => afterEachMethod.Invoke(testClassInstance, null) : null;

            var testClass = type;
            var classPriorityAttribute = type.GetCustomAttribute<PriorityAttribute>();
            int classPriority = classPriorityAttribute?.Priority ?? 0;
            var classDescriptionAttribute = type.GetCustomAttribute<DescriptionAttribute>();
            var classDescription = classDescriptionAttribute?.Description ?? null;
            // to bedzie klucz w slowniku
            DiscoveredTestClass discoveredTestClass = new DiscoveredTestClass(testClass, beforeEach, afterEach, classPriority, classDescription);
            //tu bede trzymal metody do przetestowania
            List<DiscoveredTestMethod> discoveredTestMethods = new List<DiscoveredTestMethod>(); 
            foreach(var method in testClass.GetMethods())
            {
                if(method.GetCustomAttribute<TestMethodAttribute>() is null)
                    continue;
                var testMethod = method;
                var methodPriorityAttribute = method.GetCustomAttribute<PriorityAttribute>();
                int methodPriority = methodPriorityAttribute?.Priority ?? 0;
                var dataRowAttributes = method.GetCustomAttributes<DataRowAttribute>();
                var dataRow = dataRowAttributes?.ToArray();
                var methodDescriptionAttribute = method.GetCustomAttribute<DescriptionAttribute>();
                var methodDescription = methodDescriptionAttribute?.Description ?? null;
                discoveredTestMethods.Add(new DiscoveredTestMethod(
                           testMethod,
                           methodPriority,
                           dataRow,
                           methodDescription
                           ));
                /*    if (dataRowAttributes.Any())
                    {
                        // jesli sa te atrybuty to dla kazdego zrob osobny test
                        foreach(var dataRow in dataRowAttributes)
                        {
                            discoveredTestMethods.Add(new DiscoveredTestMethod(
                                testMethod,
                                methodPriority,
                                dataRow.Data,
                                methodDescription
                                ));
                        }
                    }
                    else
                    {
                        //dodaje pojedyny test i elo
                        discoveredTestMethods.Add(new DiscoveredTestMethod(
                            testMethod,
                            methodPriority,
                            null,
                            methodDescription
                            ));
                    } */
            }
            discoveredTestMethods = discoveredTestMethods
                .OrderBy(testMethod => testMethod.Priority)
                .ThenBy(testMethod => testMethod.TestMethod.Name)
                .ToList();
     
           discoveredTests.Add(discoveredTestClass,discoveredTestMethods);

        }
        return discoveredTests
            .OrderBy(testClass => testClass.Key.Priority)
            .ThenBy(testClass => testClass.Key.TestClass.Name)
            .ToDictionary<DiscoveredTestClass, List<DiscoveredTestMethod>>();
    }

}



