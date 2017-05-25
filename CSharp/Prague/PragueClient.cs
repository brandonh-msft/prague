﻿using System.Linq;
using Prague.Interfaces;

namespace Prague
{
    static class PragueClient
    {
        //public static IPragueScoredRule<T> Best<T>(T param, params IPragueScoredRule<T>[] rules)
        //{
        //    var topMatches = rules
        //        .Where(r => r.Run(param))
        //        .OrderByDescending(r => r.Score);

        //    var retVal = topMatches.FirstOrDefault();
        //    retVal?.Action(param);

        //    return retVal;
        //}

        //public static IPragueRule<T> First<T>(T param, params IPragueRule<T>[] rules)
        //{
        //    return rules.FirstOrDefault(r => r.Run(param));
        //}

        public static IPragueScoredRule<T, R> Best<T, R>(T param, params IPragueScoredRule<T, R>[] rules) where R : class
        {
            var topMatches = rules
                .Where(r => r.Run(param) != default(R))
                .OrderByDescending(r => r.Score);

            var retVal = topMatches.FirstOrDefault();
            retVal?.Action(param);

            return retVal;
        }

        public static IPragueRule<T, R> First<T, R>(T param, params IPragueRule<T, R>[] rules) where R : class
        {
            return rules.FirstOrDefault(r => r.Run(param) != default(R));
        }
    }
}