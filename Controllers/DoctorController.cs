using MedicalStore.Dal;
using MedicalStore.Models;
using MedicalStore.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MedicalStore.Controllers
{
    public class DoctorController : Controller
    {

        public static string MailCodeGeneratedDoctor;
        public static string DoctorId;
        public static int LoginAttemptDoctor;
        public static string UserName;

        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }

        public string SqlConnectionStringBuilder { get; private set; }

        public ActionResult DoctorWindow()
        {
            return View();
        }
        public ActionResult AddNewDoctor()
        {
            DoctorDal dal = new DoctorDal();
            List<Doctor> objDoctor = dal.Doctor.ToList<Doctor>();
            DoctorViewModel cvm = new DoctorViewModel();
            cvm.doctor = new Doctor();
            cvm.doctors = objDoctor;
            return View(cvm);
        }

        public ActionResult AddDoctorToDB(Doctor doc)
        {
            DoctorViewModel cvm = new DoctorViewModel();
            Doctor obj = new Doctor();

            obj.FirstName = doc.FirstName;
            obj.LastName = doc.LastName;
            obj.Id = doc.Id;
            obj.Password = doc.Password;
            obj.Expertise = doc.Expertise;
            obj.License = doc.License;

            DoctorDal dal = new DoctorDal();

            if (ModelState.IsValid)
            {
                List<Doctor> id_exist_list =
            (from x in dal.Doctor
             where x.UserName == doc.UserName
             select x).ToList<Doctor>();
                DoctorViewModel doc2 = new DoctorViewModel();

                if (id_exist_list.Count() > 0)
                {
                    TempData["existDoc"] = "User Name is already exist";
                    cvm.doctors = dal.Doctor.ToList<Doctor>();
                    cvm.doctor = doc;
                    return View("../Doctor/AddNewDoctor", cvm);
                }
                dal.Doctor.Add(doc);
                dal.SaveChanges();
                TempData["AddNewDoctorSuccess"] = "The new Doctor was Added";
                cvm.doctors = dal.Doctor.ToList<Doctor>();
                cvm.doctor = new Doctor();
                return View("AddNewDoctor", cvm);

            }
            else
            {
                cvm.doctor = obj;
            }

            cvm.doctors = dal.Doctor.ToList<Doctor>();
            TempData["AddNewDoctorFailure"] = "The new Manager was not added";
            return View("AddNewDoctor", cvm);
        }

        public ActionResult SendMail()
        {
            DoctorDal dal = new DoctorDal();
            string id = Request.Form["Id"];
            string password = Request.Form["Password"];
            DoctorViewModel PatientView = new DoctorViewModel();
            Doctor patient = new Doctor();
            DoctorId = id;

            List<Doctor> DoctorList =
                (from x in dal.Doctor
                 where x.Id == id && x.Password == password
                 select x).ToList<Doctor>();

            List<Doctor> DoctorById =
                (from x in dal.Doctor
                 where x.Id == id
                 select x).ToList<Doctor>();

            Doctor CurrentDoctor = new Doctor();
            if (DoctorById.Count() > 0)
            {
                if (DoctorList.Count() == 0)
                {
                    if (DoctorById[0].LoginAttempts == 5)
                    {
                        DoctorById[0].Blocked = true;
                        dal.SaveChanges();
                        TempData["User_lock"] = "Sorry, You are locked from the system";
                        return View("../Patient/Login", DoctorById[0]);
                    }
                    Doctor temp = new Doctor();
                    temp.Id = DoctorId;
                    temp.Password = password;
                    TempData["failed"] = "Doctor Login failed. User name or password supplied doesn't exist.";
                    TempData["attempts"] = "you left " + (4 - DoctorById[0].LoginAttempts) + " chances";
                    DoctorById[0].LoginAttempts++;
                    dal.SaveChanges();
                    return View("../Doctor/DoctorLogin", temp);
                }
            }
            else
            {
                Doctor temp = new Doctor();
                temp.Id = DoctorId;
                temp.Password = password;
                TempData["failed"] = "Doctor Login failed. User name or password supplied doesn't exist.";
                return View("../Doctor/DoctorLogin", temp);
            }
            CurrentDoctor = DoctorList[0];

            if (CurrentDoctor == null)
            {
                return View("../Doctor/DoctorLogin", patient);
            }
            else if (CurrentDoctor.Blocked == true)
            {
                TempData["User_lock"] = "Sorry, You are locked from the system";
                return View("../DoctorPatient/DoctorLogin", patient);
            }
            else if (CurrentDoctor.LoginAttempts > 5)
            {
                CurrentDoctor.Blocked = true;
                dal.SaveChanges();
                return View("../Doctor/DoctorLogin", patient);
            }
            CurrentDoctor.LoginAttempts++;
            dal.SaveChanges();

            UserName = CurrentDoctor.UserName;
            //generate code
            Random rnd = new Random();
            string user_code = rnd.Next(100, 9999999).ToString();

            MailCodeGeneratedDoctor = user_code;

            //send mail
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("kfir2037@gmail.com");
            //mail.To.Add("kfir2037@gmail.com");
            mail.To.Add(CurrentDoctor.Mail);
            mail.Subject = "Verfication Code";
            mail.Body = "Your code is: " + MailCodeGeneratedDoctor;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("kfir2037", "0542666134");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            LoginAttemptDoctor = 0;

            return View("../Doctor/EnterMail", CurrentDoctor);
        }
        public ActionResult CheckDoctor()
        {
            DoctorDal dal = new DoctorDal();
            //string search_Id = Request.Form["Id"].ToString();
            //string search_Password = Request.Form["Password"].ToString();
            string mailCode = Request.Form["Code"].ToString();

            List<Doctor> DoctorList =
                (from x in dal.Doctor
                 where x.Id == DoctorId
                 select x).ToList<Doctor>();
            Doctor CurrentDoctor = new Doctor();
            CurrentDoctor = DoctorList[0];

            if (mailCode == MailCodeGeneratedDoctor)
            {
                Session["UserName"] = UserName;
                Session["DoctorId"] = DoctorId;
                Session["PatientLoggedIn"] = UserName;
                CurrentDoctor.LoginAttempts = 0;
                CurrentDoctor.Blocked = false;
                return View("../Home/Index", CurrentDoctor);
            }
            TempData["ErrorMailCode"] = "Your Code is incorrect, please try again";
            CurrentDoctor.Password = null;
            return View("../Doctor/DoctorLogin", CurrentDoctor);
        }

        //public ActionResult CheckDoctor()
        //{

        //    DoctorDal dal = new DoctorDal();
        //    string search_Id = Request.Form["Id"].ToString();
        //    string search_Password = Request.Form["Password"].ToString();

        //    List<Doctor> failed_login_doctor =
        //        (from x in dal.Doctor
        //         where x.Id == search_Id
        //         select x).ToList<Doctor>();

        //    //if there is no doctor with the entered id
        //    if (failed_login_doctor.Count() == 0)
        //    {
        //        Doctor temp_pat = new Doctor();
        //        TempData["again"] = "please try again!";
        //        return View("DoctorLogin", temp_pat);
        //    }

        //    List<Doctor> obj;

        //    if (TempData["UserCode_error"] == null)
        //    {
        //        obj =
        //            (from x in dal.Doctor
        //             where x.Id == search_Id && x.Password == search_Password
        //             select x).ToList<Doctor>();
        //    }
        //    else
        //    {
        //        string search_DocCode = Request.Form["DocCode"].ToString();
        //        obj =
        //            (from x in dal.Doctor
        //             where x.Id == search_Id && x.Password == search_Password && x.DocCode == search_DocCode
        //             select x).ToList<Doctor>();
        //    }
        //    if (failed_login_doctor[0].Blocked == false)
        //    {

        //        DoctorViewModel pat = new DoctorViewModel();
        //        pat.doctors = obj;
        //        pat.doctor = new Doctor();
        //        pat.doctor.Id = search_Id;

        //        if (pat.doctors.Count() == 0)
        //        {
        //            failed_login_doctor[0].LoginAttemptDoctors = failed_login_doctor[0].LoginAttemptDoctors + 1;
        //            dal.SaveChanges();

        //            if (failed_login_doctor[0].LoginAttemptDoctors > 1)
        //            {
        //                TempData["UserCode_error"] = "UserCode";
        //            }

        //            else if (failed_login_doctor[0].LoginAttemptDoctors == 5)
        //            {
        //                TempData["User_lock"] = "Sorry, You are locked from the system";
        //                failed_login_doctor[0].Blocked = true;
        //                dal.SaveChanges();
        //            }
        //            TempData["failed"] = "Doctor Login failed. one of the details supplied is wrong.";
        //            TempData["attempts"] = "you left " + (5 - (failed_login_doctor[0].LoginAttemptDoctors)).ToString() + " chances";
        //            ViewBag.UserNow = pat.doctor.UserName;
        //            return View("DoctorLogin", pat.doctor);
        //        }
        //        else
        //        {
        //            failed_login_doctor[0].LoginAttemptDoctors = 0;
        //            dal.SaveChanges();
        //            Doctor new_pat = new Doctor();
        //            new_pat = pat.doctors[0];
        //            Session["UserName"] = new_pat.UserName;
        //            Session["DoctorId"] = new_pat.Id;
        //            Session["PatientLoggedIn"] = new_pat.UserName;
        //            return View("DoctorWindow", new_pat);
        //        }
        //    }
        //    else
        //    {
        //        TempData["User_lock"] = "Sorry, You are locked from the system";
        //        return View("DoctorLogin", failed_login_doctor[0]);
        //    }
        //}


        //[HttpPost]
        //public ActionResult CheckDoctor()
        //{
        //    DoctorDal dal = new DoctorDal();
        //    string search_UserName = Request.Form["UserName"].ToString();
        //    string search_Password = Request.Form["Password"].ToString();

        //    List<Doctor> obj =
        //        (from x in dal.Doctor
        //         where x.UserName == search_UserName && x.Password == search_Password
        //         select x).ToList<Doctor>();
        //    DoctorViewModel doc = new DoctorViewModel();
        //    doc.doctors = obj;
        //    doc.doctor = new Doctor();
        //    doc.doctor.UserName = search_UserName;

        //    if (doc.doctors.Count() == 0)
        //    {
        //        TempData["failed"] = "ManagerLogin failed . User name or password supplied doesn't exist.";
        //        return View("ManagerLogin", doc.doctor);
        //    }
        //    else
        //    {
        //        HttpCookie cookie = new HttpCookie("search");
        //        //System.Web.Security.FormsAuthentication.SetAuthCookie(mng.managers[0].UserName, true);
        //        Doctor new_doc = new Doctor();
        //        Response.Cookies["UserName"].Value = new_doc.UserName;
        //        new_doc = doc.doctors[0];
        //        Session["DoctorLoggedIn"] = new_doc.UserName;
        //        Session["UserName"] = new_doc.UserName;
        //        //return View("ManagerWindow", new_mng);
        //        return View("../Home/Index", new_doc);

        //    }
        //}

        public ActionResult DoctorLogin()
         {
            ViewBag.Message = "DoctorLogin";
            ViewBag.DoctorNow = "null";
            if (Session["UserName"] != null)
            {
                ViewBag.UserName = Session["UserName"];
                return RedirectToAction("DoctorWindow", "Doctor", new { username = Session["UserName"].ToString() });
            }
            else
            {
                DoctorViewModel doc = new DoctorViewModel();
                doc.doctor = new Doctor();
                return View(doc.doctor);
            }
        }
        public ActionResult ShowPatients()
        {

            return View();

        }
        public ActionResult ShowDoctors()
        {
            DoctorDal dal = new DoctorDal();
            List<Doctor> objDoctor = dal.Doctor.ToList<Doctor>();
            DoctorViewModel cvm = new DoctorViewModel();
            cvm.doctor = new Doctor();
            cvm.doctors = objDoctor;
            return View(cvm);
        }
    }
}