using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, O => O.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, O => O.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<Talabat.Core.Entities.Identity.Address, AddressDto>();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto , BasketItem>();
            CreateMap<AddressDto , Talabat.Core.Entities.Order_Aggregate.Address>();
        }
    }
}
