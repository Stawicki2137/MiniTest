namespace MiniTest;

// marks a class as a container for test methods
[AttributeUsage(AttributeTargets.Class)]
public class TestClassAttribute : Attribute { }

// marks a method as a unit test to be executed
[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute { }

// defines a method to be executed before each test method
[AttributeUsage(AttributeTargets.Method)]
public class BeforeEachAttribute : Attribute { }

// defines a method to be executed after each test method
[AttributeUsage(AttributeTargets.Method)]
public class AfterEachAttribute : Attribute { }


// A CZY JAK MAM PRIORITY NA KLASACH TO WYBIERA 
// Z KTOREJ PIERWSZEJ ZALATWIAM METODY A W NICH PRIORTETY 
// DECYDUJA KTORA METODE NAJPIERW TESTUJE? imo tak xd

// sets a priority (integer) for test prioritization,
// with lower numerical values indicating higher priority
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PriorityAttribute : Attribute 
{ 
    public int Priority { get; }
    public PriorityAttribute(int priority) => Priority = priority;
}

// enables parameterized testing by supplying data to test methods
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DataRowAttribute : Attribute
{
    public object?[] Data { get; }
    public string? Description { get; set; }
    public DataRowAttribute(object?[] data, string? description = null)
    {
        Data = data;
        Description = description;
    }
    public DataRowAttribute(object? data, string? description = null)
    {
        Data = new object?[] { data };
        Description = description;
    }
}
// allows the inclusion of additional description to a test or a test class
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DescriptionAttribute : Attribute
{
    public string? Description { get; }
    public DescriptionAttribute(string? description) => Description = description;
}
