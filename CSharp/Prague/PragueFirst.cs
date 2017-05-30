using System;
using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    static class PragueFirst
    {
        public static PragueFirstCreator Create => new PragueFirstCreator();
    }

    class PragueFirstCreator
    {
        internal PragueFirstCreator() { }

        public PragueFirst<TParam> WithParam<TParam>(TParam param) => new PragueFirst<TParam>(new List<IPragueRule<TParam>>(), param);
        public IPragueRuleResult Easy<TParam, TResult>(TParam param, Action<TResult> action, params PraguePredicate<TParam, TResult>[] rules) where TResult : IPragueConditionResult<TParam>
        {
            return new PragueFirst<TParam>(rules.Select(r => PragueRule.Create(action, r) as IPragueRule<TParam>).ToList()).TryRun(param);
        }
    }

    class PragueFirst<TParam> : IPragueRule<TParam>
    {
        private TParam _globalParam;

        internal PragueFirst(IList<IPragueRule<TParam>> rules)
        {
            this.Rules = rules ?? throw new ArgumentException(nameof(rules));
        }

        public PragueFirst(List<IPragueRule<TParam>> list, TParam param) : this(list)
        {
            _globalParam = param;
        }

        public PragueFirst<TParam> WithRule<TResult>(Action<TResult> action, PraguePredicate<TParam, TResult> condition) where TResult : IPragueConditionResult<TParam>
        {
            this.Rules.Add(PragueRule.Create(action, condition) as IPragueRule<TParam>);
            return this;
        }

        public IPragueRuleResult TryRun(TParam param)
        {
            foreach (var r in this.Rules)
            {
                var conditionResult = r.TryRun(param);
                if (conditionResult != null)
                {
                    _evaluatedAction = conditionResult.Action;
                    return conditionResult;
                }
            }

            return null;
        }

        public IPragueRuleResult TryRun() => TryRun(_globalParam);

        internal IList<IPragueRule<TParam>> Rules { get; } = new List<IPragueRule<TParam>>();

        private Action _evaluatedAction = null;
        public Action Action => _evaluatedAction ?? TryRun(_globalParam)?.Action;

        PraguePredicate<TParam, IPragueConditionResult<TParam>> IPragueRule<TParam>.Condition => throw new NotImplementedException();

        Action<IPragueConditionResult<TParam>> IPragueRule<TParam>.Action => throw new NotImplementedException();

        //public PraguePredicate<TParam, IPragueConditionResult<TParam>> Condition => throw new NotImplementedException();

        //public Action<IPragueConditionResult<TParam>> Action => throw new NotImplementedException();
    }
}