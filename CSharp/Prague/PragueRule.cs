using System;
using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    abstract class PragueRule : IPragueRule
    {
        protected PragueRule(Delegate action, params Delegate[] conditions)
        {
            this.Action = action;
            this.Conditions = conditions ?? new Delegate[0];
        }

        public static PragueRule<TParamIn> Create<TParamIn, TFinalParam>(Action<TFinalParam> action, PraguePredicate<TParamIn, TFinalParam> condition)
            where TParamIn : class
            where TFinalParam : class, IPragueConditionResult<TParamIn>
            => new PragueRule<TParamIn>(action, condition);
        //public static PragueRule Create<TParamIn, T2, TFinalParam>(Action<TFinalParam> action, PraguePredicate<TParamIn, T2> condition1, PraguePredicate<T2, TFinalParam> condition2)
        //    where TParamIn : class
        //    where T2 : class, IPragueConditionResult<TParamIn>
        //    where TFinalParam : IPragueConditionResult<T2>
        //    => new PragueRule((Action<TFinalParam>)action, condition1, condition2);
        //public static PragueRule Create<TParamIn, T2, T3>(Action<IPragueConditionResult> action, PraguePredicate<TParamIn, IPragueConditionResult<TParamIn>> condition1, PraguePredicate<T2, IPragueConditionResult<T2>> condition2, PraguePredicate<T3, IPragueConditionResult<T3>> condition3)
        //    where T2 : class
        //    where T3 : class
        //    => new PragueRule(action, condition1, condition2, condition3);
        //public static PragueRule Create<TParamIn, T2, T3, T4>(Action<IPragueConditionResult> action, PraguePredicate<TParamIn, IPragueConditionResult<TParamIn>> condition1, PraguePredicate<T2, IPragueConditionResult<T2>> condition2, PraguePredicate<T3, IPragueConditionResult<T3>> condition3, PraguePredicate<T4, IPragueConditionResult<T4>> condition4)
        //    where T2 : class
        //    where T3 : class
        //    where T4 : class
        //    => new PragueRule(action, condition1, condition2, condition3, condition4);
        //public static PragueRule Create<TParamIn, T2, T3, T4>(Action<IPragueConditionResult> action, PraguePredicate<TParamIn, IPragueConditionResult<TParamIn>> condition1, PraguePredicate<T2, IPragueConditionResult<T2>> condition2, PraguePredicate<T3, IPragueConditionResult<T3>> condition3, PraguePredicate<T4, IPragueConditionResult<T4>> condition4, params PraguePredicate<dynamic, IPragueConditionResult<dynamic>>[] moreConditions)
        //    where T2 : class
        //    where T3 : class
        //    where T4 : class
        //    => new PragueRule(action, new dynamic[] { condition1, condition2, condition3, condition4 }.Concat(moreConditions).ToArray());

        public IList<Delegate> Conditions { get; private set; } = new List<Delegate>();

        public Delegate Action { get; private set; } = new Action(() => { });
    }

    class PragueRule<TParamIn> : PragueRule, IPragueRule<TParamIn>
    {
        internal PragueRule(Delegate action, params Delegate[] conditions) : base(action, conditions) { }

        public IPragueRuleResult TryRun(TParamIn param)
        {
            dynamic nextParam = param;
            if (this.Conditions.Any())
            {
                foreach (var c in this.Conditions)
                {
                    nextParam = c.DynamicInvoke(nextParam);
                    if (nextParam == null) return null;
                }
            }

            if (!object.ReferenceEquals(null, nextParam))
            {
                return new PragueRuleResult { Action = () => this.Action?.DynamicInvoke(nextParam) };
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