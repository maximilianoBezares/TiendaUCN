using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.CategoriesDTO
{
    public class CategoryDTO
    {
        /// <summary>
        /// Identificador de la categoría.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Nombre de la categoría.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Slug de la categoría.
        /// </summary>
        public string slug { get; set; }

        /// <summary>
        /// Descripción de la categoría.
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Cantidad de productos en la categoría.
        /// </summary>
        public int productCount { get; set; }
    }
}