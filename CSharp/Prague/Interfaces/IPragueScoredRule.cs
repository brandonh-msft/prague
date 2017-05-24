
namespace Prague.Interfaces
{
    internal interface IPragueScoredRule<TParam> : IPragueRule<TParam>
    {
        double Score { get; }
    }

    internal interface IPragueScoredRule<TParam, TResult> : IPragueScoredRule<TParam>, IPragueRule<TParam, TResult> { }
}