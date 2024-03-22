namespace NetVisionProc.Common.Attributes
{
    /// <summary>
    /// Sets StringLength[1..2048] DataAnnotation.
    /// </summary>
    public class TextLengthAttribute : StringLengthAttribute
    {
        public const int MaxLength = 2048;
        private const int MinLength = 1;

        public TextLengthAttribute()
            : base(MaxLength)
        {
            MinimumLength = MinLength;
        }
    }
}
