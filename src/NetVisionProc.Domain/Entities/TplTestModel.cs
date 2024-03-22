namespace NetVisionProc.Domain.Entities
{
    public class TplTestModel : BaseEntity
    {
        public TplTestModel(string name, string content)
        {
            Name = name;
            Content = content;
            InitDate = DateTime.Now;
            ProcessIdentifier = Guid.NewGuid();
        }

        public string Name { get; private set; }
        
        public string Content { get; private set; }
        
        public DateTime InitDate { get; private set; }

        public DateTime ModifyDate { get; private set; }
        
        public Guid ProcessIdentifier { get; private set; }
        
        [Timestamp]
        public byte[] Version { get; private set; }
        
        // https://learn.microsoft.com/en-us/ef/core/saving/concurrency#tabpanel_1_fluent-api
        [ConcurrencyCheck]
        public Guid ConcurrencyCheckVersion { get; private set; }
        
        // mutationms
        public void RandomUpdate()
        {
            Content = "New Update content: " + Guid.NewGuid();
            ModifyDate = DateTime.Now;
        }
    }
}