

using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		/* Index is the Action method here */
		private readonly IUnitOfWork _unitofWork;
		public ProductController(IUnitOfWork unitofWork)
		{
			_unitofWork = unitofWork;
		}
		public IActionResult Index()
		{
			List<Product> objProductList = _unitofWork.Product.GetAll().ToList();
			
			return View(objProductList);
		}

		public IActionResult Create()
		{
			IEnumerable<SelectListItem> CategoryList = _unitofWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name ,
					Value = u.Id.ToString() ,

				});
			//ViewBag.CategoryList = CategoryList;		
			ViewData["CategoryList"] = CategoryList;		
			return View();
		}
		[HttpPost]
		public IActionResult Create(Product obj)
		{

			if(ModelState.IsValid)
			{
				_unitofWork.Product.Add(obj);
				_unitofWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index" , "Product");
			}
			return View(obj);
		}


		public IActionResult Edit(int? id)
		{
			if(id == null || id == 0)
			{
				return NotFound();
			}
			Product? productFromDb = _unitofWork.Product.Get(u => u.Id == id);
			/*	Category? CategoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
				Category? CategoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();*/

			if(productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost]
		public IActionResult Edit(Product obj)
		{
			if(ModelState.IsValid)
			{
				_unitofWork.Product.Update(obj);
				_unitofWork.Save();
				TempData["success"] = "Product updated successfully";
				return RedirectToAction("Index" , "Product");
			}
			return View(obj);
		}


		public IActionResult Delete(int? id)
		{
			if(id == null || id == 0)
			{
				return NotFound();
			}
			Product? productFromDb = _unitofWork.Product.Get(u => u.Id == id);

			if(productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Product? obj = _unitofWork.Product.Get(u => u.Id == id);

			if(obj == null)
			{
				return NotFound();
			}
			_unitofWork.Product.Remove(obj);
			_unitofWork.Save();
			TempData["success"] = "Product deleted successfully";
			return RedirectToAction("Index" , "Product");

		}
	}
}
