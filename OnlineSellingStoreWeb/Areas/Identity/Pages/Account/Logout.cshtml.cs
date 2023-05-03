// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Utility;

namespace OnlineSellingStoreWeb.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public void calculateItemsInShoppingCart()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            if (claimsIdentity.IsAuthenticated == true)
            {
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            else
            {
                HttpContext.Session.SetInt32(SD.SessionCart, 0);
            }
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {

                calculateItemsInShoppingCart();
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.

                calculateItemsInShoppingCart();
                return RedirectToPage();
            }
        }
    }
}
