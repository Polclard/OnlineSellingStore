using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using OnlineSellingStore.DataAccess.Data;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Models;

namespace OnlineSellingStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfwork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfwork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfwork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj != null && obj.Name != null && obj.DisplayOrder != null)
            {
                if (obj.Name.ToLower() == obj.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("Name", "Name and Display Order can't have the same value");
                }

                List<Category> getListOfNames = _unitOfwork.Category.GetAll().ToList();

                foreach (var category in getListOfNames)
                {
                    if (category.Name == obj.Name)
                    {
                        ModelState.AddModelError("Name", "Category with that name already exists");
                        break;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _unitOfwork.Category.Add(obj);
                _unitOfwork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }




        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfwork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj != null && obj.Name != null && obj.DisplayOrder != null)
            {
                if (obj.Name.ToLower() == obj.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("Name", "Name and Display Order can't have the same value");
                }
            }

            if (ModelState.IsValid)
            {
                _unitOfwork.Category.Update(obj);
                _unitOfwork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfwork.Category.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Category obj)
        {

            if (obj != null)
            {
                _unitOfwork.Category.Remove(obj);
                _unitOfwork.Save();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

    }
}
