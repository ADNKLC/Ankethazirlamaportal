using Anket.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Anket.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult KayitOl()
        {
            return View();
        }

        public IActionResult GirisYap(string email, string sifre)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQuery = "SELECT * FROM kullanicilar WHERE EPOSTA = @email AND SIFRE = @sifre";

                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@sifre", sifre);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string yetki = reader["YETKI"].ToString();

                                HttpContext.Session.SetString("Yetki", yetki);
                                // Kullanıcı giriş yaptığında
                                HttpContext.Session.SetString("Eposta", email);


                                if (yetki == "ADMIN")
                                {
                                    // AdminDashboard'e yönlendir
                                    return RedirectToAction("AdminDashBoard");
                                }
                                else if (yetki == "USER")
                                {
                                    // UserDashboard'a yönlendir
                                    return RedirectToAction("UserDashBoard");
                                }
                            }
                        }
                    }
                }

                // Kullanıcı bulunamazsa veya yetki uygun değilse, giriş sayfasına geri yönlendir
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                // Hata durumunda loglama veya kullanıcıya bilgi verme işlemleri burada yapılabilir.
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost] // Bu action'ın sadece HTTP POST isteklerine yanıt vereceğini belirtir
        public IActionResult KayitOlFonk(string adsoyad, string eposta, string sifre)
        {
            try
            {
                // Kullanıcıyı veritabanına kaydetme işlemi burada gerçekleştirilir

                // Örnek: Veritabanına yeni kullanıcı ekleme
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL sorgusunu ayarlayın (örneğin, kullanıcı ekleme)
                    string sqlQuery = "INSERT INTO kullanicilar (ADSOYAD, EPOSTA, SIFRE) VALUES (@adsoyad, @eposta, @sifre)";

                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@adsoyad", adsoyad);
                        command.Parameters.AddWithValue("@eposta", eposta);
                        command.Parameters.AddWithValue("@sifre", sifre);

                        command.ExecuteNonQuery(); // Komutu çalıştır
                    }
                }

                // Başarılı kayıt sonrası giriş sayfasına yönlendir
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                // Hata durumunda loglama veya kullanıcıya bilgi verme işlemleri burada yapılabilir.
                return StatusCode(500, "Internal Server Error");
            }
        }

        public IActionResult AdminDashBoard()
        {
            // Oturum kontrolü
            if (HttpContext.Session.GetString("Yetki") == "ADMIN")
            {
                // Yetki kontrolü başarılı, AdminDashboard sayfasını göster
                return View();
            }

            // Oturum kontrolü başarısız veya yetki uygun değilse, başka bir sayfaya yönlendir
            return RedirectToAction("Index");
        }


        public IActionResult AnketOlustur()
        {
            // Oturum kontrolü
            if (HttpContext.Session.GetString("Yetki") == "ADMIN")
            {
                // Yetki kontrolü başarılı, AdminDashboard sayfasını göster
                return View();
            }

            // Oturum kontrolü başarısız veya yetki uygun değilse, başka bir sayfaya yönlendir
            return RedirectToAction("Index");
        }

        public IActionResult AnketDuzenle()
        {
            try
            {
                // Oturum kontrolü
                if (HttpContext.Session.GetString("Yetki") != "ADMIN")
                {
                    // Yetki uygun değilse, başka bir sayfaya yönlendir
                    return RedirectToAction("Index");
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                List<AnketCekModel> anketler = new List<AnketCekModel>(); // Anketleri tutacak liste

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL sorgusunu ayarlayın (örneğin, tüm sütunları çekme)
                    string sqlQuery = "SELECT * FROM anketler";

                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Her bir anket için model nesnesini oluştur ve listeye ekle
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

                                anketler.Add(anketModel); // Listeye ekle
                            }
                        }
                    }
                }

                // Tüm anket listesini view'a model olarak gönder
                return View(anketler);
            }
            catch (Exception)
            {
                // Hata durumunda loglama veya kullanıcıya bilgi verme işlemleri burada yapılabilir.
                return StatusCode(500, "Internal Server Error");
            }
        }


        public IActionResult AnketSil()
        {
            try
            {
                // Oturum kontrolü
                if (HttpContext.Session.GetString("Yetki") != "ADMIN")
                {
                    // Yetki uygun değilse, başka bir sayfaya yönlendir
                    return RedirectToAction("Index");
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                List<AnketCekModel> anketler = new List<AnketCekModel>(); // Anketleri tutacak liste

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL sorgusunu ayarlayın (örneğin, tüm sütunları çekme)
                    string sqlQuery = "SELECT * FROM anketler";

                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Her bir anket için model nesnesini oluştur ve listeye ekle
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

                                anketler.Add(anketModel); // Listeye ekle
                            }
                        }
                    }
                }

                // Tüm anket listesini view'a model olarak gönder
                return View(anketler);
            }
            catch (Exception)
            {
                // Hata durumunda loglama veya kullanıcıya bilgi verme işlemleri burada yapılabilir.
                return StatusCode(500, "Internal Server Error");
            }
        }


        public IActionResult AnketIstatistikleri()
        {
            // Oturum kontrolü
            if (HttpContext.Session.GetString("Yetki") == "ADMIN")
            {
                // Yetki kontrolü başarılı, AdminDashboard sayfasını göster
                return View();
            }

            // Oturum kontrolü başarısız veya yetki uygun değilse, başka bir sayfaya yönlendir
            return RedirectToAction("Index");
        }

        public IActionResult Cikis()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        public IActionResult UserDashBoard()
        {
            try
            {
                // Oturum kontrolü
                if (HttpContext.Session.GetString("Yetki") != "USER")
                {
                    // Yetki uygun değilse, başka bir sayfaya yönlendir
                    return RedirectToAction("Index");
                }

                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "Default Connection String Yok";

                List<AnketCekModel> anketler = new List<AnketCekModel>(); // Anketleri tutacak liste

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL sorgusunu ayarlayın (örneğin, tüm sütunları çekme)
                    string sqlQuery = "SELECT * FROM anketler";

                    using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Her bir anket için model nesnesini oluştur ve listeye ekle
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

                                anketler.Add(anketModel); // Listeye ekle
                            }
                        }
                    }
                }

                // Tüm anket listesini view'a model olarak gönder
                return View(anketler);
            }
            catch (Exception)
            {
                // Hata durumunda loglama veya kullanıcıya bilgi verme işlemleri burada yapılabilir.
                return StatusCode(500, "Internal Server Error");
            }
        }

        public IActionResult UserCikis()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}