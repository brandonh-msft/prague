using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    class PragueClient
    {
        PragueClient() { }

        public static IPragueScoredRule<T> Best<T>(T param, params IPragueScoredRule<T>[] rules)
        {
            var topMatches = rules
                .Where(r => r.EvaluateAndRun(param))
                .OrderByDescending(r => r.Score);

            var retVal = topMatches.FirstOrDefault();
            retVal?.Action(param);

            return retVal;
        }

        public static IPragueRule<T> First<T>(T param, params IPragueRule<T>[] rules)
        {
            return rules.FirstOrDefault(r => r.EvaluateAndRun(param));
        }

        public static IPragueScoredRule<T, R> Best<T, R>(T param, params IPragueScoredRule<T, R>[] rules)
        {
            var topMatches = rules
                .Where(r => r.EvaluateAndRun(param))
                .OrderByDescending(r => r.Score);

            var retVal = topMatches.FirstOrDefault();
            retVal?.Action(param);

            return retVal;
        }

        public static IPragueRule<T, R> First<T, R>(T param, params IPragueRule<T, R>[] rules)
        {
            return rules.FirstOrDefault(r => r.EvaluateAndRun(param));
        }

        public dynamic Result { get; private set; }
    }
}
