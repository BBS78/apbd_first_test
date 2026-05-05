namespace apbd_exam_group_c.DTOs;

public class CreateVendorDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // FUNCTION: products are optional. Empty list means create only vendor.
    public List<CreateVendorProductDto> Products { get; set; } = new();
}
