using JetBrains.Annotations;
using Ploch.Common.Randomizers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ploch.Common.Tests.Randomizers;

[TestSubject(typeof(IntRandomizer))]
public class IntRandomizerTest : BaseRandomizerEachDifferentTest<int>
{ }