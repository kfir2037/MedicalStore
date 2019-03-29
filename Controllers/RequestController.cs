using MedicalStore.Dal;
using MedicalStore.Models;
using MedicalStore.ModelView;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MedicalStore.Controllers
{
    public class RequestController : Controller
    {
        // GET: Request
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            RequestDal db = new RequestDal();

            Request request = db.Requests.Find(id);

            //Patient patient = db.Patient.FindAsync(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "RequestNumber,PatientId,MedId,Approve,DocId")] Request request)
        {
            RequestDal db = new RequestDal();

            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Request/watchRequests");
            }
            return View(request);
        }
        public ActionResult Details(int id)
        {
            RequestDal db = new RequestDal();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }
        public ActionResult WatchRequests()
        {
            var id = @Session["DoctorId"];
            RequestDal dal = new RequestDal();
            List<Request> requests =
                (from x in dal.Requests
                 where x.DocId == id
                 select x).ToList<Request>();

            RequestViewModel view = new RequestViewModel();
            view.requests = requests;
            view.request = new Request();

            return View(view);
        }

        public ActionResult Delete(int id)
        {
            RequestDal db = new RequestDal();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Patientsssss/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PatientDal db = new PatientDal();
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
            db.SaveChanges();
            return RedirectToAction("../Request/WatchRequests");
        }

        protected override void Dispose(bool disposing)
        {
            RequestDal db = new RequestDal();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult MakeOrder(ProductViewModel check)
        {
            Request req = new Request();
            string PatientID = Request.Form["ID"];
            req.MedId = check.product.MedId.ToString();
            req.PatientId = PatientID.ToString();
            req.DocId = check.doctor.Id;
            RequestViewModel cvm = new RequestViewModel();
            //Manager obj = new Manager();
            RequestDal dal = new RequestDal();

            dal.Requests.Add(req);
            dal.SaveChanges();
            TempData["Order"] = "Your Request sent to the doctor!";
            cvm.requests = dal.Requests.ToList<Request>();
            //return View("../User/MakeOrder");
            return RedirectToAction("MakeOrder", "Patient");

            //    if (ModelState.IsValid)
            //    {
            //        dal.Requests.Add(req);
            //        dal.SaveChanges();
            //        TempData["Order"] = "Your Request sent to the doctor!";
            //        cvm.requests = dal.Requests.ToList<Request>();
            //        //return View("../User/MakeOrder");
            //        return RedirectToAction("MakeOrder", "Patient");
            //    }
            //    else
            //    {
            //        cvm.request = req;
            //    }

            //    cvm.requests = dal.Requests.ToList<Request>();
            //    TempData["Order"] = "Something went wrong with your Order!";

            //    return RedirectToAction("MakeOrder", "Patient");

        }
        }
    }