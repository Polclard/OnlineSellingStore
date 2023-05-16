using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineSellingStore.DataAccess.Data;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Models;
using OnlineSellingStore.Models.ViewModels;
using OnlineSellingStore.Utility;

namespace OnlineSellingStoreWeb.Areas.Admin.Controllers
{
    [Area(SD.Role_Admin)]
    [Authorize(Roles = SD.Role_Admin)]
    public class AllUsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public UserManager<IdentityUser> _userManager { get; set; }
        public AllUsersController(IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Edit(string? id)
        {
            if (id == null || id == "")
            {
                return View();
            }
            ApplicationUser selectedUser = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            if (selectedUser == null)
            {
                return View();
            }

            return View(selectedUser);
        }

        public IActionResult RoleManagment(string? id)
        {
            if (id == null || id == "")
            {
                return View();
            }
            List<ApplicationUser>? usersFromDb = _db.applicationUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();


            foreach (var user in usersFromDb)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                var roleName = roles.FirstOrDefault(u => u.Id == roleId);

                if (roleName != null)
                {
                    user.Role = roleName.Name;
                    user.RoleId = roleId;
                }
            }

            UserVM userVM = new()
            {
                user = _unitOfWork.ApplicationUser.Get(u => u.Id == id),
                roles = _db.Roles.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id
                }),
                companies = _unitOfWork.Company.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                userRoles = _db.Roles.ToList().Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                })
            };
            if (userVM != null)
            {
                return View(userVM);
            }

            return View();
        }

        [HttpPost]
        public IActionResult RoleManagment(UserVM userVM)
        {
            if (userVM == null)
            {
                return View();
            }

            string roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userVM.user.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            

            if(!(userVM.user.Role == oldRole))
            {
                var newRole = _db.Roles.FirstOrDefault(u => u.Id == userVM.user.RoleId).Name;
                // The role was updated
                ApplicationUser applicationUser = _db.applicationUsers.FirstOrDefault(u => u.Id == userVM.user.Id);
                if(newRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = userVM.user.CompanyId;
                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, newRole).GetAwaiter().GetResult();

            }

            return RedirectToAction("Index", "AllUsers");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUsersList = _db.applicationUsers.Include(u => u.Company).ToList();


            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (ApplicationUser user in objUsersList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                var roleName = roles.FirstOrDefault(u => u.Id == roleId);

                if(roleName != null)
                {
                    user.Role = roleName.Name;
                }

                if(user.Company == null)
                {
                    user.Company = new() { Name = "/" };
                }
            }

            
            return Json(new { data = objUsersList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string? id)
        {
            var objFromDb = _db.applicationUsers.FirstOrDefault(u => u.Id == id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //User is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Locked/Unlocked successfull" });
        }
        #endregion

    }
}
