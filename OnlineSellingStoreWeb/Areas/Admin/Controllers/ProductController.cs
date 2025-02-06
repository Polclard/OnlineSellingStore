using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using OnlineSellingStore.DataAccess.Data;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using OnlineSellingStore.Models;
using OnlineSellingStore.Models.FriendShop;
using OnlineSellingStore.Models.ViewModels;
using OnlineSellingStore.Utility;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Immutable;

namespace OnlineSellingStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ProductController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductList);
        }
            
        public IActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),

                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //edit
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImages");
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
        {
            productVM.Product.ImageUrl = "";
            if (ModelState.IsValid)
            {

                if (productVM.Product.Id == 0 || productVM.Product.Id == null)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();


                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(files != null)
                {

                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if(!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = (int)productVM.Product.Id,
                        };

                        if(productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);

                    }
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();

                }
                if(productVM.Product.Id == 0 || productVM.Product.Id == null)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if(imageToBeDeleted != null)
            {
                if(!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {

                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                    _unitOfWork.Save();
                    TempData["success"] = "Image deleted successfully";

                }
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


        
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int ? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }


            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }


            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion



        /*
        {
            "bookName": "Inferno",
            "bookDescription": "The Inferno describes the journey of Danta",
            "genre": null,
            "bookImage": "https://matica.com.mk/wp-content/uploads/2021/08/DANTE-ALIGERI-PEKOLOT-BOZESTVENA-KOMEDIJA-300x480.jpg",
            "price": 3,
            "rating": 3,
            "bookInShoppingCarts": null,
            "booksInOrder": null,
            "authors": [
              {
                "authorName": "Dante Aligheri",
                "bio": "Hello",
                "books": [],
                "id": "a8ce481f-3bbe-443b-a719-acafb41d8ebe"
              }
            ],
            "publisher": {
              "publisherName": "Toper",
              "address": "Skopje",
              "contact": "120310301230",
              "books": [],
              "id": "f13fd121-39e7-4ba1-9ab2-563630cfb919"
            },
            "id": "e3cb295b-5bf8-4f05-bbc4-8b432941867b"
        },
        */



        public async Task<IActionResult> GetFriendShopBooks(int id)
        {
            string URL = "https://bookstoreweb20250126132219.azurewebsites.net/api/Admin/GetAllBooks";

            HttpResponseMessage response = await _httpClient.GetAsync(URL);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<FriendBook>>(jsonResponse);

                foreach (var book in books)
                {
                    // 1. Handle Publisher
                    var existingPublisher = _context.FriendPublishers.FirstOrDefault(p => p.Id == book.Publisher.Id);
                    if (existingPublisher == null)
                    {
                        _context.FriendPublishers.Add(book.Publisher);
                        await _context.SaveChangesAsync(); // Save publisher first
                        existingPublisher = book.Publisher;
                    }

                    // 2. Handle Authors
                    List<FriendAuthor> bookAuthors = new List<FriendAuthor>();
                    foreach (var author in book.Authors)
                    {
                        var existingAuthor = _context.FriendAuthors.FirstOrDefault(a => a.Id == author.Id);
                        if (existingAuthor == null)
                        {
                            _context.FriendAuthors.Add(author);
                            await _context.SaveChangesAsync(); // Save author first
                            existingAuthor = author;
                        }
                        bookAuthors.Add(existingAuthor);
                    }

                    // 3. Handle Book (Only add if it doesn't already exist)
                    var existingBook = _context.FriendBooks.FirstOrDefault(b => b.Id == book.Id);
                    if (existingBook == null)
                    {
                        book.Publisher = existingPublisher;
                        book.Authors = bookAuthors;
                        _context.FriendBooks.Add(book);
                    }
                }

                await _context.SaveChangesAsync();
            }

            var allBooks = _context.FriendBooks.ToList();
            return View(allBooks);
        }
    }

}
