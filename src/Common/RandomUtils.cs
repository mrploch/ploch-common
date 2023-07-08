using System;

namespace Ploch.Common
{
    public static class RandomUtils
    {
        public static readonly Random SharedRandom = new();
    }
}