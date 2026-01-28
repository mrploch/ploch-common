# Getting Started with Ploch.Common

This guide will help you get started with Ploch.Common libraries in your .NET projects.

## Installation

### Using .NET CLI

Install the core library:

```bash
dotnet add package Ploch.Common
```

Install additional libraries as needed:

```bash
# Dependency Injection
dotnet add package Ploch.Common.DependencyInjection

# Serialization
dotnet add package Ploch.Common.Serialization
dotnet add package Ploch.Common.Serialization.SystemTextJson

# Testing
dotnet add package Ploch.TestingSupport
dotnet add package Ploch.TestingSupport.FluentAssertions
```

### Using Package Manager Console

```powershell
Install-Package Ploch.Common
Install-Package Ploch.Common.DependencyInjection
Install-Package Ploch.Common.Serialization
```

### Using Visual Studio

1. Right-click on your project
2. Select "Manage NuGet Packages"
3. Search for "Ploch.Common"
4. Click "Install"

## Your First Steps

### 1. Add Using Directives

Add these using statements to your C# files:

```csharp
using Ploch.Common;
using Ploch.Common.Collections;
using Ploch.Common.ArgumentChecking;
```

### 2. Start Using Extension Methods

#### Example 1: Simple String Validation

```csharp
using Ploch.Common;

public class Example
{
    public void ProcessInput(string userInput)
    {
        // Old way
        if (string.IsNullOrWhiteSpace(userInput))
        {
            throw new ArgumentException("Input cannot be empty");
        }

        // New way with Ploch.Common
        if (userInput.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Input cannot be empty");
        }

        // Even better with Guard clauses
        userInput.NotNullOrEmpty(nameof(userInput));

        // Now work with validated input
        ProcessValidatedInput(userInput);
    }
}
```

#### Example 2: Parameter Validation

```csharp
using Ploch.Common.ArgumentChecking;

public class UserService
{
    public void CreateUser(string email, string password, int age)
    {
        // Validate all parameters at the start
        email.NotNullOrEmpty(nameof(email));
        password.NotNullOrEmpty(nameof(password));
        age.Positive(nameof(age));

        // Your business logic here
        var user = new User
        {
            Email = email,
            Password = password,
            Age = age
        };

        SaveUser(user);
    }
}
```

#### Example 3: Working with Collections

```csharp
using Ploch.Common.Collections;

public class OrderService
{
    public string GetOrderSummary(List<Order> orders)
    {
        // Validate collection
        if (orders.IsNullOrEmpty())
        {
            return "No orders found";
        }

        // Join order IDs into a readable string
        var orderIds = orders.Join(", ", o => o.Id);

        return $"Orders: {orderIds}";
    }

    public bool IsValidStatus(OrderStatus status)
    {
        // Check if status is in valid set
        return status.ValueIn(
            OrderStatus.Pending,
            OrderStatus.Processing,
            OrderStatus.Completed
        );
    }
}
```

#### Example 4: Using IsIn Extensions

```csharp
using Ploch.Common;

public class PermissionChecker
{
    public bool HasAdminRights(string role)
    {
        // Check if role is in admin set
        return role.In("Admin", "SuperAdmin", "Owner");
    }

    public bool IsSuccessStatusCode(int statusCode)
    {
        // HTTP success codes
        return statusCode.In(200, 201, 202, 204);
    }

    public bool IsClientError(int statusCode)
    {
        // Check if NOT a client error
        return statusCode.NotIn(400, 401, 403, 404, 422);
    }
}
```

### 3. Working with Types and Reflection

```csharp
using Ploch.Common.Reflection;

public class ServiceRegistration
{
    public void RegisterServices(IServiceCollection services, Assembly assembly)
    {
        // Find all concrete implementations of IService
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsConcreteImplementation<IService>());

        foreach (var serviceType in serviceTypes)
        {
            services.AddTransient(typeof(IService), serviceType);
        }
    }

    public bool CanProcess(Type type)
    {
        // Check if type implements a specific interface
        return type.IsImplementing(typeof(IProcessor), concreteOnly: true);
    }
}
```

### 4. Path and File Operations

```csharp
using Ploch.Common.IO;

public class FileService
{
    public string CreateSafeFileName(string userInput)
    {
        // Convert user input to safe file name
        return PathUtils.ToSafeFileName(userInput);
    }

    public string ChangeFileExtension(string filePath, string newExtension)
    {
        // Change file extension
        return filePath.WithExtension(newExtension);
    }

    public string GetRelativePathForDisplay(string fullPath, string basePath)
    {
        // Get relative path for display purposes
        return PathUtils.GetRelativePath(basePath, fullPath);
    }
}
```

## Common Use Cases

### Use Case 1: API Request Validation

```csharp
using Ploch.Common;
using Ploch.Common.ArgumentChecking;

public class CreateUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }

    public void Validate()
    {
        Email.NotNullOrEmpty(nameof(Email));
        Password.NotNullOrEmpty(nameof(Password));
        Roles.NotNullOrEmpty(nameof(Roles));

        // Validate email format
        if (!Email.ContainsAny("@"))
        {
            throw new ValidationException("Invalid email format");
        }

        // Validate password length
        if (Password.Length < 8)
        {
            throw new ValidationException("Password too short");
        }
    }
}
```

### Use Case 2: Building Dynamic Queries

```csharp
using Ploch.Common.Collections;

public class ProductRepository
{
    public List<Product> SearchProducts(
        string? category,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock)
    {
        var query = _dbContext.Products.AsQueryable();

        // Build query conditionally
        query = query
            .If(category != null, q => q.Where(p => p.Category == category))
            .If(minPrice.HasValue, q => q.Where(p => p.Price >= minPrice.Value))
            .If(maxPrice.HasValue, q => q.Where(p => p.Price <= maxPrice.Value))
            .If(inStock.HasValue, q => q.Where(p => p.InStock == inStock.Value));

        return query.ToList();
    }
}
```

### Use Case 3: Service Discovery and Registration

```csharp
using Ploch.Common.Reflection;
using Ploch.Common.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services)
    {
        // Discover all handlers
        var handlerTypes = TypeLoader.Configure(cfg => cfg
            .WithBaseType<IRequestHandler>()
            .FromAssembliesContaining("MyApp.Domain")
        ).LoadTypes();

        // Register all concrete handlers
        foreach (var handlerType in handlerTypes)
        {
            if (handlerType.IsConcreteImplementation<IRequestHandler>())
            {
                services.AddScoped(handlerType);
            }
        }

        return services;
    }
}
```

### Use Case 4: Data Export Formatting

```csharp
using Ploch.Common.Collections;

public class ExportService
{
    public string ExportToCsv<T>(IEnumerable<T> items, Func<T, string> formatter)
    {
        // Validate input
        if (items.IsNullOrEmpty())
        {
            return string.Empty;
        }

        // Format and join all items
        return items.Join(Environment.NewLine, formatter);
    }

    public string CreateReadableList(IEnumerable<string> items)
    {
        // Create human-readable list: "item1, item2 and item3"
        return items.JoinWithFinalSeparator(", ", " and ");
    }
}
```

## Testing with Ploch.Common

### Example Test with Random Data

```csharp
using Ploch.Common.Randomizers;
using Ploch.TestingSupport;
using Xunit;

public class UserServiceTests
{
    [Fact]
    public void CreateUser_ShouldSucceed_WithValidData()
    {
        // Arrange
        var stringRandomizer = Randomizer.GetRandomizer<string>();
        var intRandomizer = Randomizer.GetRandomizer<int>();

        var email = stringRandomizer.GetRandom() + "@example.com";
        var password = stringRandomizer.GetRandom();
        var age = ((IRangedRandomizer<int>)intRandomizer).GetRandom(18, 100);

        var service = new UserService();

        // Act
        var user = service.CreateUser(email, password, age);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void CreateUser_ShouldThrow_WithInvalidEmail(string email)
    {
        // Arrange
        var service = new UserService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.CreateUser(email, "password123", 25));
    }
}
```

## Best Practices

### 1. Always Validate at Method Entry Points

```csharp
public void ProcessOrder(Order order, Customer customer)
{
    // Validate first
    order.NotNull(nameof(order));
    customer.NotNull(nameof(customer));
    order.Id.Positive(nameof(order.Id));

    // Then execute business logic
    DoProcessOrder(order, customer);
}
```

### 2. Use Meaningful Parameter Names

```csharp
// Good - parameter name is clear
email.NotNullOrEmpty(nameof(email));

// Less helpful - generic name
value.NotNull("value");
```

### 3. Prefer Extension Methods Over Static Calls

```csharp
// Good - fluent and readable
if (input.IsNotNullOrEmpty())
{
    // ...
}

// Less fluent - static call
if (!string.IsNullOrEmpty(input))
{
    // ...
}
```

### 4. Chain Operations for Readability

```csharp
var result = items
    .Where(x => x.IsActive)
    .If(filter.HasValue, q => q.Where(x => x.Status == filter.Value))
    .OrderBy(x => x.Priority)
    .Join(", ", x => x.Name);
```

### 5. Use Guard Clauses for Fail-Fast Behavior

```csharp
public decimal CalculateDiscount(decimal price, int quantity)
{
    // Fail fast with clear error messages
    price.Positive(nameof(price));
    quantity.Positive(nameof(quantity));

    // Safe to proceed
    return price * quantity * 0.1m;
}
```

## Next Steps

1. **Explore the full API**: Check out the [README.md](../README.md) for comprehensive examples
2. **Use the Quick Reference**: Keep [QUICK_REFERENCE.md](QUICK_REFERENCE.md) handy for common operations
3. **Review Real-World Examples**: See practical applications in the main documentation
4. **Experiment**: Try the extensions in your own projects
5. **Contribute**: Found a bug or have a suggestion? Visit the [GitHub repository](https://github.com/mrploch/ploch-common)

## Troubleshooting

### Issue: Extension methods not appearing in IntelliSense

**Solution**: Make sure you've added the appropriate using directive:
```csharp
using Ploch.Common;
using Ploch.Common.Collections;
using Ploch.Common.ArgumentChecking;
```

### Issue: NotNull throws InvalidOperationException instead of ArgumentNullException

**Solution**: Use `NotNull` for parameter validation (throws ArgumentNullException) and `RequiredNotNull` for state validation (throws InvalidOperationException):
```csharp
// For parameters - throws ArgumentNullException
parameter.NotNull(nameof(parameter));

// For required state - throws InvalidOperationException
value.RequiredNotNull(nameof(value));
```

### Issue: Collection extensions not working

**Solution**: Make sure you're using `Ploch.Common.Collections` namespace and that your collection implements `IEnumerable<T>`.

## Additional Resources

- [Full Documentation](../README.md)
- [Quick Reference Guide](QUICK_REFERENCE.md)
- [GitHub Repository](https://github.com/mrploch/ploch-common)
- [Issue Tracker](https://github.com/mrploch/ploch-common/issues)

## Support

If you need help or have questions:
1. Check the documentation
2. Review the examples
3. Search existing GitHub issues
4. Create a new issue if needed

Happy coding!
