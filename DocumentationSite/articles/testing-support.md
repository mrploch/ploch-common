# Testing Support

`Ploch.TestingSupport` is a family of packages that remove the boilerplate from xUnit tests: data
loaded from files, mocks generated from constructor signatures, assertions that read as sentences, and
attributes that keep platform-specific tests from failing on the wrong platform.

| Package | Target | Provides |
|---|---|---|
| `Ploch.TestingSupport` | `netstandard2.0` | The core data attributes, `FluentVerifier` and `MockingExtensions`, under `Ploch.TestingSupport.*` |
| `Ploch.TestingSupport.XUnit3` | `netstandard2.0`, `net8.0` | The same set for xUnit v3, under `Ploch.TestingSupport.XUnit3.*`, plus `SupportedOSPlatformAttribute` and test ordering |
| `Ploch.TestingSupport.XUnit3.AutoMoq` | `net8.0` | `[AutoMockData]` — AutoFixture with Moq and recursion handling |
| `Ploch.TestingSupport.FluentAssertions` | `netstandard2.0` | `ContainAllEquivalentOf`, `PropertyInfo` collection assertions, `NullEmptyCollectionEquivalencyStep` |
| `Ploch.TestingSupport.FluentAssertions.IOAbstractions` | `netstandard2.0` | Assertions over `System.IO.Abstractions` file-system types |
| `Ploch.TestingSupport.MockConsoleApp` | — | A trivial console executable to launch from process-related integration tests |

> **Which package?** `Ploch.TestingSupport.XUnit3` and `Ploch.TestingSupport` expose largely the same
> members under different namespaces (and the core package's `JsonFileDataAttribute` deserialises with
> `System.Text.Json` where the XUnit3 one uses `Newtonsoft.Json`). New test projects should reference
> **`Ploch.TestingSupport.XUnit3`** — and `.AutoMoq` alongside it, which is the package almost every
> test project actually wants.

```bash
dotnet add package Ploch.TestingSupport.XUnit3.AutoMoq
dotnet add package Ploch.TestingSupport.FluentAssertions
```

---

## `[AutoMockData]` — the constructor is the arrange step

```csharp
using Ploch.TestingSupport.XUnit3.AutoMoq;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AutoMockDataAttribute(bool ignoreVirtualMembers = false) : AutoDataAttribute;
```

`[AutoMockData]` is an AutoFixture `AutoDataAttribute` pre-customised with `AutoMoqCustomization`, so
every parameter of a `[Theory]` method is created for you: interfaces and abstract classes become Moq
mocks, concrete types are constructed with their dependencies filled in, and value types get
anonymous data.

The effect is that the *system under test* and its *test doubles* both arrive as parameters, and the
arrange section of the test disappears:

```csharp
[Theory]
[AutoMockData]
public async Task PlaceOrderAsync_should_persist_the_order(
    [Frozen] Mock<IOrderRepository> repository,
    OrderService sut,
    OrderRequest request)
{
    await sut.PlaceOrderAsync(request, CancellationToken.None);

    repository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

`[Frozen]` (from `AutoFixture.Xunit3`) is what ties the two together: it pins that specimen in the
fixture so the `Mock<IOrderRepository>` you receive is the *same* instance injected into `sut`. Without
it, `sut` gets its own mock and every `Verify` fails. Declare frozen parameters **before** the system
under test — AutoFixture resolves parameters left to right.

Because `AutoDataAttribute` supports it, `[InlineAutoMockData]`-style composition is available by
combining with `[InlineData]`-carrying AutoFixture attributes, and explicit values can always be
supplied by declaring them ahead of the generated ones.

### What the customisation actually does

`AutoMockDataAttribute` applies `AutoDataCommonCustomization`, which layers four things:

| Customisation | Effect |
|---|---|
| `AutoMoqCustomization` | Interfaces and abstract classes are satisfied with Moq mocks. |
| `DoNotThrowOnRecursionCustomization` | Removes AutoFixture's default `ThrowingRecursionBehavior`. |
| `OmitOnRecursionCustomization` | Adds `OmitOnRecursionBehavior`, so a circular reference is truncated instead of throwing. |
| `IgnoreVirtualMembersCustomization` | Applied **only** when `ignoreVirtualMembers: true` — skips populating virtual properties. |

The recursion pair matters in practice. Domain and EF Core entity graphs are routinely circular
(`Order.Customer.Orders`), and stock AutoFixture throws `ObjectCreationException` on the second lap.
With these customisations the graph is simply cut short:

```csharp
[Theory]
[AutoMockData]
public void Map_should_project_the_customer_name(Order order, OrderMapper sut)
{
    // order.Customer is populated; order.Customer.Orders is omitted rather than throwing.
    var dto = sut.Map(order);

    dto.CustomerName.Should().Be(order.Customer.Name);
}
```

Pass `ignoreVirtualMembers: true` when the type under test uses virtual properties for lazy loading or
for Moq overriding, and you do not want AutoFixture to fill them:

```csharp
[Theory]
[AutoMockData(ignoreVirtualMembers: true)]
public void Handler_should_not_touch_lazy_navigations(OrderEntity entity, OrderHandler sut) { /* ... */ }
```

All three customisations are public and usable on their own, for tests that build a `Fixture`
directly:

```csharp
var fixture = new Fixture().Customize(new AutoDataCommonCustomization(ignoreVirtualMembers: false));

var order = fixture.Create<Order>();
```

`IgnoreVirtualMembersCustomization` additionally accepts a `Type`, restricting the omission to virtual
properties declared on that type:

```csharp
fixture.Customize(new IgnoreVirtualMembersCustomization(typeof(OrderEntity)));
```

---

## `MockingExtensions.Mock<T>()` — recover the mock from the mocked object

```csharp
using Ploch.TestingSupport.XUnit3.Moq;

public static Mock<T> Mock<T>(this T mockedService) where T : class
```

When a parameter is declared as the interface rather than as `Mock<T>`, you still need the `Mock<T>`
to set up or verify. `Mock<T>()` casts back through Moq's `IMocked<T>`, throwing
`InvalidOperationException` with a helpful message if the object is not in fact a mock:

```csharp
[Theory]
[AutoMockData]
public async Task Handle_should_query_the_repository([Frozen] IOrderRepository repository, OrderService sut)
{
    repository.Mock()
              .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new Order { Id = 42 });

    var order = await sut.GetAsync(42, CancellationToken.None);

    order.Id.Should().Be(42);
    repository.Mock().Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()), Times.Once);
}
```

This keeps the test signature expressed in terms of the *dependency* rather than the mocking library,
which reads better and survives a change of mocking framework in more places than you would expect.

---

## `FluentVerifier` — FluentAssertions inside a Moq argument matcher

```csharp
public static bool VerifyFluentAssertion(Action assertion)
public static Task<bool> VerifyFluentAssertionAsync(Func<Task> assertion)
```

Verifying that a method was called with an argument having particular properties is awkward in Moq.
`It.Is<T>(...)` needs a predicate returning `bool`, so a rich FluentAssertions comparison cannot be
used directly — and when the predicate returns `false`, the failure message says only "expected
invocation was not performed", telling you nothing about *which* property differed.

`FluentVerifier` bridges the two by running the assertion inside an `AssertionScope` and reporting
whether it collected any failures:

```csharp
[Theory]
[AutoMockData]
public async Task PlaceOrderAsync_should_send_a_confirmation_for_the_placed_order(
    [Frozen] Mock<INotificationService> notifications,
    OrderService sut,
    OrderRequest request)
{
    await sut.PlaceOrderAsync(request, CancellationToken.None);

    notifications.Verify(n => n.SendAsync(
        It.Is<OrderConfirmation>(confirmation => FluentVerifier.VerifyFluentAssertion(() =>
            confirmation.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers()))),
        It.IsAny<CancellationToken>()),
        Times.Once);
}
```

The `AssertionScope` is what makes this work: it collects failures rather than throwing, so an argument
that does not match returns `false` and lets Moq continue examining other invocations instead of
aborting the verification. `VerifyFluentAssertionAsync` does the same for assertions that must be
awaited.

Reach for it when the argument is a rich object and you want equivalence semantics. For a simple
property check, a plain lambda in `It.Is<T>` remains clearer.

---

## File-based test data

Inline `[InlineData]` stops scaling once a case needs more than a handful of scalar values. These
attributes move the cases into files that can be edited, diffed and reviewed on their own.

### `[JsonFileData]`

```csharp
using Ploch.TestingSupport.XUnit3.TestData;

public class JsonFileDataAttribute(string filePath, string? propertyName = null) : DataAttribute
```

Loads an array of test cases from a JSON file. Each element is an array of arguments positionally
matched to the test method's parameters, and objects and arrays are deserialised into the declared
parameter types.

With `propertyName` omitted, the **root** of the document must be the array of cases:

```json
[
  [ "GB", 20.0 ],
  [ "DE", 19.0 ],
  [ "CH", 7.7 ]
]
```

```csharp
[Theory]
[JsonFileData("TestData/vat-rates.json")]
public void GetRate_should_return_the_rate_for_the_country(string countryCode, decimal expectedRate)
{
    VatRates.GetRate(countryCode).Should().Be(expectedRate);
}
```

Supply `propertyName` to read a named property instead, which lets one file hold several sets:

```json
{
  "validOrders": [
    [ { "CustomerId": 1, "Lines": [ { "ProductCode": "SKU-1", "Quantity": 2 } ] }, true ]
  ],
  "invalidOrders": [
    [ { "CustomerId": 0, "Lines": [] }, false ]
  ]
}
```

```csharp
[Theory]
[JsonFileData("TestData/orders.json", "validOrders")]
[JsonFileData("TestData/orders.json", "invalidOrders")]
public void Validate_should_classify_the_order(OrderRequest request, bool expectedValid)
{
    _validator.Validate(request).IsValid.Should().Be(expectedValid);
}
```

Points to be aware of:

- The path is resolved with `Path.GetFullPath`, so it is relative to the **working directory** of the
  test run — normally the test project's output directory. Mark the file
  `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>` (or `Always`) in the `.csproj`, and
  use the output-relative path. A missing file raises `ArgumentException` naming the resolved path.
- `SupportsDiscoveryEnumeration()` throws `NotImplementedException`, so cases are **not** pre-enumerated
  at discovery time: the test explorer shows one entry for the theory rather than one per case. That is
  deliberate — enumerating would mean reading the file during discovery.
- The attribute is `AllowMultiple = true`, so several files (or several properties) can feed the same
  theory, as above.

### `[TextFileLinesData]`

```csharp
public sealed class TextFileLinesDataAttribute(string filePath, bool removeEmptyEntries = false) : TextFileDataAttribute
```

Treats each **line** of a text file as one test case with a single `string` parameter. This is the
right shape for corpus-style tests — parser inputs, validation samples, regression cases collected
from production:

```csharp
[Theory]
[TextFileLinesData("TestData/valid-postcodes.txt", removeEmptyEntries: true)]
public void IsValid_should_accept_every_known_good_postcode(string postcode)
{
    PostcodeValidator.IsValid(postcode).Should().BeTrue();
}
```

Adding a regression case is then a one-line diff to a text file rather than a change to test code.

Two details: lines are split on `Environment.NewLine`, so a file with Unix endings read on Windows
yields one long line — commit these files with platform-native endings, or normalise them with
`.gitattributes`. Unlike `[JsonFileData]`, this attribute **does** support discovery enumeration, so
each line appears as its own entry in the test explorer.

`TextFileDataAttribute` is the **abstract** base — it handles locating and reading the file and
delegates to `ProcessFileData`. Derive from it for a custom file format:

```csharp
public sealed class CsvFileDataAttribute(string filePath) : TextFileDataAttribute(filePath)
{
    public override bool SupportsDiscoveryEnumeration() => true;

    protected override IEnumerable<ITheoryDataRow> ProcessFileData(string fileData) =>
        fileData.Split([ Environment.NewLine ], StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)                                    // header row
                .Select(line => line.Split(','))
                .Select(fields => new TheoryDataRow(fields[0], decimal.Parse(fields[1], CultureInfo.InvariantCulture)));
}
```

---

## `[SupportedOSPlatform]` — skip instead of fail

```csharp
using Ploch.TestingSupport.XUnit3;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SupportedOSPlatformAttribute(params SupportedOS[] supportedOSes) : BeforeAfterTestAttribute

public enum SupportedOS { FreeBSD = 1, Linux = 2, macOS = 3, Windows = 4 }
```

Some tests are genuinely platform-specific: processor affinity, the Windows registry, file permissions,
path casing. Guarding them with `if (!OperatingSystem.IsWindows()) return;` makes them *pass* on other
platforms, which hides the fact that they never ran.

This attribute **dynamically skips** the test instead, so the result is honest:

```csharp
[Fact]
[SupportedOSPlatform(SupportedOS.Windows)]
public void SetSingleProcessorAffinity_should_bind_the_process_to_one_core()
{
    using var process = Process.GetCurrentProcess();

    process.SetSingleProcessorAffinity(0);

    process.GetEnabledProcessors().Should().ContainSingle().Which.Should().Be(0);
}

[Fact]
[SupportedOSPlatform(SupportedOS.Linux, SupportedOS.macOS)]
public void FileMode_should_be_preserved_on_unix() { /* ... */ }
```

Listing several platforms means "any of these". On an unlisted platform the test is reported as
skipped, with the current `RuntimeInformation.OSDescription` in the skip message — so a CI matrix shows
which legs actually exercised the test rather than silently running an empty method.

Note that this is `Ploch.TestingSupport.XUnit3.SupportedOSPlatformAttribute`, not
`System.Runtime.Versioning.SupportedOSPlatformAttribute`. The names collide; if a test file uses both,
alias one:

```csharp
using TestPlatform = Ploch.TestingSupport.XUnit3.SupportedOSPlatformAttribute;
```

---

## Test ordering

```csharp
using Ploch.TestingSupport.XUnit3.TestOrdering;

public sealed class TestPriorityAttribute(int priority) : Attribute { public int Priority { get; } }

public class AlphabeticalOrderer : ITestCaseOrderer
```

`AlphabeticalOrderer` sorts test cases by method name using `StringComparer.OrdinalIgnoreCase`, giving
a deterministic execution order within a class:

```csharp
[TestCaseOrderer(typeof(AlphabeticalOrderer))]
public class MigrationSequenceTests
{
    [Fact] public void Step01_creates_the_schema() { /* ... */ }
    [Fact] public void Step02_seeds_reference_data() { /* ... */ }
    [Fact] public void Step03_applies_the_migration() { /* ... */ }
}
```

`TestPriorityAttribute` carries an explicit priority — lower runs first — for use by a custom orderer.

> **Use this sparingly.** Tests that depend on execution order share state, and shared state is the
> usual reason a suite becomes flaky and cannot be parallelised. Ordering is defensible for a scripted
> integration sequence against a single fixture; it is a design smell in unit tests. Prefer a shared
> `IClassFixture<T>` that performs set-up once, with each test independent of the others.

---

## FluentAssertions extensions

### `ContainAllEquivalentOf` — every substring, case-insensitively

```csharp
using Ploch.TestingSupport.FluentAssertions;

public static AndConstraint<StringAssertions> ContainAllEquivalentOf(this StringAssertions assertions, params string?[] values)
public static AndConstraint<StringAssertions> ContainAllEquivalentOf(this StringAssertions assertions, IEnumerable<string?> values, string because = "", params object[] becauseArgs)
```

FluentAssertions offers `ContainEquivalentOf` for one substring; this asserts **all** of them at once,
using `OrdinalIgnoreCase`, and — crucially — the failure message names exactly which ones were
missing:

```csharp
var exception = Record.Exception(() => sut.Process(invalidRequest));

exception!.Message.Should().ContainAllEquivalentOf("customer", "not found", requestId);
```

That makes it well suited to asserting on messages that must carry diagnostic context, without
coupling the test to the exact wording:

```csharp
logEntry.Message.Should().ContainAllEquivalentOf(order.Id.ToString(), "cancelled", user.Name);
```

Passing an empty collection is itself a failure ("You have to provide at least one value to check
for."), so a mistakenly empty candidate list cannot produce a vacuously passing test.

### `PropertyInfo` collection assertions

```csharp
public static PropertyInfoCollectionAssertions Should(this IEnumerable<PropertyInfo> instance)

public AndConstraint<PropertyInfoCollectionAssertions> ContainProperty(string propertyName, ...)
public AndConstraint<PropertyInfoCollectionAssertions> ContainProperties(string[] propertyNames, ...)
```

For tests that assert on a type's shape — a DTO contract, a generated model, a reflection-driven
mapper:

```csharp
typeof(OrderDto).GetProperties()
                .Should()
                .ContainProperties([ nameof(OrderDto.Id), nameof(OrderDto.CustomerName), nameof(OrderDto.Total) ]);
```

### `NullEmptyCollectionEquivalencyStep`

```csharp
public sealed class NullEmptyCollectionEquivalencyStep : IEquivalencyStep
```

Treats a `null` collection and an empty collection as equivalent. The scenario this exists for is EF
Core: a navigation property that was not eager-loaded with `Include()` stays `null`, while the expected
object built in the test initialises it to `new List<T>()`. Without this step the comparison fails on a
difference that is not real:

```csharp
actual.Should().BeEquivalentTo(expected,
                               options => options.Using(new NullEmptyCollectionEquivalencyStep()));
```

The step intercedes **only** when one side is `null` and the other is an empty non-string enumerable;
every other comparison falls through to the rest of the pipeline, so configured options such as
`DateTimeOffset` tolerance and cyclic-reference handling still apply. Register it globally in an
`AssertionOptions.AssertEquivalencyUsing(...)` call if the pattern recurs across a suite.

### File-system assertions

```csharp
using Ploch.TestingSupport.FluentAssertions.IOAbstractions;

public static FileSystemInfoAssertions<IFileSystemInfo> Should(this IEnumerable<IFileSystemInfo> fileSystemInfos)
public static FileSystemInfoAssertions<IDirectoryInfo> Should(this IEnumerable<IDirectoryInfo> directoryInfos)
public static FileSystemInfoAssertions<IFileInfo>      Should(this IEnumerable<IFileInfo> fileInfos)

public AndConstraint<...> HaveNamesEquivalentToIgnoringCase(params string[] fileSystemInfoNames)
public AndConstraint<...> HaveNamesEquivalentToIgnoringCase(IEnumerable<string> fileSystemInfoNames, string because = "", params object[] becauseArgs)
public AndConstraint<...> HaveNamesEquivalentTo(IEnumerable<string> fileSystemInfoNames, string because = "", params object[] becauseArgs)
```

Built on [`TestableIO.System.IO.Abstractions`](https://github.com/TestableIO/System.IO.Abstractions),
so they work against a `MockFileSystem` with no disk involved. They assert on the collection's *names*
rather than its full paths, which is what the test usually means:

```csharp
var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
{
    { @"C:\input\orders.csv",   new MockFileData("...") },
    { @"C:\input\customers.csv", new MockFileData("...") },
    { @"C:\input\notes.txt",    new MockFileData("...") }
});

var sut = new CsvDiscoveryService(fileSystem);

sut.FindDataFiles(@"C:\input")
   .Should()
   .HaveNamesEquivalentToIgnoringCase("orders.csv", "customers.csv");
```

Use `HaveNamesEquivalentToIgnoringCase` for tests that must pass on both Windows and Linux;
`HaveNamesEquivalentTo` when the casing is part of what is being asserted.

Because these `Should()` extensions apply to `IEnumerable<IFileInfo>` and friends, they can shadow
FluentAssertions' generic collection `Should()`. `FileSystemInfoAssertions<T>` derives from
`GenericCollectionAssertions<T>`, so the usual collection assertions remain available on the result.

---

## `Ploch.TestingSupport.MockConsoleApp`

A minimal console executable whose only purpose is to be **launched** by integration tests that need a
real child process — process-affinity tests, command-line parsing tests, process-lifetime tests.

It writes a greeting and then waits. Critically, it detects redirected standard input and waits for a
line rather than calling `Console.ReadKey` (which throws `InvalidOperationException` when input is
redirected — precisely how a test harness starts a child process). A test can therefore end it cleanly
by writing to `StandardInput` instead of killing it:

```csharp
using var process = Process.Start(new ProcessStartInfo(mockConsoleAppPath)
                                  {
                                      RedirectStandardInput = true,
                                      RedirectStandardOutput = true
                                  })!;

process.StandardOutput.ReadLine()
       .Should()
       .Contain("mock console app");

await process.StandardInput.WriteLineAsync();   // graceful exit
await process.WaitForExitAsync();
```

---

## Putting it together

A representative test class from a real suite:

```csharp
using FluentAssertions;
using Moq;
using Ploch.TestingSupport.XUnit3.AutoMoq;
using Ploch.TestingSupport.XUnit3.Moq;
using Xunit;

public class OrderServiceTests
{
    [Theory]
    [AutoMockData]
    public async Task PlaceOrderAsync_should_persist_and_notify(
        [Frozen] Mock<IOrderRepository> repository,
        [Frozen] Mock<INotificationService> notifications,
        OrderService sut,
        OrderRequest request)
    {
        await sut.PlaceOrderAsync(request, CancellationToken.None);

        repository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        notifications.Verify(n => n.SendAsync(It.IsAny<OrderConfirmation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [JsonFileData("TestData/invalid-orders.json")]
    public async Task PlaceOrderAsync_should_reject_invalid_requests(OrderRequest request, string expectedError)
    {
        var sut = new Fixture().Customize(new AutoDataCommonCustomization(false)).Create<OrderService>();

        var act = () => sut.PlaceOrderAsync(request, CancellationToken.None);

        (await act.Should().ThrowAsync<ValidationException>()).Which.Message.Should().ContainEquivalentOf(expectedError);
    }
}
```

The conventions this repository follows for test naming, structure and framework choice are documented
in the repository's [.NET testing rules](https://github.com/mrploch/ploch-common/blob/master/.claude/rules/writing-dotnet-tests.md):
xUnit v3, FluentAssertions, AutoFixture; classes named `<TypeUnderTest>Tests`; methods named
`<Method>_should_<behaviour>`.

## See also

- [Randomizers](randomizers.md) — an injectable random-value strategy for the cases AutoFixture does not fit
- [Argument validation](argument-validation.md)
- [`Ploch.TestingSupport.XUnit3` API reference](../api/Ploch.TestingSupport.XUnit3.html)
- [`Ploch.TestingSupport.FluentAssertions` API reference](../api/Ploch.TestingSupport.FluentAssertions.html)
