using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Data
{
    public class DataSeeder
    {
        /// <summary>
        /// Método para inicializar la base de datos con datos de prueba.
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios para obtener el contexto de datos y otros servicios.</param>
        /// <returns>Tarea asíncrona que representa la operación de inicialización.</returns>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            try
            {

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                await context.Database.EnsureCreatedAsync();
                await context.Database.MigrateAsync();

                // Creación de roles
                if (!context.Roles.Any())
                {
                    var roles = new List<Role>
                        {
                            new Role { Name = "Admin", NormalizedName = "ADMIN" },
                            new Role { Name = "Customer", NormalizedName = "CUSTOMER" }
                        };
                    foreach (var role in roles)
                    {
                        var result = roleManager.CreateAsync(role).GetAwaiter().GetResult();
                        if (!result.Succeeded)
                        {
                            Log.Error("Error creando rol {RoleName}: {Errors}", role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                            throw new InvalidOperationException($"No se pudo crear el rol {role.Name}.");
                        }
                    }
                    Log.Information("Roles creados con éxito.");
                }

                // Creación de Categorias

                if (!context.Categories.Any())
                {
                    var categories = new List<Category>
                            {
                                new Category { Name = "Electronics" },
                                new Category { Name = "Clothing" },
                                new Category { Name = "Home Appliances" },
                                new Category { Name = "Books" },
                                new Category { Name = "Sports" }
                            };
                    await context.Categories.AddRangeAsync(categories);
                    await context.SaveChangesAsync();
                    Log.Information("Categorías creadas con éxito.");
                }

                // Creación de marcas
                if (!await context.Brands.AnyAsync())
                {
                    var brands = new List<Brand>
                        {
                            new Brand { Name = "Sony" },
                            new Brand { Name = "Apple" },
                            new Brand { Name = "HP" }
                        };
                    await context.Brands.AddRangeAsync(brands);
                    await context.SaveChangesAsync();
                    Log.Information("Marcas creadas con éxito.");
                }

            // Creación de productos
                if (!await context.Products.AnyAsync())
                {
                    var categoryIds = await context.Categories.Select(c => c.Id).ToListAsync();
                    var brandIds = await context.Brands.Select(b => b.Id).ToListAsync();

                    if (categoryIds.Any() && brandIds.Any())
                    {
                        var productFaker = new Faker<Product>()
                            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                            .RuleFor(p => p.Price, f => f.Random.Int(1000, 100000))
                            .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                            .RuleFor(p => p.CategoryId, f => f.PickRandom(categoryIds))
                            .RuleFor(p => p.BrandId, f => f.PickRandom(brandIds))
                            .RuleFor(p => p.Status, f => f.PickRandom<Status>());

                        var products = productFaker.Generate(50);
                        await context.Products.AddRangeAsync(products);
                        await context.SaveChangesAsync();
                        Log.Information("Productos creados con éxito.");
                    }

                    // Creación de imágenes
                    if (!await context.Images.AnyAsync())
                    {
                        var productIds = await context.Products.Select(p => p.Id).ToListAsync();
                        var imageFaker = new Faker<Image>()
                            .RuleFor(i => i.ImageUrl, f => f.Image.PicsumUrl())
                            .RuleFor(i => i.PublicId, f => f.Random.Guid().ToString())
                            .RuleFor(i => i.ProductId, f => f.PickRandom(productIds));

                        var images = imageFaker.Generate(20);
                        await context.Images.AddRangeAsync(images);
                        await context.SaveChangesAsync();
                        Log.Information("Imágenes creadas con éxito.");
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al inicializar la base de datos: {Message}", ex.Message);
            }
            
            
        }

        /// <summary>
        /// Método para generar un RUT chileno aleatorio.
        /// </summary>
        /// <returns>Un RUT en formato "XXXXXXXX-X".</returns>
        private static string RandomRut()
        {
            var faker = new Faker();
            var rut = faker.Random.Int(1000000, 99999999).ToString();
            var dv = faker.Random.Int(0, 9).ToString();
            return $"{rut}-{dv}";
        }

        /// <summary>
        /// Método para generar un número de teléfono chileno aleatorio.
        /// </summary>
        /// <returns>Un número de teléfono en formato "+569 XXXXXXXX".</returns>
        private static string RandomPhoneNumber()
        {
            var faker = new Faker();
            string firstPartNumber = faker.Random.Int(1000, 9999).ToString();
            string secondPartNumber = faker.Random.Int(1000, 9999).ToString();
            return $"+569 {firstPartNumber}{secondPartNumber}";
        }
    }
}