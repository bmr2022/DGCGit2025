using DinkToPdf;
using DinkToPdf.Contracts;
using eTactWeb.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services
{
    public   class PdfService : IPdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdfFromHtml(string html)
        {
            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
            },
                Objects = {
                new ObjectSettings() {
                    HtmlContent = html,
                    WebSettings = {
                        DefaultEncoding = "utf-8",
                        LoadImages = true
                    }
                }
            }
            };

            return _converter.Convert(document);
        }
    }
}
