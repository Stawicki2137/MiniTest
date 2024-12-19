namespace MiniTest;

[AttributeUsage(AttributeTargets.Class)]
public class TestClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class BeforeEachAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterEachAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PriorityAttribute : Attribute
{
    public int Priority { get; }
    public PriorityAttribute(int priority) => Priority = priority;
}

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

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DescriptionAttribute : Attribute
{
    public string? Description { get; }
    public DescriptionAttribute(string? description) => Description = description;
}