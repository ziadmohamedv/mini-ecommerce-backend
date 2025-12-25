using Application.DTOs;
using Application.DTOs.Order;
using Application.DTOs.OrderItem;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        #region Methods

        private async Task<Product> GetAndValidateProductAsync(CreateOrderItemDto item)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product is null)
                throw new ArgumentException($"Product with ID {item.ProductId} not found");

            if (product.Quantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for product: {product.Name}");

            return product;
        }

        private async Task ReduceProductStockAsync(Product product, int quantity)
        {
            product.Quantity -= quantity;
            await _productRepository.UpdateAsync(product);
        }

        private async Task<List<OrderItem>> CreateOrderItemsAsync(IEnumerable<CreateOrderItemDto> items)
        {
            var orderItems = new List<OrderItem>();

            foreach (var item in items)
            {
                var product = await GetAndValidateProductAsync(item);

                var ItemsTotalPrice = product.Price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    ItemPrice = product.Price,
                    TotalPrice = ItemsTotalPrice
                });

                await ReduceProductStockAsync(product, item.Quantity);
            }

            return orderItems;
        }

        private decimal CalculateDiscount(IEnumerable<CreateOrderItemDto> items, decimal totalAmount)
        {
            var ItemsTotalPrice = items.Sum(i => i.Quantity);

            return ItemsTotalPrice switch
            {
                >= 5 => totalAmount * 0.10m,
                >= 2 => totalAmount * 0.05m,
                _ => 0m
            };
        }

        private Order CreateOrder(CreateOrderDto dto, List<OrderItem> items, decimal totalAmount, decimal discount, decimal finalAmount)
        {
            return new Order
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Discount = discount,
                FinalAmount = finalAmount,
                Items = items
            };
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Discount = order.Discount,
                FinalAmount = order.FinalAmount,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    ItemPrice = i.ItemPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };
        }
        #endregion

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var orderItems = await CreateOrderItemsAsync(dto.Items);
            var totalAmount = orderItems.Sum(i => i.TotalPrice);

            var discount = CalculateDiscount(dto.Items, totalAmount);
            var finalAmount = totalAmount - discount;

            var order = CreateOrder(dto, orderItems, totalAmount, discount, finalAmount);
            var createdOrder = await _orderRepository.AddAsync(order);

            return MapToDto(createdOrder);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order is null ? null : MapToDto(order);
        }
    }
}
