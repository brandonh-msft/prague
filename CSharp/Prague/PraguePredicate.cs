using Prague.Interfaces;

namespace Prague
{
    delegate TOut PraguePredicate<TIn, out TOut>(TIn param)
        where TOut : IPragueConditionResult<TIn>;
}
