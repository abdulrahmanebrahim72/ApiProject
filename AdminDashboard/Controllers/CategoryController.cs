using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace AdminDashboard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var category = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

            return View(category);
        }

        public async Task<IActionResult> Create(ProductCategory category)
        {
            try
            {
                await _unitOfWork.Repository<ProductCategory>().AddAsync(category);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            catch (System.Exception)
            {

                ModelState.AddModelError("Name", "Please Enter New Brand");
                return View("Index", await _unitOfWork.Repository<ProductCategory>().GetAllAsync());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Repository<ProductCategory>().GetAsync(id);

            _unitOfWork.Repository<ProductCategory>().Delete(category);

            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index");
        }
    }
}
