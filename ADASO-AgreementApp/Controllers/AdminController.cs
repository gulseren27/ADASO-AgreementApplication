using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADASO_AgreementApp.Models.Entity;
using ADASO_AgreementApp.Models.ViewModel;

namespace ADASO_AgreementApp.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        ADASOEntities3 db = new ADASOEntities3();
        Class cs = new Class();

        //[Authorize]
        public ActionResult Index()
        {
            cs.Admins = db.Adminn.ToList();
            //var list = db.Adminn.ToList();

            return View(cs);
        }
    
        public ActionResult Guncelle(Adminn p)
        {
            var item = db.Adminn.Find(p.Id);
            item.Name = p.Name;
            item.Surname = p.Surname;
            item.Tel = p.Tel;
            item.Mail = p.Mail;
            item.TC = p.TC;
            item.Role = p.Role;
            item.Password=p.Password;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult KullanıcıGetir(int id)
        {
            var item = db.Adminn.Find(id);
            return View("KullanıcıGetir", item);

        }
       
    }
}