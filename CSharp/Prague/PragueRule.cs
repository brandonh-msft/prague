using System;
using System.Collections.Generic;
using Prague.Interfaces;

namespace Prague
{
    class PragueRule<T> : IPragueRule<T>
    {
        public PragueRule(IPragueRule<T> condition, Action<T> action) : this(action)
        {
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        private Predicate<T> _predicate;
        public PragueRule(Predicate<T> condition, Action<T> action)
        {
            _predicate = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        protected PragueRule(Action<T> action)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public IPragueRule<T> Condition { get; private set; }

        public Action<T> Action { get; private set; } = t => { };

        public bool Run(T param)
        {
            var retVal = ShouldRun(param);
            if (retVal)
            {
                Action(param);
            }

            return retVal;
        }

        public bool ShouldRun(T param)
        {
            return _predicate?.Invoke(param) ?? this.Condition.Run(param);
        }
    }

    class PragueRule<TParam, TResult> : IPragueRule<TParam, TResult> where TResult : class
    {
        private PragueRule(Func<TResult> action)
        {
            this.Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public static PragueRule<TParam, TResult> Create(IPragueRule<TParam, TResult> condition, Func<TResult> action)
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition);

            return p;
        }
        public static PragueRule<TParam, TResult> Create<T2>(IPragueRule<TParam, T2> condition1, IPragueRule<T2, TResult> condition2, Func<TResult> action) where T2 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition1);

            return p;
        }
        public static PragueRule<TParam, TResult> Create<T2, T3>(IPragueRule<TParam, T2> condition1, IPragueRule<T2, T3> condition2, IPragueRule<T3, TResult> condition3, Func<TResult> action) where T2 : class where T3 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition2);
            p.Conditions.Add(condition3);

            return p;
        }
        public static PragueRule<TParam, TResult> Create<T2, T3, T4>(IPragueRule<TParam, T2> condition1, IPragueRule<T2, T3> condition2, IPragueRule<T3, T4> condition3, IPragueRule<T4, TResult> condition4, Func<TResult> action) where T2 : class where T3 : class where T4 : class
        {
            var p = new PragueRule<TParam, TResult>(action);

            p.Conditions.Add(condition1);
            p.Conditions.Add(condition2);
            p.Conditions.Add(condition3);
            p.Conditions.Add(condition4);

            return p;
        }

        private IList<dynamic> Conditions { get; } = new List<dynamic>();

        public Func<TResult> Action { get; private set; } = () => default(TResult);

        public TResult Result { get; private set; } = default(TResult);

        public TResult Run(TParam param)
        {
            if (ShouldRun(param))
            {
                dynamic nextParam = param;
                foreach (var d in this.Conditions)
                {
                    nextParam = d.Run(nextParam);
                    if (object.ReferenceEquals(null, nextParam)) return null;
                }

                return (TResult)nextParam;
            }

            return default(TResult);
        }

        public bool ShouldRun(TParam param)
        {
            throw new NotImplementedException();
        }
    }

}