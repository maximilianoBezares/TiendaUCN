using Serilog;
using Mapster;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using TiendaUCN.src.Application.DTO.AuthDTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using TiendaUCN.src.Application.Mappers;

namespace TiendaUCN.src.Application.Services.Implements
{
    /// <summary>
    /// Implementación del servicio de usuarios.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly int _verificationCodeExpirationTimeInMinutes;

        public UserService(ITokenService tokenService, IUserRepository userRepository, IEmailService emailService, IVerificationCodeRepository verificationCodeRepository, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _emailService = emailService;
            _verificationCodeRepository = verificationCodeRepository;
            _configuration = configuration;
            _verificationCodeExpirationTimeInMinutes = _configuration.GetValue<int>("VerificationCode:ExpirationTimeInMinutes");
        }

        /// <summary>
        /// Elimina usuarios no confirmados.
        /// </summary>
        /// <returns>Número de usuarios eliminados</returns>
        public async Task<int> DeleteUnconfirmedAsync()
        {
            return await _userRepository.DeleteUnconfirmedAsync();
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el token JWT generado y la id del usuario.</returns>
        public async Task<(string token, int userId)> LoginAsync(LoginDTO loginDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                Log.Warning($"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress}");
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            if (!user.EmailConfirmed)
            {
                Log.Warning($"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress} - Correo no confirmado.");
                throw new InvalidOperationException("El correo electrónico del usuario no ha sido confirmado.");
            }

            var result = await _userRepository.CheckPasswordAsync(user, loginDTO.Password);
            if (!result)
            {
                Log.Warning($"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress}");
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            string roleName = await _userRepository.GetUserRoleAsync(user) ?? throw new InvalidOperationException("El usuario no tiene un rol asignado.");

            // Generamos el token
            Log.Information($"Inicio de sesión exitoso para el usuario: {loginDTO.Email} desde la IP: {ipAddress}");
            var token = _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);
            return (token, user.Id);
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el mensaje de éxito del registro.</returns>
        public async Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
            Log.Information($"Intento de registro de nuevo usuario: {registerDTO.Email} desde la IP: {ipAddress}");

            bool isRegistered = await _userRepository.ExistsByEmailAsync(registerDTO.Email);
            if (isRegistered)
            {
                Log.Warning($"El usuario con el correo {registerDTO.Email} ya está registrado.");
                throw new InvalidOperationException("El usuario ya está registrado.");
            }
            isRegistered = await _userRepository.ExistsByRutAsync(registerDTO.Rut);
            if (isRegistered)
            {
                Log.Warning($"El usuario con el RUT {registerDTO.Rut} ya está registrado.");
                throw new InvalidOperationException("El RUT ya está registrado.");
            }
            var user = registerDTO.Adapt<User>();
            var result = await _userRepository.CreateAsync(user, registerDTO.Password);
            if (!result)
            {
                Log.Warning($"Error al registrar el usuario: {registerDTO.Email}");
                throw new Exception("Error al registrar el usuario.");
            }
            Log.Information($"Registro exitoso para el usuario: {registerDTO.Email} desde la IP: {ipAddress}");
            string code = new Random().Next(100000, 999999).ToString();
            var verificationCode = new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                CodeType = CodeType.EmailVerification,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes)
            };
            var createdVerificationCode = await _verificationCodeRepository.CreateAsync(verificationCode);
            Log.Information($"Código de verificación generado para el usuario: {registerDTO.Email} - Código: {createdVerificationCode.Code}");

            await _emailService.SendVerificationCodeEmailAsync(registerDTO.Email, createdVerificationCode.Code);
            Log.Information($"Se ha enviado un código de verificación al correo electrónico: {registerDTO.Email}");
            return "Se ha enviado un código de verificación a su correo electrónico.";
        }
    }
}