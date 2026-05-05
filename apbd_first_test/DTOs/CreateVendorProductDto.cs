namespace apbd_exam_group_c.DTOs;

public class CreateVendorProductDto
{
    // FUNCTION: product must already exist in Products table.
    public int Id { get; set; }
    public int Amount { get; set; }
    public decimal PricePerUnit { get; set; }
}
