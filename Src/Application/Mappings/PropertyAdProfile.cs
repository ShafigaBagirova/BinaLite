using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class PropertyAdProfile:Profile
{
    public PropertyAdProfile()
    {
        CreateMap<PropertyAd, GetAllPropertyAdResponse>();
        CreateMap<PropertyAd, GetByIdPropertyAdResponse>();
        CreateMap<CreatePropertyAdRequest, PropertyAd>();
        CreateMap<UpdatePropertyAdRequest, PropertyAd>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
