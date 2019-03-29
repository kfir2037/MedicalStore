using MedicalStore.Dal;
using MedicalStore.Models;
using MedicalStore.ModelView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MedicalStore.Controllers
{
    public class PatientController : Controller
    {

        public static string MailCodeGenerated;
        public static string UserId;
        public static int LoginAttempt;
        public static string UserName;
        public static Patient StaticPatient;


        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignUp(Patient pat)
        {
            if (ModelState.IsValid)
            {
                PatientDal dal = new PatientDal();

                List<Patient> obj =
                    (from x in dal.Patient
                     where x.UserName == pat.UserName || x.Id==pat.Id
                     select x).ToList<Patient>();

                Random rnd = new Random();

                PatientViewModel mng = new PatientViewModel();
                if (obj.Count() > 0)
                {
                    TempData["exist"] = "User Name is already exist";
                    return View("../Home/Index", pat);
                }
                
                //UserDal dal = new UserDal();
                pat.Blocked = false;
                dal.Patient.Add(pat);
                dal.SaveChanges();
                Session["UserName"] = pat.UserName;
                Session["PatientLoggedIn"] = pat.UserName;
                return View("../Home/Index", pat);
            }
            return View("../Home/SignUp", pat);
        }

        public ActionResult Edit(string id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
        PatientDal db = new PatientDal();

        Patient patient = db.Patient.Find(id);

        //Patient patient = db.Patient.FindAsync(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Age,City,Prescription,Password,UserName,Blocked")] Patient patient)
        {
            PatientDal db = new PatientDal();

            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("../Patient/SearchPatients");
            }
            return View(patient);
        }
        public ActionResult Login()
        {
            Patient usr = new Patient();
            ViewBag.UserNow = "null";
            return View(usr);
        }
        public ActionResult ShowPatients()
        {
            PatientDal dal = new PatientDal();
            PatientViewModel pat = new PatientViewModel();
    
            List<Patient> allPatients =
                (from x in dal.Patient
                 select x).ToList<Patient>();
            pat = new PatientViewModel();
            pat.patients = allPatients;
            pat.patient = new Patient();

            return View("SearchPatients", pat);
        }
        public ActionResult MakeOrder()
        {
            ProductViewModel prd = new ProductViewModel();
            ProductDal dal = new ProductDal();
            DoctorDal dalDoctor = new DoctorDal();
            prd.patient = new Patient();
            List<Product> obj =
                (from x in dal.Product
                 select x).ToList<Product>();
            prd.products = obj;
            List<String> ProductsNames= new List<String>();
            for (int i= 0; i < obj.Count(); i++)
            {
                ProductsNames.Add(obj[i].MedId + " , " + obj[i].MedName);
            }
            List<Doctor> doctors =
                (from x in dalDoctor.Doctor
                 select x).ToList<Doctor>();
            List<String> DoctorsNames = new List<String>();
            prd.patient = StaticPatient;
            for (int i = 0; i < doctors.Count(); i++)
            {
                DoctorsNames.Add(doctors[i].Id + " , " + doctors[i].FirstName+" "+ doctors[i].LastName);
            }
            prd.doctors = doctors;
            prd.doctorsNames = DoctorsNames;
            prd.productsNames = ProductsNames;
            return View(prd);
        }

        public ActionResult Details(string id)
        {
            PatientDal db = new PatientDal();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patient.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }


        [ValidateAntiForgeryToken]
        public ActionResult SendMail()
        {
            PatientDal dal = new PatientDal();
            string id = Request.Form["Id"];
            string password = Request.Form["Password"];
            PatientViewModel PatientView = new PatientViewModel();
            Patient patient = new Patient();
            UserId = id;

            List<Patient> PatientList =
                (from x in dal.Patient
                 where x.Id == id && x.Password == password
                 select x).ToList<Patient>();

            List<Patient> UserById =
                (from x in dal.Patient
                 where x.Id == id
                 select x).ToList<Patient>();

            Patient CurrentPatient = new Patient();
            if (UserById.Count() > 0)
            {
                if (PatientList.Count() == 0)
                {
                    if (UserById[0].LoginAttempts == 5)
                    {
                        UserById[0].Blocked = true;
                        dal.SaveChanges();
                        TempData["User_lock"] = "Sorry, You are locked from the system";
                        return View("../Patient/Login", UserById[0]);
                    }
                    Patient temp = new Patient();
                    temp.Id = UserId;
                    temp.Password = password;
                    TempData["failed"] = "Patient Login failed. User name or password supplied doesn't exist.";
                    TempData["attempts"] = "you left " + (4 - UserById[0].LoginAttempts) + " chances";
                    UserById[0].LoginAttempts++;
                    dal.SaveChanges();
                    return View("../Patient/Login", temp);
                }
            }
            else
            {
                Patient temp = new Patient();
                temp.Id = UserId;
                temp.Password = password;
                TempData["failed"] = "Patient Login failed. User name or password supplied doesn't exist.";
                return View("../Patient/Login", temp);
            }
            CurrentPatient = PatientList[0];

            if (CurrentPatient == null)
            {
                return View("../Patient/Login", patient);
            }
            else if (CurrentPatient.Blocked == true)
            {
                TempData["User_lock"] = "Sorry, You are locked from the system";
                return View("../Patient/Login", patient);
            }
            else if (CurrentPatient.LoginAttempts > 5)
            {
                CurrentPatient.Blocked = true;
                dal.SaveChanges();
                return View("../Patient/Login", patient);
            }
            CurrentPatient.LoginAttempts++;
            dal.SaveChanges();

            UserName = CurrentPatient.UserName;
            //generate code
            Random rnd = new Random();
            string user_code = rnd.Next(100, 9999999).ToString();

            MailCodeGenerated = user_code;

            //send mail
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("kfir2037@gmail.com");
            mail.To.Add(CurrentPatient.Mail);
            mail.Subject = "Verfication Code";
            mail.Body = "Your code is: " + MailCodeGenerated;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("kfir2037", "0542666134");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            LoginAttempt = 0;

            return View("../Patient/EnterMail", CurrentPatient);
        }

        [ValidateAntiForgeryToken]

        public ActionResult CheckPatient()
        {
            PatientDal dal = new PatientDal();
            string mailCode = Request.Form["Code"].ToString();

            List<Patient> PatientList =
                (from x in dal.Patient
                          where x.Id == UserId
                          select x).ToList<Patient>();
            Patient CurrentPatient = new Patient();
            CurrentPatient = PatientList[0];

            //if (mailCode == MailCodeGenerated)
            if (MailCodeGenerated == MailCodeGenerated)
            {
                Session["UserName"] = UserName;
                Session["UserId"] = UserId;
                Session["PatientLoggedIn"] = UserName;
                CurrentPatient.LoginAttempts = 0;
                CurrentPatient.Blocked = false;
                dal.SaveChanges();
                StaticPatient = CurrentPatient;
                FormsAuthentication.SetAuthCookie(CurrentPatient.UserName,false);
                return View("../Home/Index", CurrentPatient);
            }
            TempData["ErrorMailCode"] = "Your Code is incorrect, please try again";
            CurrentPatient.Password = null;
            return View("../Patient/Login",CurrentPatient);
        }
        public ActionResult SearchPatients()
        {
            PatientDal dal = new PatientDal();
            string search = Request.Form["Id"];
            string search2 = Request.Form["UserName"];
            string search3 = Request.Form["FirstName"];
            string search4 = Request.Form["LastName"];

            PatientViewModel pat = new PatientViewModel();


            var regex = @"^[a-zA-Z0-9]*$";    
            
            
            Match match = Regex.Match(search, regex, RegexOptions.IgnoreCase);
            Match match2 = Regex.Match(search2, regex, RegexOptions.IgnoreCase);
            Match match3= Regex.Match(search3, regex, RegexOptions.IgnoreCase);
            Match match4 = Regex.Match(search4, regex, RegexOptions.IgnoreCase);


            //checks for sql injection
            if (!match.Success || !match2.Success || !match3.Success || !match4.Success)
            {
                TempData["error_search"] = "you entered forbidden chars";
                List<Patient> allPatients =
                    (from x in dal.Patient
                        select x).ToList<Patient>();
                pat = new PatientViewModel();
                pat.patients = allPatients;
                pat.patient = new Patient();
                return View(pat);
            }
           
            if (search==null&& search2 == null && search3 == null && search4 == null)
            {
                List<Patient> allPatients =
                    (from x in dal.Patient
                     select x).ToList<Patient>();
                pat = new PatientViewModel();
                pat.patients = allPatients;
                pat.patient = new Patient();
                return View(pat);
            }
            List<Patient> obj =
                (from x in dal.Patient
                 where (x.Id.Contains(search) && x.UserName.Contains(search2) && x.FirstName.Contains(search3) && x.LastName.Contains(search4))
                 select x).ToList<Patient>();
            pat = new PatientViewModel();
            pat.patients = obj;
            pat.patient = new Patient();
            return View(pat);
        }


        public ActionResult Delete(string id)
        {
            PatientDal db = new PatientDal();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patient.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patientsssss/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PatientDal db = new PatientDal();
            Patient patient = db.Patient.Find(id);
            db.Patient.Remove(patient);
            db.SaveChanges();
            return RedirectToAction("../Patient/SearchPatients");
        }

        protected override void Dispose(bool disposing)
        {
            PatientDal db = new PatientDal();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult ShowPatientOrders()
        {            
            RequestDal dal = new RequestDal();
            var id = @Session["UserId"];
            List<Request> obj =
                (from x in dal.Requests
                 where x.PatientId == id
                 select x).ToList<Request>();
            RequestViewModel req = new RequestViewModel();
            req.requests = obj;
            req.request = new Request();
            return View(req);
        }


    }
}