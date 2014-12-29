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
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace pEasyPrint.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DesignerController : Controller
    {
        //
        // GET: /Admin/DesignerSetting/
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
            List<tblDesigner> objtblist = new List<tblDesigner>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblist = objdb.tblDesigners.ToList();
                OrderModel OrderModelLeftMenu = new OrderModel();
                objCommonController.GetLeftMenuContent(OrderModelLeftMenu);
                objlist.Add(OrderModelLeftMenu);
                if (objtblist.Count() > 0)
                {
                    objlist = new List<OrderModel>();
                    foreach (var item in objtblist)
                    {
                        OrderModel objDesignerModel = new OrderModel();
                        objDesignerModel.DesignerId = item.PkDesignerId;
                        objDesignerModel.DesignerFirstName = item.DesignerFirstName;
                        objDesignerModel.DesigenerLastName = item.DesigenerLastName;
                        objDesignerModel.Sex = item.Sex;
                        objDesignerModel.DesignerExperience = item.DesignerExperience;
                        objDesignerModel.DesignerAddress = item.DesignerAddress;
                        objDesignerModel.Country = item.Country;
                        objDesignerModel.CountryName = GetCountryNamebyid(item.Country);
                        objDesignerModel.State = item.State;
                        objDesignerModel.StateName = GetStateNameById(item.State);
                        objDesignerModel.City = item.City;
                        objDesignerModel.CityName = GetCityNameById(item.City);
                        objDesignerModel.Zip = item.Zip;
                        objDesignerModel.Mobile = item.Mobile;
                        objDesignerModel.EmailId = item.EmailId;
                        objDesignerModel.AddedOn = item.AddedOn;
                        objDesignerModel.AddedBy = item.AddedBy;
                        objDesignerModel.Photo = item.Photo;
                        objDesignerModel.IsActive = item.IsActive;

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
            int PkDesignerId = 0;
            tblDesigner objmodel = new tblDesigner();

            try
            {
                string DefaultPassword = ConfigurationSettings.AppSettings["NewUserDefaultPassowrd"].ToString();
                WebSecurity.CreateUserAndAccount(dm.EmailId, DefaultPassword);
                Roles.AddUserToRole(dm.EmailId, "Designer");
                objmodel.PkDesignerId = WebSecurity.GetUserId(dm.EmailId);
                PkDesignerId = WebSecurity.GetUserId(dm.EmailId);
                objmodel.DesignerFirstName = dm.DesignerFirstName;
                objmodel.DesigenerLastName = dm.DesigenerLastName;
                objmodel.Sex = dm.Sex;
                objmodel.DesignerExperience = dm.DesignerExperience;
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
                objmodel.HoursAvailable = dm.HoursAvailable;
                objmodel.HoursAvailableFrom = dm.HoursAvailableFrom;
                objmodel.HoursAvailableTo = dm.HoursAvailableTo;
                objmodel.DesignerAddress = dm.DesignerAddress;
                objmodel.skypeId = dm.SkypeId;

                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    objdb.tblDesigners.Add(objmodel);
                    objdb.SaveChanges();

                }
                NewUserNotificationEmail(dm.DesignerFirstName + " " + dm.DesigenerLastName, dm.EmailId, DefaultPassword, "Welcome to PeasyPrint", new StreamReader(Server.MapPath("~/Content/EmailTemplate/NewUser.html")).ReadToEnd().ToString(), "");
            }


            catch (Exception ex)
            {
                Session["Adderror"] = ex.Message;
                return RedirectToAction("Add", "Designer", new { Error = -1 });
            }




            return RedirectToAction("Index", "Designer", new { id = PkDesignerId });
        }

        private void NewUserNotificationEmail(string user, string eMailAddress, string Password, string subject, string emailbody, string status)
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
                  .Replace("##username##", user).Replace("##Status##", status);

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


        public List<timeddl> GetTimeList()
        {
            List<timeddl> timeList = new List<timeddl>();

            var currenthour = DateTime.Now.Hour;








            return timeList;
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
            objmodel = GetDesignerById(id);
            objmodel.ListCountry = GetCountryList();
            objmodel.ListState = GetStateList();
            objmodel.ListCity = GetCityList();

            objmodel.objOrderModel = GetOrderList();
            return View(objmodel);
        }

        public DesignerModel GetDesignerById(int id)
        {
            DesignerModel objm = new DesignerModel();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                tblDesigner obtbl = objdb.tblDesigners.Where(rec => rec.PkDesignerId == id).FirstOrDefault();
                if (obtbl != null)
                {
                    CommonController ObjCommon = new CommonController();
                    objm.DesignerID = obtbl.PkDesignerId;
                    objm.DesignerFirstName = obtbl.DesignerFirstName;
                    objm.DesigenerLastName = obtbl.DesigenerLastName;
                    objm.Sex = obtbl.Sex;
                    objm.Zip = obtbl.Zip;
                    objm.Mobile = obtbl.Mobile;
                    objm.DesignerExperience = obtbl.DesignerExperience;
                    objm.DesignerAddress = obtbl.DesignerAddress;
                    objm.EmailId = obtbl.EmailId;
                    objm.SkypeId = obtbl.skypeId;
                    objm.IsActive = obtbl.IsActive;
                    objm.Country = obtbl.Country;
                    objm.CityName = obtbl.CityName;
                    objm.StateName = obtbl.StateName;
                    objm.CountryName = obtbl.CountryName;
                    objm.HoursAvailable = obtbl.HoursAvailable;
                    objm.HoursAvailableFrom = obtbl.HoursAvailableFrom;
                    objm.HoursAvailableTo = obtbl.HoursAvailableTo;

                    objm.AddedOn = Convert.ToDateTime(obtbl.AddedOn);
                }
            }

            return objm;
        }

        [HttpPost]
        public ActionResult Edit(DesignerModel objmodel)
        {
            int PkDesignerId = 0;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                tblDesigner obtbl = objdb.tblDesigners.Where(rec => rec.PkDesignerId == objmodel.DesignerID).FirstOrDefault();
                if (obtbl != null)
                {
                    obtbl.PkDesignerId = objmodel.DesignerID;
                    PkDesignerId = objmodel.DesignerID;
                    obtbl.DesignerFirstName = objmodel.DesignerFirstName;
                    obtbl.DesigenerLastName = objmodel.DesigenerLastName;
                    obtbl.Sex = objmodel.Sex;
                    obtbl.CityName = objmodel.CityName;
                    obtbl.StateName = objmodel.StateName;
                    obtbl.CountryName = objmodel.CountryName;
                    obtbl.Zip = objmodel.Zip;
                    obtbl.Mobile = objmodel.Mobile;
                    obtbl.DesignerExperience = objmodel.DesignerExperience;
                    obtbl.DesignerAddress = objmodel.DesignerAddress;
                    obtbl.EmailId = objmodel.EmailId;
                    obtbl.AddedOn = objmodel.AddedOn;
                    obtbl.AddedBy = objmodel.AddedBy;
                    obtbl.skypeId = objmodel.SkypeId;
                    if (obtbl.IsActive != objmodel.IsActive && objmodel.IsActive == true)
                    {
                        NewUserNotificationEmail(obtbl.DesignerFirstName + " " + obtbl.DesigenerLastName, obtbl.EmailId, "", "Account Active", new StreamReader(Server.MapPath("~/Content/EmailTemplate/AccountDeactivated.html")).ReadToEnd().ToString(), "Active");
                    }
                    if (obtbl.IsActive != objmodel.IsActive && objmodel.IsActive == false)
                    {
                        NewUserNotificationEmail(obtbl.DesignerFirstName + " " + obtbl.DesigenerLastName, obtbl.EmailId, "", "Your account has been deactivated", new StreamReader(Server.MapPath("~/Content/EmailTemplate/AccountDeactivated.html")).ReadToEnd().ToString(), "Deactivated");
                    }
                    obtbl.IsActive = objmodel.IsActive;
                    obtbl.HoursAvailable = objmodel.HoursAvailable;
                    obtbl.HoursAvailableFrom = objmodel.HoursAvailableFrom;
                    obtbl.HoursAvailableTo = objmodel.HoursAvailableTo;


                    objdb.SaveChanges();

                }
            }


            return RedirectToAction("Index", "Designer", new { id = PkDesignerId });



        }

    }
}
