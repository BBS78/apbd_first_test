using apbd_exam_group_c.DTOs;
using apbd_exam_group_c.Services;

namespace apbd_first_test.Repositories;
using apbd_first_test.DTOs;
using Microsoft.Data.SqlClient;

public class VendorRepository : IVendorRepository
{
    private readonly string _connectionString;

    public VendorRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")!;
    }

    public async Task<VendorDto> GetVendorDataAsync(string code)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var vendorSql = @"Select 
            v.Code, 
            v.Name, 
            vp.ProductId,
            p.Name,
            p.Description,
            p.StickerPrice,
            pt.Id,
            pt.Name,
            m.Id,
            m.Name,
            vp.Amount,
            vp.PricePerUnit
            From Vendors v
            Join VendorProducts vp ON vp.VendorCode = v.Code
            Join Products p ON p.Id = vp.ProductId
            Join ProductTypes pt ON pt.Id = p.ProductTypeId
            Join Makers m ON m.Id = p.MakerId
            Where v.code = @code 
                        ";
        
        await using var vendorCommand = new SqlCommand(vendorSql, connection);
        vendorCommand.Parameters.AddWithValue("@code", code);

        await using var vendorReader = await vendorCommand.ExecuteReaderAsync();
        VendorDto? vendor = null;

        var productsDictionary = new Dictionary<int, ProductDto>();

        while (await vendorReader.ReadAsync())
        {
            if (vendor == null)
            {
                vendor = new VendorDto()
                {
                    Code = vendorReader.GetString(0),
                    Name = vendorReader.GetString(1),
                    Products = new List<ProductDto>()
                };
            }

            var productId = vendorReader.GetInt32(2);

            if (!productsDictionary.ContainsKey(productId))
            {
                var product = new ProductDto
                {
                    Id = productId,
                    Name = vendorReader.GetString(3),
                    Description = vendorReader.GetString(4),
                    StickerPrice = vendorReader.GetDecimal(5),

                    ProductType = new ProductTypeDto
                    {
                        Id = vendorReader.GetInt32(6),
                        Name = vendorReader.GetString(7)
                    },

                    Maker = new MakerDto
                    {
                        Id = vendorReader.GetInt32(8),
                        Name = vendorReader.GetString(9)
                    },
                    VendorOffer = new VendorOfferDto
                    {
                        Amount = vendorReader.GetInt32(10),
                        PricePerUnit = vendorReader.GetDecimal(11)
                    },
                };

                productsDictionary.Add(productId, product);
            }
        }

        if (vendor == null)
        {
            return null;
        }

        vendor.Products = productsDictionary.Values.ToList();

        return vendor;    
    }
    
    public async Task<CreateVendorResult> CreateVendorAsync(CreateVendorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code) || string.IsNullOrWhiteSpace(dto.Name) || dto.Code.Length > 10)
        {
            return CreateVendorResult.InvalidInput;
        }

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var duplicateSql = @"
            SELECT 1
            FROM Vendors
            WHERE Code = @code;
        ";

        await using var duplicateCommand = new SqlCommand(duplicateSql, connection);
        duplicateCommand.Parameters.AddWithValue("@code", dto.Code);

        var duplicate = await duplicateCommand.ExecuteScalarAsync();

        if (duplicate != null)
        {
            return CreateVendorResult.VendorAlreadyExists;
        }

        // FUNCTION: check all requested products before starting insert transaction.
        var productIds = new HashSet<int>();

        foreach (var product in dto.Products)
        {
            var productSql = @"
                SELECT 1
                FROM Products
                WHERE Id = @productId;
            ";

            await using var productCommand = new SqlCommand(productSql, connection);
            productCommand.Parameters.AddWithValue("@productId", product.Id);

            var productExists = await productCommand.ExecuteScalarAsync();

            if (productExists == null)
            {
                return CreateVendorResult.ProductNotFound;
            }

            productIds.Add(product.Id);
        }

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            var insertVendorSql = @"
                INSERT INTO Vendors (Code, Name)
                VALUES (@code, @name);
            ";

            await using var insertVendorCommand = new SqlCommand(insertVendorSql, connection, transaction);
            insertVendorCommand.Parameters.AddWithValue("@code", dto.Code);
            insertVendorCommand.Parameters.AddWithValue("@name", dto.Name);

            await insertVendorCommand.ExecuteNonQueryAsync();

            var insertVendorProductSql = @"
                INSERT INTO VendorProducts (ProductId, VendorCode, Amount, PricePerUnit)
                VALUES (@productId, @vendorCode, @amount, @pricePerUnit);
            ";

            foreach (var product in dto.Products)
            {
                await using var insertProductCommand = new SqlCommand(insertVendorProductSql, connection, transaction);
                insertProductCommand.Parameters.AddWithValue("@productId", product.Id);
                insertProductCommand.Parameters.AddWithValue("@vendorCode", dto.Code);
                insertProductCommand.Parameters.AddWithValue("@amount", product.Amount);
                insertProductCommand.Parameters.AddWithValue("@pricePerUnit", product.PricePerUnit);

                await insertProductCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return CreateVendorResult.Created;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}