using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SPredios
{

    public class Item_res
    {
        public int item { get; set; }
        public int num { get; set; }
    }
    public interface IPrediosService
    {
        Task<List<Predios>> GetAll();
        Task<int> AgregarDocumento(Documentos documento);

        Task<int> DeleteDocs(int id);
        Task<List<ComparendoInfo>> GetInfoComparendo(string pcedula);
        Task<Predios> GetPrediosByCode(string comp);

        Task<List<Documentos>> GetPdfsByComp(string comp);

        Task<List<Procesos>> GetCoactivo(string comp);

        Task<List<DocumentoPredial>> Lista_Diario(DateTime pdesde, DateTime phasta);

        Task<Item_res> Pdfs_Resumen(DateTime pdesde, DateTime phasta);

    }   
}
