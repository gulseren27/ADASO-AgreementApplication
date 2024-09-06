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
    public class AgreementController : Controller
    {
        // GET: Agreement
        ADASOEntities3 db = new ADASOEntities3();
        Class cs =new Class();

        [Authorize]
        public ActionResult Index(int sayfa=1)
        {
            cs.Agreements=db.Agreementt.ToList().ToPagedList(sayfa, 10);
            cs.Maills = db.Maill.ToList();
            //var list = db.Agreementt.ToList().ToPagedList(sayfa, 10); ;
            //foreach (var agreement in list)
            //{
            //    if (agreement.EndDate < DateTime.Today)
            //    {
            //        agreement.Status = "Pasif"; // Bitiş tarihi geçmişse Pasif
            //    }
            //    else
            //    {
            //        agreement.Status = "Aktif"; // Aksi takdirde Aktif
            //    }
            //}
            return View(cs);
        }

        public ActionResult Guncelle(Agreementt p, string FilePath)
        {
            var item = db.Agreementt.Find(p.AId);
            item.Title = p.Title;
            item.Content = p.Content;
            item.EndDate = p.EndDate;
            item.StartDate = p.StartDate;
            item.File = p.File;
            item.CompanyName = p.CompanyName;
            item.Email = p.Email;

          

            db.SaveChanges();
            return RedirectToAction("Index");
}
        public ActionResult SozlesmeGetir(int Id)
        {
            var item = db.Agreementt.Find(Id);
            return View("SozlesmeGetir", item);

        }
        // yeni sözleşme ekleme
        [HttpGet]
        public ActionResult NewAgrement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewAgrement(Agreementt p1)
        {
            db.Agreementt.Add(p1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ReNewAgreement(Agreementt a)
        {
            if (ModelState.IsValid)
            {
                // Sözleşme verilerini güncelle
                var existingAgreement = db.Agreementt.Find(a.AId);
                if (existingAgreement != null)
                {
                    existingAgreement.Title = a.Title;
                    existingAgreement.Content = a.Content;
                    existingAgreement.StartDate = a.StartDate;
                    existingAgreement.EndDate = a.EndDate;
                    existingAgreement.CompanyName = a.CompanyName;
                    existingAgreement.Email = a.Email;
                    existingAgreement.Status = a.Status;

                    db.Agreementt.Add(a);
                    db.SaveChanges();
                }

                // Yenilenen sözleşme ile kullanıcıyı bilgilendirin veya yönlendirin
                TempData["Message"] = "Sözleşme başarıyla yenilendi.";
                return RedirectToAction("Index");
            }

            // Hata durumunda aynı sayfaya geri dön
            return View("Index");
        }

        [HttpGet]
        public ActionResult AddMail(int AId)
        {
            var agreement = db.Agreementt.Find(AId);
            if (agreement == null)
            {
                return HttpNotFound();
            }
            var mailList = db.Maill.Where(m => m.AId == AId).ToList();

            var MailModel = new Maill
            {
                AId = AId,
                MailList = mailList,
                
            };

            return View(MailModel);
        }

        [HttpPost]
        public ActionResult AddMail(Maill m, int AId)
        {
            if (ModelState.IsValid)
            {
                var relatedAgreement = db.Agreementt.FirstOrDefault(a => a.AId == AId);
                if (relatedAgreement != null)
                {
                    m.AId = AId;
                    db.Maill.Add(m);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Mail");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Hata durumunda aynı sayfaya geri dön
            return View("AddMail", m);
        }


    }
}