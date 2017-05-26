using Prague.Interfaces;

namespace Prague
{
    delegate TOut PraguePredicate<TIn, out TOut>(TIn param)
        where TIn : class
        where TOut : IPragueConditionResult<TIn>;
}
