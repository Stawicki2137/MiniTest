# MiniTest - Lightweight Unit Testing Framework

MiniTest is a **lightweight unit testing framework** built from scratch, designed for dynamically discovering and executing tests within .NET assemblies. It consists of two core components:

- **MiniTest** (library) – provides attributes for marking test classes and methods, as well as assertion methods.
- **MiniTestRunner** (executable) – dynamically loads test assemblies, finds test cases, executes them, and displays results in the console.

This project was created as part of an advanced programming course and aims to provide a minimalistic yet functional approach to unit testing using **reflection and assembly loading**.

---

## Features

### **MiniTest Library**
The library provides essential attributes for test discovery and execution:
- **`TestClassAttribute`** – marks a class as a test container.
- **`TestMethodAttribute`** – identifies unit test methods.
- **`BeforeEachAttribute`** / **`AfterEachAttribute`** – setup and teardown methods executed before and after each test.
- **`PriorityAttribute`** – assigns priority levels to test execution.
- **`DataRowAttribute`** – enables parameterized testing.
- **`DescriptionAttribute`** – adds descriptions to tests.

Additionally, a set of **assertion methods** is available in the `Assert` class:
- `ThrowsException<TException>(Action action, string message = "")`
- `AreEqual<T>(T? expected, T? actual, string message = "")`
- `AreNotEqual<T>(T? notExpected, T? actual, string message = "")`
- `IsTrue(bool condition, string message = "")`
- `IsFalse(bool condition, string message = "")`
- `Fail(string message = "")`

When a test fails, an `AssertionException` is thrown, providing a meaningful error message.

---

### **MiniTestRunner**
The `MiniTestRunner` is a command-line application responsible for:
1. **Dynamically loading test assemblies** without affecting the primary runtime.
2. **Discovering test classes and methods** via reflection.
3. **Executing tests** while respecting priority and test dependencies.
4. **Generating structured output** in the console.

#### **Test Execution Order**
- Tests are executed in order of `PriorityAttribute` (lower values run first).
- If multiple tests share the same priority, they execute in **alphabetical order**.

#### **Output and Formatting**
Test results are displayed in the console with colorized output for readability:
- ✅ **Green** – Passed tests
- ❌ **Red** – Failed tests (with error details)
- ⚠ **Yellow** – Warnings (e.g., configuration issues)

After running all tests, the program provides a **summary report** with:
- Total number of executed tests
- Passed and failed test counts
- Execution details per test class

---

## **Example Output**
Below are sample test execution outputs generated by MiniTestRunner:

![Example 1](https://github.com/Stawicki2137/MiniTest/blob/master/OutputImages/TestingExample.jpg)  

![Example 2](https://github.com/Stawicki2137/MiniTest/blob/master/OutputImages/TestingExample2.jpg)  

---

