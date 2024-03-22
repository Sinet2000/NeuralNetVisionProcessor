namespace NetVisionProc.Common.Attributes
{
    /// <summary>
    /// Sets StringLength[3..256] DataAnnotation.
    /// </summary>
    public class EmailLengthAttribute : StringLengthAttribute
    {
        public const int MaxLength = 256;
        private const int MinLength = 3;

        public EmailLengthAttribute()
            : base(MaxLength)
        {
            MinimumLength = MinLength;
        }
    }
}
