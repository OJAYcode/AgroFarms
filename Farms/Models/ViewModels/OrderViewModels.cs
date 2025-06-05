using System.ComponentModel.DataAnnotations;

namespace Farms.Models.ViewModels
{    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Street Address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "State")]
        public string State { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ZIP Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Display(Name = "Order Notes")]
        public string Notes { get; set; } = string.Empty;
    }

    public class OrderListViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public string UserType { get; set; } = string.Empty; // "Farmer" or "Buyer"
    }

    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = new Order();
        public string UserType { get; set; } = string.Empty;
        public bool CanUpdateStatus { get; set; }
    }

    public class UpdateOrderStatusViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
    }
}
