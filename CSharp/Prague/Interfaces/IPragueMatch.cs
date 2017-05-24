using System;

namespace Prague.Interfaces
{
    public interface IPragueMatch
    {
        Action<string> Reply { get; }
    }
}