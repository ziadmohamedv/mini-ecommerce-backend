using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public Order Order { get; set; } = null!;
    }
}