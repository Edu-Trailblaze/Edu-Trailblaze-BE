using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Domain;

namespace Cart.Domain.Entities
{
    public class Cart 
    {
        public string UserName { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public Cart()
        {

        }
        public Cart(string username)
        {
            UserName = username;
        }
        public decimal TotalPrice
        {
            get
            {
                return Items.Sum(x => x.Quantity * Convert.ToDecimal(x.ItemPrice));
            }
        }
    }
}
