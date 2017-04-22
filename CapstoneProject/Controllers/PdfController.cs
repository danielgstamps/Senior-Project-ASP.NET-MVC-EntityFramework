using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CapstoneProject.DAL;
using MvcRazorToPdf;

namespace CapstoneProject.Controllers
{
    public class PdfController : Controller
    {
        private IUnitOfWork unitOfWork = new UnitOfWork();

        // GET: Pdf
        public ActionResult Index()
        {
            return View("Index", this.unitOfWork.EvaluationRepository.Get());
        }

        /// <summary>
        /// Writes a pdf to application data directory.
        /// </summary>
        public void WriteToAppData()
        {
            var anon = new
            {
                Output = "Write me to a pdf in the application data directory"
            };
            byte[] pdfOutput = ControllerContext.GeneratePdf(anon, "");
            string fullPath = Server.MapPath("~/App_Data/MadeByPdfController.pdf");
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            System.IO.File.WriteAllBytes(fullPath, pdfOutput);
        }

        public ActionResult IndexWithAccessToDocumentAndWriter()
        {
            var anon = new
            {
                Output = "IndexWithAccessToDocumentAndWriter"
            };
            return new PdfActionResult(anon, (writer, document) =>
            {
                document.SetPageSize(new Rectangle(500f, 500f, 90));
                document.NewPage();
            });
        }

        public ActionResult DownloadPDF()
        {
            var anon = new
            {
                Output = "Download me!"
            };
            return new PdfActionResult(anon, (writer, document) =>
            {
                document.SetPageSize(new Rectangle(500f, 500f, 90));
                document.NewPage();
            })
            {
                FileDownloadName = "DownloadMe.pdf"
            };
        }

    }
}