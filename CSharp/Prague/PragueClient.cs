using System.Collections.Generic;
using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    class PragueClient
    {
        PragueClient() { }

        static IPragueScoredRule<T> Best<T>(IList<IPragueScoredRule<T>> rules, T param)
        {
            var topMatches = rules
                .Where(r => r.EvaluateAndRun(param))
                .OrderByDescending(r => r.Score);

            var retVal = topMatches.FirstOrDefault();
            retVal?.Action(param);

            return retVal;
        }

        static IPragueRule<T> First<T>(IList<IPragueRule<T>> rules, T param)
        {
            return rules.FirstOrDefault(r => r.EvaluateAndRun(param));
        }

        public dynamic Result { get; private set; }
    }
}
