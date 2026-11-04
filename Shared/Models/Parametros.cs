using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Models
{
    public class Parametros
    {
        [Key]
        [Required]
        public int id { get; set; }

        public string nit { get; set; }
        public string dir { get; set; }
        public string tel { get; set; }
        public string divipo { get; set; }
        public string ciudad { get; set; }
        [Required]
        public string nombre { get; set; }

        public DateOnly fec_ini_interes { get; set; }

        public double vlr_recibo { get; set; }

        public string correo { get; set; }
        public DateOnly fecha_Nueva_ley { get; set; }
        public DateOnly fin_nueva_ley { get; set; }
        public bool cobra_adicional { get; set; }

        public bool coactivo_man { get; set; }

        public int dias_persuasivo { get; set; }
        public int dias_coactivo { get; set; }

        public int dias_mdto { get; set; }

    }
}
