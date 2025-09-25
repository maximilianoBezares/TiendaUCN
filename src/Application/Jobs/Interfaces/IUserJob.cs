namespace Tienda_UCN_api.Src.Application.Jobs.Interfaces
{
    /// <summary>
    /// Interfaz para trabajos relacionados con usuarios.
    /// </summary>
    public interface IUserJob
    {
        Task DeleteUnconfirmedAsync();
    }
}