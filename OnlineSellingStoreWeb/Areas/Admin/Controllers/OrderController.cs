// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Models;
using OnlineSellingStore.Models.ViewModels;
using OnlineSellingStore.Utility;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineSellingStoreWeb.Areas.Admin.Controllers
{
	[Area("admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM ? OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }
        public IActionResult Index()
		{
			return View();
		}



        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier; 
            }
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber; 
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details), new { orderID = orderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusProccess);
            _unitOfWork.Save();

            TempData["Success"] = "Order Started Processing Successfully";

            return RedirectToAction(nameof(Details), new { orderID = OrderVM.OrderHeader.Id });
        }




        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);   
            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderID = OrderVM.OrderHeader.Id });
        }


        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {

            OrderVM.OrderHeader = _unitOfWork.OrderHeader
                .Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail
                .GetAll(u => u.OrderHeader.Id == OrderVM.OrderHeader.Id, includeProperties: "Product");


            var domain = "https://localhost:7113/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{domain}admin/order/PaymentConfirmation?orderHeaderid={OrderVM.OrderHeader.Id}",
                CancelUrl = $"{domain}admin/order/details?orderId={OrderVM.OrderHeader.Id}",

                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                // This is an order by the company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

            }

            return View(orderHeaderId);
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.Add(TimeSpan.FromDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Order Shipped Successfully";

            return RedirectToAction(nameof(Details), new { orderID = OrderVM.OrderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeadersList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeadersList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity) User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeadersList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }


            switch (status)
            {
                case "pending":
                    objOrderHeadersList = objOrderHeadersList.Where(u => u.OrderStatus == SD.StatusPending);
                    break;

                case "inprocess":
                    objOrderHeadersList = objOrderHeadersList.Where(u => u.OrderStatus == SD.StatusProccess);
                    break;

                case "completed":
                    objOrderHeadersList = objOrderHeadersList.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;

                case "approved":
                    objOrderHeadersList = objOrderHeadersList.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;

                default:
                    break;
            }


            return Json(new { data = objOrderHeadersList });
		}

		#endregion

	}
}
