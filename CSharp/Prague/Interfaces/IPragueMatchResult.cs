namespace Prague.Interfaces
{
    interface IPragueMatchResult<TMatch> where TMatch : IPragueMatch
    {
        TMatch Match { get; }
    }
}
