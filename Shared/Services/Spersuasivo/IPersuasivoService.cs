using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Coactivos_Predial.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Coactivos_Predial.Shared.Services.Spersuasivo
{
    public interface IPersuasivoService
    {

        Task<List<Persuasivos>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);

        Task<int> Procesa_Per(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);
    }

}
