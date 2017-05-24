using System;

namespace Prague.Interfaces
{

    internal interface IPragueRule<TParam>
    {
        bool Run(TParam param);
        bool ShouldRun(TParam param);

        IPragueRule<TParam> Condition { get; }
        Action<TParam> Action { get; }
    }

    internal interface IPragueRule<TParam, TResult>
    {
        TResult Run(TParam param);
        bool ShouldRun(TParam param);

        Func<TResult> Action { get; }

        TResult Result { get; }
    }

}