namespace NetVisionProc.Common.Attributes
{
    /// <summary>
    /// Sets Range[1..int.MaxValue].
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IdRangeAttribute : RangeAttribute
    {
        public IdRangeAttribute()
            : base(1, int.MaxValue)
        {
        }
    }
}
