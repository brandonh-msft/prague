
namespace Prague.Interfaces
{
    internal interface IPragueScoredRule<TParam> : IPragueRule<TParam>
    {
        double Score { get; }
    }

    internal interface IPragueScoredRule<TParam, TResult> : IPragueRule<TParam, TResult> where TResult : class
    {
        double Score { get; }
    }
}