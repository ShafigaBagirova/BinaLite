using Domain.Enums;

namespace Application.Dtos;

public class GetByIdPropertyAdResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsNew { get; set; }
    public bool IsRenovated { get; set; }
    public bool IsMortgage { get; set; }
    public bool IsTitleDeedAvailable { get; set; }
    public int RoomCount { get; set; }
    public double AreaInSquareMeters { get; set; }
    public string Location { get; set; }
    public int? FloorNumber { get; set; }
    public int? TotalFloors { get; set; }
    public PropertyCategory PropertyCategory { get; set; }
    public OfferType OfferType { get; set; }
}
