using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pEasyPrint.Areas.Admin.Models;
using MvcContrib.UI.Grid;
using MvcContrib.Pagination;
using System.Web.Security;
using WebMatrix.WebData;
using pEasyPrint.Filters;
using pEasyPrint.Controllers;
using System.Configuration;
using System.Net.Mail;
using System.IO;

namespace pEasyPrint.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class ContentWriterController : Controller
    {
                 //
        // GET: /Admin/ContentWriter/
        CommonController objCommonController = new CommonController();
        public ActionResult Index(int? page)
        {
            OrderModel objmodel = new OrderModel();
            List<OrderModel> designers = GetDesignerList();

            if (designers != null)
            {
                IPagination pagedModel = designers.AsPagination(page ?? 1, 10);
                return View(pagedModel);
            }
            else
            {
                designers = new List<OrderModel>();
                IPagination pagedModel = designers.AsPagination(page ?? 1, 10);
                return View(pagedModel);
            }

        }

        public List<OrderModel> GetDesignerList()
        {
            List<OrderModel> objlist = new List<OrderModel>();
            List<tblContentWriter> objtblist = new List<tblContentWriter>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblist = objdb.tblContentWriters.ToList();
                OrderModel OrderModelLeftMenu = new OrderModel();
                objCommonController.GetLeftMenuContent(OrderModelLeftMenu);
                objlist.Add(OrderModelLeftMenu);
                if (objtblist.Count() > 0)
                {
                    objlist = new List<OrderModel>();
                    foreach (var item in objtblist)
                    {
                        OrderModel objDesignerModel = new OrderModel();
                        objDesignerModel.DesignerId = item.PkContentWritterId;
                        objDesignerModel.DesignerFirstName = item.FirstName;
                        objDesignerModel.DesigenerLastName = item.LastName;
                        objDesignerModel.Sex = item.Sex;
                        objDesignerModel.DesignerExperience = item.Experience;
                        objDesignerModel.DesignerAddress = item.Address;
                        objDesignerModel.Country = item.Country;
                        objDesignerModel.CountryName = GetCountryNamebyid(item.Country);
                        objDesignerModel.State = item.State;
                        objDesignerModel.StateName = GetStateNameById(item.State);
                        objDesignerModel.City = item.City;
                        objDesignerModel.CityName = GetCityNameById(item.City);
                        objDesignerModel.Zip = item.Zip;
                        objDesignerModel.Mobile = item.Mobile;
                        objDesignerModel.EmailId = item.EmailId;
                        objDesignerModel.IsActive = item.IsActive;
                        objDesignerModel.AddedOn = item.AddedOn;
                        objDesignerModel.AddedBy = item.AddedBy;
                        objDesignerModel.Photo = item.Photo;

                        objCommonController.GetLeftMenuContent(objDesignerModel);
                        objlist.Add(objDesignerModel);

                    }
                }
            }
            return objlist;
        }

       
        public string GetCountryNamebyid(int? PkCountryId)
        {
            var conutyrname = "";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var tblcountry = objdb.tblCountries.Where(rec => rec.PkCountryId == PkCountryId).FirstOrDefault();
                if (tblcountry != null)
                {
                    conutyrname = tblcountry.CountryName;
                }

            }
            return conutyrname;
        }

        public string GetStateNameById(int? PkStateId)
        {
            var statename = "";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var tblstate = objdb.tblStates.Where(rec => rec.PkStateId == PkStateId).FirstOrDefault();
                if (tblstate != null)
                {
                    statename = tblstate.StateName;
                }
            }
            return statename;

        }
        public string GetCityNameById(int? PkCityId)
        {
            var cityname = "";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var tblcity = objdb.tblCities.Where(rec => rec.PkCityId == PkCityId).FirstOrDefault();
                if (tblcity != null)
                {
                    cityname = tblcity.CityName;
                }
            }
            return cityname;
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (Request.QueryString["Error"] == "-1")
            {
                ViewBag.Adderror = Session["Adderror"].ToString();
            }
            DesignerModel objmodel = new DesignerModel();
            objmodel.ListCountry = GetCountryList();
            objmodel.ListState = GetStateList();
            objmodel.ListCity = GetCityList();
            objmodel.objOrderModel = GetOrderList();
            return View(objmodel);
        }

        [HttpPost]
        [InitializeSimpleMembership]
        public ActionResult Add(DesignerModel dm)
        {
            int PkContentWritterId = 0;
            tblContentWriter objmodel = new tblContentWriter();
           
                try
                {
                    string DefaultPassword = ConfigurationSettings.AppSettings["NewUserDefaultPassowrd"].ToString();
                    WebSecurity.CreateUserAndAccount(dm.EmailId, DefaultPassword);                  
                    Roles.AddUserToRole(dm.EmailId, "ContentWriter");
                    objmodel.PkContentWritterId = WebSecurity.GetUserId(dm.EmailId);
                    PkContentWritterId = WebSecurity.GetUserId(dm.EmailId);
                    objmodel.FirstName = dm.DesignerFirstName;
                    objmodel.LastName = dm.DesigenerLastName;
                    objmodel.Sex = dm.Sex;
                    objmodel.Experience = dm.DesignerExperience;
                    objmodel.Country = dm.Country;
                    objmodel.CityName = dm.CityName;
                    objmodel.StateName = dm.StateName;
                    objmodel.CountryName = dm.CountryName;
                    objmodel.Mobile = dm.Mobile;
                    objmodel.EmailId = dm.EmailId;
                    objmodel.AddedOn = DateTime.Now;
                    objmodel.AddedBy = dm.AddedBy;
                    objmodel.IsActive = true;
                    objmodel.Zip = dm.Zip;
                    objmodel.Address = dm.DesignerAddress;
                    objmodel.HoursAvailable = dm.HoursAvailable;
                    objmodel.HoursAvailableFrom = dm.HoursAvailableFrom;
                    objmodel.HoursAvailableTo = dm.HoursAvailableTo;
                    objmodel.skypeId = dm.SkypeId;

                    
                    using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                    {
                        objdb.tblContentWriters.Add(objmodel);
                        objdb.SaveChanges();

                    }

                    CommonController objCommon = new CommonController();
                    NewUserNotificationEmail(dm.DesignerFirstName + " " + dm.DesigenerLastName, dm.EmailId, DefaultPassword, "Welcome to PeasyPrint", new StreamReader(Server.MapPath("~/Content/EmailTemplate/NewUser.html")).ReadToEnd().ToString(),"");
                }


                catch (Exception ex)
                {
                    Session["Adderror"] = ex.Message;
                    return RedirectToAction("Add", "ContentWriter", new { Error = -1 });
                }

            
            return RedirectToAction("Index", "ContentWriter", new { id = PkContentWritterId });

        }

        public void NewUserNotificationEmail(string user, string eMailAddress, string Password, string subject, string emailbody, string status)
        {
            try
            {


                SmtpClient mySmtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpClient"].ToString());

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = true;
                mySmtpClient.EnableSsl = true;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(ConfigurationSettings.AppSettings["fromAdd"].ToString(), ConfigurationSettings.AppSettings["password"].ToString());
                mySmtpClient.Credentials = basicAuthenticationInfo;
                MailAddress from = new MailAddress(ConfigurationSettings.AppSettings["fromAdd"].ToString());
                MailAddress to = new MailAddress(eMailAddress);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                myMail.Subject = subject;
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                var emailBody = emailbody;
                var emailTemplate = emailBody;
                string email = emailTemplate
                  .Replace("##Password##", Password)
                  .Replace("##Email##", eMailAddress)
                  .Replace("##username##", user)
                  .Replace("##Status##",status);

                myMail.Body = email;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;
                mySmtpClient.Send(myMail);


            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpGet]
        public ActionResult AddPMSCustomer()
        {
            DesignerModel objmodel = new DesignerModel();
            objmodel.ListCountry = GetCountryList();
            objmodel.ListState = GetStateList();
            objmodel.ListCity = GetCityList();
            objmodel.objOrderModel = GetOrderList();
            return View(objmodel);
        }

        [HttpPost]
        [InitializeSimpleMembership]
        public ActionResult AddPMSCustomer(DesignerModel dm)
        {
            int PkContentWritterId = 0;
            tblContentWriter objmodel = new tblContentWriter();
            
                try
                {

                    WebSecurity.CreateUserAndAccount(dm.EmailId, "123456");

                    Roles.AddUserToRole(dm.EmailId, "Customer");
                  //  objmodel.PkContentWritterId = WebSecurity.GetUserId(dm.EmailId);
                    //PkContentWritterId = WebSecurity.GetUserId(dm.EmailId);
                    //objmodel.FirstName = dm.DesignerFirstName;
                    //objmodel.LastName = dm.DesigenerLastName;
                    //objmodel.Sex = dm.Sex;
                    //objmodel.Experience = dm.DesignerExperience;
                    //objmodel.City = dm.City;
                    //objmodel.State = dm.State;
                    //objmodel.Country = dm.Country;
                    //objmodel.Mobile = dm.Mobile;
                    //objmodel.EmailId = dm.EmailId;
                    //objmodel.AddedOn = Convert.ToDateTime(dm.AddedOn);
                    //objmodel.AddedBy = dm.AddedBy;
                    //objmodel.IsActive = dm.IsActive;
                    //objmodel.Zip = dm.Zip;
                    //objmodel.Address = dm.DesignerAddress;



                    using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                    {
                       // objdb.tblContentWriters.Add(objmodel);
                       // objdb.SaveChanges();

                    }
                }


                catch (Exception ex)
                {

                }

            
            return RedirectToAction("AddPMSCustomer", "ContentWriter", new { id = PkContentWritterId });

        }

        public OrderModel GetOrderList()
        {
            OrderModel ordAssignment = new OrderModel();


            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {


                objCommonController.GetLeftMenuContent(ordAssignment);
                ordAssignment.DesingerList = ddldesigner();
                ordAssignment.ddlOrderList = ddlOrders();

            }





            return ordAssignment;
        }
        public List<ddlOrderList> ddlOrders()
        {
            List<ddlOrderList> ListOfDesigner = new List<ddlOrderList>();
            List<tblOrder> objtblOrders = new List<tblOrder>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblOrders = objdb.tblOrders.ToList();
            }

            foreach (var item in objtblOrders)
            {
                ddlOrderList objOrderList = new ddlOrderList();
                objOrderList.OrderNumber = item.OrderNumber;
                objOrderList.pkOrderId = item.pkOrderId;
                ListOfDesigner.Add(objOrderList);
            }

            return ListOfDesigner;

        }
        public List<DesingerList> ddldesigner()
        {
            List<DesingerList> ListOfDesigner = new List<DesingerList>();
            List<tblDesigner> objdesignerr = new List<tblDesigner>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objdesignerr = objdb.tblDesigners.ToList();
            }

            foreach (var item in objdesignerr)
            {
                DesingerList objDesingerList = new DesingerList();
                objDesingerList.DesignerName = item.DesignerFirstName;
                objDesingerList.DesignerId = item.PkDesignerId;
                ListOfDesigner.Add(objDesingerList);
            }

            return ListOfDesigner;

        }
        public List<tblCountry> GetCountryList()
        {
            List<tblCountry> objlist = new List<tblCountry>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objlist = objdb.tblCountries.ToList();
            }
            return objlist;
        }
        public List<tblState> GetStateList()
        {
            List<tblState> objlist = new List<tblState>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objlist = objdb.tblStates.ToList();
            }
            return objlist;
        }
        public List<tblCity> GetCityList()
        {
            List<tblCity> objlist = new List<tblCity>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objlist = objdb.tblCities.ToList();
            }
            return objlist;
        }

        public ActionResult GetStatelistBycountryID(int id)
        {
            List<tblState> objtblist = new List<tblState>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblist = (from t1 in objdb.tblStates where t1.FkCountryId == id select t1).ToList();
            }
            return Json(new SelectList(objtblist, "PkStateId", "Statename"));
        }
        public ActionResult GetCitylistBystateID(int id)
        {
            List<tblCity> objtblist = new List<tblCity>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblist = (from t1 in objdb.tblCities where t1.FkStateId == id select t1).ToList();
            }
            return Json(new SelectList(objtblist, "PkCityId", "CityName"));
        }

        [HttpGet]

        public ActionResult Edit(int id)
        {
            DesignerModel objmodel = new DesignerModel();
            objmodel = GetContentWriterById(id);
            objmodel.ListCountry = GetCountryList();
            objmodel.ListState = GetStateList();
            objmodel.ListCity = GetCityList();

            objmodel.objOrderModel = GetOrderList();
            return View(objmodel);
        }

        public DesignerModel GetContentWriterById(int id)
        {
            DesignerModel objm = new DesignerModel();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                tblContentWriter obtbl = objdb.tblContentWriters.Where(rec => rec.PkContentWritterId == id).FirstOrDefault();
                if (obtbl != null)
                {
                    objm.DesignerID = obtbl.PkContentWritterId;
                    objm.DesignerFirstName = obtbl.FirstName;
                    objm.DesigenerLastName = obtbl.LastName;
                    objm.Sex = obtbl.Sex;
                    objm.City = obtbl.City;
                    objm.State = obtbl.State;
                    objm.Country = obtbl.Country;
                    objm.Zip = obtbl.Zip;
                    objm.Mobile = obtbl.Mobile;
                    objm.DesignerExperience = obtbl.Experience;
                    objm.DesignerAddress = obtbl.Address;
                    objm.EmailId = obtbl.EmailId;
                    objm.IsActive = obtbl.IsActive;
                    objm.StateName = obtbl.StateName;
                    objm.CityName = obtbl.CityName;
                    objm.CountryName = obtbl.CountryName;




                    objm.AddedOn = Convert.ToDateTime(obtbl.AddedOn);
                }
            }

            return objm;
        }

        [HttpPost]
        public ActionResult Edit(DesignerModel objmodel)
        {
            int PkContentWritterId = 0;
            //if (ModelState.IsValid )
            //{

                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    tblContentWriter obtbl = objdb.tblContentWriters.Where(rec => rec.PkContentWritterId == objmodel.DesignerID).FirstOrDefault();
                    if (obtbl != null)
                    {
                        obtbl.PkContentWritterId = objmodel.DesignerID;
                        PkContentWritterId = objmodel.DesignerID;
                        obtbl.FirstName = objmodel.DesignerFirstName;
                        obtbl.LastName = objmodel.DesigenerLastName;
                        obtbl.Sex = objmodel.Sex;
                        obtbl.City = objmodel.City;
                        obtbl.State = objmodel.State;
                        obtbl.Country = objmodel.Country;
                        obtbl.Zip = objmodel.Zip;
                        obtbl.Mobile = objmodel.Mobile;
                        obtbl.Experience = objmodel.DesignerExperience;
                        obtbl.Address = objmodel.DesignerAddress;
                        obtbl.EmailId = objmodel.EmailId;
                        if (obtbl.IsActive != objmodel.IsActive && objmodel.IsActive == true)
                        {
                            NewUserNotificationEmail(obtbl.FirstName + " " + obtbl.LastName, obtbl.EmailId, "", "Account Active", new StreamReader(Server.MapPath("~/Content/EmailTemplate/AccountDeactivated.html")).ReadToEnd().ToString(), "Active");
                        }
                        if (obtbl.IsActive != objmodel.IsActive && objmodel.IsActive == false)
                        {
                            NewUserNotificationEmail(obtbl.FirstName + " " + obtbl.LastName, obtbl.EmailId, "", "Your account has been deactivated", new StreamReader(Server.MapPath("~/Content/EmailTemplate/AccountDeactivated.html")).ReadToEnd().ToString(), "Deactivated");
                        }
                        obtbl.IsActive = objmodel.IsActive;
                        obtbl.AddedOn = objmodel.AddedOn;
                        obtbl.AddedBy = objmodel.AddedBy;
                        
                        objdb.SaveChanges();

                    }
                }

                return RedirectToAction("Index", "ContentWriter", new { id = PkContentWritterId });

          //  }

        }

    }
}
