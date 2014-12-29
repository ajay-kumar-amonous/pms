using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pEasyPrint.Areas.Admin.Models;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using System.IO;
using System.Text.RegularExpressions;
using pEasyPrint.Models;
using pEasyPrint.Controllers;
using System.Net.Mail;
using System.Configuration;


namespace pEasyPrint.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {

        // GET: /Admin/Order/
        CommonController objCommonController = new CommonController();

        public ActionResult Index(int? page, int? pagesize, string fromdate, string todate, string SearchType, string SearchValue, string SearchBy, string OrderStage)
        {

            OrderModel objModel = new OrderModel();
            int PageSize = pagesize == null ? 20 : Convert.ToInt32(pagesize);

            ViewBag.pagesize = PageSize;
            //objModel.OrderList = GetOrderList();
            List<OrderModel> orders = GetOrderList();
            if (!string.IsNullOrEmpty(fromdate) && !string.IsNullOrEmpty(todate))
            {
                DateTime _fromdate = Convert.ToDateTime(fromdate);
                DateTime _todate = Convert.ToDateTime(todate);

                orders = GetOrderByDate(_fromdate, _todate);
            }
            else if (SearchBy == "Order")
            {
                orders = GetOrderByOrdesField(SearchType, SearchValue);
            }

            else if (SearchBy == "Designer")
            {
                orders = GetOrderByDesignerFiled(SearchType, SearchValue);
            }
            else if (!string.IsNullOrEmpty(OrderStage))
            {
                orders = GetOrderByOrderStage(Convert.ToInt32(OrderStage));
            }
            if (orders != null && orders.Count() > 0)
            {
                var currentUrl = Request.CurrentExecutionFilePath;
                if (currentUrl.Contains("Order/Edit"))
                {
                    //  Edit(orders.FirstOrDefault().pkOrderId);
                    return RedirectToAction("Edit", "Order", new { id = orders.FirstOrDefault().pkOrderId });
                }
                else
                {
                    IPagination pagedModel = orders.AsPagination(page ?? 1, PageSize);
                    return View(pagedModel);
                }


            }
            else
            {
                orders = new List<OrderModel>();
                IPagination pagedModel = orders.AsPagination(page ?? 1, PageSize);
                return View(pagedModel);
            }

        }
        public ActionResult Reports(int? page)
        {


            List<OrderModel> orders = GetOrderList();
            if (orders != null && orders.Count() > 0)
            {
                var currentUrl = Request.CurrentExecutionFilePath;
                if (currentUrl.Contains("Order/Edit"))
                {
                    //  Edit(orders.FirstOrDefault().pkOrderId);
                    return RedirectToAction("Edit", "Order", new { id = orders.FirstOrDefault().pkOrderId });
                }
                else
                {
                    IPagination pagedModel = orders.AsPagination(page ?? 1, 10);
                    return View(pagedModel);
                }


            }
            else
            {
                orders = new List<OrderModel>();
                IPagination pagedModel = orders.AsPagination(page ?? 1, 10);
                return View(pagedModel);
            }


        }
        public List<OrderModel> GetOrderByDesignerFiled(string SearchType, string SearchValue)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                tblDesigner designer = new tblDesigner();
                OrderModel objOrderModel = new OrderModel();
                objCommonController.GetLeftMenuContent(objOrderModel);
                ObjtblOrderList.Add(objOrderModel);
                if (SearchType == "Name")
                {
                    designer = objdb.tblDesigners.Where(rec => rec.DesignerFirstName == SearchValue).FirstOrDefault();
                }
                else if (SearchType == "Experience")
                {
                    int DesignerExperience = Convert.ToInt32(SearchValue);
                    designer = objdb.tblDesigners.Where(rec => rec.DesignerExperience == DesignerExperience).FirstOrDefault();
                }
                else if (SearchType == "Phone")
                {
                    designer = objdb.tblDesigners.Where(rec => rec.Mobile == SearchValue).FirstOrDefault();

                }
                else if (SearchType == "Email")
                {
                    designer = objdb.tblDesigners.Where(rec => rec.EmailId == SearchValue).FirstOrDefault();
                }


                if (designer != null)
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == designer.PkDesignerId).ToList();
                    var list = (from t1 in objdb.tblOrders.AsEnumerable()
                                join t3 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t3.OrderID
                                where t3.DesignerId == designer.PkDesignerId && t3.OrderStage != 6
                                select new
                                {
                                    t1.pkOrderId,
                                    t1.OrderNumber,
                                    t3.ProjectName,
                                    t1.ClientName,
                                    t1.ProjectType,
                                    t1.Phone,
                                    t1.Email,

                                    t1.IsStatus,
                                    t1.IsRejected,
                                    t1.IsAccepted,
                                    t1.OrderDate,
                                    t1.DesignerId,
                                }).ToList();

                    foreach (var item in list)
                    {
                        tblOrder objOrderModel1 = new tblOrder();
                        objOrderModel1.pkOrderId = item.pkOrderId;
                        objOrderModel1.OrderNumber = item.OrderNumber.ToString();
                        objOrderModel1.ClientName = item.ClientName;
                        objOrderModel1.ProjectName = item.ProjectName;
                        objOrderModel1.ProjectType = item.ProjectType;
                        objOrderModel1.Phone = item.Phone;
                        objOrderModel1.Email = item.Email;
                        objOrderModel1.IsStatus = item.IsStatus;
                        objOrderModel1.IsAccepted = item.IsAccepted;
                        objOrderModel1.OrderDate = item.OrderDate;
                        objOrderModel1.DesignerId = item.DesignerId;
                        ObjtblOrderList1.Add(objOrderModel1);
                    }


                }

            }

            if (ObjtblOrderList1.Count > 1)
            {
                ObjtblOrderList = new List<OrderModel>();
                foreach (var item in ObjtblOrderList1)
                {
                    OrderModel objOrderModel = new OrderModel();
                    objOrderModel.pkOrderId = item.pkOrderId;
                    objOrderModel.OrderNumber = item.OrderNumber;
                    objOrderModel.OrderNumber1 = Convert.ToInt32(item.OrderNumber);
                    objOrderModel.ClientName = item.ClientName;
                    objOrderModel.ProjectName = item.ProjectName;
                    objOrderModel.ProjectType = item.ProjectType;
                    objOrderModel.Phone = item.Phone;
                    objOrderModel.Email = item.Email;
                    objOrderModel.IsStatus = item.IsStatus;
                    objOrderModel.IsRejected = item.IsRejected;
                    objOrderModel.IsAccepted = item.IsAccepted;
                    objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                    objOrderModel.DesignerId = item.DesignerId;

                    objOrderModel.DesingerList = ddldesigner();
                    objCommonController.GetLeftMenuContent(objOrderModel);
                    ObjtblOrderList.Add(objOrderModel);
                }
            }
            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber1).ToList();
        }

        public List<OrderModel> GetOrderByOrdesField(string SearchType, string SearchValue)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                OrderModel objOrderModelLeft = new OrderModel();
                objCommonController.GetLeftMenuContent(objOrderModelLeft);
                ObjtblOrderList.Add(objOrderModelLeft);
                if (SearchType == "DesignerId")
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.pkOrderId == Convert.ToInt32(SearchValue)).ToList();
                }
                else if (SearchType == "OrderNumber")
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.OrderNumber == SearchValue).ToList();
                }
                else if (SearchType == "OrderEmail")
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.Email == SearchValue).ToList();

                }
                else if (SearchType == "Project")
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.ProjectName.Contains(SearchValue)).ToList();
                }
                else if (SearchType == "ProjectType")
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.ProjectType.Contains(SearchValue)).ToList();
                }
                else if (SearchType == "ClientName")
                {
                    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.ClientName.Contains(SearchValue)).ToList();
                }



            }
            CommonController objCommonfn = new CommonController();
            if (ObjtblOrderList1.Count() > 0)
            {
                ObjtblOrderList = new List<OrderModel>();
                foreach (var item in ObjtblOrderList1)
                {
                    OrderModel objOrderModel = new OrderModel();
                    objOrderModel.pkOrderId = item.pkOrderId;
                    objOrderModel.OrderNumber = item.OrderNumber;
                    objOrderModel.OrderNumber1 = Convert.ToInt32(item.OrderNumber);

                    objOrderModel.ClientName = item.ClientName;
                    objOrderModel.ProjectName = objCommonfn.GetProjectNamebyID(item.pkOrderId);
                    objOrderModel.ProjectType = item.ProjectType;
                    objOrderModel.Phone = item.Phone;
                    objOrderModel.Email = item.Email;
                    objOrderModel.IsStatus = item.IsStatus;
                    objOrderModel.IsRejected = item.IsRejected;
                    objOrderModel.IsAccepted = item.IsAccepted;
                    objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                    objOrderModel.DesignerId = item.DesignerId;
                    objOrderModel.CategoryType = "Orders By Order Number";

                    objOrderModel.DesingerList = ddldesigner();
                    objCommonController.GetLeftMenuContent(objOrderModel);
                    ObjtblOrderList.Add(objOrderModel);
                }
            }
            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber1).ToList();
        }

        [HttpPost]
        public ActionResult SaveCustomerEmail(int pkOrderId, string CustomEmail)
        {
            string result = "Fail";
            string AdminSaleEmail = ConfigurationSettings.AppSettings["AdminSaleEmail"].ToString();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var tblOrders = objdb.tblOrders.Where(rec => rec.Email == AdminSaleEmail && rec.pkOrderId == pkOrderId).FirstOrDefault();

                if (tblOrders != null)
                {
                    var tblOrderItemsList = objdb.tblOrderItems.Where(rec => rec.OrderID == tblOrders.pkOrderId).ToList();
                    foreach (tblOrderItem item in tblOrderItemsList)
                    {
                        item.CustEmail = CustomEmail;
                        objdb.SaveChanges();
                    }


                }
            }
            return Json(new { result = result });
        }

        [HttpPost]
        public ActionResult AssignContentWriter(int OrderId, int ContentWritterId, string OrderNumber, bool IsContentAssign, string Comments)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var ObjtblOrder = objdb.tblOrderItems.Where(rec => rec.ID == OrderId).FirstOrDefault();
                var tblOrders = objdb.tblOrders.Where(rec => rec.pkOrderId == OrderId).FirstOrDefault();
                tblContentWriter objtblContentWriters = new tblContentWriter();
                string type = "ContentWriter";
                objtblContentWriters = GetContenWriterById(Convert.ToInt16(ContentWritterId));
                if (ObjtblOrder != null)
                {
                    ObjtblOrder.fkContentWritterId = Convert.ToInt16(ContentWritterId);
                    ObjtblOrder.AdminCommentForContent = Comments;

                    objdb.SaveChanges();
                    string emailbody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/JobAssigned.html")).ReadToEnd().ToString();
                    NotificationEmail(objtblContentWriters.FirstName + " " + objtblContentWriters.LastName, objtblContentWriters.EmailId, ConfigurationSettings.AppSettings["subjectToContentwriter"].ToString() + " " + DateTime.Now.ToString(), type, tblOrders.ProjectName, "", emailbody);
                    result = "Done";
                }
            }
            return Json(new { result = result });
        }


        [HttpPost]
        public ActionResult AddFrntAndBckproofComment(int PkOrderProofId, string frontcomment, string backcomment)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var ObjtblOrder = objdb.tblOrderProofs.Where(rec => rec.PkOrderProofId == PkOrderProofId).FirstOrDefault();
                if (ObjtblOrder != null)
                {
                    ObjtblOrder.AdminBackComment = backcomment;
                    ObjtblOrder.AdminFrontComment = frontcomment;
                    objdb.SaveChanges();
                    result = "Done";
                }
            }
            return Json(new { result = result });
        }





        public FileResult DownloadContentFiles(string FileName)
        {
            string txt = FileName;
            char[] splitedcBy = new char[] { '.' };

            char[] splitedcBy2 = new char[] { '_' };
            var filenamw = FileName.Split(splitedcBy2);
            var ext = FileName.Split(splitedcBy);
            string finalName = filenamw[2] + "." + ext[1];
            return File(Server.MapPath("~/Images/OrderContent1/" + FileName), System.Net.Mime.MediaTypeNames.Application.Octet, finalName);
        }

        [HttpPost]
        public ActionResult AddComments(int OrderId, string Comments, string OrderNumber)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var OrderAssignments = objdb.OrderAssignments.Where(REC => REC.OrderNumber == OrderNumber).FirstOrDefault();

                if (OrderAssignments != null)
                {
                    //  Edit Assignment
                    OrderAssignments.Comments = Comments;
                    objdb.SaveChanges();
                }
                else
                {
                    //  Add new Assignment
                    OrderAssignment OBJOrderAssignments = new OrderAssignment();
                    OBJOrderAssignments.Comments = Comments;
                    OBJOrderAssignments.AssignDate = DateTime.Now;
                    OBJOrderAssignments.AssignedBy = "Admin";
                    OBJOrderAssignments.OrderNumber = OrderNumber;
                    objdb.OrderAssignments.Add(OBJOrderAssignments);
                    objdb.SaveChanges();



                }
            }
            return Json(new { result = result });

        }




        [HttpGet]
        public ActionResult Edit(int id, string SearchType, string SearchValue, string SearchBy, int? page)
        {
            List<OrderModel> model = new List<OrderModel>();
            tblOrder objbtl = new tblOrder();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objbtl = objdb.tblOrders.Where(rec => rec.pkOrderId == id).FirstOrDefault();
            }
            if (objbtl != null)
            {
                model = GetOrderInfoView(id);
            }
            if (SearchBy == "Order")
            {
                model = new List<OrderModel>();
                model = GetOrderInfoByField(id, SearchType, SearchValue);
            }
            try
            {
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    IPagination pagedModel = model.AsPagination(page ?? 1, 10);
                    return View(pagedModel);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        [HttpGet]
        public ActionResult item(int id, string SearchType, string SearchValue, string SearchBy)
        {
            OrderModel model = GetItemInfobyId(id);

            try
            {
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    OrderModel objmodel = new OrderModel();
                    return View(objmodel);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        [HttpPost]
        public Boolean approveAndinform(int OrderProofId)
        {
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var tblOrderProofs = objdb.tblOrderProofs.Where(t => t.PkOrderProofId == OrderProofId).FirstOrDefault();
                if (tblOrderProofs != null)
                {
                    tblOrderProofs.IsAdminApproved = true;
                    tblOrder _tblOrder = objdb.tblOrders.AsEnumerable().Where(i => i.pkOrderId == tblOrderProofs.OrderNumber).FirstOrDefault();
                    int saved = objdb.SaveChanges();


                    var orderItem = objdb.tblOrderItems.Where(i => i.ID == tblOrderProofs.OrderNumber).FirstOrDefault();
                    string emailbody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/ProofUploaded.html")).ReadToEnd().ToString();
                    try
                    {
                        SmtpClient mySmtpClient1 = new SmtpClient("smtp.gmail.com");
                        SmtpClient mySmtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpClient"].ToString());
                        // set smtp-client with basicAuthentication
                        mySmtpClient.UseDefaultCredentials = false;
                        mySmtpClient.EnableSsl = true;
                        System.Net.NetworkCredential basicAuthenticationInfo = new
                           System.Net.NetworkCredential(ConfigurationSettings.AppSettings["fromAdd"].ToString(), ConfigurationSettings.AppSettings["password"].ToString());
                        mySmtpClient.Credentials = basicAuthenticationInfo;

                        // add from,to mailaddresses
                        MailAddress from = new MailAddress(ConfigurationSettings.AppSettings["fromAdd"].ToString());
                        MailAddress to = new MailAddress(orderItem.CustEmail);

                        MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                        // set subject and encoding
                        myMail.Subject = "Proof is uploaded. Please check.";
                        myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                        string email = emailbody.Replace("##ProjectName##", _tblOrder.ProjectName);

                        myMail.Body = email;
                        myMail.BodyEncoding = System.Text.Encoding.UTF8;
                        myMail.IsBodyHtml = true;
                        mySmtpClient.Send(myMail);

                        return true;

                    }

                    catch (SmtpException ex)
                    {
                        throw new ApplicationException
                          ("SmtpException has occured: " + ex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                        return false;
                    }


                }
                else
                {
                    return false;
                }
            }



        }



        [HttpPost]
        public Boolean disApprove(int OrderProofId)
        {
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var tblOrderProofs = objdb.tblOrderProofs.Where(t => t.PkOrderProofId == OrderProofId).FirstOrDefault();
                if (tblOrderProofs != null)
                {
                    tblOrderProofs.IsAdminApproved = false;
                    tblOrderProofs.ParentProofId = OrderProofId;
                    tblOrderProofs.RejectedDate = DateTime.Now;
                    int saved = objdb.SaveChanges();
                    if (saved > 0)
                    {
                        var orderItem = objdb.tblOrderItems.Where(i => i.ID == tblOrderProofs.OrderNumber).FirstOrDefault();
                        var designer = objdb.tblDesigners.Where(i => i.PkDesignerId == orderItem.DesignerId).FirstOrDefault();
                        string emailbody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/ProofDisapproved.html")).ReadToEnd().ToString();
                        try
                        {
                            SmtpClient mySmtpClient1 = new SmtpClient("smtp.gmail.com");
                            SmtpClient mySmtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpClient"].ToString());
                            // set smtp-client with basicAuthentication
                            mySmtpClient.UseDefaultCredentials = false;
                            mySmtpClient.EnableSsl = true;
                            System.Net.NetworkCredential basicAuthenticationInfo = new
                               System.Net.NetworkCredential(ConfigurationSettings.AppSettings["fromAdd"].ToString(), ConfigurationSettings.AppSettings["password"].ToString());
                            mySmtpClient.Credentials = basicAuthenticationInfo;

                            // add from,to mailaddresses
                            MailAddress from = new MailAddress(ConfigurationSettings.AppSettings["fromAdd"].ToString());
                            MailAddress to = new MailAddress(designer.EmailId);

                            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                            // set subject and encoding
                            myMail.Subject = "Proof is Disapprove. Please check.";
                            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                            string email = emailbody;

                            myMail.Body = email;
                            myMail.BodyEncoding = System.Text.Encoding.UTF8;
                            myMail.IsBodyHtml = true;
                            mySmtpClient.Send(myMail);

                            return true;

                        }

                        catch (SmtpException ex)
                        {
                            throw new ApplicationException
                              ("SmtpException has occured: " + ex.Message);
                            return false;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }



        }


        public List<OrderModel> GetOrderInfoByField(int Id, string SearchType, string SearchValue)
        {
            List<OrderModel> lst_OrderModel = new List<OrderModel>();
            OrderModel objOrderModel = new OrderModel();
            List<tblOrder> objTbl = new List<tblOrder>();

            objOrderModel.DesingerList = ddldesigner();
            objOrderModel.ddlOrderList = ddlOrders();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objTbl = objdb.tblOrders.Where(rec => rec.pkOrderId == Id).ToList();


                if (SearchType == "DesignerId")
                {
                    int pkOrderId = Convert.ToInt32(SearchValue);
                    objTbl = objdb.tblOrders.Where(rec => rec.pkOrderId == pkOrderId).ToList();
                }
                else if (SearchType == "OrderNumber")
                {
                    objTbl = objdb.tblOrders.Where(rec => rec.OrderNumber == SearchValue).ToList();

                }
                else if (SearchType == "OrderEmail")
                {
                    objTbl = objdb.tblOrders.Where(rec => rec.Email == SearchValue).ToList();


                }
                else if (SearchType == "Project")
                {
                    objTbl = objdb.tblOrders.Where(rec => rec.ProjectName == SearchValue).ToList();

                }
                else if (SearchType == "ProjectType")
                {
                    objTbl = objdb.tblOrders.Where(rec => rec.ProjectType == SearchValue).ToList();

                }
                else if (SearchType == "ClientName")
                {
                    objTbl = objdb.tblOrders.Where(rec => rec.ClientName == SearchValue).ToList();

                }



                //objOrderModel.AssignOrder = objdb.OrderAssignments.Count(t => t.DesignerID != null);
                //objOrderModel.UnAssignOrder = objdb.tblOrders.Count(t => t.IsStatus == false);

                //objOrderModel.AssignOrder = objdb.tblOrders.Count(t => t.DesignerId != null && (t.IsAccepted == true || t.IsAccepted == null));

                //objOrderModel.UnAssignOrder = objdb.tblOrders.Count(t => t.DesignerId == null || t.IsAccepted == false);


                //var ProofStagerec = (from t1 in objdb.tblOrders
                //                     join t2 in objdb.OrderAssignments on t1.OrderNumber equals t2.OrderNumber
                //                     where t2.OrderStage == 3 && t1.IsAccepted == true
                //                     select new
                //                     {
                //                         t1.pkOrderId,
                //                     }).ToList();

                //objOrderModel.NewOrderCount = objdb.OrderAssignments.Count(t => t.OrderStage == 1);
                //objOrderModel.DesignStagerCount = objdb.OrderAssignments.Count(t => t.OrderStage == 2);
                //objOrderModel.ProofStageCount = ProofStagerec.Count();
                //objOrderModel.ApprovedCount = objdb.OrderAssignments.Count(t => t.OrderStage == 4);
                //objOrderModel.PrintingCount = objdb.OrderAssignments.Count(t => t.OrderStage == 5);
                //objOrderModel.CompletedCount = objdb.OrderAssignments.Count(t => t.OrderStage == 6);
                //objOrderModel.DesingerList = ddldesigner();
                //objOrderModel.ddlOrderList = ddlOrders();
                //objOrderModel.ContentFile = "fdsf";





            }
            if (objTbl != null)
            {
                foreach (var item in objTbl)
                {
                    objOrderModel.pkOrderId = item.pkOrderId;
                    objOrderModel.OrderNumber = item.OrderNumber;
                    objOrderModel.ClientName = item.ClientName;
                    objOrderModel.ProjectName = item.ProjectName;
                    objOrderModel.ProjectType = item.ProjectType;
                    objOrderModel.Phone = item.Phone;
                    objOrderModel.Email = item.Email;
                    objOrderModel.IsStatus = item.IsStatus;
                    objOrderModel.IsRejected = item.IsRejected;
                    objOrderModel.IsAccepted = item.IsAccepted;
                    objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                    objOrderModel.DesignerId = item.DesignerId;
                    // objOrderModel.CustomEmail= objdb.tbl

                    objOrderModel.DesingerList = ddldesigner();
                    objOrderModel.ListOrderProof = OrderProofListByNumber(Convert.ToInt32(item.OrderNumber));



                }
            }
            lst_OrderModel.Add(objOrderModel);
            return (lst_OrderModel);
        }


        [HttpPost]
        public ActionResult Edit(OrderModel orderInfo)
        {
            if (ModelState.IsValid)
            {
                List<OrderModel> item = GetOrderInfoView(orderInfo.pkOrderId);
                try
                {
                    if (item != null)
                    {
                        tblOrder objtbl = new tblOrder();

                        UpdateOrder(orderInfo);


                    }
                }
                catch (Exception)
                {

                    throw;
                }
                return View(GetOrderInfoView(orderInfo.pkOrderId));
            }
            else
            {
                return View(GetOrderInfoView(orderInfo.pkOrderId));
            }
        }

        public int UpdateOrder(OrderModel item)
        {
            int result = -1;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var objtbl = objdb.tblOrders.Where(rec => rec.pkOrderId == item.pkOrderId).FirstOrDefault();
                if (objtbl != null)
                {


                    objtbl.ClientName = item.ClientName;
                    objtbl.ProjectName = item.ProjectName;
                    objtbl.ProjectType = item.ProjectType;
                    objtbl.Phone = item.Phone;
                    objtbl.Email = item.Email;
                    objtbl.DesignerId = item.DesignerId;
                    objdb.SaveChanges();
                }
                result = -1;
            }
            return result;
        }

        [HttpPost]
        public ActionResult SelectProofforCustomer(int OrderId, string orderNumber)
        {
            string result = "Fail";
            int orderNumber1 = Convert.ToInt32(orderNumber);

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderProofs = objdb.tblOrderProofs.Where(REC => REC.OrderNumber == orderNumber1).ToList();
                if (tblOrderProofs.Count() > 0)
                {
                    foreach (var item in tblOrderProofs)
                    {
                        item.IsProofSelected = false;
                        objdb.SaveChanges();
                    }
                }
                var tblOrderProof1 = objdb.tblOrderProofs.Where(REC => REC.PkOrderProofId == OrderId).FirstOrDefault();

                if (tblOrderProof1 != null)
                {
                    tblOrderProof1.IsProofSelected = true;

                    objdb.SaveChanges();
                }

                return Json(new { result = result });

            }
        }



        [HttpPost]
        public ActionResult ReopenContentWriterJob(int OrderId)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == OrderId).FirstOrDefault();

                if (tblOrderItems != null)
                {
                    // Set Accepted
                    tblOrderItems.IsContentWriterJobClosed = false;



                    objdb.SaveChanges();
                }



                return Json(new { result = result });

            }
        }


        [HttpPost]
        public ActionResult UpdateOrderStage(string OrderNumber, int OrderStage)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                int pkOrderitemId = Convert.ToInt32(OrderNumber);
                var OrderAssignments = objdb.tblOrderItems.Where(rec => rec.ID == pkOrderitemId).FirstOrDefault();

                if (OrderAssignments != null)
                {
                    // Set Accepted
                    OrderAssignments.OrderStage = OrderStage;
                    objdb.SaveChanges();
                }

            }
            return Json(new { result = result });

        }

        public OrderModel GetItemInfobyId(int Id)
        {
            OrderModel objOrderModel = new OrderModel();
            List<tblOrderItem> objTbl = new List<tblOrderItem>();
            CommonController objCommonController1 = new CommonController();
            objOrderModel.DesingerList = ddldesigner();
            objOrderModel.ddlOrderList = ddlOrders();
            string OrderContents = string.Empty;

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objTbl = objdb.tblOrderItems.Where(rec => rec.ID == Id).ToList();
                if (objTbl.Count() > 0)
                {
                    int pkOrderId = objTbl.ElementAt(0).OrderID;
                    var tblOrders = objdb.tblOrders.Where(rec => rec.pkOrderId == pkOrderId).FirstOrDefault();
                    objOrderModel.AssignOrder = objdb.OrderAssignments.Count(t => t.DesignerID != null);
                    objOrderModel.UnAssignOrder = objdb.tblOrders.Count(t => t.IsStatus == false);
                     objCommonController1.GetLeftMenuContent(objOrderModel);
                    objOrderModel.DesingerList = ddldesigner();
                    objOrderModel.ddlOrderList = ddlOrders();
                    objOrderModel.ContentWriterList = GetWritterList();
                    objOrderModel.StageList = objdb.tblOrderStages.ToList();
                    objOrderModel.ProjectType = objTbl.ElementAt(0).ProjectType;
                    objOrderModel.ListOrderProof = OrderProofListByNumber(Convert.ToInt32(Id));
                    var PkOrderContent = objTbl.ElementAt(0).ID;
                    var tblOrderContents = objdb.tblOrderContents.Where(rec => rec.OrderNumber == PkOrderContent).FirstOrDefault();
                    if (tblOrderContents != null)
                    {
                        OrderContents = tblOrderContents.Files;
                    }



                    CommonController objCommonController = new CommonController();

                    if (objTbl != null)
                    {
                        foreach (var item in objTbl)
                        {
                            objOrderModel.GetOrderItems = objCommonController.GetOrderItemBypKOrderId(item.ID);
                            objOrderModel.OrderNumber = tblOrders.OrderNumber;
                            objOrderModel.DesingerList = ddldesigner();
                            objOrderModel.ContentWriterList = GetWritterList();
                            objOrderModel.ContentFile = OrderContents;


                        }
                    }
                }
            }
            return (objOrderModel);
        }

        public List<OrderModel> GetOrderInfoView(int Id)
        {
            List<OrderModel> lst_OrderModel = new List<OrderModel>();
            OrderModel objOrderModel = new OrderModel();
            List<tblOrder> objTbl = new List<tblOrder>();

            objOrderModel.DesingerList = ddldesigner();
            objOrderModel.ddlOrderList = ddlOrders();
            string OrderContents = string.Empty;

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objTbl = objdb.tblOrders.Where(rec => rec.pkOrderId == Id).ToList();
                CommonController objCommonController = new CommonController();

                objOrderModel.DesingerList = ddldesigner();
                objOrderModel.ddlOrderList = ddlOrders();
                objOrderModel.ContentWriterList = GetWritterList();
                int OrderNumber = Convert.ToInt32(objTbl.ElementAt(0).OrderNumber);
                var tblOrderContents = objdb.tblOrderContents.Where(rec => rec.OrderNumber == OrderNumber).FirstOrDefault();
                if (tblOrderContents != null)
                {
                    OrderContents = tblOrderContents.Files;
                }



                 objCommonController.GetLeftMenuContent(objOrderModel);
                if (objTbl != null)
                {
                    var OrderStage = "Open";
                    List<tblOrderItem> Orderitem = objdb.tblOrderItems.AsEnumerable().Where(rec => rec.OrderID == objTbl.ElementAt(0).pkOrderId).ToList();
                    var itemcount = 0;
                    foreach (var item in Orderitem)
                    {
                        if (item.OrderStage == 6)
                        {
                            itemcount++;
                        }
                    }
                    if (Orderitem.Count == itemcount)
                    {
                        OrderStage = "Closed";
                    }


                    foreach (var item in objTbl)
                    {
                        objOrderModel.pkOrderId = item.pkOrderId;
                        objOrderModel.OrderNumber = item.OrderNumber;
                        objOrderModel.ClientName = item.ClientName;
                        objOrderModel.ProjectName = item.ProjectName;
                        objOrderModel.ProjectType = item.ProjectType;
                        objOrderModel.Phone = item.Phone;
                        objOrderModel.Email = item.Email;
                        objOrderModel.IsStatus = item.IsStatus;
                        objOrderModel.IsRejected = item.IsRejected;
                        objOrderModel.OrderStatus = OrderStage;


                        objOrderModel.IsAccepted = item.IsAccepted;
                        objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                        objOrderModel.OrderNumber = item.OrderNumber;

                        objOrderModel.DesignerId = item.DesignerId;
                        objOrderModel.ContentFile = OrderContents;
                        objOrderModel.PkContentWritterId = item.fkContentWritterId;
                        objOrderModel.IsContentAssign = Convert.ToBoolean(item.IsContentAssign);
                        objOrderModel.DesingerList = ddldesigner();
                        objOrderModel.ListOrderProof = OrderProofListByNumber(Convert.ToInt32(item.OrderNumber));
                        objOrderModel.GetOrderItems = objCommonController.GetOrderItemByOrderId(item.pkOrderId);



                    }
                }
            }
            lst_OrderModel.Add(objOrderModel);
            return (lst_OrderModel);
        }

        public FileResult DownloadDesignerProofFile(string FileName)
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
        public ActionResult TakeJobBack(int ItemID = 0)
        {
            pEasyPrintEntities _context = new pEasyPrintEntities();
            tblOrderItem oItem = _context.tblOrderItems.Where(i => i.ID == ItemID).FirstOrDefault();
            tblDesigner designer = _context.tblDesigners.Where(i => i.PkDesignerId == oItem.DesignerId).FirstOrDefault();
            tblOrder order = _context.tblOrders.Where(i => i.pkOrderId == oItem.OrderID).FirstOrDefault();

            if (oItem != null)
            {
                oItem.DesignerId = null;
                oItem.IsItemAccepted = null;
                _context.SaveChanges();
                string emailbody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/JobBack.html")).ReadToEnd().ToString();
                NotificationEmail(designer.DesignerFirstName + " " + designer.DesigenerLastName, designer.EmailId, ConfigurationSettings.AppSettings["subjectTojobtookback"].ToString() + " " + DateTime.Now.ToString(), "designer", order.ProjectName, order.OrderNumber, emailbody);
                return Json(new { result = "done" });
            }
            else
            {
                return Json(new { result = "fail" });
            }

        }


        [HttpPost]
        public ActionResult TakeJobBackContent(int ItemID = 0)
        {
            pEasyPrintEntities _context = new pEasyPrintEntities();
            tblOrderItem oItem = _context.tblOrderItems.Where(i => i.ID == ItemID).FirstOrDefault();
            tblContentWriter designer = _context.tblContentWriters.Where(i => i.PkContentWritterId == oItem.fkContentWritterId).FirstOrDefault();
            tblOrder order = _context.tblOrders.Where(i => i.pkOrderId == oItem.OrderID).FirstOrDefault();

            if (oItem != null)
            {
                oItem.DesignerId = null;
                oItem.IsItemAccepted = null;
                _context.SaveChanges();
                string emailbody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/JobBack.html")).ReadToEnd().ToString();
                NotificationEmail(designer.FirstName + " " + designer.LastName, designer.EmailId, ConfigurationSettings.AppSettings["subjectTojobtookback"].ToString() + " " + DateTime.Now.ToString(), "ContentWriter", order.ProjectName, order.OrderNumber, emailbody);
                return Json(new { result = "done" });
            }
            else
            {
                return Json(new { result = "fail" });
            }

        }


        [HttpPost]
        public ActionResult AssignDesignerWithFile(IEnumerable<HttpPostedFileBase> files)
        {

            int PkOrderID = Convert.ToInt32(Request["ID"]);

            try
            {
                string fileName = string.Empty;
                string Uploadedfile = string.Empty;
                string BackfileName = string.Empty;


                string OrderNumber = Request["OrderNumber"];
                int DesignerId = Convert.ToInt32(Request["DesignerId"]);


                string Comment = Request["txtAdminComment"];


                HttpFileCollectionBase file = Request.Files;



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

                                fileName = OrderNumber + "_" + "front_" + UidNumber + "_" + FileName;
                                Uploadedfile = Uploadedfile + "," + fileName;
                                item2.SaveAs(Server.MapPath("~/Images/proofUpload/" + fileName));
                                counter++;
                            }
                        }
                    }


                    AssignDesigner(PkOrderID, DesignerId, OrderNumber, Comment, Uploadedfile);



                    //   SaveProofInDb(proofComment, BackComments, FrontfileName, BackfileName, PkOrderID);
                    // string EmailAddress = GetEmailAddress(PkOrderID);

                    // SendCustomerNotificationEmail(EmailAddress, files, files2);


                }
                return RedirectToAction("item", "Order", new { id = PkOrderID });

            }
            catch (Exception)
            {

                return RedirectToAction("item", "Order", new { id = PkOrderID });
            }



        }

        public FileResult DownloadAdminProofFile(string FileName)
        {
            string txt = FileName;
            char[] splitedcBy = new char[] { '.' };

            char[] splitedcBy2 = new char[] { '_' };
            var filenamw = FileName.Split(splitedcBy2);
            var ext = FileName.Split(splitedcBy);
            string finalName = filenamw[2] + "." + ext[2];
            return File(Server.MapPath("~/Images/proofUpload/" + FileName), System.Net.Mime.MediaTypeNames.Application.Octet, finalName);
        }

        public ActionResult AssignDesigner(int OrderId, int DesignerId, string OrderNumber, string Comments, string Uploadedfile)
        {
            string result = "Fail";
            string type = "designer";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                int OrderItemId = 0;
                var tblOrderItem = objdb.tblOrderItems.Where(rec => rec.ID == OrderId).FirstOrDefault();
                var tblOrder = objdb.tblOrders.Where(rec => rec.pkOrderId == tblOrderItem.OrderID).FirstOrDefault();

                var designer = objdb.tblDesigners.Where(p => p.PkDesignerId == DesignerId).FirstOrDefault();
                if (tblOrderItem != null)
                {
                    if (tblOrderItem.OrderStage < 1)
                    {
                        tblOrderItem.OrderStage = 1;
                    }
                    OrderItemId = tblOrderItem.ID;
                    tblOrderItem.DesignerId = Convert.ToInt16(DesignerId);
                    tblOrderItem.Comments = tblOrderItem.Comments + " ," + Comments;
                    tblOrderItem.IsItemStatus = true;
                    tblOrderItem.IsItemAccepted = null;
                    tblOrderItem.AssignDate = DateTime.Now;
                    tblOrderItem.AdminAssignFile = tblOrderItem.AdminAssignFile + Uploadedfile;

                    objdb.SaveChanges();
                    string emailbody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/JobAssigned.html")).ReadToEnd().ToString();
                    NotificationEmail(designer.DesignerFirstName + " " + designer.DesigenerLastName, designer.EmailId, ConfigurationSettings.AppSettings["subjectToDesigner"].ToString() + " " + DateTime.Now.ToString(), type, tblOrder.ProjectName, "", emailbody);
                    result = "Done";
                }

            }
            return Json(new { result = result });

        }


        public FileResult DownloadFiles(string ImageName)
        {
            return File(Server.MapPath("~/Images/proofUpload/" + ImageName), System.Net.Mime.MediaTypeNames.Application.Octet);
        }




        public List<tblOrderProof> OrderProofListByNumber(int? OrderNumber)
        {
            List<tblOrderProof> objtblOrderProof = new List<tblOrderProof>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblOrderProof = objdb.tblOrderProofs.Where(rec => rec.OrderNumber == OrderNumber).ToList();
            }
            return objtblOrderProof;
        }
        public List<OrderModel> GetOrderByDesigner(int DesignerId)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == DesignerId).ToList();
            }

            foreach (var item in ObjtblOrderList1)
            {
                OrderModel objOrderModel = new OrderModel();
                objOrderModel.pkOrderId = item.pkOrderId;
                objOrderModel.OrderNumber = item.OrderNumber;
                objOrderModel.ClientName = item.ClientName;
                objOrderModel.ProjectName = item.ProjectName;
                objOrderModel.ProjectType = item.ProjectType;
                objOrderModel.Phone = item.Phone;
                objOrderModel.Email = item.Email;
                objOrderModel.IsStatus = item.IsStatus;
                objOrderModel.IsRejected = item.IsRejected;
                objOrderModel.IsAccepted = item.IsAccepted;
                objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                objOrderModel.DesignerId = item.DesignerId;
                objOrderModel.CategoryType = "Order In Design Stage";

                objOrderModel.DesingerList = ddldesigner();
                objCommonController.GetLeftMenuContent(objOrderModel);
                ObjtblOrderList.Add(objOrderModel);
            }
            return ObjtblOrderList;
        }

        public List<OrderModel> GetOrderByOrderStage(int? OrderStage)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();
            var CategoryType = "";
            OrderModel objOrderModel1 = new OrderModel();
            objCommonController.GetLeftMenuContent(objOrderModel1);
            ObjtblOrderList.Add(objOrderModel1);
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                CategoryType = objdb.tblOrderStages.Where(rec => rec.PkStageId == OrderStage).Select(rec => rec.StageName).FirstOrDefault();
                if (OrderStage == 0)
                {
                    var tblOrderList = (from t1 in objdb.tblOrders
                                        join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                        where t2.OrderStage == 1 && t2.ProjectType.ToLower() != "printing job"
                                        select new
                                        {
                                            t1.pkOrderId,
                                            t1.OrderNumber,
                                            t1.ClientName,
                                            t2.ProjectType,
                                            t1.Phone,
                                            t1.Email,
                                            t1.OrderDate
                                        }).Distinct().ToList();

                    if (tblOrderList.Count > 0)
                    {


                        foreach (var item in tblOrderList)
                        {
                            tblOrder objtblOrder = new tblOrder();
                            objtblOrder.pkOrderId = item.pkOrderId;
                            objtblOrder.OrderNumber = item.OrderNumber;
                            objtblOrder.ClientName = item.ClientName;
                            objtblOrder.ProjectType = item.ProjectType;
                            objtblOrder.Phone = item.Phone;
                            objtblOrder.Email = item.Email;
                            objtblOrder.OrderDate = item.OrderDate;
                            ObjtblOrderList1.Add(objtblOrder);

                        }
                    }


                    //objCommonController.GetLeftMenuContent(ordOrderModelLeftMenu);

                    //ObjtblOrderList.Add(ordOrderModelLeftMenu);




                    if (ObjtblOrderList1.Count() >= 1)
                    {
                        ObjtblOrderList = new List<OrderModel>();
                        foreach (var item in ObjtblOrderList1)
                        {
                            OrderModel objOrderModel = new OrderModel();
                            objOrderModel.pkOrderId = item.pkOrderId;
                            objOrderModel.OrderNumber1 = Convert.ToInt32(item.OrderNumber);
                            objOrderModel.OrderNumber = item.OrderNumber;
                            objOrderModel.ClientName = item.ClientName;
                            objOrderModel.ProjectType = item.ProjectType;
                            objOrderModel.Phone = item.Phone;
                            objOrderModel.Email = item.Email;
                            objOrderModel.DesignerId = 0;
                            objOrderModel.DesignerID = 0;
                            objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                            objOrderModel.CategoryType = "Unassigned Order";
                            objCommonController.GetLeftMenuContent(objOrderModel);

                            ObjtblOrderList.Add(objOrderModel);
                        }
                    }
                }
                else if (OrderStage == 1)
                {
                    var assign = (from t1 in objdb.tblOrders
                                  join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                  where t2.OrderStage == 1 && t2.DesignerId != null && t2.OrderStage != 6
                                  select new
                                  {
                                      t1.pkOrderId,
                                      t1.OrderNumber,
                                      t2.ProjectName,
                                      t1.ClientName,
                                      t1.ProjectType,
                                      t1.Phone,
                                      t1.Email,
                                      t1.IsStatus,
                                      t1.IsRejected,
                                      t1.IsAccepted,
                                      t1.OrderDate,
                                      t1.DesignerId,
                                  }).GroupBy(x => new
                                  {
                                      x.pkOrderId,
                                      x.OrderNumber,
                                      x.ProjectName,
                                      x.ClientName,
                                      x.ProjectType,
                                      x.Phone,
                                      x.Email,
                                      x.IsStatus,
                                      x.IsRejected,
                                      x.IsAccepted,
                                      x.OrderDate,
                                      x.DesignerId



                                  }
                                           ).ToList();

                    if (assign.Count > 0)
                    {
                        ObjtblOrderList = new List<OrderModel>();

                        foreach (var item in assign)
                        {
                            OrderModel objOrderModel = new OrderModel();
                            objOrderModel.pkOrderId = item.ElementAt(0).pkOrderId;
                            objOrderModel.OrderNumber = item.ElementAt(0).OrderNumber;
                            objOrderModel.ClientName = item.ElementAt(0).ClientName;
                            objOrderModel.ProjectName = item.ElementAt(0).ProjectName;
                            objOrderModel.ProjectType = item.ElementAt(0).ProjectType;
                            objOrderModel.Phone = item.ElementAt(0).Phone;
                            objOrderModel.Email = item.ElementAt(0).Email;
                            objOrderModel.IsStatus = item.ElementAt(0).IsStatus;
                            objOrderModel.IsRejected = item.ElementAt(0).IsRejected;
                            objOrderModel.IsAccepted = item.ElementAt(0).IsAccepted;
                            objOrderModel.OrderDate = Convert.ToDateTime(item.ElementAt(0).OrderDate);
                            objOrderModel.DesignerId = item.ElementAt(0).DesignerId;
                            objOrderModel.CategoryType = "Assign Order";

                            objOrderModel.DesingerList = ddldesigner();
                             objCommonController.GetLeftMenuContent(objOrderModel);
                            ObjtblOrderList.Add(objOrderModel);
                        }
                    }
                }


                else if (OrderStage == 7)
                {
                    var tblOrderList = (from t1 in objdb.tblOrders
                                        join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                        where t2.OrderStage == 1 && t2.ProjectType.ToLower() == "printing job"
                                        select new
                                        {
                                            t1.pkOrderId,
                                            t1.OrderNumber,
                                            t1.ClientName,
                                            t2.ProjectType,
                                            t1.Phone,
                                            t1.Email,
                                            t1.OrderDate
                                        }).Distinct().ToList();

                    if (tblOrderList.Count > 0)
                    {


                        foreach (var item in tblOrderList)
                        {
                            tblOrder objtblOrder = new tblOrder();
                            objtblOrder.pkOrderId = item.pkOrderId;
                            objtblOrder.OrderNumber = item.OrderNumber;
                            objtblOrder.ClientName = item.ClientName;
                            objtblOrder.ProjectType = item.ProjectType;
                            objtblOrder.Phone = item.Phone;
                            objtblOrder.Email = item.Email;
                            objtblOrder.OrderDate = item.OrderDate;
                            ObjtblOrderList1.Add(objtblOrder);

                        }
                    }


                   // objCommonController.GetLeftMenuContent(ordOrderModelLeftMenu);

                   // ObjtblOrderList.Add(ordOrderModelLeftMenu);




                    if (ObjtblOrderList1.Count() >= 1)
                    {
                        ObjtblOrderList = new List<OrderModel>();
                        foreach (var item in ObjtblOrderList1)
                        {
                            OrderModel objOrderModel = new OrderModel();
                            objOrderModel.pkOrderId = item.pkOrderId;
                            objOrderModel.OrderNumber1 = Convert.ToInt32(item.OrderNumber);
                            objOrderModel.OrderNumber = item.OrderNumber;
                            objOrderModel.ClientName = item.ClientName;
                            objOrderModel.ProjectType = item.ProjectType;
                            objOrderModel.Phone = item.Phone;
                            objOrderModel.Email = item.Email;
                            objOrderModel.DesignerId = 0;
                            objOrderModel.DesignerID = 0;
                            objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                            objOrderModel.CategoryType = "Unassigned Order";
                            objCommonController.GetLeftMenuContent(objOrderModel);

                            ObjtblOrderList.Add(objOrderModel);
                        }
                    }
                }



                else
                {
                    var AssignOrder = (from t1 in objdb.tblOrders
                                       join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                       where t2.OrderStage == OrderStage
                                       select new
                                       {
                                           t1.pkOrderId,
                                           t1.OrderNumber,
                                           t2.ProjectName,
                                           t1.ClientName,
                                           t1.ProjectType,
                                           t1.Phone,
                                           t1.Email,
                                           t1.IsStatus,
                                           t1.IsRejected,
                                           t1.IsAccepted,
                                           t1.OrderDate,
                                           t1.DesignerId,
                                       }).GroupBy(x => new
                                       {
                                           x.pkOrderId,
                                           x.OrderNumber,
                                           x.ProjectName,
                                           x.ClientName,
                                           x.ProjectType,
                                           x.Phone,
                                           x.Email,
                                           x.IsStatus,
                                           x.IsRejected,
                                           x.IsAccepted,
                                           x.OrderDate,
                                           x.DesignerId



                                       }
                                           ).ToList();

                    if (AssignOrder.Count > 0)
                    {
                        ObjtblOrderList = new List<OrderModel>();
                        foreach (var item in AssignOrder)
                        {
                            OrderModel objOrderModel = new OrderModel();
                            objOrderModel.pkOrderId = item.ElementAt(0).pkOrderId;
                            objOrderModel.OrderNumber = item.ElementAt(0).OrderNumber;
                            objOrderModel.OrderNumber1 = Convert.ToInt32(item.ElementAt(0).OrderNumber);


                            objOrderModel.ClientName = item.ElementAt(0).ClientName;
                            objOrderModel.ProjectName = item.ElementAt(0).ProjectName;
                            objOrderModel.ProjectType = item.ElementAt(0).ProjectType;
                            objOrderModel.Phone = item.ElementAt(0).Phone;
                            objOrderModel.Email = item.ElementAt(0).Email;
                            objOrderModel.IsStatus = item.ElementAt(0).IsStatus;
                            objOrderModel.IsRejected = item.ElementAt(0).IsRejected;
                            objOrderModel.IsAccepted = item.ElementAt(0).IsAccepted;
                            objOrderModel.OrderDate = Convert.ToDateTime(item.ElementAt(0).OrderDate);
                            objOrderModel.DesignerId = item.ElementAt(0).DesignerId;
                            objOrderModel.CategoryType = objCommonController.GetStageNamebyId(OrderStage.ToString());
                            objOrderModel.DesingerList = ddldesigner();
                            objCommonController.GetLeftMenuContent(objOrderModel);
                            ObjtblOrderList.Add(objOrderModel);
                        }
                    }
                }





            }


            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber1).ToList();
        }

        public List<OrderModel> GetOrderByOrderEmail(string OrderEmail)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.Email == OrderEmail).ToList();
            }

            foreach (var item in ObjtblOrderList1)
            {
                OrderModel objOrderModel = new OrderModel();
                objOrderModel.pkOrderId = item.pkOrderId;
                objOrderModel.OrderNumber = item.OrderNumber;
                objOrderModel.ClientName = item.ClientName;
                objOrderModel.ProjectName = item.ProjectName;
                objOrderModel.ProjectType = item.ProjectType;
                objOrderModel.Phone = item.Phone;
                objOrderModel.Email = item.Email;
                objOrderModel.IsStatus = item.IsStatus;
                objOrderModel.IsRejected = item.IsRejected;
                objOrderModel.IsAccepted = item.IsAccepted;
                objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                objOrderModel.DesignerId = item.DesignerId;
                objOrderModel.CategoryType = "Orders By Email";

                objOrderModel.DesingerList = ddldesigner();
                objCommonController.GetLeftMenuContent(objOrderModel);
                ObjtblOrderList.Add(objOrderModel);
            }
            return ObjtblOrderList;
        }

        public List<OrderModel> GetOrderByOrderNumber(int pkOrderId)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.pkOrderId == pkOrderId).ToList();
            }

            foreach (var item in ObjtblOrderList1)
            {
                OrderModel objOrderModel = new OrderModel();
                objOrderModel.pkOrderId = item.pkOrderId;
                objOrderModel.OrderNumber = item.OrderNumber;
                objOrderModel.ClientName = item.ClientName;
                objOrderModel.ProjectName = item.ProjectName;
                objOrderModel.ProjectType = item.ProjectType;
                objOrderModel.Phone = item.Phone;
                objOrderModel.Email = item.Email;
                objOrderModel.IsStatus = item.IsStatus;
                objOrderModel.IsRejected = item.IsRejected;
                objOrderModel.IsAccepted = item.IsAccepted;
                objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                objOrderModel.DesignerId = item.DesignerId;
                objOrderModel.CategoryType = "Orders By Order Number";

                objOrderModel.DesingerList = ddldesigner();
                objCommonController.GetLeftMenuContent(objOrderModel);
                ObjtblOrderList.Add(objOrderModel);
            }
            return ObjtblOrderList;
        }
        public List<OrderModel> GetOrderByDate(DateTime fromdate, DateTime todate)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                ObjtblOrderList1 = objdb.tblOrders.AsEnumerable().Where(rec => Convert.ToDateTime(rec.OrderDate) >= fromdate && Convert.ToDateTime(rec.OrderDate) <= todate).ToList();

            }
            CommonController objCommonfn = new CommonController();

            foreach (var item in ObjtblOrderList1)
            {
                OrderModel objOrderModel = new OrderModel();
                objOrderModel.pkOrderId = item.pkOrderId;
                objOrderModel.OrderNumber = item.OrderNumber;
                objOrderModel.OrderNumber1 = Convert.ToInt32(item.OrderNumber);
                objOrderModel.ClientName = item.ClientName;
                objOrderModel.ProjectName = objCommonfn.GetProjectNamebyID(item.pkOrderId);
                objOrderModel.ProjectType = item.ProjectType;
                objOrderModel.Phone = item.Phone;
                objOrderModel.Email = item.Email;
                objOrderModel.IsStatus = item.IsStatus;
                objOrderModel.IsRejected = item.IsRejected;
                objOrderModel.IsAccepted = item.IsAccepted;
                objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                objOrderModel.DesignerId = item.DesignerId;
                objOrderModel.CategoryType = "Orders By Dates";
                objOrderModel.DesingerList = ddldesigner();

                ObjtblOrderList.Add(objOrderModel);
            }
            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber1).ToList();
        }






        public List<OrderModel> GetOrderList()
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();
            OrderModel ordOrderModelLeftMenu = new OrderModel();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                //ObjtblOrderList1 = objdb.tblOrders.Where(i => i.ProjectType.ToLower() != "printing job").ToList();

                var tblOrderList = (from t1 in objdb.tblOrders
                                    join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                    where t2.OrderStage == 1 && t2.ProjectType.ToLower() != "printing job"
                                    select new
                                    {
                                        t1.pkOrderId,
                                        t1.OrderNumber,
                                        t1.ClientName,
                                        t2.ProjectType,
                                        t1.Phone,
                                        t1.Email,
                                        t1.OrderDate
                                    }).Distinct().ToList();

                if (tblOrderList.Count > 0)
                {


                    foreach (var item in tblOrderList)
                    {
                        tblOrder objtblOrder = new tblOrder();
                        objtblOrder.pkOrderId = item.pkOrderId;
                        objtblOrder.OrderNumber = item.OrderNumber;
                        objtblOrder.ClientName = item.ClientName;
                        objtblOrder.ProjectType = item.ProjectType;
                        objtblOrder.Phone = item.Phone;
                        objtblOrder.Email = item.Email;
                        objtblOrder.OrderDate = item.OrderDate;
                        ObjtblOrderList1.Add(objtblOrder);

                    }
                }


                objCommonController.GetLeftMenuContent(ordOrderModelLeftMenu);

                ObjtblOrderList.Add(ordOrderModelLeftMenu);
            }


            ObjtblOrderList = new List<OrderModel>();
            if (ObjtblOrderList1.Count() >= 1)
            {
                foreach (var item in ObjtblOrderList1)
                {
                    OrderModel objOrderModel = new OrderModel();
                    objOrderModel.pkOrderId = item.pkOrderId;
                    objOrderModel.OrderNumber1 = Convert.ToInt32(item.OrderNumber);
                    objOrderModel.OrderNumber = item.OrderNumber;
                    objOrderModel.ClientName = item.ClientName;
                    objOrderModel.ProjectType = item.ProjectType;
                    objOrderModel.Phone = item.Phone;
                    objOrderModel.Email = item.Email;
                    objOrderModel.DesignerId = 0;
                    objOrderModel.DesignerID = 0;
                    objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                    objOrderModel.CategoryType = "Unassigned Order";
                    objCommonController.GetLeftMenuContent(objOrderModel);

                    ObjtblOrderList.Add(objOrderModel);
                }
            }
            else
            {
                OrderModel objOrderModel = new OrderModel();
                objOrderModel.CategoryType = "Unassigned Order";
                ObjtblOrderList.Add(objOrderModel);

            }
            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber1).ToList();
        }

        public List<ConentWritterList> GetWritterList()
        {
            List<ConentWritterList> ListOfWriter = new List<ConentWritterList>();
            List<tblContentWriter> objtblContentWriters = new List<tblContentWriter>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblContentWriters = objdb.tblContentWriters.Where(i => i.IsActive == true).ToList();
            }
            foreach (var item in objtblContentWriters)
            {
                ConentWritterList WritterList = new ConentWritterList();
                WritterList.pkWriterId = item.PkContentWritterId;

                WritterList.WriterName = item.FirstName;

                ListOfWriter.Add(WritterList);
            }
            return ListOfWriter;
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

                objOrderList.OrderNumber = item.OrderNumber.ToString();
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
                objdesignerr = objdb.tblDesigners.Where(i => i.IsActive == true).ToList();
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


        private void NotificationEmail(string user, string eMailAddress, string subject, string type, string ProjectName, string ordernumber, string eMailBody)
        {
            try
            {
                SmtpClient mySmtpClient1 = new SmtpClient("smtp.gmail.com");
                SmtpClient mySmtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpClient"].ToString());
                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                mySmtpClient.EnableSsl = true;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(ConfigurationSettings.AppSettings["fromAdd"].ToString(), ConfigurationSettings.AppSettings["password"].ToString());
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(ConfigurationSettings.AppSettings["fromAdd"].ToString(), "PeasyPrint Designs");
                MailAddress to = new MailAddress(eMailAddress);

                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = subject;
               

                myMail.SubjectEncoding = System.Text.Encoding.UTF8;
                string project = type == "designer" ? "design " : "content";
                // set body-message and encoding

                var emailBody = eMailBody;
                var emailTemplate = emailBody;
                string email = emailTemplate
                  .Replace("##DesignerName##", user)
                 .Replace("##ProjectName##", ProjectName)
                 .Replace("##OrderNumber##", ordernumber);


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



        public tblContentWriter GetContenWriterById(int PkContentWritterId)
        {
            tblContentWriter objtbl = new tblContentWriter();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtbl = objdb.tblContentWriters.Where(rec => rec.PkContentWritterId == PkContentWritterId).FirstOrDefault();
            }
            return objtbl;
        }


        [HttpGet]
        public ActionResult CustomOrder()
        {

            OrderItemModel OrderItem = new OrderItemModel();

            return View(OrderItem);
        }


        [HttpPost]
        public ActionResult CustomOrder(OrderItemModel viewmodel, IEnumerable<HttpPostedFileBase> files)
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            Random rnd = new Random();
            int myRandomNo = rnd.Next(200000000, 999999999);
            tblOrder Order = new tblOrder();
            Order.ProjectName = viewmodel.ProjectName;
            Order.OrderNumber = myRandomNo.ToString();
            Order.ClientName = viewmodel.ClientName;
            Order.Phone = viewmodel.Phone;
            Order.Email = viewmodel.Email;
            Order.ProjectType = "Design Job";
            Order.OrderDate = DateTime.Now.ToString();
            Order.BusinessName = viewmodel.BusinessName;
            Order.IsCustomeOrder = true;
            objdb.tblOrders.Add(Order);
            objdb.SaveChanges();

            tblShippingAddress Shipping = new tblShippingAddress();

            Shipping.OrderID = Order.pkOrderId;
            Shipping.Address = viewmodel.ShippingAddress1;
            Shipping.City = viewmodel.ShippingCity;
            Shipping.State = viewmodel.ShippingState;
            Shipping.Country = viewmodel.ShippingCountry;
            Shipping.Zip = viewmodel.ShippingZip;
            objdb.tblShippingAddresses.Add(Shipping);
            objdb.SaveChanges();


            tblOrderItem OrderItem = new tblOrderItem();

            if (Request.Files.Count > 0)
            {
                string fileName = string.Empty;
                var rootDir = Server.MapPath("~/Images/CustomFiles/");
                int counter = 1;
                if (files != null)
                {
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string ext = Path.GetExtension(file.FileName);

                            var Dirpath = Path.Combine(rootDir);
                            if (!Directory.Exists(Dirpath))
                            {
                                Directory.CreateDirectory(Dirpath);
                            }

                            DateTime thisDate1 = DateTime.Now;

                            fileName = FileName + "-" + Order.OrderNumber + ext;
                            if (counter == 1)
                            {
                                OrderItem.CustomFile1 = "~/Images/CustomFiles/" + fileName;
                                file.SaveAs(Server.MapPath("~/Images/CustomFiles/") + fileName);
                            }
                            else if (counter == 2)
                            {
                                OrderItem.CustomFile2 = "~/Images/CustomFiles/" + fileName;
                                file.SaveAs(Server.MapPath("~/Images/CustomFiles/") + fileName);
                            }
                            else
                            {
                                OrderItem.CustomFile3 = "~/Images/CustomFiles/" + fileName;
                                file.SaveAs(Server.MapPath("~/Images/CustomFiles/") + fileName);
                            }

                            counter++;
                        }
                    }
                }
            }

            OrderItem.OrderID = Order.pkOrderId;
            OrderItem.ProjectName = viewmodel.ProjectName;
            OrderItem.ClientName = viewmodel.ClientName;
            OrderItem.Phone = viewmodel.Phone;
            OrderItem.Email = viewmodel.Email;
            OrderItem.ItemName = viewmodel.ItemName;
            OrderItem.size = viewmodel.size;
            OrderItem.color = viewmodel.color;
            OrderItem.paper = viewmodel.paper;
            OrderItem.quantity = viewmodel.quantity;
            OrderItem.qty = viewmodel.quantity;
            OrderItem.UVCoating = viewmodel.UVCoating == true ? "Yes" : "No";
            OrderItem.Panel = viewmodel.Panel;
            OrderItem.generalInstruction = viewmodel.generalInstruction;
            OrderItem.Binding = viewmodel.Binding;
            OrderItem.Cover = viewmodel.Cover;
            OrderItem.ProjectType = "Design Job";
            OrderItem.DesignInstructionsFront = viewmodel.DesignInstructionsFront;
            OrderItem.ItemFrontComment = viewmodel.ItemFrontComment;
            OrderItem.DesignInstructionsBack = viewmodel.DesignInstructionsBack;
            OrderItem.ItemBackComment = viewmodel.ItemBackComment;
            OrderItem.CreatedOn = DateTime.Now.ToString();
            OrderItem.IsContentAssign = false;
            OrderItem.OrderStage = 1;
            objdb.tblOrderItems.Add(OrderItem);
            objdb.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
