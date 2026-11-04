using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models
{
    public class Recaudo_Externo
    {
        [Required]
        [Key]
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string hora { get; set; }
        public int conc { get; set; }
        public string fecha_real { get; set; }
        public string cod_canal { get; set; }
        public double efectivo { get; set; }
        public double recaudo { get; set; }
        public double cheque { get; set; }
        public double vlr_adicional { get; set; }
        public string comparendo { get; set; }
        public string infractor { get; set; }
        
        public string divipo { get; set; }
        public int tipo_rec { get; set; }

        public string otro { get; set; }
        public string descrip { get; set; }

        public string num_con { get; set; }
    }
}
