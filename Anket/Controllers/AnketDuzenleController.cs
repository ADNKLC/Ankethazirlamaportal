// AnketDuzenleController.cs
using Microsoft.AspNetCore.Mvc;
using Anket.Models;

namespace Anket.Controllers
{
    public class AnketDuzenleController : Controller
    {
        public IActionResult AnketDuzenle()
        {
            // Anket verilerini oluştur
            var anketModel = new AnketCekModel
            {
                AnketAdi = "Anket Adı"
            };

            // View'a modeli gönder
            return View(anketModel);
        }
    }
}
