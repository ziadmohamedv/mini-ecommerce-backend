using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var existingProduct = await _productRepository.GetByNameAsync(dto.Name);
            if (existingProduct != null)
            {
                throw new InvalidOperationException($"A product with the name '{dto.Name}' already exists.");
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Quantity = dto.Quantity
            };

            var createdProduct = await _productRepository.AddAsync(product);

            return new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Price = createdProduct.Price,
                Quantity = createdProduct.Quantity
            };
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity
            }).ToList();
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetPagedAsync(pageNumber, pageSize);

            return new PagedResult<ProductDto>
            {
                Items = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
