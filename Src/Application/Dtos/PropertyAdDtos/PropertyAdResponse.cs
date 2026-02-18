using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos.PropertyAdDtos;

public class PropertyAdResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public PropertyStatus Status { get; set; } 
    public string? RejectionReason { get; set; }
}
