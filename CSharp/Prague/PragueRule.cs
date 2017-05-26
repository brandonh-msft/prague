using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prague.Interfaces;

namespace Prague
{
    class PragueRule<TParamIn, TFinalParam> : IPragueRule<TParamIn, TFinalParam> where TFinalParam : class
    {
        private PragueRule(Action<TFinalParam> action, params dynamic[] conditions)
        {
            this.Action = action;
            this.Conditions = conditions ?? new dynamic[0];
        }

        public static PragueRule<TParamIn, TFinalParam> Create(Action<TFinalParam> action, PraguePredicate<TParamIn, TFinalParam> condition) => new PragueRule<TParamIn, TFinalParam>(action, condition);
        public static PragueRule<TParamIn, TFinalParam> Create<T2>(Action<TFinalParam> action, PraguePredicate<TParamIn, T2> condition1, PraguePredicate<T2, TFinalParam> condition2) where T2 : class => new PragueRule<TParamIn, TFinalParam>(action, condition1, condition2);
        public static PragueRule<TParamIn, TFinalParam> Create<T2, T3>(Action<TFinalParam> action, PraguePredicate<TParamIn, T2> condition1, PraguePredicate<T2, T3> condition2, PraguePredicate<T3, TFinalParam> condition3)
            where T2 : class
            where T3 : class
            => new PragueRule<TParamIn, TFinalParam>(action, condition1, condition2, condition3);
        public static PragueRule<TParamIn, TFinalParam> Create<T2, T3, T4>(Action<TFinalParam> action, PraguePredicate<TParamIn, T2> condition1, PraguePredicate<T2, T3> condition2, PraguePredicate<T3, T4> condition3, PraguePredicate<T4, TFinalParam> condition4)
            where T2 : class
            where T3 : class
            where T4 : class
            => new PragueRule<TParamIn, TFinalParam>(action, condition1, condition2, condition3, condition4);
        public static PragueRule<TParamIn, TFinalParam> Create<T2, T3, T4>(Action<TFinalParam> action, PraguePredicate<TParamIn, T2> condition1, PraguePredicate<T2, T3> condition2, PraguePredicate<T3, T4> condition3, PraguePredicate<T4, dynamic> condition4, params PraguePredicate<dynamic, dynamic>[] moreConditions)
            where T2 : class
            where T3 : class
            where T4 : class
            => new PragueRule<TParamIn, TFinalParam>(action, new dynamic[] { condition1, condition2, condition3, condition4 }.Concat(moreConditions).ToArray());

        public IList<dynamic> Conditions { get; private set; } = new List<dynamic>();

        public Action<TFinalParam> Action { get; private set; } = t => { };

        public IPragueRuleResult TryRun(TParamIn param)
        {
            dynamic nextParam = param;
            if (this.Conditions.Any())
            {
                foreach (var c in this.Conditions)
                {
                    nextParam = c(nextParam);
                    if (nextParam == null) return null;
                }
            }

            if (!object.ReferenceEquals(null, nextParam))
            {
                return new PragueRuleResult { Action = () => this.Action(nextParam as TFinalParam) };
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

    class PragueFirst<TParam>
    {
        private IList<dynamic> _rules;
        private PragueFirst(IList<dynamic> list)
        {
            _rules = list;
        }

        public static IPragueRuleResult Create<TLast>(TParam param, IPragueRule<TParam, TLast> rule1) where TLast : class
        {
            return new PragueFirst<TParam>(new List<dynamic> { rule1 })
                .Run(param);
        }

        private IPragueRuleResult Run(TParam param)
        {
            foreach (object r in this._rules)
            {
                var conditionResult = r.GetType().GetMethod(@"TryRun").Invoke(r, new object[] { param }) as IPragueRuleResult;
                if (!object.ReferenceEquals(null, conditionResult)) return conditionResult as IPragueRuleResult;
            }

            return null;
        }

    }
}