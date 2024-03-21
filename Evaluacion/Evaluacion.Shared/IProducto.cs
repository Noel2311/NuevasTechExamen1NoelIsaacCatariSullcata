using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.Shared
{
    public interface IProducto
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        public double Precio { get; set; }
        public string NombreProveedor { get; set; }
    }
}
