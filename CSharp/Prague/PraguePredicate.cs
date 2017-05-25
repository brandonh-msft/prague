namespace Prague
{
    public delegate TOut PraguePredicate<TIn, out TOut>(TIn param) where TOut : class;
}
