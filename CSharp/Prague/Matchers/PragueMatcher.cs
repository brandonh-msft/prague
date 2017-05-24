using System;
using Prague.Interfaces;

namespace Prague.Matchers
{
    class PragueMatcher<TMatchResult, TMatch>
        where TMatchResult : IPragueMatchResult<TMatch>
        where TMatch : IPragueMatch
    {
        private PragueMatcher() { }

        public static PragueMatcher<TMatchResult, TMatch> Create(Func<TMatch, TMatchResult> evaluate)
        {
            return new PragueMatcher<TMatchResult, TMatch>
            {
                Evaluate = evaluate
            };
        }

        public Func<TMatch, TMatchResult> Evaluate { get; protected set; }
    }
}
