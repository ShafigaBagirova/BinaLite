
using Application.Dtos.PropertyAdDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class PropertyAdProfile:Profile
{
    public PropertyAdProfile()
    {
        CreateMap<PropertyAd, GetAllPropertyAdResponse>()
    .ForMember(d => d.FirstMediaObjectKey,
        o => o.MapFrom(s => s.MediaItems
            .OrderBy(m => m.Order)
            .Select(m => m.ObjectKey)
            .FirstOrDefault()));
        CreateMap<PropertyAd, GetByIdPropertyAdResponse>();
        CreateMap<CreatePropertyAdRequest, PropertyAd>();
        CreateMap<PropertyMedia, PropertyAdMediaItemDto>();
        CreateMap<UpdatePropertyAdRequest, PropertyAd>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
