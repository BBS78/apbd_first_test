using apbd_exam_group_c.DTOs;
using apbd_exam_group_c.Services;
using apbd_first_test.DTOs;

namespace apbd_first_test.Repositories;

public interface IVendorRepository
{
    Task<VendorDto?> GetVendorDataAsync(string code);

    Task<CreateVendorResult> CreateVendorAsync(CreateVendorDto dto);
}