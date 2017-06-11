# Dahomey.ExpressionEvaluator
Evaluate C# Formulas at Runtime

## Supported Platforms
* .Net Standard 2.0 (Compatible with .Net Core 2.0 and .Net Framework 4.6.1)
* .Net Framework 3.5 (Compatible with Unity)

Dahomey.ExpressionEvaluator code does not trigger any AOT complilation. It means it can be used safely with Unity IL2CPP.

## Building
Supports Visual Studio 2017 only

# Examples
## Parse a numeric expression
```csharp
ExpressionParser parser = new ExpressionParser();
parser.RegisterVariable<int>("a");
INumericExpression expr = parser.ParseNumericExpression("1 + a");

int a = 2;
double result = expr.Evaluate(new Dictionary<string, object> { { "a", a } });
Console.WriteLine(result);
```
The result will be:
```csharp
3
```

## Parse a numeric expression with member access
```csharp
class A
{
    public B B { get; set; }
}

class B
{
    public int Id { get; set; }
}

ExpressionParser parser = new ExpressionParser();
parser.RegisterVariable<A>("a");
INumericExpression expr = parser.ParseNumericExpression("1 + a.B.Id");

A a = new A { B = new B { Id = 12 } };
double result = expr.Evaluate(new Dictionary<string, object> { { "a", a } });
Console.WriteLine(result);
```
The result will be:
```csharp
13
```

## Parse a numeric expression with array or list access
```csharp
ExpressionParser parser = new ExpressionParser();
parser.RegisterVariable<List<int>>("a");
INumericExpression expr = parser.ParseNumericExpression("1 + a[1]");

List<int> a = new List<int> { 1, 2 };
double result = expr.Evaluate(new Dictionary<string, object> { { "a", a } });
Console.WriteLine(result);
```
The result will be:
```csharp
3
```

## Parse a numeric expression with function access
```csharp
Func<double, double> func = n => Math.Cos(n);
ExpressionParser parser = new ExpressionParser();
parser.RegisterFunction("cos", func);
INumericExpression expr = parser.ParseNumericExpression("1 + cos(12)");

double result = expr.Evaluate()
Console.WriteLine(result);
```
The result will be:
```csharp
1.8438539587324922
```

