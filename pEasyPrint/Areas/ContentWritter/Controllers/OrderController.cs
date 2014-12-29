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
using System.IO;
using System.Text.RegularExpressions;
using pEasyPrint.Controllers;
using pEasyPrint.Models;
using System.Net.Mail;
using System.Configuration;

namespace pEasyPrint.Areas.ContentWritter.Controllers
{
     [Authorize(Roles = "ContentWriter")]
    public class OrderController : Controller
    {
        //
        // GET: /ContentWritter/Order/

        public ActionResult Index(int? page, int? pagesize, string SearchType, string SearchValue, string SearchBy, string OrderStage)
        {
            int PageSize = pagesize == null ? 20 : Convert.ToInt32(pagesize);
            OrderModel objModel = new OrderModel();
            //objModel.OrderList = GetOrderList();
            int CurrentDesignerID = WebSecurity.CurrentUserId;
            List<OrderModel> orders = GetOrderListByContentWritterId(CurrentDesignerID);
            if (SearchBy == "Designer")
            {
                orders = GetOrderListByEmail(CurrentDesignerID, SearchType, SearchValue);
            }
            else if (!string.IsNullOrEmpty(OrderStage))
            {
                orders = GetOrderListByOrderStage(CurrentDesignerID, Convert.ToInt32(OrderStage));
            }
            if (orders != null)
            {
                IPagination pagedModel = orders.AsPagination(page ?? 1, PageSize);

                return View(pagedModel);
            }
            else
            {
                orders = new List<OrderModel>();
                IPagination pagedModel = orders.AsPagination(page ?? 1, PageSize);

                return View(pagedModel);
            }
        }





        [HttpPost]
        public ActionResult AcceptFullOrder(int id)
        {
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.OrderID == id).ToList();
                if (tblOrderItems != null)
                {
                    foreach (tblOrderItem item in tblOrderItems)
                    {
                        item.IsContentWriterAccepted = true;
                        item.fkContentWritterId = WebSecurity.CurrentUserId;
                        objdb.SaveChanges();
                    }
                }
            }
            return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reports(int? page)
        {
            OrderModel objModel = new OrderModel();
            //objModel.OrderList = GetOrderList();
            int CurrentDesignerID = WebSecurity.CurrentUserId;
            List<OrderModel> orders = GetOrderListByContentWritterId(CurrentDesignerID);
           
            if (orders != null)
            {
                IPagination pagedModel = orders.AsPagination(page ?? 1, 10);

                return View(pagedModel);
            }
            else
            {
                orders = new List<OrderModel>();
                IPagination pagedModel = orders.AsPagination(page ?? 1, 10);

                return View(pagedModel);
            }
        }


        public List<OrderModel> GetOrderListByOrderStage(int CurrentDesignerID, int? OrderStage)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();
            OrderAssignment ordAssignment = new OrderAssignment();
            int CurrentUserID = WebSecurity.CurrentUserId;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                if (OrderStage == 1)
                {
                    #region Content Writter Fillter
                                       
                    var list = (from t1 in objdb.tblOrders
                                join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                where t2.fkContentWritterId == CurrentUserID && t2.IsContentWriterAccepted != true && t2.OrderStage != 6
                                select new
                                {
                                    t1.pkOrderId,
                                    t1.OrderNumber,
                                    t1.ProjectName,
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
                        OrderModel objOrderModel = new OrderModel();
                        GetleftMenu(objOrderModel, CurrentUserID);
                        objOrderModel.pkOrderId = item.pkOrderId;
                        objOrderModel.OrderNumber = item.OrderNumber.ToString();
                        objOrderModel.ClientName = item.ClientName;
                        objOrderModel.ProjectName = item.ProjectName;
                        objOrderModel.ProjectType = item.ProjectType;
                        objOrderModel.Phone = item.Phone;
                        objOrderModel.Email = item.Email;
                        objOrderModel.IsStatus = item.IsStatus;
                        objOrderModel.IsAccepted = item.IsAccepted;
                        objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                        objOrderModel.DesignerId = item.DesignerId;

                        objOrderModel.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                        ObjtblOrderList.Add(objOrderModel);
                    }

                    #endregion



                }
                else if(OrderStage ==2)
                {
                    #region Content Writter Fillter

                    var list = (from t1 in objdb.tblOrders
                                join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                where t2.fkContentWritterId == CurrentUserID && t2.IsContentWriterAccepted == true && t2.IsContentWriterJobClosed != true && t2.OrderStage != 6
                                select new
                                {
                                    t1.pkOrderId,
                                    t1.OrderNumber,
                                    t1.ProjectName,
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
                        OrderModel objOrderModel = new OrderModel();
                        GetleftMenu(objOrderModel, CurrentUserID);
                        objOrderModel.pkOrderId = item.pkOrderId;
                        objOrderModel.OrderNumber = item.OrderNumber.ToString();
                        objOrderModel.ClientName = item.ClientName;
                        objOrderModel.ProjectName = item.ProjectName;
                        objOrderModel.ProjectType = item.ProjectType;
                        objOrderModel.Phone = item.Phone;
                        objOrderModel.Email = item.Email;
                        objOrderModel.IsStatus = item.IsStatus;
                        objOrderModel.IsAccepted = item.IsAccepted;
                        objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                        objOrderModel.DesignerId = item.DesignerId;

                        objOrderModel.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                        ObjtblOrderList.Add(objOrderModel);
                    }

                    #endregion
                }
                else if (OrderStage == 3)
                {
                    #region Content Writter Fillter

                    var list = (from t1 in objdb.tblOrders
                                join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                where t2.fkContentWritterId == CurrentUserID && t2.IsContentWriterJobClosed == true && t2.OrderStage != 6
                                select new
                                {
                                    t1.pkOrderId,
                                    t1.OrderNumber,
                                    t1.ProjectName,
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
                        OrderModel objOrderModel = new OrderModel();
                        GetleftMenu(objOrderModel, CurrentUserID);
                        objOrderModel.pkOrderId = item.pkOrderId;
                        objOrderModel.OrderNumber = item.OrderNumber.ToString();
                        objOrderModel.ClientName = item.ClientName;
                        objOrderModel.ProjectName = item.ProjectName;
                        objOrderModel.ProjectType = item.ProjectType;
                        objOrderModel.Phone = item.Phone;
                        objOrderModel.Email = item.Email;
                        objOrderModel.IsStatus = item.IsStatus;
                        objOrderModel.IsAccepted = item.IsAccepted;
                        objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                        objOrderModel.DesignerId = item.DesignerId;

                        objOrderModel.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                        ObjtblOrderList.Add(objOrderModel);
                    }

                    #endregion
                }





            }


            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber).ToList(); ;
        }


        public List<OrderModel> GetOrderListByEmail(int CurrentUserID, string SearchType, string SearchValue)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();
            OrderAssignment ordAssignment = new OrderAssignment();

            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.fkContentWritterId == CurrentUserID && rec.Email == SearchValue).ToList();
                #region Left Menu Data
                tblDesigner designer = new tblDesigner();
                OrderModel objOrderModelleftmenu = new OrderModel();
                GetleftMenu(objOrderModelleftmenu, CurrentUserID);
                ObjtblOrderList.Add(objOrderModelleftmenu);
                #endregion

                if (SearchType == "DesignerId")
                {

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t2.fkContentWritterId == CurrentUserID  && t2.OrderStage != 6
                                               select new
                                               {
                                                   t1.pkOrderId,
                                                   t1.OrderNumber,
                                                   t1.ProjectName,
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

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.pkOrderId;
                        objtbl.OrderNumber = item.OrderNumber;
                        objtbl.ProjectName = item.ProjectName;
                        objtbl.ClientName = item.ClientName;
                        objtbl.ProjectType = item.ProjectType;
                        objtbl.Phone = item.Phone;
                        objtbl.Email = item.Email;
                        objtbl.IsStatus = item.IsStatus;
                        objtbl.IsRejected = item.IsRejected;
                        objtbl.IsAccepted = item.IsAccepted;
                        objtbl.OrderDate = item.OrderDate;
                        objtbl.DesignerId = item.DesignerId;



                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion



                }
                else if (SearchType == "OrderNumber")
                {

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t2.fkContentWritterId == CurrentUserID && t1.OrderNumber == SearchValue && t2.OrderStage != 6
                                               select new
                                               {
                                                   t1.pkOrderId,
                                                   t1.OrderNumber,
                                                   t1.ProjectName,
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

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.pkOrderId;
                        objtbl.OrderNumber = item.OrderNumber;
                        objtbl.ProjectName = item.ProjectName;
                        objtbl.ClientName = item.ClientName;
                        objtbl.ProjectType = item.ProjectType;
                        objtbl.Phone = item.Phone;
                        objtbl.Email = item.Email;
                        objtbl.IsStatus = item.IsStatus;
                        objtbl.IsRejected = item.IsRejected;
                        objtbl.IsAccepted = item.IsAccepted;
                        objtbl.OrderDate = item.OrderDate;
                        objtbl.DesignerId = item.DesignerId;



                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion

                }
                else if (SearchType == "OrderEmail")
                {
                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t2.fkContentWritterId == CurrentUserID && t2.Email == SearchValue && t2.OrderStage != 6
                                               select new
                                               {
                                                   t1.pkOrderId,
                                                   t1.OrderNumber,
                                                   t1.ProjectName,
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

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.pkOrderId;
                        objtbl.OrderNumber = item.OrderNumber;
                        objtbl.ProjectName = item.ProjectName;
                        objtbl.ClientName = item.ClientName;
                        objtbl.ProjectType = item.ProjectType;
                        objtbl.Phone = item.Phone;
                        objtbl.Email = item.Email;
                        objtbl.IsStatus = item.IsStatus;
                        objtbl.IsRejected = item.IsRejected;
                        objtbl.IsAccepted = item.IsAccepted;
                        objtbl.OrderDate = item.OrderDate;
                        objtbl.DesignerId = item.DesignerId;



                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion
                }
                else if (SearchType == "Project")
                {

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t2.fkContentWritterId == CurrentUserID && t2.ProjectName == SearchValue && t2.OrderStage != 6
                                               select new
                                               {
                                                   t1.pkOrderId,
                                                   t1.OrderNumber,
                                                   t1.ProjectName,
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

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.pkOrderId;
                        objtbl.OrderNumber = item.OrderNumber;
                        objtbl.ProjectName = item.ProjectName;
                        objtbl.ClientName = item.ClientName;
                        objtbl.ProjectType = item.ProjectType;
                        objtbl.Phone = item.Phone;
                        objtbl.Email = item.Email;
                        objtbl.IsStatus = item.IsStatus;
                        objtbl.IsRejected = item.IsRejected;
                        objtbl.IsAccepted = item.IsAccepted;
                        objtbl.OrderDate = item.OrderDate;
                        objtbl.DesignerId = item.DesignerId;



                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion



                }
                else if (SearchType == "ProjectType")
                {
                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t2.fkContentWritterId == CurrentUserID && t2.ProjectType == SearchValue && t2.OrderStage != 6
                                               select new
                                               {
                                                   t1.pkOrderId,
                                                   t1.OrderNumber,
                                                   t1.ProjectName,
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

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.pkOrderId;
                        objtbl.OrderNumber = item.OrderNumber;
                        objtbl.ProjectName = item.ProjectName;
                        objtbl.ClientName = item.ClientName;
                        objtbl.ProjectType = item.ProjectType;
                        objtbl.Phone = item.Phone;
                        objtbl.Email = item.Email;
                        objtbl.IsStatus = item.IsStatus;
                        objtbl.IsRejected = item.IsRejected;
                        objtbl.IsAccepted = item.IsAccepted;
                        objtbl.OrderDate = item.OrderDate;
                        objtbl.DesignerId = item.DesignerId;



                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion

                }
                else if (SearchType == "ClientName")
                {

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t2.fkContentWritterId == CurrentUserID && t2.ClientName == SearchValue && t2.OrderStage != 6
                                               select new
                                               {
                                                   t1.pkOrderId,
                                                   t1.OrderNumber,
                                                   t1.ProjectName,
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

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.pkOrderId;
                        objtbl.OrderNumber = item.OrderNumber;
                        objtbl.ProjectName = item.ProjectName;
                        objtbl.ClientName = item.ClientName;
                        objtbl.ProjectType = item.ProjectType;
                        objtbl.Phone = item.Phone;
                        objtbl.Email = item.Email;
                        objtbl.IsStatus = item.IsStatus;
                        objtbl.IsRejected = item.IsRejected;
                        objtbl.IsAccepted = item.IsAccepted;
                        objtbl.OrderDate = item.OrderDate;
                        objtbl.DesignerId = item.DesignerId;



                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion
                }




            }
            if (ObjtblOrderList1.Count > 0)
            {
                ObjtblOrderList = new List<OrderModel>();
                foreach (var item in ObjtblOrderList1)
                {
                    OrderModel objOrderModel = new OrderModel();
                    objOrderModel.pkOrderId = item.pkOrderId;
                    objOrderModel.OrderNumber = item.OrderNumber.ToString();
                    objOrderModel.ClientName = item.ClientName;
                    objOrderModel.ProjectName = item.ProjectName;
                    objOrderModel.ProjectType = item.ProjectType;
                    objOrderModel.Phone = item.Phone;
                    objOrderModel.Email = item.Email;
                    objOrderModel.IsStatus = item.IsStatus;
                    objOrderModel.IsAccepted = item.IsAccepted;
                    objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                    objOrderModel.DesignerId = item.DesignerId;
                    objOrderModel.TotalEarnings = ordAssignment.TotalEarnings ?? default(int);
                    objOrderModel.MonthEarnings = ordAssignment.MonthEarnings;
                    objOrderModel.NewOrderCount = ordAssignment.NewOrderCount;
                    objOrderModel.DesignStagerCount = ordAssignment.DesignStagerCount;
                    objOrderModel.ProofStageCount = ordAssignment.ProofStageCount;
                    objOrderModel.PrintingCount = ordAssignment.PrintingCount;
                    objOrderModel.CompletedCount = ordAssignment.CompletedCount;
                    objOrderModel.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                    ObjtblOrderList.Add(objOrderModel);
                }
            }
            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber).ToList(); ;
        }



        [HttpPost]
        public ActionResult UploadDraft(IEnumerable<HttpPostedFileBase> files)
        {
            string fileName = string.Empty;
            string MultiplefileName = string.Empty;


            HttpFileCollectionBase file = Request.Files;

            int PkOrderID = Convert.ToInt32(Request["hdnPkOrderID"]);
            int OrderNumber = Convert.ToInt32(Request["hdnOrderNumber"]);

            try
            {
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                if (Request.Files.Count > 0)
                {

                    var rootDir = Server.MapPath("~/Images/OrderContent1");
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

                            fileName = r.Replace(fileName, "");
                            string[] words = FileName.Split('.');
                            string fileWitheoutExt = words[0];

                            string CurrentDate = DateTime.Now.ToString("mmyddyy");
                            String ext = System.IO.Path.GetExtension(item2.FileName);
                            DateTime thisDate1 = DateTime.Now;
                            var UidNumber = thisDate1.ToString("MMMMddyyyyHmmss") + ext;


                            fileName = OrderNumber + "_" + CurrentDate + "_" + fileWitheoutExt + "_" + UidNumber;
                            MultiplefileName = MultiplefileName + "," + fileName;
                            item2.SaveAs(Server.MapPath("~/Images/OrderContent1/" + fileName));
                        }
                    }
                    SaveContentInDb(MultiplefileName, OrderNumber);
                }



                return RedirectToAction("item", "Order", new { id = PkOrderID });
            }
            catch (Exception)
            {
                
                return RedirectToAction("item", "Order", new { id = PkOrderID });
            }

        }
        public int SaveContentInDb(string Files, int OrderNumber)
        {
            int result = -1;

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var OrderContent = objdb.tblOrderContents.Where(rec => rec.OrderNumber == OrderNumber).FirstOrDefault();
                if (OrderContent == null)
                {
                    // ADD new
                    tblOrderContent tbltblOrderContent = new tblOrderContent();
                    tbltblOrderContent.Files = Files;
                    tbltblOrderContent.OrderNumber = OrderNumber;
                    tbltblOrderContent.CreatedDate = DateTime.Now;

                    objdb.tblOrderContents.Add(tbltblOrderContent);
                    objdb.SaveChanges();
                    result = 1;
                }
                else
                {
                    // Edit File
                    OrderContent.Files = OrderContent.Files + Files;
                    objdb.SaveChanges();
                    result = 2;
                }

            }
            return result;
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

        public OrderModel GetItemInfobyId(int id)
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            tblOrderItem objtblOrderItem = objdb.tblOrderItems.Where(p => p.ID == id).SingleOrDefault();
            CommonController objCommonController = new CommonController();

            tblOrder objtblOrder = new tblOrder();
            objtblOrder = objdb.tblOrders.Where(rec => rec.pkOrderId == objtblOrderItem.OrderID).FirstOrDefault();
            OrderAssignment ordAssignment = new OrderAssignment();
            string contentfile = string.Empty;
            string AdminComment = string.Empty;

            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);


            int CurrentUserID = WebSecurity.CurrentUserId;
           
            string orderid = objtblOrderItem.ID.ToString();
            var ordAssignment1 = objdb.OrderAssignments.Where(t => t.OrderNumber == orderid).FirstOrDefault();
            if (ordAssignment1 != null)
            {
                AdminComment = ordAssignment1.Comments;
            }

            ordAssignment.OrderStage = objdb.OrderAssignments.Where(t => t.OrderNumber == orderid).Select(t => t.OrderStage).FirstOrDefault();
            int OrderNumber = Convert.ToInt32(objtblOrderItem.ID);
            var tblOrderContents = objdb.tblOrderContents.Where(rec => rec.OrderNumber == OrderNumber).FirstOrDefault();
            if (tblOrderContents != null)
            {
                contentfile = tblOrderContents.Files;
            }
            OrderModel model = new OrderModel();
            GetleftMenu(model, CurrentUserID);
          
            model.parentId = objtblOrder.pkOrderId;
            model.ProjectName = objtblOrder.ProjectName;
            model.ProjectType = objtblOrder.ProjectType;
            model.ClientName = objtblOrder.ClientName;
            model.Email = objtblOrder.Email;
            model.OrderNumber = objtblOrder.OrderNumber.ToString();
            model.Phone = objtblOrder.Phone;
            model.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
            model.ContentFile = contentfile;
            model.GetOrderItems = objCommonController.GetOrderItemBypKOrderId(id);

            return (model);
        }

        public List<OrderModel> GetOrderListByContentWritterId(int? CurrentUserID)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();            
            List<OrderItemModel> OrderItemList = new List<OrderItemModel>();
            OrderAssignment ordAssignment = new OrderAssignment();


            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
               


                #region serach region

                             

                var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                           join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                           where t2.fkContentWritterId == CurrentUserID && t2.OrderStage != 6
                                           select new
                                           {
                                               t1.pkOrderId,
                                               t1.OrderNumber,
                                               t1.ProjectName,
                                               t1.ClientName,
                                               t1.ProjectType,
                                               t1.Phone,
                                               t1.Email,

                                               t1.IsStatus,
                                               t1.IsRejected,
                                               t1.IsAccepted,
                                               t1.OrderDate,
                                               t1.fkContentWritterId,

                                           }).ToList();
                if (tempObjtblOrderList.Count > 0)
                {
                    foreach (var ODitem in tempObjtblOrderList)
                    {


                        OrderModel objOrderModel = new OrderModel();
                        objOrderModel.pkOrderId = ODitem.pkOrderId;
                        objOrderModel.OrderNumber = ODitem.OrderNumber;
                        objOrderModel.ClientName = ODitem.ProjectName;
                        objOrderModel.ProjectName = ODitem.ClientName;
                        objOrderModel.ProjectType = ODitem.ProjectType;
                        objOrderModel.Phone = ODitem.Phone;
                        objOrderModel.Email = ODitem.Email;
                        objOrderModel.IsStatus = ODitem.IsStatus;
                        objOrderModel.IsRejected = ODitem.IsRejected;
                        objOrderModel.IsAccepted = ODitem.IsAccepted;
                        objOrderModel.PkContentWritterId = ODitem.fkContentWritterId;
                        objOrderModel.OrderDate = Convert.ToDateTime(ODitem.OrderDate);
                      
                        GetleftMenu(objOrderModel, CurrentUserID);

                        ObjtblOrderList.Add(objOrderModel);
                    }
                }
                else
                {
                    OrderModel objOrderModel = new OrderModel();
                    GetleftMenu(objOrderModel, CurrentUserID);
                    ObjtblOrderList.Add(objOrderModel);
                }


                #endregion



            }


            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber).ToList(); ;
        }


        public OrderModel GetleftMenu(OrderModel objOrderModelleftmenu, int? CurrentUserID)
        {
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                DateTime baseDate = DateTime.Today;
                var today = baseDate;
                var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                //  objOrderModelleftmenu.TotalEarnings = objdb.OrderAssignments.Where(t => t.DesignerID == CurrentDesignerID).Sum(t => t.TotalEarnings);
                objOrderModelleftmenu.TotalEarnings = objdb.OrderAssignments.Where(t => t.DesignerID == CurrentUserID).Sum(t => t.TotalEarnings); 

                objOrderModelleftmenu.MonthEarnings = objdb.OrderAssignments.Where(t => t.DesignerID == CurrentUserID && (t.AcceptRejectDate >= thisMonthStart && t.AcceptRejectDate <= thisMonthEnd)).Sum(t => t.TotalEarnings) ?? default(int);


                var tblOrdersList = (from t1 in objdb.tblOrders
                                     join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                     where t2.fkContentWritterId == CurrentUserID
                                     select new
                                     {
                                         t1.pkOrderId,
                                         t2.IsContentWriterAccepted,
                                         t2.IsContentWriterJobClosed
                                     }).ToList();


                objOrderModelleftmenu.NewOrderCount = tblOrdersList.Where(rec => rec.IsContentWriterAccepted != true).Count();//objdb.OrderAssignments.Count(t => t.DesignerID == CurrentUserID && t.OrderStage == 1);
                //objOrderModelleftmenu.DesignStagerCount = objdb.OrderAssignments.Count(t => t.DesignerID == CurrentDesignerID && t.OrderStage == 2);
               // objOrderModelleftmenu.ProofStageCount = ProofStagerec.Count();
                objOrderModelleftmenu.ApprovedCount = tblOrdersList.Where(rec => rec.IsContentWriterAccepted == true && rec.IsContentWriterJobClosed != true).Count(); ;
              //  objOrderModelleftmenu.PrintingCount = objdb.OrderAssignments.Count(t => t.DesignerID == CurrentDesignerID && t.OrderStage == 5);
                objOrderModelleftmenu.CompletedCount = tblOrdersList.Where(rec => rec.IsContentWriterJobClosed == true).Count();
               // objOrderModelleftmenu.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);

            }
            return objOrderModelleftmenu;
        }

        public List<ddlOrderList> ddlOrders(int DesignerId)
        {
            List<ddlOrderList> ListOfDesigner = new List<ddlOrderList>();
            List<tblOrder> objtblOrders = new List<tblOrder>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objtblOrders = objdb.tblOrders.Where(rec => rec.DesignerId == DesignerId).ToList();
            }

            foreach (var item in objtblOrders)
            {
                ddlOrderList objOrderList = new ddlOrderList();
                objOrderList.OrderNumber = item.OrderNumber.ToString();
                objOrderList.pkOrderId = item.pkOrderId;
                ListOfDesigner.Add(objOrderList);
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


        [HttpPost]
        public ActionResult CloseContentWriterJob(int OrderId)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == OrderId).FirstOrDefault();

                if (tblOrderItems != null)
                {
                    // Set Accepted
                    tblOrderItems.IsContentWriterJobClosed = true;
                    tblOrderItems.fkContentWritterId = WebSecurity.CurrentUserId;


                    objdb.SaveChanges();
                }



                return Json(new { result = result });

            }
        }


        [HttpPost]
        public ActionResult AcceptOrderByContentWritter(int OrderId, bool IsAccepted)
        {
            string result = "Fail";
            string JobType= string.Empty;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == OrderId).FirstOrDefault();

                if (tblOrderItems != null)
                {
                    // Set Accepted
                    tblOrderItems.IsContentWriterAccepted = IsAccepted;
                    tblOrderItems.fkContentWritterId = WebSecurity.CurrentUserId;
                    if (IsAccepted == false)
                    {
                        tblOrderItems.IsItemStatus = false;
                        JobType = "Rejected";

                    }
                    else
                    {
                        JobType = "Accepted";
 
                    }
                     DesignerModel objModel=   GetContentWriterById(WebSecurity.CurrentUserId);
                    NotificationEmail(objModel.DesignerFirstName, objModel.EmailId, "Content", tblOrderItems.ItemName, JobType);
                    objdb.SaveChanges();
                }


                


            }
            return Json(new { result = result });

        }

        private void NotificationEmail(string user, string eMailAddress, string type, string ProjectName, string JobType)
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
                MailAddress to = new MailAddress(ConfigurationSettings.AppSettings["toAdd"].ToString());// new MailAddress(eMailAddress); 
                // MailAddress from = new MailAddress("lakhvinder.happy@gmail.com");
                // MailAddress to = new MailAddress("lakhvinder.kumar@team.amonous.com");// new MailAddress(eMailAddress); 

                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = type == "designer" ? ConfigurationSettings.AppSettings["subjectToDesigner"].ToString() + " " + DateTime.Now.ToString() : ConfigurationSettings.AppSettings["subjectToContentwriter"].ToString() + " " + DateTime.Now.ToString();
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;
                string project = type == "designer" ? "design " : "content";


                var emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/JobPicked.html")).ReadToEnd().ToString();
                var emailTemplate = emailBody;
                string email = emailTemplate
                  .Replace("##ProjectName##", ProjectName)
                  .Replace("##JobType##", JobType)
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



        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            List<OrderModel> ListOrderModel = new List<OrderModel>();
            CommonController objCommonController = new CommonController();
            int? page = 1;
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            tblOrder orderTable = objdb.tblOrders.Where(p => p.pkOrderId == id).SingleOrDefault();
            OrderAssignment ordAssignment = new OrderAssignment();
            string contentfile = string.Empty;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            int CurrentUserID = WebSecurity.CurrentUserId;
            ordAssignment.Comments = objdb.OrderAssignments.Where(t => t.OrderNumber == orderTable.OrderNumber).Select(t => t.Comments).FirstOrDefault();
            ordAssignment.OrderStage = objdb.OrderAssignments.Where(t => t.OrderNumber == orderTable.OrderNumber).Select(t => t.OrderStage).FirstOrDefault();
            int OrderNumber = Convert.ToInt32(orderTable.OrderNumber);
            var tblOrderContents = objdb.tblOrderContents.Where(rec => rec.OrderNumber == OrderNumber).FirstOrDefault();
            if (tblOrderContents != null)
            {
                contentfile = tblOrderContents.Files;
            }

            var ObjOrderItem1 = objdb.tblOrderItems.Where(rec => rec.OrderID == id).ToList();

            var byUserId = ObjOrderItem1.Where(rec => rec.fkContentWritterId == CurrentUserID).Count();
            var byisnull = ObjOrderItem1.Where(rec => rec.fkContentWritterId == null).Count();
            var total = byisnull + byUserId;

            bool isfullorderaccept = false;

            if (total == ObjOrderItem1.Count())
            {
                isfullorderaccept = true;
            }

            var OrderStage = "Open";
            List<tblOrderItem> Orderitem = objdb.tblOrderItems.AsEnumerable().Where(rec => rec.OrderID == orderTable.pkOrderId).ToList();
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

            OrderModel model = new OrderModel();
            GetleftMenu(model, CurrentUserID);
            model.pkOrderId = orderTable.pkOrderId;
            model.ProjectName = orderTable.ProjectName;
            model.ProjectType = orderTable.ProjectType;
            model.ClientName = orderTable.ClientName;
            model.DesignerId = orderTable.DesignerId;
            model.Email = orderTable.Email;
            model.IsAcceptFullOrder = isfullorderaccept;
            model.OrderNumber = orderTable.OrderNumber;
            model.OrderStatus = OrderStage;


           



            model.IsAccepted = orderTable.IsAccepted;
            model.IsRejected = orderTable.IsRejected;
            model.IsStatus = orderTable.IsStatus;
            model.OrderDate = Convert.ToDateTime(orderTable.OrderDate);
            model.Phone = orderTable.Phone;
            model.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
            model.ContentFile = contentfile;
            model.GetOrderItems = objCommonController.GetOrderItemByOrderIdbyCurrentUserId(id);
            ListOrderModel.Add(model);
            IPagination pagedModel = ListOrderModel.AsPagination(page ?? 1, 10);
            return View(pagedModel);




        }
       
        public FileResult DownloadContentFiles(string FileName)
        {
            string txt = FileName;
            char[] splitedcBy = new char[] { '.' };

            char[] splitedcBy2 = new char[] { '_' };
            var filenamw = FileName.Split(splitedcBy2);
            var ext = FileName.Split(splitedcBy);
            string finalName = filenamw[2] +"."+ ext[1];
            return File(Server.MapPath("~/Images/OrderContent1/" + FileName), System.Net.Mime.MediaTypeNames.Application.Octet, finalName);
        }
        public OrderModel GetOrderList()
        {
            OrderModel ordAssignment = new OrderModel();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {


                GetLeftMenuContent(ordAssignment);
                ordAssignment.DesingerList = ddldesigner();
                ordAssignment.ddlOrderList = ddlOrders();

            }
            return ordAssignment;
        }
        public OrderModel GetLeftMenuContent(OrderModel objOrderModel)
        {

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                objOrderModel.AssignOrder = objdb.tblOrders.Count(t => t.DesignerId != null && (t.IsAccepted == true || t.IsAccepted == null));

                objOrderModel.UnAssignOrder = objdb.tblOrders.Count(t => t.DesignerId == null || t.IsAccepted == false);

                var ProofStagerec = (from t1 in objdb.tblOrders
                                     join t2 in objdb.OrderAssignments on t1.OrderNumber equals t2.OrderNumber
                                     where t2.OrderStage == 3 && t1.IsAccepted == true
                                     select new
                                     {
                                         t1.pkOrderId,
                                     }).ToList();

                objOrderModel.NewOrderCount = objdb.OrderAssignments.Count(t => t.OrderStage == 1);
                objOrderModel.DesignStagerCount = objdb.OrderAssignments.Count(t => t.OrderStage == 2);
                objOrderModel.ProofStageCount = ProofStagerec.Count();
                objOrderModel.ApprovedCount = objdb.OrderAssignments.Count(t => t.OrderStage == 4);
                objOrderModel.PrintingCount = objdb.OrderAssignments.Count(t => t.OrderStage == 5);
                objOrderModel.CompletedCount = objdb.OrderAssignments.Count(t => t.OrderStage == 6);
                objOrderModel.DesingerList = ddldesigner();
                objOrderModel.ddlOrderList = ddlOrders();
            }
            return objOrderModel;
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
                    objm.AddedOn = Convert.ToDateTime(obtbl.AddedOn);
                }
            }

            return objm;
        }

        [HttpPost]
        public ActionResult Edit(DesignerModel objmodel)
        {
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                tblContentWriter obtbl = objdb.tblContentWriters.Where(rec => rec.PkContentWritterId == objmodel.DesignerID).FirstOrDefault();
                if (obtbl != null)
                {
                    obtbl.PkContentWritterId = objmodel.DesignerID;
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
                    obtbl.AddedOn = objmodel.AddedOn;
                    obtbl.AddedBy = objmodel.AddedBy;
                    objdb.SaveChanges();

                }
            }

            return RedirectToAction("Add", objmodel);

        }
    }
}
