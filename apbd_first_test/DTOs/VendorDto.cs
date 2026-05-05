namespace apbd_first_test.DTOs;

public class VendorDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public List<ProductDto> Products { get; set; } = new List<ProductDto>();

}