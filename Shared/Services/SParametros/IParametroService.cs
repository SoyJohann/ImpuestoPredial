using Coactivos_Predial.Models;
using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Services.SParametros
{

    public class Estados_proc
    {
        [Required]
        [Key]
        public int id { get; set; }
        [StringLength(20)]
        [Required]
        public string nombre { get; set; }

        [Required]
        public float porc { get; set; }
    }

    public interface IParametroService
    {
        Task<List<Parametros>> GetAll();

        Task<int> EditParametros(int id, Parametros parametros);
        Task<int> EditEstados(Estados_proc agentes_obj);
        Task<Parametros>GetParametroById(int id);
        Task<List<Estados_proc>> Getlist();

    }
}
