using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TequlaisRestaurant.Models;

namespace TequlaisRestaurant.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Define compsite key and relationship for productIngredient
            modelBuilder.Entity<ProductIngredient>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(pi => pi.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(pi => pi.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);

            //Seed data
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Apitizer"},
                new Category { CategoryId = 2, Name = "Entree"},
                new Category { CategoryId = 3, Name = "Side Dish"},
                new Category { CategoryId = 4, Name = "Dissert"},
                new Category { CategoryId = 5, Name = "Beverage"}
            );

            modelBuilder.Entity<Ingredient>().HasData(
                //Add Bangladeshi restaurant ingredients here
                new Ingredient { IngredientId = 1, Name = "Beef" },
                new Ingredient { IngredientId = 2, Name = "Chicken" },
                new Ingredient { IngredientId = 3, Name = "Fish" },
                new Ingredient { IngredientId = 4, Name = "Lettuce" },
                new Ingredient { IngredientId = 5, Name = "Tomato" },
                new Ingredient { IngredientId = 6, Name = "Tortolla"}
            );

            modelBuilder.Entity<Product>().HasData(
                //Add Bangladeshi restaurant food enties here
                new Product {
                    ProductId = 1, 
                    Name = "Beef",
                    Description = "A delicious teco",
                    Price =2.50m,
                    Stock = 100,
                    CategoryId = 2

                },

                new Product { 
                    ProductId = 2, 
                    Name = "Chicken",
                    Description = "A delicious teco",
                    Price = 3.50m,
                    Stock = 100,
                    CategoryId = 2
                },

                new Product { 
                    ProductId = 3, 
                    Name = "Fish",
                    Description = "A delicious teco",
                    Price = 1.50m,
                    Stock = 100,
                    CategoryId = 2
                },

                new Product { 
                    ProductId = 4, 
                    Name = "Lettuce",
                    Description = "A delicious teco",
                    Price = 2.70m,
                    Stock = 100,
                    CategoryId = 2
                },

                new Product
                {
                    ProductId = 5,
                    Name = "Tomato",
                    Description = "A delicious teco",
                    Price = 1.90m,
                    Stock = 100,
                    CategoryId = 2
                },

                new Product { 
                    ProductId = 6, 
                    Name = "Tortolla",
                    Description = "A delicious teco",
                    Price = 2.30m,
                    Stock = 100,
                    CategoryId = 2
                }
            );


            modelBuilder.Entity<ProductIngredient>().HasData(
                new ProductIngredient { ProductId = 1, IngredientId = 1 },
                new ProductIngredient { ProductId = 1, IngredientId = 2 },
                new ProductIngredient { ProductId = 1, IngredientId = 3 },
                new ProductIngredient { ProductId = 2, IngredientId = 4 },
                new ProductIngredient { ProductId = 2, IngredientId = 5 },
                new ProductIngredient { ProductId = 3, IngredientId = 6}
            );
        }
    }
}
