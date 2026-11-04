using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.JSInterop;
using Coactivos_Predial.Shared.Services.SPropietarios;
using Coactivos_Predial.Shared.Services.SPropietarios;

namespace Coactivos_Predial.Models
{
    public class Propietarios
    {
        private readonly PropietariosService _infractorService;
        public Propietarios(PropietariosService infractorService)
        {
            _infractorService = infractorService;
        }

        public Propietarios()
        {

        }

        [Required]
        [Key]
        public int id { get; set; }
        [Required]
        public string documento { get; set; }
        [Required]
        public int tipo_id { get; set; }

        [StringLength(40)]
        public string nombre { get; set; }
        
        public string estado_civil { get; set; }
        public string tel { get; set; }
        public string correo { get; set; }
        public string fallecido { get; set; }

        public string? ntipo { get; set; }

    }
}


