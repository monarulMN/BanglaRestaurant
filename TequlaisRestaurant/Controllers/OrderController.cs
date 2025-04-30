using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using TequlaisRestaurant.Data;
using TequlaisRestaurant.Models;

namespace TequlaisRestaurant.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private Repository<Product> _product;
        private Repository<Order> _order;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _product = new Repository<Product>(dbContext);
            _order = new Repository<Order>(dbContext);
        }

        [Authorize]
        [HttpGet]
        
        public async Task<IActionResult> Create()
        {
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel") ?? new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>(),
                Products = await _product.GetAllAsync()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddItem(int productId, int productQuantity)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            //Retrieve or Create an OrderViewModel from Session or other state management
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel") ?? new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>(),
                Products = await _product.GetAllAsync()
            };

            //Check if the product already in the order
            var existingItem = model.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);

            //If the product is already in the Order, Update the Quantity
            if(existingItem == null)
            {
                existingItem.Quantity += productQuantity;
            }
            else
            {
                model.OrderItems.Add(new OrderItemViewModel
                {
                    ProductId = product.ProductId,
                    Price = product.Price,
                    Quantity = productQuantity,
                    ProductName = product.Name
                });
            }

            //Update the Total amount
            model.TotalAmount = model.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            //save updated OrderViewModel to session
            HttpContext.Session.Set("OrderViewModel", model);

            //Redirect back to Create to show updated order items
            return RedirectToAction("Create", model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            //Retrieve the OrderViewModel from session or other state management
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel");
            if(model == null || model.OrderItems.Count == 0)
            {
                return RedirectToAction("Create");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder()
        {
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel");
            if(model == null || model.OrderItems.Count == 0)
            {
                return RedirectToAction("Create");
            }

            //Create a new Order entity
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                UserId = _userManager.GetUserId(User)
            };

            //Add OrderItems to the Order entity
            foreach(var item in model.OrderItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            //Save the OrderQuantity in the database
            await _order.AddAsync(order);

            //Clear the OrderViewModel from the session or other state management
            HttpContext.Session.Remove("OrderViewModel");

            //Redirect to the Order Confirmation page
            return RedirectToAction("ViewOrders");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewOrders()
        {
            var userId = _userManager.GetUserId(User);
            var userOrders = await _order.GetAllByIdAsync(userId, "UserId", new QueryOptions<Order>
            {
                Includes = "OrderItems.Product"
            });
            return View(userOrders);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
