using System;
using FluentAssertions;
using Ploch.Common.ArgumentChecking;
using Xunit;

namespace Ploch.Common.Tests.ArgumentChecking;

/// <summary>
///     Coverage for <see cref="Guard.NotOutOfRange{TEnum}(TEnum, string)" /> across every enum underlying type
///     supported by <c>ToUInt64</c>'s switch (sbyte, byte, short, ushort, int, uint, long, ulong) plus the
///     negative-cast edge case discussed on PR #205.
/// </summary>
public class GuardFlagsEnumTests
{
    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_sbyte_underlying_enum()
    {
        var combined = SByteFlags.A | SByteFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_byte_underlying_enum()
    {
        var combined = ByteFlags.A | ByteFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_short_underlying_enum()
    {
        var combined = ShortFlags.A | ShortFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_ushort_underlying_enum()
    {
        var combined = UShortFlags.A | UShortFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_int_underlying_enum()
    {
        var combined = IntFlags.A | IntFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_uint_underlying_enum()
    {
        var combined = UIntFlags.A | UIntFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_long_underlying_enum()
    {
        var combined = LongFlags.A | LongFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_accept_combined_flags_for_ulong_underlying_enum()
    {
        var combined = ULongFlags.A | ULongFlags.B;

        combined.NotOutOfRange().Should().Be(combined);
    }

    [Fact]
    public void NotOutOfRange_should_reject_combined_flags_with_undefined_bit_for_int_enum()
    {
        var invalid = (IntFlags)((int)IntFlags.A | 0x10);

        var action = () => invalid.NotOutOfRange();

        action.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalid}*not defined*{nameof(IntFlags)}*");
    }

    [Fact]
    public void NotOutOfRange_should_reject_negative_cast_value_for_signed_enum_when_no_negative_member_defined()
    {
        // The codereviewbot-ai concern on PR #205 hypothesised that unchecked casts of signed enum values let
        // negative bit-patterns through. With no member equal to ~0 the mask only covers bits 0-2, so the upper
        // bits set by sign-extension fail the (value & ~mask) == 0 check and the value is rejected as expected.
        var invalid = (IntFlags)(-1);

        var action = () => invalid.NotOutOfRange();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NotOutOfRange_should_throw_for_non_flags_enum_with_combined_value()
    {
        var combined = (PlainEnum)((int)PlainEnum.A | (int)PlainEnum.B);

        var action = () => combined.NotOutOfRange();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NotOutOfRange_should_accept_zero_value_when_zero_is_defined_for_flags_enum()
    {
        IntFlags.None.NotOutOfRange().Should().Be(IntFlags.None);
    }

    [Fact]
    public void NotOutOfRange_should_accept_zero_for_flags_enum_without_explicit_zero_member()
    {
        // Documents the behaviour relied on by [Flags] enums that omit a `None = 0` member: a value
        // with no bits set is treated as "no flags chosen", matching C# conventions for flags enums.
        var noFlags = default(FlagsWithoutZero);

        noFlags.NotOutOfRange().Should().Be(noFlags);
    }

    [Fact]
    public void NotOutOfRange_should_reject_zero_for_non_flags_enum_without_explicit_zero_member()
    {
        // Regression check: the flags-enum zero-acceptance MUST NOT leak into non-flags enums; a
        // value of 0 on an enum without [Flags] and without a 0 member is still out of range.
        var zero = default(NonFlagsWithoutZero);

        var action = () => zero.NotOutOfRange();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Flags]
    private enum FlagsWithoutZero
    {
        A = 1,
        B = 2,
        C = 4
    }

    private enum NonFlagsWithoutZero
    {
        A = 1,
        B = 2
    }

    [Flags]
    private enum SByteFlags : sbyte
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum ByteFlags : byte
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum ShortFlags : short
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum UShortFlags : ushort
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum IntFlags
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum UIntFlags : uint
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum LongFlags : long
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    [Flags]
    private enum ULongFlags : ulong
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    private enum PlainEnum
    {
        A = 1,
        B = 2
    }
}
