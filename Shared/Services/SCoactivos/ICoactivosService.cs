using Coactivos_Predial.Shared.Models;

using Microsoft.Extensions.Configuration;

namespace Coactivos_Predial.Shared.Services.SCoactivos
{
    public class Transac_Report
    {
        public int transaccion { get; set; }
        public DateOnly fecha { get; set; }
        public int No { get; set; }

    }

    public interface ICoactivosService
    {
        Task<List<Coactivos>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);

        Task<List<Coactivos>> GetList1(string x);
        Task<int> Procesa_Coactivo(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);

        Task<List<Coactivos>> Lista_Coactivos(int opcion, string pcomp, int pvigencia, int ptran, int ptipo);
        Task<List<Transac_Report>> GetListtran(string ptipo, int pclase);
    }
}
