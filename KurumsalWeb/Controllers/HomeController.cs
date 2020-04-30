using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using PagedList;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace KurumsalWeb.Controllers
{
    public class HomeController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Home
        [Route("")]
        [Route("Anasayfa")]
        
        public ActionResult Index()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId);
           return View();
        }
        public ActionResult SliderPartial()
        {

            return View(db.Slider.ToList().OrderBy(x=>x.SliderId));
        }
        public ActionResult HizmetPartial()
        {
            return View(db.Hizmet.ToList());
        }
        [Route("Hakkimizda")]
        public ActionResult Hakkimizda()
        {

            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hakkimizda.SingleOrDefault()) ;
        }
        [Route("Hizmetlerimiz")]
        public ActionResult Hizmetlerimiz()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId)) ;
        }
        [Route("Iletisim")]
        public ActionResult Iletisim()
        {
           
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Iletisim.SingleOrDefault());
        }
        [HttpPost]
        public ActionResult Iletisim(string adsoyad=null,string email=null,string konu=null,string mesaj=null)
        {
            if(adsoyad!=null&&email!=null)
            {
                //WebMail.SmtpServer = "smtp.live.com";
                //WebMail.EnableSsl = true;
                //WebMail.UserName = "kemal_seday@hotmail.com";
                //WebMail.Password = "kemal01seday56";
                //WebMail.SmtpPort = 587;
                //WebMail.Send("kemal_seday@hotmail.com", konu,email+"-"+ mesaj);
             


                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("kemal_seday@hotmail.com", "kemal01seday56");
                client.Port = 587;
                client.Host = "smtp.live.com";
                client.EnableSsl = true;
                message.To.Add("kemal_seday@hotmail.com");
                message.From = new MailAddress("kemal_seday@hotmail.com");
                message.Subject = konu;
                message.Body = "  "+email+"   "+mesaj;
                client.Send(message);
                ViewBag.Uyari = "Mesajınız Başarıyla Gönderilmiştir";


            }
            else
            {
                ViewBag.Uyari = "Hata Oluştu.Tekrar Deneyiniz.";
            }
            return View(db.Iletisim.SingleOrDefault());
        }

        [Route("BlogPost")]
        public ActionResult Blog(int Sayfa = 1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).ToPagedList(Sayfa,5));
        }
        [Route("BlogPost/{kategoriad}/{id:int}")]
        public ActionResult KategoriBlog(int id,int Sayfa=1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).Where(x=>x.Kategori.KategoriId==id).ToPagedList(Sayfa,5);
            return View(b);
        }
        [Route("BlogPost/{baslik}-{id:int}")]
        public ActionResult BlogDetay(int id)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").Include("Yorums").Where(x => x.BlogId == id).SingleOrDefault();

            return View(b);
        }
        public JsonResult YorumYap(string adsoyad,string eposta,string icerik,int blogid)
        {
            if(icerik==null)
            {
                return Json(true,JsonRequestBehavior.AllowGet);
            }
            db.Yorum.Add(new Yorum {AdSoyad=adsoyad,Eposta=eposta,Icerik=icerik,BlogId=blogid,Onay=false });
            db.SaveChanges();
            //Response.Redirect("/Home/BlogDetay/" + blogid);

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BlogKategoriPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderBy(x=>x.KategoriAd));
        }
        public ActionResult BlogKayitPartial()
        {

            return PartialView(db.Blog.ToList().OrderByDescending(x=>x.BlogId));
        }

        public ActionResult FooterPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            ViewBag.Iletisim = db.Iletisim.SingleOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);

            return PartialView();
        }
    

    }
}