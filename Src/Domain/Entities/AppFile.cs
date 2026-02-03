namespace Domain.Entities;

public class AppFile
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
}
