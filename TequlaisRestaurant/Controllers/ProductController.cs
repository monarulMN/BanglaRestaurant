using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Reflection;
using TequlaisRestaurant.Data;
using TequlaisRestaurant.Models;

namespace TequlaisRestaurant.Controllers
{
    public class ProductController : Controller
    {
        private Repository<Product> products;
        private Repository<Ingredient> ingredients;
        private Repository<Category> categories;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(dbContext);
            ingredients = new Repository<Ingredient>(dbContext);  
            categories = new Repository<Category>(dbContext);
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Ingredients = await ingredients.GetAllAsync();
            ViewBag.Category = await categories.GetAllAsync();
            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {
                Product product = await products.GetByIdAsync(id, new QueryOptions<Product>
                {
                    Includes = "ProductIngredients.Ingredient, Category"
                });
                ViewBag.Operation = "Edit";
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product, int[] ingredientIds)
        {
            if(ModelState.IsValid)
            {
                if(product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = uniqueFileName;
                }
                if(product.ProductId == 0)
                {
                    ViewBag.Ingredients = await ingredients.GetAllAsync();
                    ViewBag.Category = await categories.GetAllAsync();
                    foreach (int id in ingredientIds)
                    {
                        product.ProductIngredients?.Add(new ProductIngredient 
                            { IngredientId = id, ProductId = product.ProductId });
                    }
                    await products.AddAsync(product);
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    var existingProduct = await products.GetByIdAsync(product.ProductId, new QueryOptions<Product>
                        { Includes = "ProductIngredients" });
                    if(existingProduct == null)
                    {
                        ModelState.AddModelError("", "Product not found");
                        ViewBag.Ingredents = await ingredients.GetAllAsync();
                        ViewBag.Category = await categories.GetAllAsync();
                        return View(product);
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;


                    return RedirectToAction("Index", "Product");
                }
            }
            else
            {
                return View(product);
            }

        }

    }
}
