using NetVisionProc.Common.Attributes;
using NetVisionProc.Domain.Enums;

namespace NetVisionProc.Domain.Entities
{
    public class ProcessedImageBlobItem : BaseEntity
    {
        [NameLength]
        public string Identifier { get; private set; } = null!;
        
        public string SourcePath { get; private set; }
        
        public string BlobType { get; private set; }
        
        public FileExtension? Extension { get; private set; }
        
        public string? ResultPath { get; private set; }
        
        public DateTime CreatedDate { get; private set; }
        
        public string? Description { get; private set; }
    }
}