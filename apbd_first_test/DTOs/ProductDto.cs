namespace apbd_first_test.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name  { get; set; }
    public string Description { get; set; }
    public decimal StickerPrice { get; set; }
    public ProductTypeDto ProductType { get; set; }
    public MakerDto Maker { get; set; }
    public VendorOfferDto VendorOffer { get; set; }
}