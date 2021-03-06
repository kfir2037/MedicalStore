﻿using MedicalStore.Dal;
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

namespace MedicalStore.Controllers
{
    public class PatientController : Controller
    {

        public static string MailCodeGenerated;
        public static string UserId;
        public static int LoginAttempt;
        public static string UserName;


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
                string user_code = rnd.Next(100, 9999999).ToString();

                bool check_user_code = true;
                while (check_user_code) {
                    List<Patient> user_codes =
                         (from x in dal.Patient
                          where x.UserCode == user_code
                          select x).ToList<Patient>();
                    if (user_codes.Count() == 0)
                    {
                        check_user_code = false;
                    }
                    user_code = rnd.Next(100, 9999999).ToString();
                }
                PatientViewModel mng = new PatientViewModel();
                if (obj.Count() > 0)
                {
                    TempData["exist"] = "User Name is already exist";
                    return View("../Home/SignUp", pat);
                }
                
                //UserDal dal = new UserDal();
                pat.UserCode = user_code;
                pat.Blocked = false;
                dal.Patient.Add(pat);
                dal.SaveChanges();
                Session["UserName"] = pat.UserName;
                Session["PatientLoggedIn"] = pat.UserName;

                return View("ConfirmSignUp", pat);
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
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Age,City,UserCode,Prescription,Password,UserName,Blocked")] Patient patient)
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
            prd.patient = new Patient();
            List<Product> obj =
            (from x in dal.Product
             select x).ToList<Product>();
            prd.products = obj;
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
        public ActionResult SendMail()
        {
            PatientDal dal = new PatientDal();
            string id= Request.Form["Id"];
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

            if (CurrentPatient==null)
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
            mail.To.Add("kfir2037@gmail.com");
            mail.Subject = "Verfication Code";
            mail.Body = "Your code is: "+ MailCodeGenerated;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("kfir2037", "0542666134");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            LoginAttempt = 0;

            return View("../Patient/EnterMail",CurrentPatient);
        }
        public ActionResult CheckPatient()
        {
            PatientDal dal = new PatientDal();
            //string search_Id = Request.Form["Id"].ToString();
            //string search_Password = Request.Form["Password"].ToString();
            string mailCode = Request.Form["Code"].ToString();

            List<Patient> PatientList =
                (from x in dal.Patient
                          where x.Id == UserId
                          select x).ToList<Patient>();
            Patient CurrentPatient = new Patient();
            CurrentPatient = PatientList[0];

            if (mailCode == MailCodeGenerated)
            {
                Session["UserName"] = UserName;
                Session["UserId"] = UserId;
                Session["PatientLoggedIn"] = UserName;
                CurrentPatient.LoginAttempts = 0;
                CurrentPatient.Blocked = false;
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



        //public ActionResult CheckPatient()
        //{

        //    PatientDal dal = new PatientDal();
        //    string search_Id = Request.Form["Id"].ToString();
        //    string search_Password = Request.Form["Password"].ToString();

        //    List<Patient> failed_login_patient =
        //        (from x in dal.Patient
        //         where x.Id == search_Id
        //         select x).ToList<Patient>();

        //    if (failed_login_patient.Count()==0)
        //    {
        //        Patient temp_pat = new Patient();
        //        TempData["again"] = "please try again!";
        //        return View("Login",temp_pat);
        //    }

        //    List<Patient> obj;

        //    if (TempData["UserCode_error"]==null)
        //    {
        //        obj =
        //            (from x in dal.Patient
        //             where x.Id == search_Id && x.Password == search_Password
        //             select x).ToList<Patient>();
        //    }
        //    else
        //    {
        //        string search_UserCode = Request.Form["UserCode"].ToString();
        //        obj =
        //            (from x in dal.Patient
        //             where x.Id == search_Id && x.Password == search_Password && x.UserCode==search_UserCode
        //             select x).ToList<Patient>();
        //    }
        //    if (failed_login_patient[0].Blocked == false)
        //    {

        //        PatientViewModel pat = new PatientViewModel();
        //        pat.patients = obj;
        //        pat.patient = new Patient();
        //        pat.patient.Id = search_Id;

        //        if (pat.patients.Count() == 0)
        //        {
        //            failed_login_patient[0].LoginAttempts = failed_login_patient[0].LoginAttempts + 1;
        //            dal.SaveChanges();

        //            if (failed_login_patient[0].LoginAttempts > 0)
        //            {
        //                TempData["UserCode_error"] = "UserCode";
        //            }

        //            else if (failed_login_patient[0].LoginAttempts == 5)
        //            {
        //                TempData["User_lock"] = "Sorry, You are locked from the system";
        //                failed_login_patient[0].Blocked = true;
        //                dal.SaveChanges();
        //            }
        //            TempData["failed"] = "Patient Login failed. User name or password supplied doesn't exist.";
        //            TempData["attempts"] = "you left " + (5 - (failed_login_patient[0].LoginAttempts)).ToString() + " chances";
        //            ViewBag.UserNow = pat.patient.UserName;
        //            return View("Login", pat.patient);
        //        }
        //        else
        //        {
        //            failed_login_patient[0].LoginAttempts = 0;
        //            dal.SaveChanges();
        //            Patient new_pat = new Patient();
        //            new_pat = pat.patients[0];
        //            Session["UserName"] = new_pat.UserName;
        //            Session["UserId"] = new_pat.Id;
        //            Session["PatientLoggedIn"] = new_pat.UserName;
        //            return View("../Home/Index", new_pat);
        //        }
        //    }
        //    else
        //    {
        //        TempData["User_lock"] = "Sorry, You are locked from the system";
        //        return View("Login", failed_login_patient[0]);
        //    }
        //}

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
                 //have to change the user name
                 where x.PatientId == id
                 select x).ToList<Request>();
            RequestViewModel req = new RequestViewModel();
            req.requests = obj;
            req.request = new Request();
            return View(req);
        }


    }
}