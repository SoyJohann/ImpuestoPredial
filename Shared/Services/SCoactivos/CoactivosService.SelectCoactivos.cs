namespace Coactivos_Predial.Shared.Services.SCoactivos;

public partial class CoactivosService
{
    private static string SelectCoactivos(string source) =>
        "SELECT id AS \"Id\", codigo AS \"Codigo\", cedula AS \"Cedula\", " +
        "resolucion AS \"Resolucion\", pnombres AS \"PrimerosNombres\", dir AS \"Direccion\", " +
        "make_date(desde::integer, 1, 1) AS \"Desde\", make_date(hasta::integer, 1, 1) AS \"Hasta\", " +
        "sem1 AS \"SemestreUno\", sem2 AS \"SemestreDos\", fecha AS \"Fecha\", " +
        "fecha_sancion AS \"FechaSancion\", fecha_mdto AS \"FechaMandamiento\", " +
        "fecha_proceso AS \"FechaProceso\", valor AS \"Valor\" FROM " + source;
}
