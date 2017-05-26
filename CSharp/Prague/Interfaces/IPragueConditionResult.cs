using System.Dynamic;

namespace Prague.Interfaces
{
    internal interface IPragueConditionResult { }
    internal interface IPragueConditionResult<TIn> : IPragueConditionResult where TIn : class
    {
        TIn Source { get; }
    }

    class ConditionResult<TIn> : DynamicObject, IPragueConditionResult<TIn> where TIn : class
    {
        public ConditionResult(TIn source)
        {
            this.Source = source;
        }

        public static ConditionResult<TIn> FromBoolean(TIn source, bool value) => new ConditionResult<TIn>(value ? source : default(TIn));

        public TIn Source { get; private set; }

        public static implicit operator ConditionResult<TIn>((TIn s, bool b) a) => new ConditionResult<TIn>(a.b ? a.s : default(TIn));
    }
}