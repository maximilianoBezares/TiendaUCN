using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.CategoriesDTO
{
    public class ListedCategoriesDTO
    {
        /// <summary>
        /// Lista de categorías.
        /// </summary>
        public List<CategoryDTO> categories { get; set; } = new List<CategoryDTO>();

        /// <summary>
        /// Cantidad total de categorías.
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        /// Total de páginas.
        /// </summary>
        public int totalPages { get; set; }

        /// <summary>
        /// Página actual.
        /// </summary>
        public int currentPage { get; set; }

        /// <summary>
        /// Página actual.
        /// </summary>
        public int pageSize { get; set; }
    }
}