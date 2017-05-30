using System;
using Prague.Interfaces;

namespace Prague
{
    static class PragueRule
    {
        public static PragueRule<TParam, TResult> Create<TParam, TResult>(Action<TResult> action, PraguePredicate<TParam, TResult> condition)
            where TResult : IPragueConditionResult<TParam>
            => new PragueRule<TParam, TResult>(action, condition);
    }

    class PragueRule<TParam, TResult> : IPragueRule<TParam> where TResult : IPragueConditionResult<TParam>
    {
        internal PragueRule(Action<TResult> action, PraguePredicate<TParam, TResult> condition)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
            this.Condition = condition;
        }

        public PraguePredicate<TParam, TResult> Condition { get; private set; } = null;

        public Action<TResult> Action { get; } = t => { };

        PraguePredicate<TParam, IPragueConditionResult<TParam>> IPragueRule<TParam>.Condition => throw new NotImplementedException();

        Action<IPragueConditionResult<TParam>> IPragueRule<TParam>.Action => throw new NotImplementedException();

        public IPragueRuleResult TryRun(TParam param)
        {
            var result = this.Condition == null ? default(TResult) : this.Condition(param);

            if (!ReferenceEquals(default(TResult), result))
            {
                return new PragueRuleResult
                {
                    Action = () => this.Action(result),
                    Score = result.Score,
                };
            }
            else
            {
                return null;
            }
        }
    }

    class PragueRuleResult : IPragueRuleResult
    {
        public double Score { get; internal set; } = 1d;

        public Action Action { get; internal set; }
    }
}