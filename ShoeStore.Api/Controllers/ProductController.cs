using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.Api.DTOs;
using ShoeStore.Services.Interfaces;
using ShoeStore.Services.Models;

namespace ShoeStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetCatalog(
        [FromQuery] int? categoryId,
        [FromQuery] int? subcategoryId,
        [FromQuery] string? manufacturer,
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var filter = new CatalogFilter
        {
            CategoryId = categoryId,
            SubcategoryId = subcategoryId,
            Manufacturer = manufacturer,
            SearchQuery = search,
            SortBy = sort,
            Page = page,
            PageSize = size
        };

        var result = await _productService.GetProductsAsync(filter);

        return Ok(new PagedResultDto<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            Size = result.PageSize,
            TotalPages = result.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)result.TotalCount / result.PageSize)
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpGet("{id:int}/sizes")]
    public async Task<ActionResult<List<ProductItemDto>>> GetSizes(int id)
    {
        var items = await _productService.GetAvailableSizesAsync(id);
        return Ok(_mapper.Map<List<ProductItemDto>>(items));
    }
}