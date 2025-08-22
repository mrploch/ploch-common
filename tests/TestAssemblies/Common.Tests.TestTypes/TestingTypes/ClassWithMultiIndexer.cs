using System;
using System.Collections.Generic;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithMultiIndexer
{
    private readonly Dictionary<(int, int), string> _storage = new();

    public string this[int index, int key]
    {
        get => _storage.TryGetValue((index, key), out var value) ? value : throw new ArgumentOutOfRangeException(nameof(index));
        set => _storage[(index, key)] = value;
    }
}
