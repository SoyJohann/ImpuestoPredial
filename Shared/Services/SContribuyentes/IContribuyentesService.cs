using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPDios.Responses;

namespace Coactivos_Predial.Shared.Services.SContribuyentes;

public interface IContribuyentesService
{
    /// <summary>Consulta los negocios registrados a un NIT (documento en Propietarios).</summary>
    Task<Result<List<ContribuyenteDto>>> GetByNit(string nit);

    /// <summary>Documentos de industria y comercio asociados al contribuyente.</summary>
    Task<Result<List<DocumentoDto>>> GetDocumentos(int contribuyenteId);
}
