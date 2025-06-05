using Microsoft.AspNetCore.Mvc;
using Farms.Services;
using Farms.Models;
using Farms.Models.ViewModels;

namespace Farms.Controllers
{
    public class FarmerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public FarmerController(IUserService userService, IProductService productService, IOrderService orderService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            var farmerId = HttpContext.Session.GetString("UserId")!;
            var products = await _productService.GetProductsByFarmerAsync(farmerId);
            var orders = await _orderService.GetOrdersByFarmerAsync(farmerId);

            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalRevenue = orders.Where(o => o.Status == OrderStatus.Delivered)
                .Sum(o => o.Items.Where(i => i.FarmerId == farmerId).Sum(i => i.TotalPrice));

            ViewBag.RecentProducts = products.Take(5).ToList();
            ViewBag.RecentOrders = orders.Take(5).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var farmerId = HttpContext.Session.GetString("UserId")!;
            var farmer = await _userService.GetFarmerByIdAsync(farmerId);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                Quantity = model.Quantity,
                Unit = model.Unit,
                ImageUrl = model.ImageUrl,
                HarvestDate = model.HarvestDate,
                ExpiryDate = model.ExpiryDate,
                FarmerId = farmerId,
                FarmerName = $"{farmer!.FirstName} {farmer.LastName}"
            };

            await _productService.CreateProductAsync(product);
            TempData["Success"] = "Product created successfully!";
            return RedirectToAction("Products");
        }

        public async Task<IActionResult> Products()
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            var farmerId = HttpContext.Session.GetString("UserId")!;
            var products = await _productService.GetProductsByFarmerAsync(farmerId);

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(string id)
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.FarmerId != HttpContext.Session.GetString("UserId"))
                return NotFound();

            var model = new CreateProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Quantity = product.Quantity,
                Unit = product.Unit,
                ImageUrl = product.ImageUrl,
                HarvestDate = product.HarvestDate,
                ExpiryDate = product.ExpiryDate
            };

            ViewBag.ProductId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(string id, CreateProductViewModel model)
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = id;
                return View(model);
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.FarmerId != HttpContext.Session.GetString("UserId"))
                return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Category = model.Category;
            product.Quantity = model.Quantity;
            product.Unit = model.Unit;
            product.ImageUrl = model.ImageUrl;
            product.HarvestDate = model.HarvestDate;
            product.ExpiryDate = model.ExpiryDate;

            await _productService.UpdateProductAsync(product);
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.FarmerId != HttpContext.Session.GetString("UserId"))
                return NotFound();

            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Products");
        }

        public async Task<IActionResult> Orders()
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            var farmerId = HttpContext.Session.GetString("UserId")!;
            var orders = await _orderService.GetOrdersByFarmerAsync(farmerId);

            var viewModel = new OrderListViewModel
            {
                Orders = orders,
                UserType = "Farmer"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusViewModel model)
        {
            if (!IsValidFarmer())
                return RedirectToAction("Login", "Account");

            await _orderService.UpdateOrderStatusAsync(model.OrderId, model.Status);
            TempData["Success"] = "Order status updated successfully!";
            return RedirectToAction("Orders");
        }

        private bool IsValidFarmer()
        {
            var userType = HttpContext.Session.GetString("UserType");
            var userId = HttpContext.Session.GetString("UserId");
            return userType == "Farmer" && !string.IsNullOrEmpty(userId);
        }
    }
}
