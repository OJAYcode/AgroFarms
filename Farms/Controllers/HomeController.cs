using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Farms.Models;
using Farms.Services;

namespace Farms.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await _productService.GetAllProductsAsync();
        var categories = await _productService.GetCategoriesAsync();
        
        ViewBag.FeaturedProducts = featuredProducts.Take(8).ToList();
        ViewBag.Categories = categories.Take(6).ToList();
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
