using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static iTextSharp.text.pdf.AcroFields;
using Coactivos_Predial.Shared.Models;
using System.Runtime.Intrinsics.X86;





namespace Coactivos_Predial.Reportes
{
    public class Recibo_pago
    {
        
        public async Task MdtoPdf(List<Coactivos> mdtos)
        {
            try
            {
                PdfDocument doc = new PdfDocument();
                doc.Info.Title = "Mandamiento_pago"; string texto1=""; int vlt = 0;
                var fullName = "wwwroot/logos/logo_cim1.jpg";
                var image = XImage.FromFile(fullName);
                fullName = "wwwroot/logos/logo_cim2.jpg";
                var image2 = XImage.FromFile(fullName);
                fullName = "wwwroot/logos/firma_cim.jpg";
                var firma = XImage.FromFile(fullName);
                fullName = "wwwroot/logos/pie_cim.jpg";
                var image3 = XImage.FromFile(fullName);

                foreach (var item in mdtos)
                {
                    PdfPage page = doc.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XTextFormatter tf = new XTextFormatter(gfx);
                    XRect rect = new XRect(40, 90, 250, 550);
                    XFont font = new XFont("Verdana", 9);
                    gfx.DrawImage(image, 43, 19, 72, 60);
                    gfx.DrawImage(image2, 480, 19, 72, 60);
                    gfx.DrawLine(new XPen(XColor.FromArgb(32, 32, 36)), new XPoint(40, 80), new XPoint(562, 80));
                    gfx.DrawString("ALCALDÍA MUNICIPAL", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 55), XStringFormat.Center);
                    gfx.DrawString("CIMITARRA SANTANDER", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 80), XStringFormat.Center);
                    gfx.DrawString("PROCESO DE COBRO COACTIVO POR IMPUESTO PREDIAL", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 99), XStringFormat.Center);
                    gfx.DrawString("MUNICIPIO DE CIMITARRA DEPARTAMENTO DE SANTANDER", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 175), XStringFormat.Center);
                    gfx.DrawString("SECRETARIA DE HACIENDA Y DEL TESORO PÚBLICO", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 195), XStringFormat.Center);
                    gfx.DrawString("COBRO COACTIVO", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 220), XStringFormat.Center);
                    gfx.DrawString("Municipio de Cimitarra, " + item.fecha_mdto.ToShortDateString(), new XFont("Arial", 8, XFontStyleEx.Regular), XBrushes.Black, new XPoint(40, 124));
                    gfx.DrawString("Señor(a)" , new XFont("Arial", 9, XFontStyleEx.Regular), XBrushes.Black, new XPoint(40, 134));
                    gfx.DrawString(item?.pnombres, new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XPoint(40, 144));
                    gfx.DrawString($" {item?.dir ?? ""}", font, XBrushes.Black, new XPoint(40, 154));
                    Console.WriteLine($"segundo reten: {fullName}"); // 000100010013000
                    gfx.DrawString(item?.cedula, new XFont("Arial", 9, XFontStyleEx.Regular), XBrushes.Black, new XPoint(40, 164));
                    vlt = (int)item.valor;
                    texto1 = "Obra al despacho para su cobro por jurisdicción coactiva la RESOLUCION No. " + item.resolucion + " de fecha " + item.fecha + " publicada en la pagina web www.cimitarra-santander.gov.co el dia 23/08/2024 sin que interpusiera recurso de" +
                    "reconsideracion alguno, POR MEDIO DEL CUAL SE LIQUIDA OFICIALMENTE LA DEUDA POR CONCEPTO DE IMPUESTO PREDIAL UNIFICADO A FAVOR DEL MUNICIPIO DE CIMITARRA - SANTANDER, en el cual consta una obligación clara, expresa y actualmente " +
                    "exigible, y en contra de "+item.pnombres.Trim()+" Identificado con cc No "+item.cedula+" en calidad de CONTRIBUYENTE, PROPIETARIO, POSEEDOR, USUFRUCTUARIO, HEREDERO, por concepto de impuesto predial unificado correspondiente a los periodos fiscales" +
                    "del año "+item.desde+" "+item.sem2+" - "+ item.hasta + " " + item.sem2+" en cuantía de "+item.valor.ToString("C")+" sin incluir intereses y costas procesales, documento que presta merito ejecutivo de conformidad con el art. 828 del Estatuto Tributario, sumas que no han sido pagadas por el contribuyente, " +
                    "por lo cual cabe iniciar el procedimiento de cobro administrativo coactivo contenido en los artículos 823 y siguientes del Estatuto Tributario para obtener su pago." +
                    "\r\n \r\nLa suscrita funcionaria es competente para conocer del procedimiento, según lo dispuesto en el Acuerdo Municipal 132 del 2018(Estatuto de Rentas), por lo cual";

                    rect = new XRect(40, 175, 520, 170); 
                    gfx.DrawRectangle(XBrushes.White, rect);  
                    tf.Alignment = XParagraphAlignment.Justify;
                    tf.DrawString(texto1, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                    gfx.DrawString("RESUELVE", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 720), XStringFormat.Center);
                    texto1 = "PRIMERO: librar orden de pago por la vía administrativa coactiva a favor del Municipio de Cimitarra y a cargo de "+item.pnombres.Trim()+" Identificado con cc No "+item.cedula+" por la suma "+item.valor.ToString("C")+", por los conceptos y periodos señalados en la " +
                    "parte motiva, más los intereses que se causen desde cuando se hizo exigible cada obligación y hasta cuando se cancelen conforme lo disponen los artículos 634, 635 y 867-1 del Estatuto Tributario, más las costas del presente proceso." +
                    "\r\n \r\nSEGUNDO: Notificar este mandamiento de pago de conformidad con el artículo 8 de la ley 2213 del 2022, es decir no requerirá citación previa para notificar el presente acto administrativo.\r\n \r\nTERCERO: Advertir al deudor que dispone de quince " +
                    "(15) días después de notificada esta providencia, para cancelar la deuda o proponer las excepciones legales que estime pertinentes, conforme al art. 831 del Estatuto Tributario.\r\n \r\nCUARTRO: ORDENESE el embargo y secuestro de los dineros que se encuentren " +
                    "en las cuentas corrientes, de ahorro o CDT, que tenga el deudor "+item.pnombres.Trim()+" Identificado con cc No "+item.cedula+", en las siguientes entidades bancarias: Bancolombia, Banco Agrario, Banco Bogota, Av Villas, Caja Social, Davivienda, Occidente, Helm Bank, " +
                    "Banco Pichincha, Banco popular, Colpatria, Bbva, Coopcentral, Citibank así como también ofíciese a Instrumentos Públicos de Cimitarra con el fin de que informe el número de matrícula inmobiliaria al cual pertenece el numero predial.\r\n \r\nQUINTO: líbrense los oficios correspondientes.";
                    rect = new XRect(40, 380, 520, 237);
                    gfx.DrawRectangle(XBrushes.White, rect);
                    tf.Alignment = XParagraphAlignment.Justify;
                    tf.DrawString(texto1, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                    gfx.DrawString("NOTIFIQUESE Y CUMPLASE", new XFont("Arial", 9, XFontStyleEx.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 1250), XStringFormat.Center);
                    gfx.DrawImage(firma, 83, 670, 260, 80);
                    gfx.DrawImage(image3, 40, 775, 520, 54);
                }
                const string filename = "e:\\prueba_predial\\Mdtos.pdf";
                doc.Save(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        

    }
}
