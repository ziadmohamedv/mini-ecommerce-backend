using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByNameAsync(string name);
        Task<List<Product>> GetAllAsync();
        Task<(List<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
    }
}
