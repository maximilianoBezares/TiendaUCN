using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.BrandDTO
{
    public class BrandDTO
    {
        /// <summary>
        /// Identificador de la marca.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Nombre de la marca.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Slug de la marca.
        /// </summary>
        public string slug { get; set; }

        /// <summary>
        /// Descripción de la marca.
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Cantidad de productos en la marca.
        /// </summary>
        public int productCount { get; set; }
    }
}