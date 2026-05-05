using apbd_exam_group_c.DTOs;
using apbd_exam_group_c.Services;
using Microsoft.AspNetCore.Mvc;
using apbd_first_test.Services;
namespace apbd_first_test.Conrollers;

[ApiController]
[Route("api/vendors/")]
public class VenderController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VenderController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetVendorDataAsync(string code)
    {
        var vendor = await _vendorService.GetVendorAsync(code);
        if (vendor is null)
        {
            return NotFound($"Vendor with code {code} was not found.");
        }

        return Ok(vendor);
    }
    [HttpPost]
    public async Task<IActionResult> CreateVendorAsync([FromBody] CreateVendorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code) || string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Vendor code and name are required.");
        }

        if (dto.Code.Length > 10)
        {
            return BadRequest("Vendor code must be at most 10 characters.");
        }

        if (dto.Products.Any(p => p.Amount < 0 || p.PricePerUnit < 0))
        {
            return BadRequest("Amount and pricePerUnit cannot be negative.");
        }

        if (dto.Products.Select(p => p.Id).Distinct().Count() != dto.Products.Count)
        {
            return BadRequest("Duplicate product ids are not allowed in one request.");
        }

        var result = await _vendorService.CreateVendorAsync(dto);

        return result switch
        {
            CreateVendorResult.Created => Created($"/api/vendors/{dto.Code}", null),
            CreateVendorResult.VendorAlreadyExists => Conflict("Vendor already exists."),
            CreateVendorResult.ProductNotFound => NotFound("Product not found."),
            _ => BadRequest()
        };
    }
    
}
