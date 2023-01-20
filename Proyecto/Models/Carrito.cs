using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class Carrito
    {
        public int IdCarrito { get; set; }
        public Producto producto { get; set; }
        public Usuario usuario { get; set; }
    }
}