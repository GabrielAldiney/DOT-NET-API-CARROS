using AutoMapper;
using FirstAPI.Domain.DTOs;
using FirstAPI.Domain.Model.CarroAggregate;

public class DomainToDTOMapping : Profile
{
    public DomainToDTOMapping()
    {
        CreateMap<Carro, CarroDTO>()
            .ForMember(dest => dest.NameCarro, opt => opt.MapFrom(src => src.name))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.photo));
    }
}
