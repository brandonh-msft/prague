using System;

namespace Prague.Interfaces
{
    internal interface IPragueRule<TParam>
    {
        IPragueRuleResult TryRun(TParam param);
        PraguePredicate<TParam, IPragueConditionResult<TParam>> Condition { get; }
        Action<IPragueConditionResult<TParam>> Action { get; }
    }

    internal interface IPragueRuleResult : IPragueConditionResult
    {
        Action Action { get; }
    }
}