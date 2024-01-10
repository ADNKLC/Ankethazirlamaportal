using Anket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;

namespace Anket.Controllers
{
    public class AnketCekController : Controller
    {
        private readonly IConfiguration _configuration;

        public AnketCekController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult AnketCek([FromBody] AnketViewModel model)
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

                    // View'den gelen veriye göre select sorgusu
                    string sqlQuery = "SELECT * FROM anketler WHERE ANKET_ADI = @anketAdi";

                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@anketAdi", model.AnketAdi);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var anketModel = new AnketCekModel
                                {
                                    AnketId = reader["KAYIT_ID"].ToString(),
                                    AnketAdi = reader["ANKET_ADI"].ToString(),
                                    Soru1 = reader["SORU1"].ToString(),
                                    Soru2 = reader["SORU2"].ToString(),
                                    Soru3 = reader["SORU3"].ToString(),
                                    Soru4 = reader["SORU4"].ToString(),
                                    Soru5 = reader["SORU5"].ToString(),
                                    Soru6 = reader["SORU6"].ToString(),
                                    Soru7 = reader["SORU7"].ToString(),
                                    Soru8 = reader["SORU8"].ToString(),
                                    Soru9 = reader["SORU9"].ToString(),
                                    Soru10 = reader["SORU10"].ToString()
                                };

                                return Json(new { success = true, message = "Anket başarıyla çekildi.", anketVerileri = anketModel });
                            }
                        }
                    }
                }

                return Json(new { success = false, message = "Belirtilen anket adına sahip anket bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Anket çekilirken bir hata oluştu. Hata detayı: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AnketGuncelle([FromBody] AnketViewModel model)
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
                        cmd.CommandText = "UPDATE anketler SET ANKET_ADI = @anketAdi, " +
                                          "SORU1 = @soru1, SORU2 = @soru2, SORU3 = @soru3, SORU4 = @soru4, SORU5 = @soru5, " +
                                          "SORU6 = @soru6, SORU7 = @soru7, SORU8 = @soru8, SORU9 = @soru9, SORU10 = @soru10 WHERE KAYIT_ID = @anketId";

                        // Parametreleri ekle
                        cmd.Parameters.AddWithValue("@anketId", model.AnketId);
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

                return Json(new { success = true, message = "Anket başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Anket güncellenirken bir hata oluştu. Hata detayı: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AnketSil([FromBody] int anketId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "DELETE FROM anketler WHERE KAYIT_ID = @anketId";

                        // Parametreleri ekle
                        cmd.Parameters.AddWithValue("@anketId", anketId);

                        // Komutu execute et
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Anket başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Anket silinirken bir hata oluştu. Hata detayı: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AnketEkle([FromBody] AnketEkleModel model)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "INSERT INTO tamamlanan (ANKET_ID, EPOSTA, SORU1, CEVAP1, SORU2, CEVAP2, SORU3, CEVAP3, SORU4, CEVAP4, SORU5, CEVAP5, " +
                                          "SORU6, CEVAP6, SORU7, CEVAP7, SORU8, CEVAP8, SORU9, CEVAP9, SORU10, CEVAP10) " +
                                          "VALUES (@anketid ,@eposta, @soru1, @cevap1, @soru2, @cevap2, @soru3, @cevap3, @soru4, @cevap4, @soru5, @cevap5, " +
                                          "@soru6, @cevap6, @soru7, @cevap7, @soru8, @cevap8, @soru9, @cevap9, @soru10, @cevap10)";

                        // Parametreleri ekle
                        cmd.Parameters.AddWithValue("@anketid", model.AnketId);
                        cmd.Parameters.AddWithValue("@eposta", model.Eposta);
                        cmd.Parameters.AddWithValue("@soru1", model.Soru1);
                        cmd.Parameters.AddWithValue("@cevap1", model.Cevap1);
                        cmd.Parameters.AddWithValue("@soru2", model.Soru2);
                        cmd.Parameters.AddWithValue("@cevap2", model.Cevap2);
                        cmd.Parameters.AddWithValue("@soru3", model.Soru3);
                        cmd.Parameters.AddWithValue("@cevap3", model.Cevap3);
                        cmd.Parameters.AddWithValue("@soru4", model.Soru4);
                        cmd.Parameters.AddWithValue("@cevap4", model.Cevap4);
                        cmd.Parameters.AddWithValue("@soru5", model.Soru5);
                        cmd.Parameters.AddWithValue("@cevap5", model.Cevap5);
                        cmd.Parameters.AddWithValue("@soru6", model.Soru6);
                        cmd.Parameters.AddWithValue("@cevap6", model.Cevap6);
                        cmd.Parameters.AddWithValue("@soru7", model.Soru7);
                        cmd.Parameters.AddWithValue("@cevap7", model.Cevap7);
                        cmd.Parameters.AddWithValue("@soru8", model.Soru8);
                        cmd.Parameters.AddWithValue("@cevap8", model.Cevap8);
                        cmd.Parameters.AddWithValue("@soru9", model.Soru9);
                        cmd.Parameters.AddWithValue("@cevap9", model.Cevap9);
                        cmd.Parameters.AddWithValue("@soru10", model.Soru10);
                        cmd.Parameters.AddWithValue("@cevap10", model.Cevap10);

                        // Komutu execute et
                        cmd.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Anket başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Anket eklenirken bir hata oluştu. Hata detayı: " + ex.Message });
            }
        }

    }
}