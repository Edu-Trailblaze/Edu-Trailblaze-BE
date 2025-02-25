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
