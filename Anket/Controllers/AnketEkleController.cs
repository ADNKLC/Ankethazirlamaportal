using Anket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;

namespace Anket.Controllers
{
    public class AnketEkleController : Controller
    {
        private readonly IConfiguration _configuration;

        public AnketEkleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult AnketOlustur([FromBody] AnketViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Gönderilen model null." });
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "INSERT INTO anketler (ANKET_ADI, SORU1, SORU2, SORU3, SORU4, SORU5, SORU6, SORU7, SORU8, SORU9, SORU10) VALUES (@anketAdi, @soru1, @soru2, @soru3, @soru4, @soru5, @soru6, @soru7, @soru8, @soru9, @soru10)";

                        // Parametreleri ekle
                        cmd.Parameters.AddWithValue("@anketAdi", model.AnketAdi);
                        cmd.Parameters.AddWithValue("@soru1", model.Soru1);
                        cmd.Parameters.AddWithValue("@soru2", model.Soru2);
                        cmd.Parameters.AddWithValue("@soru3", model.Soru3);
                        cmd.Parameters.AddWithValue("@soru4", model.Soru4);
                        cmd.Parameters.AddWithValue("@soru5", model.Soru5);
                        cmd.Parameters.AddWithValue("@soru6", model.Soru6);
                        cmd.Parameters.AddWithValue("@soru7", model.Soru7);
                        cmd.Parameters.AddWithValue("@soru8", model.Soru8);
                        cmd.Parameters.AddWithValue("@soru9", model.Soru9);
                        cmd.Parameters.AddWithValue("@soru10", model.Soru10);

                        // Komutu execute et
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Anket başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Anket oluşturulurken bir hata oluştu. Hata detayı: " + ex.Message });
            }
        }
    }
}
