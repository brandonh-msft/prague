using System;
using System.Collections.Generic;

namespace Prague.Interfaces
{
    internal interface IPragueRule
    {
        IList<dynamic> Conditions { get; }
    }

    internal interface IPragueRule<TParamIn, TFinalParam> : IPragueRule
        where TFinalParam : class
    {
        Action<TFinalParam> Action { get; }
        IPragueRuleResult TryRun(TParamIn param);
    }

    internal interface IPragueRule<TParamIn> : IPragueRule<TParamIn, object> { }

    internal interface IPragueRuleResult
    {
        double Score { get; }

        Action Action { get; }
    }
}