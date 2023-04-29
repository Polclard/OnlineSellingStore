using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineSellingStore.DataAccess.Repository;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineSellingStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitofWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productsList = _unitofWork.Product.GetAll(includeProperties: "Category");
            return View(productsList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitofWork.Product.Get(u => u.Id == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = null;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitofWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && 
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitofWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                //add cart record
                _unitofWork.ShoppingCart.Add(shoppingCart);
            }

            TempData["success"] = "Cart updated successfully";


            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}