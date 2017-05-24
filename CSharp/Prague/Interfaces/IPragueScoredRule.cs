using Prague.Interfaces;

namespace Prague
{
    internal interface IPragueScoredRule<T> : IPragueRule<T>
    {
        double Score { get; }
    }
}