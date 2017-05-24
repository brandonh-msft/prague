using System;
using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    class PragueRule<T> : IPragueRule<T>
    {
        public PragueRule(Action<T> action, params Predicate<T>[] conditions) : this(conditions)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        protected PragueRule(params Predicate<T>[] conditions)
        {
            if (conditions.Any()) this.Conditions = conditions;
        }

        public IList<Predicate<T>> Conditions { get; private set; } = new List<Predicate<T>>();

        public Action<T> Action { get; private set; } = (t) => { };

        public IPragueScoredRule<T> Best(T param, params IPragueScoredRule<T>[] rules)
        {
            var topMatches = rules
                .Where(r => r.EvaluateAndRun(param))
                .OrderByDescending(r => r.Score);

            var retVal = topMatches.FirstOrDefault();
            retVal?.Action(param);

            return retVal;
        }

        public IPragueScoredRule<T, TResult> Best<TResult>(T param, params IPragueScoredRule<T, TResult>[] rules)
        {
            var topMatches = rules
                .Where(r => r.EvaluateAndRun(param))
                .OrderByDescending(r => r.Score);

            var retVal = topMatches.FirstOrDefault();
            retVal?.Action(param);

            return retVal;
        }

        public bool Evaluate(T param)
        {
            return this.Conditions?.All(c => c(param)) == true;
        }

        public virtual bool EvaluateAndRun(T param)
        {
            var retVal = Evaluate(param);
            if (retVal) this.Action(param);

            return retVal;
        }

        public IPragueRule<T> First(T param, params IPragueRule<T>[] rules)
        {
            return rules.FirstOrDefault(r => r.EvaluateAndRun(param));
        }

        public IPragueRule<T, TResult> First<TResult>(T param, params IPragueRule<T, TResult>[] rules)
        {
            return rules.FirstOrDefault(r => r.EvaluateAndRun(param));
        }
    }

    class PragueRule<TParam, TResult> : PragueRule<TParam>, IPragueRule<TParam, TResult>
    {
        public PragueRule(Func<TParam, TResult> action, params Predicate<TParam>[] conditions) : base(conditions)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public override bool EvaluateAndRun(TParam param)
        {
            var retVal = Evaluate(param);
            if (retVal)
            {
                this.Result = this.Action(param);
            }

            return retVal;
        }

        public TResult Result { get; private set; }

        new public Func<TParam, TResult> Action { get; private set; }
    }
}
