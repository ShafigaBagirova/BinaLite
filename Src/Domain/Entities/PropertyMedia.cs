namespace Domain.Entities;

public class PropertyMedia: BaseEntity<int>
{
    public string MediaUrl { get; set; }
    public string MediaType { get; set; } 
    public int Order { get; set; }
    public int PropertyAdId { get; set; }
    public PropertyAd? PropertyAd { get; set; }

}
