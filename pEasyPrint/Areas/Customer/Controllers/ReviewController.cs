using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MvcContrib.Pagination;
using pEasyPrint.Areas.Admin.Models;
using pEasyPrint.Models;
using WebMatrix.WebData;

namespace pEasyPrint.Areas.Customer.Controllers
{
   
    public class ReviewController : Controller
    {
        //
        // GET: /Customer/Review/

        public ActionResult Index(int? page)
        {           
            List<OrderModel> Orders = GetAllCustomerOrder();

            try
            {
                if (Orders != null)
                {
                    IPagination pagedModel = Orders.AsPagination(page ?? 1, 10);
                    return View(pagedModel);
                }
                else
                {
                    Orders = new List<OrderModel>();
                    IPagination pagedModel = Orders.AsPagination(page ?? 1, 10);
                    return View(pagedModel);
                }
            }
            catch (Exception)
            {
                
                throw;
            }      
        }
        
        public ActionResult Items(string Id)
        {
            int ID = Convert.ToInt32(Encdy.Decode(Id));
            List<OrderItemModel> Orders = GetAllItems(ID);

            try
            {
                if (Orders != null)
                {

                    return View(Orders);
                }
                else
                {
                    Orders = new List<OrderItemModel>();
                    return View(Orders);
                }
            }
            catch (Exception)
            {
                
                throw;
            }    
        }

        public ActionResult PsdProof( string  design, bool ? downloadEnabled, bool ? backgroundButtonEnabled, bool?  rectEllipseButtonsEnabled, string  mockup, bool? VisiblePrintArea, int ? userID = 0)
        {
          string  header = "1-sided%20Business%20Card";
          string psdUrl = "http://editor.peasyprint.com/ParameterizedPage.aspx?userID=" + userID +
                          "?header=" + header +
                        header + "&design=" + design + "&downloadEnabled=" + downloadEnabled +
                        "&backgroundButtonEnabled=" + backgroundButtonEnabled + "&rectEllipseButtonsEnabled="
                        + rectEllipseButtonsEnabled + "&mockup" + mockup + "&VisiblePrintArea" + VisiblePrintArea;

             ViewBag.Paramatter = psdUrl;
            try
            {
                                   
                    return View();
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult Item(string Id)
        {
            int ID = 0;
            try
            {
                 ID = Convert.ToInt32(Encdy.Decode(Id));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Review");
            }
            OrderProofViewModel OpF = new OrderProofViewModel();
            pEasyPrintEntities objDb = new pEasyPrintEntities();
            try
            {
                //ID = 1;
               
                tblOrderItem OrderItem = objDb.tblOrderItems.Where(i => i.ID == ID).FirstOrDefault();
              var CurrentUserName=  WebSecurity.CurrentUserName;
              if (OrderItem.Email == CurrentUserName)
                {
                    tblOrder Orders = objDb.tblOrders.Where(i => i.pkOrderId == OrderItem.OrderID).FirstOrDefault();
                    tblOrderProof OrderProof = objDb.tblOrderProofs.Where(i => i.OrderNumber == ID).FirstOrDefault();
                    if (OrderProof != null)
                    {
                        OpF.BackComment = OrderProof.BackComments;
                        OpF.CreatedDate = OrderProof.CreatedDate;
                        OpF.OrderNumber = OrderProof.OrderNumber;
                        OpF.CustomerFiles = OrderProof.CustomerFiles;
                        OpF.DesignerFiles = OrderProof.DesignerFiles;
                        OpF.FrontComment = OrderProof.FrontComments;
                        OpF.ItemProofs = objDb.tblOrderProofs.Where(i => i.OrderNumber == OrderProof.OrderNumber && i.IsAdminApproved == true).ToList();
                    }
                    OpF.ItemID = ID;
                    if (OrderItem != null)
                    {
                        OpF.OrderID = OrderItem.OrderID;
                        OpF.IsProofApproved = OrderItem.IsProofApproved;
                        OpF.CustomerBackComment = OrderItem.ItemBackComment;
                        OpF.CustomerFrontComment = OrderItem.ItemFrontComment;
                        OpF.ProjectName = Orders.ProjectName == null ? string.Empty : Orders.ProjectName;
                        OpF.ItemName = OrderItem.ItemName;
                    }
                }
                
               
                
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(OpF);
        }

        private List<OrderModel> GetAllCustomerOrder()
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
             List<OrderModel> CustomerOrderList = new List<OrderModel>();
             try
             {
                // string userEmail = WebSecurity.CurrentUserName;// "test@abc.com";
                 string userEmail = WebSecurity.CurrentUserName;
                 // string requestQueryString=Request.QueryString["UserName"];
                 List<tblOrder> Orders = objdb.tblOrders.Where(i => i.Email == userEmail).ToList();
                 foreach (tblOrder order in Orders)
                 {
                     OrderModel om = new OrderModel();
                     om.pkOrderId = order.pkOrderId;
                     om.OrderNumber = order.OrderNumber;
                     om.OrderDate = Convert.ToDateTime(order.OrderDate);
                     om.ProjectName = order.ProjectName;
                     CustomerOrderList.Add(om);
                 }
             }
             catch (Exception)
             {
                 
                 throw;
             }

             return CustomerOrderList;
        }

        [HttpPost]
        public ActionResult CustomerFilesUpload(IEnumerable<HttpPostedFileBase> files)
        {

            int PkOrderID = Convert.ToInt32(Request["hdnPkOrderID"]);

            try
            {
                string fileName = string.Empty;
                string FrontfileName = string.Empty;
                string BackfileName = string.Empty;



                HttpFileCollectionBase file = Request.Files;
                int  PkOrderProofId = Convert.ToInt32(Request["hdnPkOrderProofId"].ToString());
                string FileType = Request["FileType"].ToString();
                bool IsFrontComment = false;
                if (FileType == "FrontFile")
                {
                    IsFrontComment = true;
 
                }


                string CustomerBackComment = Request["CustomerBackComment"].ToString();
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                if (Request.Files.Count > 0)
                {

                    var rootDir = Server.MapPath("~/Images/proofUpload");
                    int counter = 1;
                    if (files != null)
                    {
                        foreach (HttpPostedFileBase item2 in files)
                        {
                            if (item2 != null)
                            {
                                var FileName = Path.GetFileName(item2.FileName);

                                fileName = Path.Combine(rootDir, FileName);

                                var Dirpath = Path.Combine(rootDir);
                                if (!Directory.Exists(Dirpath))
                                {
                                    Directory.CreateDirectory(Dirpath);
                                }

                                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

                                String ext = System.IO.Path.GetExtension(item2.FileName);
                                DateTime thisDate1 = DateTime.Now;
                                var UidNumber = thisDate1.ToString("MMMMddyyyyhhmmssss") + counter + ext;

                                fileName = r.Replace(fileName, "");

                                fileName = PkOrderProofId + "_" + "CustomerBackfront_" + UidNumber;
                                BackfileName = BackfileName + "," + fileName;
                                item2.SaveAs(Server.MapPath("~/Images/proofUpload/" + fileName));
                                counter++;
                            }
                        }
                    }



                    SaveCustomerProofInDb(CustomerBackComment, PkOrderProofId, IsFrontComment, BackfileName);
                   
                }
                return RedirectToAction("Item", "review", new { id = Encdy.Encode(PkOrderID) });

            }
            catch (Exception)
            {

                return RedirectToAction("Item", "review", new { id = Encdy.Encode(PkOrderID) });
            }



        }
      //  public int SaveCustomerProofInDb(string Comments,  string Files, int PkOrderProofId, bool IsFrontComment)

        [HttpPost]
        public ActionResult SaveCustomerProofInDb(string Comments, int PkOrderProofId, bool IsFrontComment, string files)
        {
            try
            {
                string result = "fail";


                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    #region proof stage
                    var prrof = objdb.tblOrderProofs.Where(rec=>rec.PkOrderProofId ==PkOrderProofId).FirstOrDefault();
                    if( prrof != null)
                    {
                        if (IsFrontComment == true)
                        {
                            prrof.CustomerFrontComment = Comments;
                            prrof.CustomerFiles = files;
                           
                        }
                        else
                        {
                            prrof.CustomerBackComment = Comments;
                            prrof.CustomerBackFiles = files;
                           
                        }
                        objdb.SaveChanges();

                    }
                   

                   
                    
                    #endregion
                    
                    result = "done";
                }
                return Json(new { status = result });

            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public ActionResult ApproveProof(int PkOrderProofId)
        {
            string result = "Fail";
            try
            {
                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {

                    var tblOrderProofs = objdb.tblOrderProofs.Where(REC => REC.PkOrderProofId == PkOrderProofId).FirstOrDefault();


                    if (tblOrderProofs != null)
                    {
                        var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == tblOrderProofs.OrderNumber).FirstOrDefault();
                        var OrderItems = objdb.tblOrders.Where(REC => REC.pkOrderId == tblOrderItems.OrderID).FirstOrDefault();
                        if (tblOrderItems != null)
                        {
                            tblOrderItems.IsProofApproved = true;
                            tblOrderItems.OrderStage = 4;// Order Stage set for ApprovedProof in back end db
                        }
                        tblOrderProofs.IsCustomerApproved = true;
                        objdb.SaveChanges();
                        tblDesigner objtbl = GetDesignerById(tblOrderItems.DesignerId);
                        NotificationEmail(objtbl.DesignerFirstName + " " + objtbl.DesigenerLastName, objtbl.EmailId, OrderItems.ProjectName);
                        result = "done";
                    }

                    

                    return Json(new { result = result });

                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }


        public tblDesigner GetDesignerById(int ? PkDesignerId )
        {
            tblDesigner objtbl = new tblDesigner();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtbl = objdb.tblDesigners.Where(rec => rec.PkDesignerId == PkDesignerId).FirstOrDefault();
            }
            return objtbl;
        }
        private void NotificationEmail(string user, string eMailAddress, string ProjectName)
        {
            try
            {

                SmtpClient mySmtpClient1 = new SmtpClient("smtp.gmail.com");
                int portNumber = 587;
                SmtpClient mySmtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpClient"].ToString());


                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = true;
                mySmtpClient.EnableSsl = true;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(ConfigurationSettings.AppSettings["fromAdd"].ToString(), ConfigurationSettings.AppSettings["password"].ToString());
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(ConfigurationSettings.AppSettings["fromAdd"].ToString());
                MailAddress to = new MailAddress(eMailAddress);// new MailAddress(eMailAddress); 
                // MailAddress from = new MailAddress("lakhvinder.happy@gmail.com");
                // MailAddress to = new MailAddress("lakhvinder.kumar@team.amonous.com");// new MailAddress(eMailAddress); 

                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                myMail.Subject= "Proof Approved";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;
                

                var emailBody = string.Empty;
                
                emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/Approved.html")).ReadToEnd().ToString();
               
                var emailTemplate = emailBody;
                string email = emailTemplate
                  .Replace("##ProjectName##", ProjectName)
                  .Replace("##username##", user);

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


        

        public FileResult DownloadProofFile(string FileName)
        {
            string txt = FileName;
            char[] splitedcBy = new char[] { '.' };

            char[] splitedcBy2 = new char[] { '_' };
            var filenamw = FileName.Split(splitedcBy2);
            var ext = FileName.Split(splitedcBy);
            string finalName = filenamw[2] + "." + ext[1];
            return File(Server.MapPath("~/Images/proofUpload/" + FileName), System.Net.Mime.MediaTypeNames.Application.Octet, finalName);
        }

        [HttpPost]
        public ActionResult UploadCustomerFile(IEnumerable<HttpPostedFileBase> files, IEnumerable<HttpPostedFileBase> files2)
        {
            int PkOrderID =Convert.ToInt32(Request.UrlReferrer.Segments[4]);
            try
            {
                string fileName = string.Empty;
                string FrontfileName = string.Empty;
                string BackfileName = string.Empty;



                HttpFileCollectionBase file = Request.Files;
                //string CustomerFrontItemComment = Request["CustomerFrontItemComment"].ToString();
                //string CustomerBackItemComment = Request["CustomerBackItemComment"].ToString();
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                if (Request.Files.Count > 0)
                {

                    var rootDir = Server.MapPath("~/Images/proofUpload");
                    int counter = 1;
                    if (files != null)
                    {
                        foreach (HttpPostedFileBase item2 in files)
                        {
                            if (item2 != null)
                            {
                                var FileName = Path.GetFileName(item2.FileName);

                                fileName = Path.Combine(rootDir, FileName);

                                var Dirpath = Path.Combine(rootDir);
                                if (!Directory.Exists(Dirpath))
                                {
                                    Directory.CreateDirectory(Dirpath);
                                }

                                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

                                String ext = System.IO.Path.GetExtension(item2.FileName);
                                DateTime thisDate1 = DateTime.Now;
                                var UidNumber = thisDate1.ToString("MMMMddyyyyhhmmssss") + counter + ext;

                                fileName = r.Replace(fileName, "");

                                fileName = PkOrderID + "_" + "frontitemComment_" + UidNumber;
                                FrontfileName = FrontfileName + "," + fileName;
                                item2.SaveAs(Server.MapPath("~/Images/proofUpload/" + fileName));
                                counter++;
                            }
                        }
                    }






                    SaveCustomerFiles(PkOrderID, FrontfileName);
                }
                return RedirectToAction("item", "Review", new { id = PkOrderID });

            }
            catch (Exception)
            {

                return RedirectToAction("item", "Project", new { id = PkOrderID });
            }



        }

       
        public int SaveCustomerFiles( int PkItemId, string CustomerFiles)
        {

            int result = -1;
            try
            {
                
                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    #region proof stage
                    var tblOrderItems = objdb.tblOrderItems.Where(rec => rec.ID == PkItemId).FirstOrDefault();
                    if (tblOrderItems != null)
                    {
                        if (!string.IsNullOrEmpty(CustomerFiles))
                        {
                            var previous = tblOrderItems.CustomerUploadedFiles;
                            tblOrderItems.CustomerUploadedFiles = previous + CustomerFiles;
                        }                       


                        objdb.SaveChanges();

                    }

                    #endregion
                    result = 1;
                }
                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }



        private List<OrderItemModel> GetAllItems(int OrderId)
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            List<OrderItemModel> CustomerOrderItemList = new List<OrderItemModel>();


            try
            {
                List<tblOrderItem> Items = objdb.tblOrderItems.Where(i => i.OrderID == OrderId).ToList();
                foreach (tblOrderItem item in Items)
                {
                    OrderItemModel oim = new OrderItemModel();
                    oim.ProjectName = item.ProjectName;
                    oim.ItemName = item.ItemName;
                    oim.Sku = item.Sku;
                    oim.OrderNumber = item.OrderID;
                    oim.ID = item.ID;
                    oim.CreatedOn = item.CreatedOn;
                    oim.size = item.size;
                    oim.folding = item.folding;
                    oim.paper = item.paper;
                    oim.color = item.color;
                    oim.turnaround = item.turnaround;
                    CustomerOrderItemList.Add(oim);
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return CustomerOrderItemList;
        }



    }
}
