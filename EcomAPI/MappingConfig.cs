using AutoMapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;

namespace EcomAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<PatchProductCategoryDTO, ProductCategory>().ReverseMap();
            CreateMap<PatchProductCategoryDTO, ProductCategory>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}