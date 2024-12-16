using MiniTest;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestRunner;

public struct Counter
{
    public int passed;
    public int failed;
    public int total => passed + failed;
}
public static class TestExecution
{
    public static void ExecuteTests(Dictionary<DiscoveredTestClass,List<DiscoveredTestMethod>> discoveredTests)
    {
         Counter totalCount = new Counter();
        foreach (var testClass in  discoveredTests)
        {
            Counter classCount = new Counter();
            if (!testClass.Key.TestClass.Name.Contains("<>"))
            {
                Console.WriteLine($"Running tests from class {testClass.Key.TestClass.FullName}...");
            }
            else
            {
                continue;
            }
            var classInstance = Activator.CreateInstance(testClass.Key.TestClass);
           
            foreach(var testMethod in testClass.Value)
            {
              
                
                try
                {
                    testClass.Key.BeforeEach?.Invoke();
                    if(testMethod.DataRow is not null)
                    {
                        testMethod.TestMethod.Invoke(classInstance, testMethod.DataRow);
                    }
                    else
                    {
                        testMethod.TestMethod.Invoke(classInstance, null);
                    }
                    testClass.Key.AfterEach?.Invoke();
                    classCount.passed++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{testMethod.TestMethod.Name.PadRight(60)}: PASSED");
                    Console.ResetColor();
                    if (testMethod.Description is not null)
                    {
                        Console.WriteLine($"{testMethod.Description}");
                    }
                   
                }
                //problem 1 nie lapie mi AssertionException 
                // w aktualnym kodzie wypisze mi samo twoj stary i na koncy pare dobrze 
                // pora to zdebugowac xd
                //ok chuj z debugownia wyszedl 
                catch (AssertionException ex)
                {
                    classCount.failed++;
                    if (testMethod.Description is not null)
                    {
                        Console.WriteLine($"{testMethod.Description}");
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{testMethod.TestMethod.Name.PadRight(60)}: FAILED");
                    Console.ResetColor();
                   // Console.WriteLine(ex.Message);
                    //nw juz kurwa
                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("twoj stary");
                    Console.WriteLine(testMethod.Description); 
                }
               
            }
            totalCount.passed += classCount.passed;
            totalCount.failed += classCount.failed;
            Console.WriteLine(new string('*', 30));
            Console.WriteLine($"* Test passed: {classCount.passed,6} / {classCount.total,-4} *");
            Console.WriteLine($"* Failed: {classCount.failed,11} {"*",30-11-11}");
            Console.WriteLine(new string('*', 30));
            Console.WriteLine(new string('#',80));

        }
        Console.WriteLine($"Summary of running tests from {"[nwm jak]"}:");
        Console.WriteLine(new string('*', 30));
        Console.WriteLine($"* Test passed: {totalCount.passed,6} / {totalCount.total,-4} *");
        Console.WriteLine($"* Failed: {totalCount.failed,11} {"*",30 - 11 - 11}");
        Console.WriteLine(new string('*', 30));
    }
}
