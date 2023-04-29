using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Models;
using OnlineSellingStore.Models.ViewModels;
using System.Security.Claims;

namespace OnlineSellingStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product")
            };

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            
            ViewBag.Length = ShoppingCartVM.ShoppingCartList.Count();

            return View(ShoppingCartVM);
        }




        public IActionResult Plus(int? cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            shoppingCartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            _unitOfWork.Save();
            return Redirect(nameof(Index));
        }

        public IActionResult Minus(int? cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
           if(shoppingCartFromDb.Count <= 1)
           {
                //remove from Shopping Cart
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
           }
           else
           {
                //reduce Count by one
                shoppingCartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
           }
            _unitOfWork.Save();
            return Redirect(nameof(Index));
        }

        public IActionResult Remove(int? cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

            //remove from Shopping Cart
            _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
            _unitOfWork.Save();
            
            return Redirect(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
