using Microsoft.AspNetCore.Mvc;
using Farms.Services;
using Farms.Models;
using Farms.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Farms.Controllers
{
    // This is a temporary class for reference purposes only
    public class TempAddToCartController : Controller
    {
        private readonly IStaticProductService _staticProductService;
        private readonly IEnhancedCartService _enhancedCartService;

        public TempAddToCartController(IStaticProductService staticProductService, IEnhancedCartService enhancedCartService)
        {
            _staticProductService = staticProductService;
            _enhancedCartService = enhancedCartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartViewModel model)
        {
            try
            {
                Console.WriteLine($"AddToCart called with ProductId: '{model?.ProductId}', Quantity: {model?.Quantity}");
                
                // Validate authentication
                if (!IsValidBuyer())
                {
                    Console.WriteLine("AddToCart - User not authenticated");
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Validate model
                if (model == null)
                {
                    Console.WriteLine("AddToCart - Model is null");
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(model.ProductId))
                {
                    Console.WriteLine("AddToCart - ProductId is null or empty");
                    return Json(new { success = false, message = "Product ID is required" });
                }

                if (model.Quantity <= 0)
                {
                    Console.WriteLine($"AddToCart - Invalid quantity: {model.Quantity}");
                    return Json(new { success = false, message = "Quantity must be greater than 0" });
                }

                // Get product from StaticProductService to ensure consistency with EnhancedCartService
                var product = _staticProductService.GetProductById(model.ProductId);
                
                if (product == null)
                {
                    Console.WriteLine($"AddToCart - Product not found for ID: '{model.ProductId}'");
                    return Json(new { success = false, message = $"Product not found with ID: {model.ProductId}" });
                }

                // Check stock using StaticProductService
                if (!_staticProductService.IsProductAvailable(model.ProductId, model.Quantity))
                {
                    Console.WriteLine($"AddToCart - Insufficient stock. Available: {product.StockQuantity}, Requested: {model.Quantity}");
                    return Json(new { success = false, message = $"Only {product.StockQuantity} items available" });
                }

                var buyerId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(buyerId))
                {
                    Console.WriteLine("AddToCart - User ID not found in session");
                    return Json(new { success = false, message = "Session expired" });
                }

                Console.WriteLine($"AddToCart - Calling EnhancedCartService.AddToCartAsync with BuyerId: '{buyerId}', ProductId: '{product.Id}', Quantity: {model.Quantity}");

                // Add to cart using EnhancedCartService
                var cartItem = await _enhancedCartService.AddToCartAsync(buyerId, product.Id, model.Quantity);
                
                if (cartItem == null)
                {
                    Console.WriteLine("AddToCart - Failed to add item to cart - service returned null");
                    return Json(new { success = false, message = "Failed to add product to cart" });
                }

                Console.WriteLine($"AddToCart - Successfully added to cart: {model.Quantity} x {product.Name}");
                
                // Get updated cart count
                var cartItems = await _enhancedCartService.GetCartItemsAsync(buyerId);
                var cartCount = cartItems.Sum(c => c.Quantity);                Console.WriteLine($"AddToCart - Updated cart count: {cartCount}");

                return Json(new { 
                    success = true, 
                    message = $"Added {model.Quantity} {product.Name} to cart",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in AddToCart: {ex.Message}");
                Console.WriteLine($"ERROR Stack Trace: {ex.StackTrace}");
                return Json(new { success = false, message = "An error occurred while adding to cart" });
            }
        }

        // Helper method to validate buyer
        private bool IsValidBuyer()
        {
            var userType = HttpContext.Session.GetString("UserType");
            var userId = HttpContext.Session.GetString("UserId");
            return userType == "Buyer" && !string.IsNullOrEmpty(userId);
        }
    }
}
