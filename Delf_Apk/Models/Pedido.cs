using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delf_Apk.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string? Numero { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }


        //Clave foranea a Cliente
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        //Relacion uno a muchos con articulos        
        public ICollection<ArticuloCantidad>? ArticuloCantidades { get; set; } = new List<ArticuloCantidad>();

    }
}
