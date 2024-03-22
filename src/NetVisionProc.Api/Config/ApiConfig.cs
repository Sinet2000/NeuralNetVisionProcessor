namespace NetVisionProc.Api.Config
{
    public record ApiConfig
    {
        public const string SectionName = "Api";
        public bool AddMissingMemberHandling { get; set; }
        public string[] AllowedOrigins { get; set; } = null!;
    }
}