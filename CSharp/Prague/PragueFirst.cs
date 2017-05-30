using System;
using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    class PragueFirst
    {
        private readonly IList<IPragueRule> _rules;
        private PragueFirst(IList<IPragueRule> rules)
        {
            _rules = rules;
        }

        public static IPragueRuleResult Create<TParam, TResult>(TParam param, Action<TResult> action, params PraguePredicate<TParam, TResult>[] rules)
            where TParam : class
            where TResult : class, IPragueConditionResult<TParam>
        {
            return new PragueFirst(rules.Select(r => PragueRule.Create(action, r) as IPragueRule).ToList()).Run(param);
        }

        private IPragueRuleResult Run<TParam>(TParam param)
        {
            foreach (IPragueRule<TParam> r in this._rules)
            {
                var conditionResult = r.TryRun(param);
                if (conditionResult != null) return conditionResult;
            }

            return null;
        }
    }
}