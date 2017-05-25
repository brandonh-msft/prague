using System;

namespace Prague.Interfaces
{
    internal interface IPragueRule { }

    internal interface IPragueRule<TParam> : IPragueRule
    {
        bool Run(TParam param);
        bool ShouldRun(TParam param);

        IPragueRule<TParam> Condition { get; }
        Action<TParam> Action { get; }
    }

    internal interface IPragueRule<TParam, TResult> where TResult : class
    {
        TResult Run(TParam param);
        bool ShouldRun(TParam param);

        dynamic Action { get; }
        TResult Result { get; }
    }
}