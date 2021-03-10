﻿using System;

namespace Ploch.Common
{
    /// <summary>
    ///     Extension methods for <see cref="DateTime" /> and related.
    /// </summary>
    public static class DateTimeExtensions
    {
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

        /// <summary>
        /// Converts Epoch Seconds value to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="epochSeconds">The epoch seconds.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FromEpochSeconds(this long epochSeconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(epochSeconds).DateTime;
        }
    }
}