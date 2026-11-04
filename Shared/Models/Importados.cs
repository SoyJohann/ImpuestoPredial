using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models
{
    public class Importados
    {
        [Key]
        public int id { get; set; }
        public string codigo { get; set; }

        public string cedula { get; set; }
        public string nombre { get; set; }
        public string dir { get; set; }
        public string matricula { get; set; }

        public int area_tet { get; set; }

        public int area_con { get; set; }
        public int tipo { get; set; }

        public int estado { get; set; }
        public bool municipio { get; set; }
        public bool exento { get; set; }
        public double avaluo { get; set; }
        public string estrato { get; set; }
        public bool desenglobo { get; set; }
        public string zona { get; set; }

    }
}
