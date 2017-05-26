using System;
using System.Collections.Generic;

namespace Prague.Interfaces
{
    internal interface IPragueRule
    {
        IList<Delegate> Conditions { get; }
        Delegate Action { get; }
    }

    internal interface IPragueRule<TParamIn> : IPragueRule
    {
        IPragueRuleResult TryRun(TParamIn param);
    }

    internal interface IPragueRuleResult
    {
        double Score { get; }

        Action Action { get; }
    }
}