using JetBrains.Annotations;
using Ploch.Common.Randomizers;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests.Randomizers;

[TestSubject(typeof(DateTimeRandomizer))]
public class DateTimeRandomizerTest : BaseRandomizerEachDifferentTest<DateTime>
{ }