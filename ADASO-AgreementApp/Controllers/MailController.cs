using ADASO_AgreementApp.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using ADASO_AgreementApp.Models.ViewModel;

namespace ADASO_AgreementApp.Controllers
{
    public class MailController : Controller
    {
        ADASOEntities3 db = new ADASOEntities3();
        Class cs = new Class();
        // GET: Mail
        public ActionResult Index()
        {
            cs.Agreements = db.Agreementt.ToList();
            cs.Maills = db.Maill.ToList();
            return View(cs);
        }
        //[HttpGet]
        //public ActionResult AddMail(int AId)
        //{
        //    var item = db.Agreementt.Find(AId);
        //    ViewBag.AId = AId;
        //    return View(item);
        //}

        //[HttpPost]
        //public ActionResult AddMail(Maill m, int AId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        m.AId = AId;
        //        db.Maill.Add(m);
        //        db.SaveChanges();
        //        return RedirectToAction("Index","Agreement");
        //    }
        //    else
        //    {
        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //        {
        //            Console.WriteLine(error.ErrorMessage); // Hata mesajını yazdırın
        //        }
        //    }

        //    // Hata durumunda aynı sayfaya geri dön
        //    return View("AddMail", m);
        //}

    }
}
