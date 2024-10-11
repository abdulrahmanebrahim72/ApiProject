using AdminDashboard.Helpers;
using AdminDashboard.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace AdminDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();

            var mappedProduct = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(products);

            return View(mappedProduct);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null)
                {
                    model.PictureUrl = PictureSetting.UploadFile(model.Image, "products");
                }

                else
                    model.PictureUrl = "images/products/1.png";

                var mappedProduct = _mapper.Map<ProductViewModel, Product>(model);

                await _unitOfWork.Repository<Product>().AddAsync(mappedProduct);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index", "Product");


            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetAsync(id);

            var mappedProduct = _mapper.Map<Product, ProductViewModel>(product);

            return View(mappedProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (model.Image != null)
                {
                    if (model.PictureUrl != null)
                    {
                        PictureSetting.DeleteFile(model.PictureUrl, "products");
                        model.PictureUrl = PictureSetting.UploadFile(model.Image, "products");
                    }

                    else
                        model.PictureUrl = PictureSetting.UploadFile(model.Image, "products");

                    var mappedProduct = _mapper.Map<ProductViewModel, Product>(model);

                    _unitOfWork.Repository<Product>().Update(mappedProduct);

                    var result = await _unitOfWork.CompleteAsync();

                    if (result > 0)
                        return RedirectToAction("Index");
                }
            }

            return View(model);

        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetAsync(id);

            var mappedProduct = _mapper.Map<Product, ProductViewModel>(product);

            return View(mappedProduct);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(int id, ProductViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            try
            {
                var product = await _unitOfWork.Repository<Product>().GetAsync(id);

                if (product.PictureUrl != null)
                    PictureSetting.DeleteFile(product.PictureUrl, "products");

                _unitOfWork.Repository<Product>().Delete(product);

                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            catch (System.Exception)
            {

                return View(model);
            }
        }


    }

}
