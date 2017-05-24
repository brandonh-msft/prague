using System;
using System.Collections.Generic;

namespace Prague.Interfaces
{
    interface IPragueRule<T>
    {
        void First(IList<IPragueRule> rules);
        void Best(IList<IPragueRule> rules);

        bool EvaluateAndRun(T param);
        IList<Predicate<T>> Conditions { get; }
        Action<T> Action { get; }
    }
}
