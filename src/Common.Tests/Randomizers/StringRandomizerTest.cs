using JetBrains.Annotations;
using Ploch.Common.Randomizers;
using Xunit;
using FluentAssertions;

namespace Ploch.Common.Tests.Randomizers;

[TestSubject(typeof(StringRandomizer))]
public class StringRandomizerTest : BaseRandomizerEachDifferentTest<string>
{ }