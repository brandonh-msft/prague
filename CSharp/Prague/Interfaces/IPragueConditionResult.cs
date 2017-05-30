﻿namespace Prague.Interfaces
{
    internal interface IPragueConditionResult
    {
        double Score { get; }
    }
    internal interface IPragueConditionResult<TIn> : IPragueConditionResult
    {
        TIn Source { get; }
    }

    abstract class ConditionResult : IPragueConditionResult
    {
        public double Score { get; } = 1d;

        public static ConditionResult<TIn> FromBoolean<TIn>(TIn source, bool value) => new ConditionResult<TIn>(value ? source : default(TIn));
    }

    class ConditionResult<TIn> : ConditionResult, IPragueConditionResult<TIn>
    {
        protected internal ConditionResult(TIn source)
        {
            this.Source = source;
        }


        public TIn Source { get; private set; }

        public static implicit operator ConditionResult<TIn>((TIn s, bool b) a) => new ConditionResult<TIn>(a.b ? a.s : default(TIn));
    }
}