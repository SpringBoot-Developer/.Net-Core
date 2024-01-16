

using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
	{
		/* Index is the Action method here */
		private readonly IUnitOfWork _unitofWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitofWork ,
			IWebHostEnvironment webHostEnvironment)
		{
			_unitofWork = unitofWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();

			return View(objProductList);
		}

		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				CategoryList = _unitofWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name ,
					Value = u.Id.ToString() ,

				}) ,
				Product = new Product()
			};
			if(id == null || id == 0)
			{
				//Create
				return View(productVM);
			}
			else
			{
				//Update
				productVM.Product = _unitofWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}

		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM , IFormFile? file)
		{

			if(ModelState.IsValid)
			{
				string wwwRootpath = _webHostEnvironment.WebRootPath;

				if(file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootpath , @"images\product");

					if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						//delete the old Image
						var oldImagepath =
							Path.Combine(wwwRootpath , productVM.Product.ImageUrl.TrimStart('\\'));

						if(System.IO.File.Exists(oldImagepath))
						{
							System.IO.File.Delete(oldImagepath);
						}
					}


					using(var fileStream = new FileStream(Path.Combine(productPath , fileName) , FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				if(productVM.Product.Id == 0)
				{
					_unitofWork.Product.Add(productVM.Product);
				}
				else
				{
					_unitofWork.Product.Update(productVM.Product);
				}

				_unitofWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index" , "Product");
			}
			else
			{
				productVM.CategoryList = _unitofWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name ,
					Value = u.Id.ToString() ,

				});

				return View(productVM);
			}

		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitofWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProductList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitofWork.Product.Get(u => u.Id == id);

			if(productToBeDeleted == null)
			{
				return Json(new { success = false , message = "Error while deleting"});
			}

			var oldImagePath =
				Path.Combine(_webHostEnvironment.WebRootPath,
				productToBeDeleted.ImageUrl.TrimStart('\\'));

			if(System.IO.File.Exists(oldImagePath))
			{
					System.IO.File.Delete(oldImagePath);
			}


			_unitofWork.Product.Remove(productToBeDeleted);
			_unitofWork.Save();

			return Json(new { success = true , message = "Delete Successful" });
		}

		#endregion
	}
}
