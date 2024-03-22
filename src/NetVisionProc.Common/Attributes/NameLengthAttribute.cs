namespace NetVisionProc.Common.Attributes
{
    /// <summary>
    /// Sets StringLength[1..256] DataAnnotation.
    /// </summary>
    public class NameLengthAttribute : StringLengthAttribute
    {
        public const int MaxLength = 256;
        private const int MinLength = 1;

        public NameLengthAttribute()
            : base(MaxLength)
        {
            MinimumLength = MinLength;
        }
    }
}
