﻿using System;

namespace Ploch.Common;

/// <summary>
///     A thread-safe wrapper class for generating random numbers.
/// </summary>
public static class ThreadSafeRandom
{
    private static readonly Random Global = new();

    [ThreadStatic] private static Random? LocalRandom;

    /// <summary>
    ///     Gets the shared instance of the random number generator.
    /// </summary>
    /// <remarks>
    ///     This property provides a thread-safe way to access a shared instance of the <see cref="Random" /> class.
    ///     The first time this property is accessed from a thread, it creates a new instance of <see cref="Random" />
    ///     with a seed generated from the global <see cref="System.Random" /> instance. Subsequent accesses on the same
    ///     thread will return the same instance, maintaining the same sequence of random numbers.
    /// </remarks>
    /// <value>The shared instance of the random number generator.</value>
    public static Random Shared
    {
        get
        {
            if (LocalRandom is null)
            {
                int seed;
                lock (Global)
                {
                    seed = Global.Next();
                }

                LocalRandom = new Random(seed);
            }

            return LocalRandom;
        }
    }
}