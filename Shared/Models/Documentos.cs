namespace Coactivos_Predial.Shared.Models
{
    public class Documentos
    {
        public int id { get; set; }

        public string codigo_predial { get; set; }
        public string nombre { get; set; }
        
        public DateOnly fecha { get; set; }

        public  string archivo { get; set; }

        public int num { get; set; }
    }
}
