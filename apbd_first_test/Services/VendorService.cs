using apbd_exam_group_c.DTOs;
using apbd_exam_group_c.Services;
using apbd_first_test.DTOs;
using apbd_first_test.Repositories;

namespace apbd_first_test.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;

    public VendorService(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }
    
    public async Task<VendorDto?> GetVendorAsync(string code)
    {
        return await _vendorRepository.GetVendorDataAsync(code);
    }
    
    public async Task<CreateVendorResult> CreateVendorAsync(CreateVendorDto dto)
    {
        return await _vendorRepository.CreateVendorAsync(dto);
    }
}