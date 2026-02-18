namespace Application.Abstracts.Services;

public interface IPropertyModerationService
{
    Task ApproveAsync(int propertyId, CancellationToken ct);
    Task RejectAsync(int propertyId, string reason, CancellationToken ct);
}
