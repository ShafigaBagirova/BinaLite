using Domain.Enums;

namespace Application.Dtos;

public class CreatePropertyAdRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsNew { get; set; }
    public bool IsRenovated { get; set; }
    public bool IsMortgage { get; set; }
    public bool IsTitleDeedAvailable { get; set; }
    public int RoomCount { get; set; }
    public decimal AreaInSquareMeters { get; set; }
    public string Location { get; set; }
    public int? FloorNumber { get; set; }
    public int? TotalFloors { get; set; }
    public PropertyCategory PropertyCategory { get; set; }
    public OfferType OfferType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
