﻿using System;

namespace Ploch.Common
{
    /// <summary>
    ///     Extension methods for <see cref="DateTime" /> and related.
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        /// <summary>
        ///     Converts a <see cref="DateTime" /> to Epoch Seconds (Unix Timestamp - seconds since 00:00:00 UTC on 1 January
        ///     1970).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Helper method which simply casts <c>DateTime</c> to <see cref="DateTimeOffset" /> and calls
        ///         <see cref="DateTimeOffset.ToUnixTimeSeconds" />.
        ///     </para>
        ///     <para>
        ///         This class doesn't do any manipulations of time-zones, so the time zone of the provided instance of
        ///         <c>DateTime</c> will be used.
        ///     </para>
        /// </remarks>
        /// <param name="dateTime">The date time.</param>
        /// <returns>Epoch seconds value.</returns>
        public static long ToEpochSeconds(this DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

        public static long? ToEpochSeconds(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }

            return ToEpochSeconds(dateTime.Value);
        }

        /// <summary>
        ///     Converts Epoch Seconds value to <see cref="DateTime" />.
        /// </summary>
        /// <param name="epochSeconds">The epoch seconds.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTime(this long epochSeconds)
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(epochSeconds).DateTime;
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, UtcTimeZone);
        }

        /// <summary>
        ///     Converts Epoch Seconds value to <see cref="DateTime" />.
        /// </summary>
        /// <param name="epochSeconds">The epoch seconds.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTime<T>(this T epochSeconds) where T : struct,
                                                                            IComparable,
                                                                            IComparable<T>,
                                                                            IConvertible,
                                                                            IEquatable<T>,
                                                                            IFormattable
        {
            var epochSecondsAsLong = Convert.ToInt64(epochSeconds);
            return epochSecondsAsLong.ToDateTime();
        }

        public static DateTime? ToDateTime(this long? epochSeconds)
        {
            if (!epochSeconds.HasValue)
            {
                return null;
            }

            return ToDateTime(epochSeconds.Value);
        }
    }
}