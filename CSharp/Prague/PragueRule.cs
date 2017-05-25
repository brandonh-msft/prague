using System;
using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    //class PragueRule<T> : IPragueRule<T>
    //{
    //    public PragueRule(IPragueRule<T> condition, Action<T> action) : this(action)
    //    {
    //        this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
    //    }

    //    private Predicate<T> _predicate;
    //    public PragueRule(Predicate<T> condition, Action<T> action)
    //    {
    //        _predicate = condition ?? throw new ArgumentNullException(nameof(condition));
    //    }

    //    protected PragueRule(Action<T> action)
    //    {
    //        this.Action = action ?? throw new ArgumentNullException(nameof(action));
    //    }

    //    public IPragueRule<T> Condition { get; private set; }

    //    public Action<T> Action { get; private set; } = t => { };

    //    public bool Run(T param)
    //    {
    //        var retVal = ShouldRun(param);
    //        if (retVal)
    //        {
    //            Action(param);
    //        }

    //        return retVal;
    //    }

    //    public bool ShouldRun(T param)
    //    {
    //        return _predicate?.Invoke(param) ?? this.Condition.Run(param);
    //    }

    //    public static implicit operator PragueRule<T>(Action<T> action) => new PragueRule<T>(action);
    //}

    class PragueRule<TParam, TResult> : IPragueRule<TParam, TResult> where TResult : class
    {
        private TResult _conditionEvalResult;

        private PragueRule(Func<TResult, TResult> action)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public static PragueRule<TParam, TResult> Create(Func<TResult, TResult> action) => new PragueRule<TParam, TResult>(action);


        public static PragueRule<TParam, TResult> Create(Func<TResult, TResult> action, IPragueRule<TParam, TResult> condition)
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition);

            return p;
        }

        public static PragueRule<TParam, TResult> Create<T2>(Func<TResult, TResult> action, IPragueRule<TParam, T2> condition1, IPragueRule<T2, TResult> condition2)
            where T2 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition2);

            return p;
        }

        public static PragueRule<TParam, TResult> Create<T2, T3>(Func<TResult, TResult> action, IPragueRule<TParam, T2> condition1, IPragueRule<T2, T3> condition2, IPragueRule<T3, TResult> condition3)
            where T2 : class
            where T3 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition2);
            p.Conditions.Add(condition3);

            return p;
        }

        public static PragueRule<TParam, TResult> Create<T2, T3, T4>(Func<TResult, TResult> action, IPragueRule<TParam, T2> condition1, IPragueRule<T2, T3> condition2, IPragueRule<T3, T4> condition3, IPragueRule<T4, TResult> condition4)
            where T2 : class
            where T3 : class
            where T4 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition2);
            p.Conditions.Add(condition3);
            p.Conditions.Add(condition4);

            return p;
        }

        public static PragueRule<TParam, TResult> Create<T2, T3, T4, T5>(Func<TResult, TResult> action, IPragueRule<TParam, T2> condition1, IPragueRule<T2, T3> condition2, IPragueRule<T3, T4> condition3, IPragueRule<T4, TResult> condition4, params IPragueRule[] otherConditions)
            where T2 : class
            where T3 : class
            where T4 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition2);
            p.Conditions.Add(condition3);
            p.Conditions.Add(condition4);

            foreach (var c in otherConditions) p.Conditions.Add(c);

            return p;
        }

        private IList<dynamic> Conditions { get; } = new List<dynamic>();

        public dynamic Action { get; private set; } = new Func<TParam, TResult>(t => t as TResult);

        public TResult Result { get; private set; } = default(TResult);

        public TResult Run(TParam param) => ShouldRun(param) ? (this.Result = Action(_conditionEvalResult)) : (this.Result = default(TResult));

        public bool ShouldRun(TParam param)
        {
            dynamic nextParam = param;
            if (this.Conditions.Any())
            {
                foreach (var d in this.Conditions)
                {
                    nextParam = d.Run(nextParam);
                    if (object.ReferenceEquals(null, nextParam)) return false;
                }
            }
            _conditionEvalResult = (TResult)nextParam;

            return true;
        }

        public static implicit operator PragueRule<TParam, TResult>(Func<TResult, TResult> func) => new PragueRule<TParam, TResult>(func);
    }
}