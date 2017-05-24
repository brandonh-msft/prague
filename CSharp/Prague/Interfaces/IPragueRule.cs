using System;
using System.Collections.Generic;

namespace Prague.Interfaces
{

    internal interface IPragueRule<TParam>
    {
        IPragueRule<TParam> First(TParam param, params IPragueRule<TParam>[] rules);
        IPragueScoredRule<TParam> Best(TParam param, params IPragueScoredRule<TParam>[] rules);

        IPragueRule<TParam, TResult> First<TResult>(TParam param, params IPragueRule<TParam, TResult>[] rules);
        IPragueScoredRule<TParam, TResult> Best<TResult>(TParam param, params IPragueScoredRule<TParam, TResult>[] rules);

        bool EvaluateAndRun(TParam param);
        bool Evaluate(TParam param);

        IList<Predicate<TParam>> Conditions { get; }
        Action<TParam> Action { get; }
    }

    internal interface IPragueRule<TParam, TResult> : IPragueRule<TParam>
    {
        new Func<TParam, TResult> Action { get; }
        TResult Result { get; }
    }

}