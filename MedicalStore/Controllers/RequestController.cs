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
            var id = @Session["UserId"];
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
        public ActionResult MakeOrder()
        {
            Request req = new Request();
            string UserName =Request.Form["UserName"];
            string MedicineId = Request.Form["MedicineId"];
            string ID = Request.Form["ID"];
            string DocId = Request.Form["DocId"];

            req.MedId = MedicineId.ToString();
            req.PatientId = ID.ToString();
            req.DocId = DocId.ToString();
            RequestViewModel cvm = new RequestViewModel();
            //Manager obj = new Manager();
            RequestDal dal = new RequestDal();

            if (ModelState.IsValid)
            {
                dal.Requests.Add(req);
                dal.SaveChanges();
                TempData["Order"] = "Your Request sent to the doctor!";
                cvm.requests = dal.Requests.ToList<Request>();
                //return View("../User/MakeOrder");
                return RedirectToAction("MakeOrder", "Patient");
            }
            else
            {
                cvm.request = req;
            }

            cvm.requests = dal.Requests.ToList<Request>();
            TempData["Order"] = "Something went wrong with your Order!";
            //return View("../User/MakeOrder");

            //Request req = new Request();
            ////var x = prd.product.Name;
            //var dish = "";
            //var toppings = "";
            //string toppings1 = "";
            //string toppings2 = "";
            //string toppings3 = "";
            //string toppings4 = "";
            //string toppings5 = "";
            //string toppings6 = "";
            //string toppings7 = "";
            //string toppings8 = "";
            //string address = "";


            //if (Request.Form["Name"] != null)
            //{
            //    dish = Request.Form["Name"].ToString();
            //}

            //if (Request.Form["Tuna"] != null)
            //{
            //    toppings1 = Request.Form["Tuna"].ToString();
            //    toppings += toppings1 + ",";

            //}
            //if (Request.Form["Tomato"] != null)
            //{
            //    toppings2 = Request.Form["Tomato"].ToString();
            //    toppings += toppings2 + ",";

            //}
            //if (Request.Form["Mushrooms"] != null)
            //{
            //    toppings3 = Request.Form["Mushrooms"].ToString();
            //    toppings += toppings3 + ",";

            //}
            //if (Request.Form["Garlic"] != null)
            //{
            //    toppings4 = Request.Form["Garlic"].ToString();
            //    toppings += toppings4 + ",";

            //}
            //if (Request.Form["Olives"] != null)
            //{
            //    toppings5 = Request.Form["Olives"].ToString();
            //    toppings += toppings5 + ",";
            //}
            //if (Request.Form["Corn"] != null)
            //{
            //    toppings6 = Request.Form["Corn"].ToString();
            //    toppings += toppings6 + ",";

            //}
            //if (Request.Form["Eggplant"] != null)
            //{
            //    toppings7 = Request.Form["Eggplant"].ToString();
            //    toppings += toppings7 + ",";

            //}
            //if (Request.Form["Onion"] != null)
            //{
            //    toppings8 = Request.Form["Onion"].ToString();
            //    toppings += toppings8 + ",";

            //}

            //if (Request.Form["Address"] != null)
            //{
            //    address = Request.Form["Address"].ToString();
            //}

            //if (address == "")
            //{
            //    TempData["Order"] = "You hava to enter an address !";
            //    //return View("../User/MakeOrder");
            //    return RedirectToAction("MakeOrder", "User");
            //}

            //RequestViewModel cvm = new RequestViewModel();
            ////Manager obj = new Manager();

            //req.Price = "20";
            //req.Address = address;
            //req.Dish = dish;
            ////ord.Dish = x;
            //req.Toppings = toppings1 + toppings2 + toppings3 + toppings4 + toppings5 + toppings6 + toppings7 + toppings8;
            //req.UserName = Request.Form["UserName"].ToString();

            //OrderDal dal = new OrderDal();

            //if (ModelState.IsValid)
            //{
            //    dal.Orders.Add(req);
            //    dal.SaveChanges();
            //    TempData["Order"] = "Your Order is on it's way!";
            //    cvm.requests = dal.Orders.ToList<Request>();
            //    //return View("../User/MakeOrder");
            //    return RedirectToAction("MakeOrder", "User");
            //}
            //else
            //{
            //    cvm.request = req;
            //}

            //cvm.requests = dal.Orders.ToList<Request>();
            //TempData["Order"] = "Something went wrong with your Order!";
            ////return View("../User/MakeOrder");
            return RedirectToAction("MakeOrder", "Patient");

        }
    }
}