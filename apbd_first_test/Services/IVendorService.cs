using apbd_exam_group_c.DTOs;
using apbd_exam_group_c.Services;
using apbd_first_test.DTOs;

namespace apbd_first_test.Services;

public interface IVendorService
{
    Task<VendorDto?> GetVendorAsync(string code);
    Task<CreateVendorResult> CreateVendorAsync(CreateVendorDto dto);
}