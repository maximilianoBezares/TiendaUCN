using System.Xml.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using TiendaUCN.src.Application.DTO.UserProfileDTO;

namespace TiendaUCN.src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementación del repositorio de usuarios.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly int _daysOfDeleteUnconfirmedUsers;
        public UserRepository(DataContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _daysOfDeleteUnconfirmedUsers = configuration.GetValue<int?>("Jobs:DaysOfDeleteUnconfirmedUsers") ?? throw new InvalidOperationException("La configuración 'Jobs:DaysOfDeleteUnconfirmedUsers' no está definida.");
        }

        /// <summary>
        /// Verifica si la contraseña proporcionada es correcta para el usuario.
        /// </summary>
        /// <param name="user">Usuario al que se le verificará la contraseña</param>
        /// <param name="password">Contraseña a verificar</param>
        /// <returns>True si la contraseña es correcta, false en caso contrario</returns>
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// Confirma el correo electrónico del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>True si la confirmación fue exitosa, false en caso contrario</returns>
        public async Task<bool> ConfirmEmailAsync(string email)
        {
            var result = await _context.Users.Where(u => u.Email == email).ExecuteUpdateAsync(u => u.SetProperty(x => x.EmailConfirmed, true));
            return result > 0;
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="user">Usuario a crear</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>True si es exitoso, false en caso contrario</returns>
        public async Task<bool> CreateAsync(User user, string password)
        {
            var userResult = await _userManager.CreateAsync(user, password);
            if (userResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                return roleResult.Succeeded;
            }
            return false;
        }

        /// <summary>
        /// Elimina un usuario por su ID.
        /// </summary>
        /// <param name="userId">ID del usuario a eliminar</param>
        /// <returns>True si la eliminación fue exitosa, false en caso contrario</returns>
        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.DeleteAsync(user!);
            return result.Succeeded;
        }

        /// <summary>
        /// Elimina usuarios no confirmados.
        /// </summary>
        /// <returns>Número de usuarios eliminados</returns>
        public async Task<int> DeleteUnconfirmedAsync()
        {
            Log.Information("Iniciando eliminación de usuarios no confirmados");

            var cutoffDate = DateTime.UtcNow.AddDays(_daysOfDeleteUnconfirmedUsers);

            var unconfirmedUsers = await _context.Users
                .Where(u => !u.EmailConfirmed && u.RegisteredAt < cutoffDate)
                .Include(u => u.VerificationCodes)
                .ToListAsync();

            if (!unconfirmedUsers.Any())
            {
                Log.Information("No se encontraron usuarios no confirmados para eliminar");
                return 0;
            }

            _context.Users.RemoveRange(unconfirmedUsers);
            await _context.SaveChangesAsync();

            Log.Information($"Eliminados {unconfirmedUsers.Count} usuarios no confirmados");
            return unconfirmedUsers.Count;
        }

        /// <summary>
        /// Verifica si un usuario existe por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>True si el usuario existe, false en caso contrario</returns>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Verifica si un usuario existe por su RUT.
        /// </summary>
        /// <param name="rut">RUT del usuario</param>
        /// <returns>True si el usuario existe, false en caso contrario</returns>
        public async Task<bool> ExistsByRutAsync(string rut)
        {
            return await _context.Users.AnyAsync(u => u.Rut == rut);
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        /// <summary>
        /// Obtiene un usuario por su RUT.
        /// </summary>
        /// <param name="rut">RUT del usuario</param>
        /// <param name="trackChanges">Indica si se debe rastrear los cambios en la entidad</param>
        /// <returns>Usuario encontrado o nulo</returns>
        public async Task<User?> GetByRutAsync(string rut, bool trackChanges = false)
        {
            if (trackChanges)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Rut == rut);
            }

            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Rut == rut);
        }

        /// <summary>
        /// Obtiene el rol del usuario.
        /// </summary>
        /// <param name="user">Usuario del cual se desea obtener el rol</param>
        /// <returns>Nombre del rol del usuario</returns>
        public async Task<string> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault()!;
        }

        /// <summary>
        /// Actualiza la contraseña del usuario.
        /// </summary>
        /// <param name="user">Usuario al cual se le actualizará la contraseña</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>True si la actualización fue exitosa, false en caso contrario</returns
        public async Task<bool> UpdatePasswordAsync(User user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                Log.Error($"Error al actualizar la contraseña del usuario con ID: {user.Id}. Errores: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            await _userManager.UpdateSecurityStampAsync(user);
            return result.Succeeded;
        }

        /// <summary>
        /// Actualiza el perfil de un usuario por su ID.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="updateProfileDTO">DTO que contiene los datos actualizados del perfil del usuario.</param>
        /// <returns>True si la actualización fue exitosa, false en caso contrario</returns
        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            if (!string.Equals(user.Email, updateProfileDTO.Email, StringComparison.OrdinalIgnoreCase))
            {
                bool emailExists = await _context.Users.AnyAsync(u => u.Email == updateProfileDTO.Email && u.Id != userId);
                if (emailExists)
                    throw new ArgumentException("El correo electrónico ya está registrado por otro usuario.");
            }
            if (!string.Equals(user.Rut, updateProfileDTO.Rut, StringComparison.OrdinalIgnoreCase))
            {
                bool rutExists = await _context.Users.AnyAsync(u => u.Rut == updateProfileDTO.Rut && u.Id != userId);
                if (rutExists)
                    throw new ArgumentException("El RUT ya está registrado por otro usuario.");
            }
            user.FirstName = updateProfileDTO.FirstName;
            user.LastName = updateProfileDTO.LastName;
            user.BirthDate = updateProfileDTO.BirthDate;
            user.Rut = updateProfileDTO.Rut;
            user.Email = updateProfileDTO.Email;
            user.PhoneNumber = updateProfileDTO.PhoneNumber;
            if (!string.IsNullOrEmpty(updateProfileDTO.Gender))
            {
                if (Enum.TryParse<Gender>(updateProfileDTO.Gender, out var gender))
                {
                    user.Gender = gender;
                }
                else
                {
                    throw new ArgumentException("Género inválido.");
                }
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }    
}