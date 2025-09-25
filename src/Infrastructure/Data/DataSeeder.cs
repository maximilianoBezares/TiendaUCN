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