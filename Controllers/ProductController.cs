using MedicalStore.Dal;
using MedicalStore.Models;
using MedicalStore.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddProduct()
        {
            ProductViewModel obj = new ProductViewModel();
            obj.product = new Product();
            return View(obj);
        }
        public ActionResult getJson()
        {
            ProductDal Dal = new ProductDal();
            List<Product> val =
                    (from x in Dal.Product
                     select x).ToList<Product>();

            return Json(val, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddProductToDb(Product prd)
        {
            ProductViewModel cvm = new ProductViewModel();

            Product obj = new Product();
            obj.Description = prd.Description;
            obj.MedId = prd.MedId;
            obj.MedName = prd.MedName;
            obj.Price = prd.Price;


            ProductDal dal = new ProductDal();

            if (ModelState.IsValid)
            {
                List<Product> obj2 =
                        (from x in dal.Product
                         where x.MedId == prd.MedId
                         select x).ToList<Product>();
                ProductViewModel mng2 = new ProductViewModel();

                if (obj2.Count() > 0)
                {
                    TempData["existPrd"] = "This product is already exist";
                    cvm.products = dal.Product.ToList<Product>();
                    cvm.product = obj;
                    return View("AddProduct", cvm);
                }

                dal.Product.Add(prd);
                dal.SaveChanges();
                cvm.product = new Product();
                TempData["existPrd"] = "The product was added";
                return View("AddProduct", cvm);
            }
            else
            {
                cvm.product = obj;
            }

            cvm.products = dal.Product.ToList<Product>();
            List<Product> objProduct = dal.Product.ToList<Product>();
            return Json(objProduct, JsonRequestBehavior.AllowGet);
            //return View("AddProduct", cvm);

        }
    }
}