using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KurumsalWeb.Controllers
{
    public class AdminController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Admin
        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.BlogSay = db.Blog.Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.HizmetSay = db.Hizmet.Count();
            ViewBag.YorumSay = db.Hizmet.Count();
            ViewBag.YorumOnay = db.Yorum.Where(x=>x.Onay==false).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }
        [Route("yonetimpaneli/giris")]
       public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
           
            var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
            if (login.Eposta == admin.Eposta && login.Sifre == Crypto.Hash(admin.Sifre,"MD5"))
            {
                Session["adminid"] = login.AdminId;
                Session["eposta"] = login.Eposta;
                Session["yetki"] = login.Yetki;
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Uyari = "Kullanıcı Adı Veya Şifre Hatalı Girdiniz..";
            return View(admin);
        }
        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["eposta"]= null;
            Session.Abandon();

            return RedirectToAction("Login","Admin");
        }
        public ActionResult SifremiUnuttum()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string eposta)
        {
            var mail = db.Admin.Where(x=>x.Eposta==eposta).SingleOrDefault();
            if ( mail!=null)
            {
                Random rnd = new Random();
                int yenisifre = rnd.Next();
                Admin admin = new Admin();
                mail.Sifre = Crypto.Hash(Convert.ToString(yenisifre),"MD5");
                db.SaveChanges();


                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("kemal_seday@hotmail.com", "kemal01seday56");
                client.Port = 587;
                client.Host = "smtp.live.com";
                client.EnableSsl = true;
                message.To.Add(eposta);
                message.From = new MailAddress("kemal_seday@hotmail.com");
                message.Subject = "Şifre Hatırlatma";
                message.Body = "Şifreniz : " + yenisifre;
                client.Send(message);
                ViewBag.Uyari = "Şifreniz Başarıyla Gönderilmiştir";


            }
            else
            {
                ViewBag.Uyari = "Hata Oluştu.Tekrar Deneyiniz.";
            }
            return View();

           
        }
        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }
        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(Admin admin,string sifre,string eposta)
        {
            if(ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre,"MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }

            return View(admin);
        }
        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x=>x.AdminId==id).SingleOrDefault();
            return View(a);
        }
        [HttpPost]
        public ActionResult Edit(int id,Admin admin,string sifre,string eposta)
        {
           if(ModelState.IsValid)
            {
                var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre,"MD5");
                a.Eposta = admin.Eposta;
                a.Yetki = admin.Yetki;
                db.SaveChanges();
                return RedirectToAction("Adminler");

            }
            return View(admin);
        }

        public ActionResult Delete(int id)
        {
            var a = db.Admin.Where(x=>x.AdminId==id).SingleOrDefault();
            if (a != null)
            {
                db.Admin.Remove(a);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }

    }
}