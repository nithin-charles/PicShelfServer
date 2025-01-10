namespace PicShelfServer.Models.Domain
{
    public class Folder
    {
        public Guid Id { get; set; }

        public required string FolderName { get; set; }
        
        public bool IsEmpty { get; set; } = true;
    }
}
