using Microsoft.AspNetCore.Mvc;
using Farms.Models;
using Farms.Models.ViewModels;
using Farms.Services;

namespace Farms.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (IsUserLoggedIn())
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.UserType == "Farmer")
            {
                var farmer = await _userService.AuthenticateFarmerAsync(model.Email, model.Password);
                if (farmer != null)
                {
                    SetUserSession(farmer.Id, "Farmer", $"{farmer.FirstName} {farmer.LastName}");
                    return RedirectToAction("Dashboard", "Farmer");
                }
            }
            else if (model.UserType == "Buyer")
            {
                var buyer = await _userService.AuthenticateBuyerAsync(model.Email, model.Password);
                if (buyer != null)
                {
                    SetUserSession(buyer.Id, "Buyer", $"{buyer.FirstName} {buyer.LastName}");
                    return RedirectToAction("Dashboard", "Buyer");
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterFarmer()
        {
            if (IsUserLoggedIn())
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterFarmer(RegisterFarmerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);            var farmer = new Farmer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FarmName = model.FarmName,
                FarmLocation = $"{model.FarmAddress}, {model.FarmCity}, {model.FarmZipCode}",
                FarmSize = "", // Set default or add to ViewModel if needed
                Description = model.FarmDescription
            };

            var result = await _userService.RegisterFarmerAsync(farmer, model.Password);
            if (result != null)
            {
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Email already exists or registration failed.");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterBuyer()
        {
            if (IsUserLoggedIn())
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterBuyer(RegisterBuyerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var buyer = new Buyer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City,
                ZipCode = model.ZipCode,
                Company = model.Company
            };

            var result = await _userService.RegisterBuyerAsync(buyer, model.Password);
            if (result != null)
            {
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Email already exists or registration failed.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            ClearUserSession();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Dashboard()
        {
            var userType = HttpContext.Session.GetString("UserType");
            
            if (userType == "Farmer")
                return RedirectToAction("Dashboard", "Farmer");
            else if (userType == "Buyer")
                return RedirectToAction("Dashboard", "Buyer");
            else
                return RedirectToAction("Login");
        }

        private void SetUserSession(string userId, string userType, string userName)
        {
            HttpContext.Session.SetString("UserId", userId);
            HttpContext.Session.SetString("UserType", userType);
            HttpContext.Session.SetString("UserName", userName);
        }

        private void ClearUserSession()
        {
            HttpContext.Session.Clear();
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }
    }
}
