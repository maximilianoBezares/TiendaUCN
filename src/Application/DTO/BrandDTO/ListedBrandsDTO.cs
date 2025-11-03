using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.BrandDTO
{
    public class ListedBrandsDTO
    {
        /// <summary>
        /// Lista de marcas.
        /// </summary>
        public List<BrandDTO> brands { get; set; } = new List<BrandDTO>();

        /// <summary>
        /// Cantidad total de marcas.
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