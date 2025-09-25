using Mapster;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Application.DTO.AuthDTO;

namespace TiendaUCN.src.Application.Mappers
{
    /// <summary>
    /// Clase para mapear objetos de tipo DTO a User y viceversa.
    /// </summary>
    public class UserMapper
    {
        /// <summary>
        /// Configura el mapeo de RegisterDTO a User.
        /// </summary>
        public static void ConfigureAllMappings()
        {
            ConfigureAuthMappings();
        }

        /// <summary>
        /// Configura el mapeo de RegisterDTO a User.
        /// </summary>
        public static void ConfigureAuthMappings()
        {
            TypeAdapterConfig<RegisterDTO, User>.NewConfig()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.BirthDate, src => src.BirthDate)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.EmailConfirmed, src => false);
        }
    }
}