using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pEasyPrint.Areas.Designer.Models;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using System.Web.Security;
using WebMatrix.WebData;
using System.IO;
using System.Text.RegularExpressions;
using pEasyPrint.Controllers;
using pEasyPrint.Models;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using Cryptor;

namespace pEasyPrint.Areas.Designer.Controllers
{
    [Authorize(Roles = "Designer")]
    public class ProjectController : Controller
    {
        //
        // GET: /Designer/Project/

        public ActionResult Index(int? page, int? pagesize, string SearchType, string SearchValue, string SearchBy, string OrderStage)
        {
            int PageSize = pagesize == null ? 20 : Convert.ToInt32(pagesize);
            OrderModel objModel = new OrderModel();
            //objModel.OrderList = GetOrderList();
            int CurrentUserId = WebSecurity.CurrentUserId;
            List<OrderModel> orders = GetOrderListByDesignerId(CurrentUserId);
            if (SearchBy == "Designer")
            {
                orders = GetOrderListByEmail(CurrentUserId, SearchType, SearchValue);
            }
            else if (!string.IsNullOrEmpty(OrderStage))
            {
                orders = GetOrderListByOrderStage(CurrentUserId, Convert.ToInt32(OrderStage));
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



        public ActionResult Resource(int? page)
        {
            int CurrentUserId = WebSecurity.CurrentUserId;
            OrderModel orders = GetResourcesForDesigner(CurrentUserId);


            return View(orders);
        }
        public ActionResult PickNewJob(int? page, int? pagesize, string SearchType, string SearchValue, string SearchBy, string OrderStage)
        {
            int PageSize = pagesize == null ? 20 : Convert.ToInt32(pagesize);
            OrderModel objModel = new OrderModel();
            //objModel.OrderList = GetOrderList();
            int CurrentUserId = WebSecurity.CurrentUserId;
            List<OrderModel> orders = GetNewOrderforDesigner(CurrentUserId);
            if (SearchBy == "Designer")
            {
                orders = GetOrderListByOrderFilter(CurrentUserId, SearchType, SearchValue);
            }
            else if (!string.IsNullOrEmpty(OrderStage))
            {
                orders = GetOrderListByOrderStage(CurrentUserId, Convert.ToInt32(OrderStage));
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


        public ActionResult Reports(int? page)
        {


            int CurrentUserId = WebSecurity.CurrentUserId;
            List<OrderModel> orders = GetOrderListByDesignerId(CurrentUserId);
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

        public List<OrderModel> GetOrderListByOrderFilter(int CurrentDesignerID, string SearchType, string SearchValue)
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
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == CurrentDesignerID && rec.Email == SearchValue).ToList();
                #region Left Menu Data
                tblDesigner designer = new tblDesigner();
                OrderModel objOrderModelleftmenu = new OrderModel();
                GetleftMenu(objOrderModelleftmenu, CurrentDesignerID);
                ObjtblOrderList.Add(objOrderModelleftmenu);
                #endregion

                if (SearchType == "DesignerId")
                {
                    int pkOrderId = Convert.ToInt32(SearchValue);

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.pkOrderId == pkOrderId && t2.OrderStage == 0
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
                                                   x.DesignerId,
                                               }).ToList();

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber;
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate;
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion
                }
                else if (SearchType == "OrderNumber")
                {
                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.OrderNumber == SearchValue && t2.OrderStage == 0
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
                                                   x.DesignerId,
                                               }).ToList();

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber;
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate;
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
                        ObjtblOrderList1.Add(objtbl);
                    }



                    #endregion

                }
                else if (SearchType == "OrderEmail")
                {

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.Email == SearchValue && t2.OrderStage == 0
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
                                                   x.DesignerId,
                                               }).ToList();

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber;
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate;
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion


                }
                else if (SearchType == "Project")
                {

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.ProjectName == SearchValue && t2.OrderStage == 0
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
                                                   x.DesignerId,
                                               }).ToList();

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber;
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate;
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion

                }
                else if (SearchType == "ProjectType")
                {



                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.ProjectType == SearchValue && t2.OrderStage == 0
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
                                                   x.DesignerId,
                                               }).ToList();

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber;
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate;
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
                        ObjtblOrderList1.Add(objtbl);
                    }


                    #endregion
                }
                else if (SearchType == "ClientName")
                {


                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.ClientName == SearchValue && t2.OrderStage == 0
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
                                                   x.DesignerId,
                                               }).ToList();

                    foreach (var item in tempObjtblOrderList)
                    {
                        tblOrder objtbl = new tblOrder();

                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber;
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate;
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
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
                    objOrderModel.OrderNumber = Convert.ToInt32(item.OrderNumber);
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


        public List<OrderModel> GetOrderListByOrderStage(int CurrentDesignerID, int? OrderStage)
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
                //  ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == CurrentDesignerID && rec.or == OrderEmail).ToList();



                //if (OrderStage == 1)
                //{
                var list = (from t1 in objdb.tblOrders.AsEnumerable()
                            join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID

                            where t2.OrderStage == OrderStage && t2.DesignerId == CurrentDesignerID // && t3.DesignerId == CurrentDesignerID
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
                            }).GroupBy(x1 => new
                            {
                                x1.pkOrderId,
                                x1.OrderNumber,
                                x1.ProjectName,
                                x1.ClientName,
                                x1.ProjectType,
                                x1.Phone,
                                x1.Email,
                                x1.IsStatus,
                                x1.IsRejected,
                                x1.IsAccepted,
                                x1.OrderDate,
                                x1.DesignerId,





                            }).ToList();

                foreach (var item in list)
                {
                    OrderModel objOrderModel = new OrderModel();
                    GetleftMenu(objOrderModel, CurrentDesignerID);
                    objOrderModel.pkOrderId = item.ElementAt(0).pkOrderId;
                    objOrderModel.OrderNumber = Convert.ToInt32(item.ElementAt(0).OrderNumber);
                    objOrderModel.ClientName = item.ElementAt(0).ClientName;
                    objOrderModel.ProjectName = item.ElementAt(0).ProjectName;
                    objOrderModel.ProjectType = item.ElementAt(0).ProjectType;
                    objOrderModel.Phone = item.ElementAt(0).Phone;
                    objOrderModel.Email = item.ElementAt(0).Email;
                    objOrderModel.IsStatus = item.ElementAt(0).IsStatus;
                    objOrderModel.IsAccepted = item.ElementAt(0).IsAccepted;
                    objOrderModel.OrderDate = Convert.ToDateTime(item.ElementAt(0).OrderDate);
                    objOrderModel.DesignerId = item.ElementAt(0).DesignerId;

                    objOrderModel.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                    ObjtblOrderList.Add(objOrderModel);
                }
                //}
                //else
                //{
                //    var list = (from t1 in objdb.tblOrders.AsEnumerable()
                //                join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                //                join t3 in objdb.OrderAssignments.AsEnumerable() on t2.OrderID.ToString() equals t3.OrderNumber

                //                where t3.OrderStage == OrderStage && t2.DesignerId == CurrentDesignerID // && t3.DesignerId == CurrentDesignerID
                //                select new
                //                {
                //                    t1.pkOrderId,
                //                    t1.OrderNumber,
                //                    t1.ProjectName,
                //                    t1.ClientName,
                //                    t1.ProjectType,
                //                    t1.Phone,
                //                    t1.Email,

                //                    t1.IsStatus,
                //                    t1.IsRejected,
                //                    t1.IsAccepted,
                //                    t1.OrderDate,
                //                    t1.DesignerId,
                //                }).ToList();

                //    foreach (var item in list)
                //    {
                //        OrderModel objOrderModel = new OrderModel();
                //        GetleftMenu(objOrderModel, CurrentDesignerID);
                //        objOrderModel.pkOrderId = item.pkOrderId;
                //        objOrderModel.OrderNumber = Convert.ToInt32(item.OrderNumber);
                //        objOrderModel.ClientName = item.ClientName;
                //        objOrderModel.ProjectName = item.ProjectName;
                //        objOrderModel.ProjectType = item.ProjectType;
                //        objOrderModel.Phone = item.Phone;
                //        objOrderModel.Email = item.Email;
                //        objOrderModel.IsStatus = item.IsStatus;
                //        objOrderModel.IsAccepted = item.IsAccepted;
                //        objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);
                //        objOrderModel.DesignerId = item.DesignerId;

                //        objOrderModel.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                //        ObjtblOrderList.Add(objOrderModel);
                //    }
                //}








            }


            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber).ToList(); ;
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

        public FileResult DownloadTemplaeFiles(string FileName)
        {
            string filename = "spasbrochures103_front.zip";
            return File(Server.MapPath("http://peasyprint.com/downloadtemplate/" + filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);

            // return File(Server.MapPath("~/Images/OrderContent1/" + FileName), System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }
        public List<OrderModel> GetOrderListByEmail(int CurrentDesignerID, string SearchType, string SearchValue)
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
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == CurrentDesignerID && rec.Email == SearchValue).ToList();
                #region Left Menu Data
                tblDesigner designer = new tblDesigner();
                OrderModel objOrderModelleftmenu = new OrderModel();
                GetleftMenu(objOrderModelleftmenu, CurrentDesignerID);
                ObjtblOrderList.Add(objOrderModelleftmenu);
                #endregion

                if (SearchType == "DesignerId")
                {
                    int pkOrderId = Convert.ToInt32(SearchValue);

                    #region serach region
                    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                               where t1.pkOrderId == pkOrderId && t2.DesignerId == CurrentDesignerID && t2.OrderStage != 6
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
                                               where t1.OrderNumber == SearchValue && t2.DesignerId == CurrentDesignerID && t2.OrderStage != 6
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
                                               where t1.Email == SearchValue && t2.DesignerId == CurrentDesignerID && t2.OrderStage != 6
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
                                               where t2.ProjectName == SearchValue && t2.DesignerId == CurrentDesignerID && t2.OrderStage != 6
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
                                               where t1.ProjectType == SearchValue && t2.DesignerId == CurrentDesignerID
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
                                               where t1.ClientName == SearchValue && t2.DesignerId == CurrentDesignerID && t2.OrderStage != 6
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
                    objOrderModel.OrderNumber = Convert.ToInt32(item.OrderNumber);
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

        public List<OrderModel> GetNewOrderforDesigner(int CurrentDesignerID)
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


               // objCommonController.GetLeftMenuContent(ordOrderModelLeftMenu);

               // ObjtblOrderList.Add(ordOrderModelLeftMenu);
            }


            ObjtblOrderList = new List<OrderModel>();
            if (ObjtblOrderList1.Count() >= 1)
            {
                foreach (var item in ObjtblOrderList1)
                {
                    OrderModel objOrderModel = new OrderModel();
                    objOrderModel.pkOrderId = item.pkOrderId;
                    objOrderModel.OrderNumber = Convert.ToInt32(item.OrderNumber);

                    objOrderModel.ClientName = item.ClientName;
                    objOrderModel.ProjectType = item.ProjectType;
                    objOrderModel.Phone = item.Phone;
                    objOrderModel.Email = item.Email;
                    objOrderModel.DesignerId = 0;

                    objOrderModel.OrderDate = Convert.ToDateTime(item.OrderDate);

                    //objCommonController.GetLeftMenuContent(objOrderModel);

                    ObjtblOrderList.Add(objOrderModel);
                }
            }
            else
            {
                OrderModel objOrderModel = new OrderModel();
                objOrderModel.CompletedCount = 1;
                ObjtblOrderList.Add(objOrderModel);

            }
            return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber).ToList();
            //List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            //List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();
            //OrderAssignment ordAssignment = new OrderAssignment();


            //using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            //{
            //    ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == CurrentDesignerID || rec.IsAccepted == null || rec.IsAccepted == false).ToList();

            //    #region Left Menu Data

            //    OrderModel objOrderModelleftmenu = new OrderModel();
            //    GetleftMenu(objOrderModelleftmenu, CurrentDesignerID);
            //    ObjtblOrderList.Add(objOrderModelleftmenu);
            //    #endregion

            //    #region serach region
            //    var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
            //                               join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
            //                               where t2.OrderStage == 1 && t2.DesignerId == null
            //                               select new
            //                               {
            //                                   t1.pkOrderId,
            //                                   t1.OrderNumber,
            //                                   t2.ProjectName,
            //                                   t1.ClientName,
            //                                   t1.ProjectType,
            //                                   t1.Phone,
            //                                   t1.Email,
            //                                   t1.IsStatus,
            //                                   t1.IsRejected,
            //                                   t1.IsAccepted,
            //                                   t1.OrderDate,
            //                                   t2.DesignerId,


            //                               }).GroupBy(x => new
            //                               {
            //                                   x.pkOrderId,
            //                                   x.OrderNumber,
            //                                   x.ProjectName,
            //                                   x.ClientName,
            //                                   x.ProjectType,
            //                                   x.Phone,
            //                                   x.Email,
            //                                   x.IsStatus,
            //                                   x.IsRejected,
            //                                   x.IsAccepted,
            //                                   x.OrderDate,
            //                                   x.DesignerId



            //                               }
            //                               ).ToList();
            //    if (tempObjtblOrderList.Count > 0)
            //    {
            //        ObjtblOrderList = new List<OrderModel>();
            //        foreach (var item in tempObjtblOrderList)
            //        {
            //            OrderModel objtbl = new OrderModel();
            //            GetleftMenu(objtbl, CurrentDesignerID);
            //            objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
            //            objtbl.OrderNumber = item.ElementAt(0).OrderNumber == null ? 0 : Convert.ToInt32(item.ElementAt(0).OrderNumber);
            //            objtbl.ProjectName = item.ElementAt(0).ProjectName;
            //            objtbl.ClientName = item.ElementAt(0).ClientName;
            //            objtbl.ProjectType = item.ElementAt(0).ProjectType;
            //            objtbl.Phone = item.ElementAt(0).Phone;
            //            objtbl.Email = item.ElementAt(0).Email;
            //            objtbl.IsStatus = item.ElementAt(0).IsStatus;
            //            objtbl.IsRejected = item.ElementAt(0).IsRejected;
            //            objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
            //            objtbl.OrderDate = item.ElementAt(0).OrderDate == null ? DateTime.Now : Convert.ToDateTime(item.ElementAt(0).OrderDate);// Convert.ToInt32(item.OrderDate);
            //            objtbl.DesignerId = item.ElementAt(0).DesignerId;



            //            ObjtblOrderList.Add(objtbl);
            //        }
            //    }


            //    #endregion





            //}
            //return ObjtblOrderList.OrderByDescending(rec => rec.OrderNumber).ToList(); 
        }

        public OrderModel GetResourcesForDesigner(int CurrentDesignerID)
        {
            #region Left Menu Data

            OrderModel objOrderModelleftmenu = new OrderModel();
            GetleftMenu(objOrderModelleftmenu, CurrentDesignerID);

            #endregion







            return objOrderModelleftmenu;
        }


        public List<OrderModel> GetOrderListByDesignerId(int CurrentDesignerID)
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            List<tblOrder> ObjtblOrderList1 = new List<tblOrder>();
            OrderAssignment ordAssignment = new OrderAssignment();


            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                ObjtblOrderList1 = objdb.tblOrders.Where(rec => rec.DesignerId == CurrentDesignerID || rec.IsAccepted == null || rec.IsAccepted == false).ToList();

                #region Left Menu Data

                OrderModel objOrderModelleftmenu = new OrderModel();
                GetleftMenu(objOrderModelleftmenu, CurrentDesignerID);
                ObjtblOrderList.Add(objOrderModelleftmenu);
                #endregion

                #region serach region
                var tempObjtblOrderList = (from t1 in objdb.tblOrders.AsEnumerable()
                                           join t2 in objdb.tblOrderItems.AsEnumerable() on t1.pkOrderId equals t2.OrderID
                                           where t2.DesignerId == CurrentDesignerID && t2.OrderStage != 6
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
                                               t2.DesignerId,
                                               t2.AssignDate,


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
                                               x.DesignerId,
                                               x.AssignDate



                                           }
                                           ).ToList();
                if (tempObjtblOrderList.Count > 0)
                {
                    ObjtblOrderList = new List<OrderModel>();
                    foreach (var item in tempObjtblOrderList)
                    {
                        OrderModel objtbl = new OrderModel();
                        GetleftMenu(objtbl, CurrentDesignerID);
                        objtbl.pkOrderId = item.ElementAt(0).pkOrderId;
                        objtbl.OrderNumber = item.ElementAt(0).OrderNumber == null ? 0 : Convert.ToInt32(item.ElementAt(0).OrderNumber);
                        objtbl.ProjectName = item.ElementAt(0).ProjectName;
                        objtbl.ClientName = item.ElementAt(0).ClientName;
                        objtbl.ProjectType = item.ElementAt(0).ProjectType;
                        objtbl.Phone = item.ElementAt(0).Phone;
                        objtbl.Email = item.ElementAt(0).Email;
                        objtbl.IsStatus = item.ElementAt(0).IsStatus;
                        objtbl.IsRejected = item.ElementAt(0).IsRejected;
                        objtbl.IsAccepted = item.ElementAt(0).IsAccepted;
                        objtbl.OrderDate = item.ElementAt(0).OrderDate == null ? DateTime.Now : Convert.ToDateTime(item.ElementAt(0).OrderDate);// Convert.ToInt32(item.OrderDate);
                        objtbl.DesignerId = item.ElementAt(0).DesignerId;
                        objtbl.AssignDate = item.ElementAt(0).AssignDate;




                        ObjtblOrderList.Add(objtbl);
                    }
                }


                #endregion





            }
            return ObjtblOrderList.OrderByDescending(rec => rec.AssignDate).ToList();
        }
        public OrderModel GetleftMenu(OrderModel objOrderModelleftmenu, int CurrentDesignerID)
        {
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                DateTime baseDate = DateTime.Today;
                var today = baseDate;
                var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
                objOrderModelleftmenu.TotalEarnings = objdb.OrderAssignments.Where(t => t.DesignerID == CurrentDesignerID).Sum(t => t.TotalEarnings);
                objOrderModelleftmenu.MonthEarnings = objdb.OrderAssignments.Where(t => t.DesignerID == CurrentDesignerID && (t.AcceptRejectDate >= thisMonthStart && t.AcceptRejectDate <= thisMonthEnd)).Sum(t => t.TotalEarnings) ?? default(int);


                var ProofStagerec = (from t1 in objdb.tblOrders
                                     join t2 in objdb.OrderAssignments on t1.OrderNumber equals t2.OrderNumber
                                     where t2.OrderStage == 3 && t1.IsAccepted == true && t1.DesignerId == CurrentDesignerID
                                     select new
                                     {
                                         t1.pkOrderId,
                                     }).ToList();

                objOrderModelleftmenu.NewOrderCount = objdb.tblOrderItems.Count(t => t.DesignerId == CurrentDesignerID && t.OrderStage == 1);
                objOrderModelleftmenu.DesignStagerCount = objdb.tblOrderItems.Count(t => t.DesignerId == CurrentDesignerID && t.OrderStage == 2);
                objOrderModelleftmenu.ProofStageCount = objdb.tblOrderItems.Count(t => t.DesignerId == CurrentDesignerID && t.OrderStage == 3); ;
                objOrderModelleftmenu.ApprovedCount = objdb.tblOrderItems.Count(t => t.DesignerId == CurrentDesignerID && t.OrderStage == 4);
                objOrderModelleftmenu.PrintingCount = objdb.tblOrderItems.Count(t => t.DesignerId == CurrentDesignerID && t.OrderStage == 5);
                objOrderModelleftmenu.CompletedCount = objdb.tblOrderItems.Count(t => t.DesignerId == CurrentDesignerID && t.OrderStage == 6);
                objOrderModelleftmenu.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);

            }
            return objOrderModelleftmenu;
        }
        [HttpPost]
        public ActionResult FileUpload(IEnumerable<HttpPostedFileBase> files, IEnumerable<HttpPostedFileBase> files2)
        {

            int PkOrderID = Convert.ToInt32(Request["hdnPkOrderID"]);
            pEasyPrintEntities _context = new pEasyPrintEntities();
            try
            {
                string fileName = string.Empty;
                string FrontfileName = string.Empty;
                string BackfileName = string.Empty;



                HttpFileCollectionBase file = Request.Files;
                string proofComment = Request["DesignerFrontComment"].ToString();

                int OrderNumber = Convert.ToInt32(Request["hdnOrderNumber"]);
                string BackComments = Request["DesignerBackComment"].ToString();
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

                                fileName = OrderNumber + "_" + "front_" + UidNumber;
                                FrontfileName = FrontfileName + "," + fileName;
                                item2.SaveAs(Server.MapPath("~/Images/proofUpload/" + fileName));
                                counter++;
                            }
                        }
                    }



                    if (files2 != null)
                    {
                        foreach (HttpPostedFileBase item3 in files2)
                        {
                            if (item3 != null)
                            {
                                var FileName = Path.GetFileName(item3.FileName);

                                string fileName1 = Path.Combine(rootDir, FileName);
                                var Dirpath = Path.Combine(rootDir);
                                if (!Directory.Exists(Dirpath))
                                {
                                    Directory.CreateDirectory(Dirpath);
                                }

                                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                                String ext = System.IO.Path.GetExtension(item3.FileName);


                                DateTime thisDate1 = DateTime.Now;
                                var UidNumber = thisDate1.ToString("MMMMddyyyyHmmss") + counter + ext;
                                fileName1 = r.Replace(fileName1, "");
                                fileName1 = OrderNumber + "_" + "back_" + UidNumber;
                                BackfileName = BackfileName + "," + fileName1;
                                item3.SaveAs(Server.MapPath("~/Images/proofUpload/" + fileName1));
                                counter++;
                            }
                        }
                    }


                    SaveProofInDb(proofComment, BackComments, FrontfileName, BackfileName, PkOrderID);
                    string EmailAddress = GetEmailAddress(PkOrderID);
                    var designer = _context.tblDesigners.Where(i => i.EmailId == WebSecurity.CurrentUserName).FirstOrDefault();
                    SendAdminNotificationEmail("tom@ataink.com", files, files2, designer.DesignerFirstName + " " + designer.DesigenerLastName, OrderNumber);
                    //SendCustomerNotificationEmail(EmailAddress, files, files2);


                }
                return RedirectToAction("item", "Project", new { id =Encdy.Encode(PkOrderID)});

            }
            catch (Exception)
            {

                return RedirectToAction("item", "Project", new { id =Encdy.Encode(PkOrderID)});
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

        public string GetEmailAddress(int PkOrderID)
        {
            string EmailAddress = string.Empty;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                tblOrderItem tblOrderItem = objdb.tblOrderItems.Where(rec => rec.ID == PkOrderID).FirstOrDefault();
                if (tblOrderItem != null)
                {
                    if (tblOrderItem.CustEmail != null)
                    {
                        EmailAddress = tblOrderItem.CustEmail;
                    }
                    else
                    {
                        EmailAddress = tblOrderItem.Email;

                    }
                }
            }

            return EmailAddress;
        }
        public int SaveProofInDb(string FrontComments, string BackComments, string FrontFiles, string Backfiles, int OrderNumber)
        {
            try
            {
                int result = -1;


                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    #region proof stage
                    var prrof = objdb.tblOrderProofs.FirstOrDefault();
                    tblOrderProof tblObjOrderProof = new tblOrderProof();
                    tblObjOrderProof.FrontComments = FrontComments;
                    tblObjOrderProof.BackComments = BackComments;
                    tblObjOrderProof.DesignerFiles = FrontFiles;
                    tblObjOrderProof.DesignerBackFiles = Backfiles;

                    tblObjOrderProof.OrderNumber = OrderNumber;
                    tblObjOrderProof.CreatedDate = DateTime.Now;


                    objdb.tblOrderProofs.Add(tblObjOrderProof);
                    objdb.SaveChanges();
                    #endregion
                    tblOrderItem tblOrderItem = objdb.tblOrderItems.Where(rec => rec.ID == OrderNumber).FirstOrDefault();
                    if (tblOrderItem != null)
                    {
                        tblOrderItem.OrderStage = 3;
                        objdb.SaveChanges();
                    }

                    result = 1;
                }
                return result;

            }
            catch (Exception)
            {

                throw;
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
                        item.IsItemAccepted = true;
                        item.DesignerId = WebSecurity.CurrentUserId;
                        objdb.SaveChanges();
                    }
                }
            }
            return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult item(string id, string SearchType, string SearchValue, string SearchBy)
        {
            int uid = WebSecurity.CurrentUserId;
            int id1 = 0;
            try
            {
                id1 = Convert.ToInt32(Encdy.Decode(id));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }
            try
            {
                
                //int id1 = Convert.ToInt32(id);

                OrderModel model = GetItemInfobyId(id1);
                if (model.DesignerId != uid)
                {
                    return View();
                }
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
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }


        }
        public OrderModel GetItemInfobyId(int id)
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            OrderModel model = new OrderModel();
            tblOrderItem orderTable = objdb.tblOrderItems.Where(p => p.ID == id).SingleOrDefault();

            tblOrder objtblOrder = new tblOrder();
            if (orderTable != null)
            {
                objtblOrder = objdb.tblOrders.Where(rec => rec.pkOrderId == orderTable.OrderID).FirstOrDefault();

                OrderAssignment ordAssignment = new OrderAssignment();
                string contentfile = string.Empty;
                string AdminComment = string.Empty;

                DateTime baseDate = DateTime.Today;
                var today = baseDate;
                var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);


                int CurrentDesignerID = WebSecurity.CurrentUserId;
                string orderid = orderTable.ID.ToString();
                var ordAssignment1 = objdb.OrderAssignments.Where(t => t.OrderNumber == orderid).FirstOrDefault();
                if (ordAssignment1 != null)
                {
                    AdminComment = ordAssignment1.Comments;
                }
                int? orderstage = 0;
                var getOrderStage = objdb.OrderAssignments.Where(t => t.OrderNumber == orderid).FirstOrDefault();
                if (getOrderStage != null)
                {
                    orderstage = getOrderStage.OrderStage;
                }
                int OrderNumber = Convert.ToInt32(orderTable.ID);
                var tblOrderContents = objdb.tblOrderContents.Where(rec => rec.OrderNumber == OrderNumber).FirstOrDefault();
                if (tblOrderContents != null)
                {
                    contentfile = tblOrderContents.Files;
                }
                CommonController ObjCommonFunction = new CommonController();

                GetleftMenu(model, CurrentDesignerID);
                model.pkOrderId = orderTable.ID;
                model.ProjectName = ObjCommonFunction.GetProjectNamebyID(objtblOrder.pkOrderId);
                model.ProjectType = objtblOrder.ProjectType;
                model.ClientName = objtblOrder.ClientName;
                model.DesignerId = orderTable.DesignerId;
                model.Email = objtblOrder.Email;
                model.IsAccepted = orderTable.IsItemAccepted;
                model.IsRejected = orderTable.IsItemRejected;
                model.IsStatus = orderTable.IsItemStatus;
                model.OrderDate = Convert.ToDateTime(orderTable.CreatedOn);
                model.OrderNumber = Convert.ToInt32(objtblOrder.OrderNumber);
                model.Phone = objtblOrder.Phone;
                model.Comment = AdminComment;
                model.PkStageId = orderstage;
                model.StageList = objdb.tblOrderStages.ToList();
                model.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                model.ContentFile = contentfile;
                model.GetOrderItems = ObjCommonFunction.GetOrderItemBypKOrderIdForDesigner(id);
                model.ProjectType = orderTable.ProjectType;
                model.OrderPrrof = objdb.tblOrderProofs.AsEnumerable().Where(rec => rec.OrderNumber == Convert.ToInt32(orderTable.ID)).ToList();
            }
            //  model.CusomerReviewList = objdb.tblCustomerOrderReviews.AsEnumerable().Where(rec => rec.OrderNumber == Convert.ToInt32(orderTable.ID)).ToList();
            return (model);
        }

        [HttpPost]
        public ActionResult SendContentRequest(int OrderId)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == OrderId).FirstOrDefault();
                if (tblOrderItems != null)
                {
                    tblOrderItems.IsContentRequset = true;
                    objdb.SaveChanges();
                }

            }
            return Json(new { Message = result });
        }

        [HttpPost]
        public ActionResult AcceptOrderStatus(int OrderId, bool IsAccepted)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                string JobType = string.Empty;
                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == OrderId).FirstOrDefault();
                var tblOrders = objdb.tblOrders.Where(REC => REC.pkOrderId == tblOrderItems.OrderID).FirstOrDefault();
                int DesignerId = WebSecurity.CurrentUserId;
                if (tblOrderItems != null)
                {
                    if (tblOrderItems.DesignerId > 0 && tblOrderItems.DesignerId != DesignerId)
                    {
                        return Json(new { Message = "Already" });
                    }
                    else
                    {
                        tblOrderItems.IsItemAccepted = IsAccepted;

                        if (IsAccepted == true)
                        {
                            tblOrderItems.OrderStage = 2;// two value for design stage set up in db
                            JobType = "Accepted";
                            if (tblOrderItems.DesignerId == null)
                            {
                                JobType = "JobPicked";
                            }
                            tblOrderItems.DesignerId = WebSecurity.CurrentUserId;
                        }
                        if (IsAccepted == false)
                        {
                            tblOrderItems.IsItemStatus = false;
                            JobType = "Rejected";

                        }
                        string user = string.Empty, eMailAddress = string.Empty, type = string.Empty;
                        tblDesigner Objtbl = GetDesignerInfoById(DesignerId);
                        if (Objtbl != null)
                        {
                            eMailAddress = Objtbl.EmailId;

                            user = Objtbl.DesignerFirstName + " " + Objtbl.DesigenerLastName;
                        }
                        GetDesignernamebyId(DesignerId);
                        NotificationEmail(user, eMailAddress, type, tblOrders.ProjectName, JobType);
                        result = "done";
                    }

                    objdb.SaveChanges();
                }




            }
            return Json(new { Message = result });

        }


        [HttpPost]
        public ActionResult CloseDesignerJob(int OrderId)
        {
            string result = "Fail";
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var tblOrderItems = objdb.tblOrderItems.Where(REC => REC.ID == OrderId).FirstOrDefault();

                if (tblOrderItems != null)
                {
                    // Set Accepted
                    tblOrderItems.IsDesignerJobClosed = true;
                    objdb.SaveChanges();
                }



                return Json(new { result = result });

            }
        }



        [HttpGet]
        public ActionResult Edit(int? newJob, string id)
        {
            int Id =0;
            try
            {
                Id = Encdy.Decode(id);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }
            CommonController ObjCommonFunction = new CommonController();
            List<OrderModel> ListOrderModel = new List<OrderModel>();
            int? page = 1;
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            tblOrder orderTable = objdb.tblOrders.Where(p => p.pkOrderId == Id).SingleOrDefault();
            OrderAssignment ordAssignment = new OrderAssignment();
            string contentfile = string.Empty;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            int CurrentUserId = WebSecurity.CurrentUserId;
            var ObjOrderItem1 = objdb.tblOrderItems.Where(rec => rec.OrderID == Id).ToList();

            var byUserId = ObjOrderItem1.Where(rec => rec.DesignerId == CurrentUserId).Count();
            var byisnull = ObjOrderItem1.Where(rec => rec.DesignerId == null).Count();
            var total = byisnull + byUserId;

            bool isfullorderaccept = false;

            if (total == ObjOrderItem1.Count())
            {
                isfullorderaccept = true;
            }


            int CurrentDesignerID = WebSecurity.CurrentUserId;
            if (orderTable != null)
            {
                ordAssignment.Comments = objdb.OrderAssignments.Where(t => t.OrderNumber == orderTable.OrderNumber).Select(t => t.Comments).FirstOrDefault();
                ordAssignment.OrderStage = objdb.OrderAssignments.Where(t => t.OrderNumber == orderTable.OrderNumber).Select(t => t.OrderStage).FirstOrDefault();
                int OrderNumber = Convert.ToInt32(orderTable.OrderNumber);
                var tblOrderContents = objdb.tblOrderContents.Where(rec => rec.OrderNumber == OrderNumber).FirstOrDefault();
                if (tblOrderContents != null)
                {
                    contentfile = tblOrderContents.Files;
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
                GetleftMenu(model, CurrentDesignerID);
                model.pkOrderId = orderTable.pkOrderId;
                model.ProjectName = orderTable.ProjectName;
                model.ProjectType = orderTable.ProjectType;
                model.IsAcceptFullOrder = isfullorderaccept;
                model.ClientName = orderTable.ClientName;
                model.DesignerId = orderTable.DesignerId;
                model.Email = orderTable.Email;
                model.IsAccepted = orderTable.IsAccepted;
                model.IsRejected = orderTable.IsRejected;
                model.IsStatus = orderTable.IsStatus;
                model.OrderDate = Convert.ToDateTime(orderTable.OrderDate);
                model.OrderNumber = Convert.ToInt32(orderTable.OrderNumber);
                model.Phone = orderTable.Phone;
                model.OrderStage = OrderStage;
                model.Comment = ordAssignment.Comments;
                model.PkStageId = ordAssignment.OrderStage;
                model.StageList = objdb.tblOrderStages.ToList();
                model.ddlOrderList = ddlOrders(WebSecurity.CurrentUserId);
                model.ContentFile = contentfile;
                model.GetOrderItems = GetOrderItemByOrderId(Id, newJob);
                model.OrderPrrof = objdb.tblOrderProofs.AsEnumerable().Where(rec => rec.OrderNumber == Convert.ToInt32(orderTable.OrderNumber)).ToList();
                // model.CusomerReviewList = objdb.tblCustomerOrderReviews.AsEnumerable().Where(rec => rec.OrderNumber == Convert.ToInt32(orderTable.OrderNumber)).ToList();
                ListOrderModel.Add(model);
            }
            IPagination pagedModel = ListOrderModel.AsPagination(page ?? 1, 10);
            return View(pagedModel);


        }

        public List<OrderItemModel> GetOrderItemByOrderId(int OrderId, int? newjob)
        {
            List<OrderItemModel> ObjList = new List<OrderItemModel>();
            List<tblOrderItem> OrderItem = new List<tblOrderItem>();
            int CurrentUserId = WebSecurity.CurrentUserId;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                if (newjob == 1)
                {
                    OrderItem = objdb.tblOrderItems.Where(rec => rec.OrderID == OrderId && rec.OrderStage == 1).ToList();
                }
                else
                {
                    OrderItem = objdb.tblOrderItems.Where(rec => rec.OrderID == OrderId && rec.DesignerId == CurrentUserId).ToList();
                }

            }

            foreach (tblOrderItem item in OrderItem)
            {
                OrderItemModel objOrderItemModel = new OrderItemModel();

                objOrderItemModel.ID = item.ID;
                objOrderItemModel.OrderID = item.OrderID;
                objOrderItemModel.ItemName = item.ItemName;
                objOrderItemModel.Sku = item.Sku;
                objOrderItemModel.CreatedOn = item.CreatedOn;
                objOrderItemModel.DesignerId = item.DesignerId;
                objOrderItemModel.fkContentWritterId = item.fkContentWritterId;
                objOrderItemModel.ProjectName = item.ProjectName;
                objOrderItemModel.ClientName = item.ClientName;
                objOrderItemModel.ProjectType = item.ProjectType;
                objOrderItemModel.Phone = item.Phone;
                objOrderItemModel.Email = item.Email;
                objOrderItemModel.IsItemStatus = item.IsItemStatus;
                objOrderItemModel.IsItemRejected = item.IsItemRejected;
                objOrderItemModel.IsItemAccepted = item.IsItemAccepted;
                objOrderItemModel.IsContentAssign = (bool)item.IsContentAssign;
                objOrderItemModel.DesignerName = GetDesignernamebyId(Convert.ToInt32(item.DesignerId));
                objOrderItemModel.ContentWriterName = GetContentWriterbyId(Convert.ToInt32(item.fkContentWritterId));

                ObjList.Add(objOrderItemModel);
            }

            return ObjList;
        }
        public string GetDesignernamebyId(int Id)
        {
            string DesignerName = string.Empty;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var Designer = objdb.tblDesigners.Where(rec => rec.PkDesignerId == Id).FirstOrDefault();
                if (Designer != null)
                {
                    DesignerName = Designer.DesignerFirstName + " " + Designer.DesigenerLastName;
                }
            }

            return DesignerName;
        }

        public tblDesigner GetDesignerInfoById(int Id)
        {
            tblDesigner Designer = new tblDesigner();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                Designer = objdb.tblDesigners.Where(rec => rec.PkDesignerId == Id).FirstOrDefault();

            }

            return Designer;
        }
        public string GetContentWriterbyId(int Id)
        {
            string ContentWriterName = string.Empty;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var Designer = objdb.tblContentWriters.Where(rec => rec.PkContentWritterId == Id).FirstOrDefault();
                if (Designer != null)
                {
                    ContentWriterName = Designer.FirstName + " " + Designer.LastName;
                }
            }
            return ContentWriterName;
        }
        public FileResult DownloadSelectedFile(string ImageName)
        {
            return File(Server.MapPath("~/Images/proofUpload/") + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet);
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
                objOrderList.OrderNumber = Convert.ToInt32(item.OrderNumber);
                objOrderList.pkOrderId = item.pkOrderId;
                ListOfDesigner.Add(objOrderList);
            }

            return ListOfDesigner;

        }
        [HttpPost]
        public ActionResult Edit(OrderModel model)
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            tblOrder obj = objdb.tblOrders.Where(i => i.pkOrderId == model.pkOrderId).FirstOrDefault();
            obj.IsAccepted = model.IsAccepted;
            objdb.SaveChanges();
            return View(model);

        }

        private void SendAdminNotificationEmail(string EmailAddress, IEnumerable<HttpPostedFileBase> files, IEnumerable<HttpPostedFileBase> files2, String UName, Int32 OrderNumber)
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
                MailAddress to = new MailAddress(EmailAddress);// new MailAddress(eMailAddress); 
                // MailAddress from = new MailAddress("lakhvinder.happy@gmail.com");
                // MailAddress to = new MailAddress("lakhvinder.kumar@team.amonous.com");// new MailAddress(eMailAddress); 

                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = "Proof Uploaded Notification";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;


                //foreach (HttpPostedFileBase item2 in files)
                //{
                //    if (item2.ContentLength > 0)
                //    {
                //        myMail.Attachments.Add(new Attachment(item2.InputStream, "FrontProof"));
                //    }
                //}
                //foreach (HttpPostedFileBase item2 in files2)
                //{
                //    if (item2 != null)
                //    {
                //        myMail.Attachments.Add(new Attachment(item2.InputStream, "BackProof"));
                //    }
                //}





                var emailBody = string.Empty;

                emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/ProofUploadedAdmin.html")).ReadToEnd().ToString();
                emailBody = emailBody.ToString().Replace("##DESIGNER##", UName).Replace("##PROJECT##", Convert.ToString(OrderNumber));
                var emailTemplate = emailBody;






                myMail.Body = emailTemplate;
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
        private void SendCustomerNotificationEmail(string EmailAddress, IEnumerable<HttpPostedFileBase> files, IEnumerable<HttpPostedFileBase> files2)
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
                MailAddress to = new MailAddress(EmailAddress);// new MailAddress(eMailAddress); 
                // MailAddress from = new MailAddress("lakhvinder.happy@gmail.com");
                // MailAddress to = new MailAddress("lakhvinder.kumar@team.amonous.com");// new MailAddress(eMailAddress); 

                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = "Proof Uploaded Notification";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                foreach (HttpPostedFileBase item2 in files)
                {
                    myMail.Attachments.Add(new Attachment(item2.InputStream, "FrontProof"));
                }
                foreach (HttpPostedFileBase item2 in files2)
                {
                    myMail.Attachments.Add(new Attachment(item2.InputStream, "BackProof"));
                }





                var emailBody = string.Empty;

                emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/ProofUploaded.html")).ReadToEnd().ToString();

                var emailTemplate = emailBody;






                myMail.Body = emailTemplate;
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

                var emailBody = string.Empty;
                if (JobType == "Accepted")
                {
                    emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/Accepted.html")).ReadToEnd().ToString();
                }
                else if (JobType == "Rejected")
                {
                    emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/Rejected.html")).ReadToEnd().ToString();
                }
                else if (JobType == "JobPicked")
                {
                    emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/JobPicked.html")).ReadToEnd().ToString();
                }
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


        [HttpPost]
        public JsonResult SearchOrder(OrderModel model)
        {
            pEasyPrintEntities objdb = new pEasyPrintEntities();
            bool currnetUser = WebSecurity.IsCurrentUser(WebSecurity.CurrentUserName);
            if (currnetUser)
            {
                tblOrder order = objdb.tblOrders.AsEnumerable().Where(p => Convert.ToInt32(p.OrderNumber) == model.OrderNumber).FirstOrDefault();
                RedirectToActionPermanent("Edit", order.pkOrderId);
            }
            else
            {
                return Json(new { Status = "fail" });
            }
            return Json(new { Status = "succuss" });
        }
    }
}
