
namespace Prague.Interfaces
{
    internal interface IPragueScoredRule<TParam> : IPragueRule<TParam>
    {
        double Score { get; }
    }
}