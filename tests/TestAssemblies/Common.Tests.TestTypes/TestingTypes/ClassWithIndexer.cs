using System;
using System.Collections.Generic;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithIndexer
{
    private readonly Dictionary<int, string> _storage = new();

    public string this[int index]
    {
        get => _storage.TryGetValue(index, out var value) ? value : throw new ArgumentOutOfRangeException(nameof(index));
        set => _storage[index] = value;
    }
}
