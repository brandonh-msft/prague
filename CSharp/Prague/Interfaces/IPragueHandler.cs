namespace Prague.Interfaces
{
    public interface IPragueHandler<TMatch> where TMatch : IPragueMatch
    {
        TMatch Match { get; }
    }
}
