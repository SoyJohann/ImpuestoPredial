using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coactivos_Predial.Models
{
    public class Predios
    {
        [Required]
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(20)]
        public string codigo { get; set; }
        [Required]
        [StringLength(14)]
        public string cedula { get; set; }
        [Required]

        public string nombre { get; set; }
        public string dir { get; set; }
        public string matricula { get; set; }
        
        public int area_tet { get; set; }
        
        public int area_con { get; set; }
        public int? tipo { get; set; }
        public string? ntipo { get; set; }
        public int estado { get; set; }
        public bool municipio { get; set; }
        
        public bool exento { get; set; }
        public double avaluo { get; set; }
        public int tipo_id { get; set; }
        public string estrato { get; set; }
        public bool desenglobo { get; set; }
        public string zona { get; set; }
        public int barrio  { get; set; }
        public bool ley550 { get; set; }
    }
}
