using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Domain.Entities
{
    public class CartItem
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be at least 0.01")]
        public decimal ItemPrice { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
    }
}
