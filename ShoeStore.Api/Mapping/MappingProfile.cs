using AutoMapper;
using ShoeStore.Api.DTOs;
using ShoeStoreData.Models;

namespace ShoeStore.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();

        CreateMap<Subcategory, SubcategoryDto>()
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<Material, MaterialDto>();

        CreateMap<ProductItem, ProductItemDto>();

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.SubcategoryName,
                o => o.MapFrom(s => s.Subcategory != null ? s.Subcategory.Name : string.Empty))
            .ForMember(d => d.CategoryId,
                o => o.MapFrom(s => s.Subcategory != null ? s.Subcategory.CategoryId : 0))
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s =>
                    s.Subcategory != null && s.Subcategory.Category != null
                        ? s.Subcategory.Category.Name
                        : string.Empty))
            .ForMember(d => d.Materials, o => o.MapFrom(s => s.Materials))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.ProductItems));

        CreateMap<User, UserDto>()
            .ForMember(d => d.RoleName,
                o => o.MapFrom(s => s.Role != null ? s.Role.Name : string.Empty));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductId,
                o => o.MapFrom(s => s.ProductItem != null ? s.ProductItem.ProductId : 0))
            .ForMember(d => d.ProductName,
                o => o.MapFrom(s =>
                    s.ProductItem != null && s.ProductItem.Product != null
                        ? s.ProductItem.Product.Name
                        : string.Empty))
            .ForMember(d => d.Size,
                o => o.MapFrom(s => s.ProductItem != null ? s.ProductItem.Size : 0))
            .ForMember(d => d.PricePerUnit,
                o => o.MapFrom(s => s.UnitPrice));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.CustomerFullName,
                o => o.MapFrom(s => s.User != null
                    ? $"{s.User.LastName} {s.User.FirstName} {s.User.MiddleName}".Trim()
                    : string.Empty))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems));
    }
}