using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.Api.DTOs;
using ShoeStore.Services.Interfaces;

namespace ShoeStore.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class ReferenceController : ControllerBase
{
    private readonly IReferenceService _referenceService;
    private readonly IMapper _mapper;

    public ReferenceController(IReferenceService referenceService, IMapper mapper)
    {
        _referenceService = referenceService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var cats = await _referenceService.GetCategoriesAsync();
        return Ok(_mapper.Map<List<CategoryDto>>(cats));
    }

    [HttpGet("{id:int}/subcategories")]
    public async Task<ActionResult<List<SubcategoryDto>>> GetSubcategories(int id)
    {
        var subs = await _referenceService.GetSubcategoriesAsync(id);
        return Ok(_mapper.Map<List<SubcategoryDto>>(subs));
    }

    [HttpGet("materials")]
    public async Task<ActionResult<List<MaterialDto>>> GetMaterials()
    {
        var mats = await _referenceService.GetMaterialsAsync();
        return Ok(_mapper.Map<List<MaterialDto>>(mats));
    }
}