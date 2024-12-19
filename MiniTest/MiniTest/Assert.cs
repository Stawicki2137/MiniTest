using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTest;

public static class Assert
{
    public static void ThrowsException<TException>(Action action, string message = "")
    {
        try
        {
            action();
        }
        catch (Exception ex) 
        {
                if (ex is not TException) throw new AssertionException(message);
                return; 
        }
        throw new AssertionException(message);
    }
    public static void AreEqual<T>(T? expected , T? actual, string message = "")
    {
        if(!Equals(expected, actual)) throw new AssertionException(message);
    }
    public static void AreNotEqual<T>(T? notExpected, T? actual, string message = "")
    {
        if(Equals(notExpected, actual))  throw new AssertionException(message); 
    }
    public static void IsTrue(bool condition, string message = "")
    {
        if(!condition)
        {
            throw new AssertionException(message);
        }
    }
    public static void IsFalse(bool condition, string message = "")
    {
        if (condition)
        {
            throw new AssertionException(message);
        }
    }
    public static void Fail(string message = "") => throw new AssertionException(message);

}

public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}