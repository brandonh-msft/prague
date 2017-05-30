using System;
using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    class PragueBest
    {
        private readonly IList<IPragueRule> _rules;

        public PragueBest(IList<IPragueRule> rules)
        {
            _rules = rules;
        }

        public static IPragueRuleResult Create<TParam, TResult>(TParam param, Action<TResult> action, params PraguePredicate<TParam, TResult>[] rules)
            where TParam : class
            where TResult : class, IPragueConditionResult<TParam>
        {
            return new PragueBest(rules.Select(r => PragueRule.Create(action, r) as IPragueRule).ToList()).Run(param);
        }

        private IPragueRuleResult Run<TParam>(TParam param) =>
            _rules
                .OfType<IPragueRule<TParam>>()
                .Select(r => r.TryRun(param))
                .Where(r => r != null)
                .OrderByDescending(r => r.Score)
                .FirstOrDefault();
    }
}