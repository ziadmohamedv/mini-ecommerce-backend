using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Product;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize);
    }
}
