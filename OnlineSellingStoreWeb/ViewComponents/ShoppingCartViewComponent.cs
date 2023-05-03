using Microsoft.AspNetCore.Mvc;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Utility;
using System.Security.Claims;

namespace OnlineSellingStoreWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claimsIdentity.IsAuthenticated == true)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                   _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
               
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.SetInt32(SD.SessionCart, 0);
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
        }

    }
}
