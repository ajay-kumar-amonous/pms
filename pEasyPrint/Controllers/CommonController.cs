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
using pEasyPrint.Models;
using System.Net.Mail;
using System.Configuration;
namespace pEasyPrint.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/

        public ActionResult Index()
        {
            return View();
        }
        public OrderModel GetLeftMenuContent(OrderModel objOrderModel)
        {

            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {

                var Unassign = (from t1 in objdb.tblOrders
                                join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                where t2.OrderStage ==1 && t2.DesignerId == null
                                select new
                                {
                                    t1.pkOrderId,
                                }).GroupBy(x => new { x.pkOrderId }).ToList();
                var AssignOrder = (from t1 in objdb.tblOrders
                                   join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                   where t2.OrderStage==1 && t2.DesignerId !=null
                                   select new
                                   {
                                       t1.pkOrderId,
                                   }).GroupBy(x => new { x.pkOrderId }).ToList();
                objOrderModel.AssignOrder = AssignOrder.Count();

                objOrderModel.UnAssignOrder = Unassign.Count();

                var ProofStagerec = (from t1 in objdb.tblOrders
                                     join t2 in objdb.OrderAssignments on t1.OrderNumber equals t2.OrderNumber
                                     where t2.OrderStage == 3
                                     select new
                                     {
                                         t1.pkOrderId,
                                     }).ToList();

                var DesignStage = (from t1 in objdb.tblOrders
                                     join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                     where t2.OrderStage == 2
                                     select new     {
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
                                     }).ToList();

                var NewStage = (from t1 in objdb.tblOrders
                                   join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                   where t2.OrderStage == 1
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
                                   }).ToList();



                var ProofStage = (from t1 in objdb.tblOrders
                                   join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                   where t2.OrderStage == 3
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
                                   }).ToList();

                var ApprovedStage = (from t1 in objdb.tblOrders
                                  join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                  where t2.OrderStage == 4
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
                                  }).ToList();

                var PrintingStage = (from t1 in objdb.tblOrders
                                     join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                     where t2.OrderStage == 5
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
                                     }).ToList();


                var CompleteStage = (from t1 in objdb.tblOrders
                                     join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                     where t2.OrderStage == 6
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
                                     }).ToList();


                var PrintJobs = (from t1 in objdb.tblOrders
                                     join t2 in objdb.tblOrderItems on t1.pkOrderId equals t2.OrderID
                                 where t2.JobType == "Printing"
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
                                     }).ToList();
                objOrderModel.NewOrderCount = NewStage.Count();
                objOrderModel.DesignStagerCount = DesignStage.Count();
                objOrderModel.ProofStageCount =ProofStage.Count();
                objOrderModel.ApprovedCount = ApprovedStage.Count();
                objOrderModel.PrintingCount = PrintingStage.Count();
                objOrderModel.CompletedCount = CompleteStage.Count();
                objOrderModel.PrintJobsCount = PrintJobs.Count(); 

                objOrderModel.DesingerList = ddldesigner();
                objOrderModel.ddlOrderList = ddlOrders();
            }
            return objOrderModel;
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

        public List<OrderItemModel> GetOrderItemBypKOrderId(int OrderId)
        {
            List<OrderItemModel> ObjList = new List<OrderItemModel>();
            List<tblOrderItem> OrderItem = new List<tblOrderItem>();
           
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                OrderItem = objdb.tblOrderItems.Where(rec => rec.ID == OrderId).ToList();
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
                objOrderItemModel.DesignerName = GetDesignernamebyId(Convert.ToInt32(item.DesignerId));
                objOrderItemModel.ContentWriterName = GetDesignernamebyId(Convert.ToInt32(item.fkContentWritterId));
                objOrderItemModel.fkContentWritterId = item.fkContentWritterId;
                objOrderItemModel.ProjectName = item.ProjectName;
                objOrderItemModel.ClientName = item.ClientName;
                objOrderItemModel.ProjectType = item.ProjectType;
                objOrderItemModel.Phone = item.Phone;
                objOrderItemModel.Email = item.Email;
                objOrderItemModel.IsItemStatus = item.IsItemStatus;
                objOrderItemModel.IsItemRejected = item.IsItemRejected;
                objOrderItemModel.IsItemAccepted = item.IsItemAccepted;
                objOrderItemModel.IsContentWriterAccepted = item.IsContentWriterAccepted;
                objOrderItemModel.IsContentWriterJobClosed = item.IsContentWriterJobClosed;
                objOrderItemModel.IsContentAssign = (bool)item.IsContentAssign;
                objOrderItemModel.product = item.product;
                objOrderItemModel.qty = item.qty;
                objOrderItemModel.zipcode = item.zipcode;
                objOrderItemModel.color = item.color;
                objOrderItemModel.CardColor = item.CardColor;
                objOrderItemModel.quantity = item.quantity;
                objOrderItemModel.paper = item.paper;
                objOrderItemModel.size = item.size;
                objOrderItemModel.turnaround = item.turnaround;
                objOrderItemModel.country = item.country;
                objOrderItemModel.folding = item.folding;
                objOrderItemModel.agree = item.agree;
                objOrderItemModel.uenc = item.uenc;
                objOrderItemModel.JobType = item.JobType;
                objOrderItemModel.AdminCommentforContent = item.AdminCommentForContent;
                objOrderItemModel.Comments = item.Comments;
                objOrderItemModel.PkStageId = item.OrderStage.ToString();
                objOrderItemModel.IsDesignerJobClosed = item.IsDesignerJobClosed;
                objOrderItemModel.IsContentRequset = item.IsContentRequset;
                objOrderItemModel.StageName = GetStageNamebyId(item.OrderStage.ToString());
                objOrderItemModel.TemplateFile = item.TemplateFile;
                objOrderItemModel.ItemBackComment = item.ItemBackComment;
                objOrderItemModel.ItemFrontComment = item.ItemFrontComment;
                objOrderItemModel.CustomerUploadedFiles = item.CustomerUploadedFiles;


                objOrderItemModel.fileupload1            =item.fileupload1                ;
                objOrderItemModel.fileupload2            =item.fileupload2                ;
                objOrderItemModel.fileupload3            =item.fileupload3                ;
                objOrderItemModel.front                  =item.front                      ;
                objOrderItemModel.Back                   =item.Back                       ;
                objOrderItemModel.PrintingPrice          =item.PrintingPrice              ;
                objOrderItemModel.TotalPrice             =item.TotalPrice                 ;
                objOrderItemModel.generalInstruction     =item.generalInstruction         ;
                objOrderItemModel.TemplateFile           =item.TemplateFile               ;
                objOrderItemModel.DesignInstructionsBack =item.DesignInstructionsBack     ;
                objOrderItemModel.DesignInstructionsFront = item.DesignInstructionsFront;
                objOrderItemModel.AdminAssignFile = item.AdminAssignFile;


                ObjList.Add(objOrderItemModel);
            }

            return ObjList;
        }

        public List<OrderItemModel> GetOrderItemBypKOrderIdForDesigner(int OrderId)
        {
            List<OrderItemModel> ObjList = new List<OrderItemModel>();
            List<tblOrderItem> OrderItem = new List<tblOrderItem>();
            int CurrentUserId = WebSecurity.CurrentUserId;
            string CurrentUserName = WebSecurity.CurrentUserName;


            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                OrderItem = objdb.tblOrderItems.Where(rec => rec.ID == OrderId ).ToList();
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
                objOrderItemModel.DesignerName = GetDesignernamebyId(Convert.ToInt32(item.DesignerId));
                objOrderItemModel.ContentWriterName = GetDesignernamebyId(Convert.ToInt32(item.fkContentWritterId));
                objOrderItemModel.fkContentWritterId = item.fkContentWritterId;
                objOrderItemModel.ProjectName = item.ProjectName;
                objOrderItemModel.ClientName = item.ClientName;
                objOrderItemModel.ProjectType = item.ProjectType;
                objOrderItemModel.Phone = item.Phone;
                objOrderItemModel.Email = item.Email;
                objOrderItemModel.IsItemStatus = item.IsItemStatus;
                objOrderItemModel.IsItemRejected = item.IsItemRejected;
                objOrderItemModel.IsItemAccepted = item.IsItemAccepted;
                objOrderItemModel.IsContentWriterAccepted = item.IsContentWriterAccepted;
                objOrderItemModel.IsContentWriterJobClosed = item.IsContentWriterJobClosed;
                objOrderItemModel.IsContentAssign = (bool)item.IsContentAssign;
                objOrderItemModel.product = item.product;
                objOrderItemModel.qty = item.qty;
                objOrderItemModel.zipcode = item.zipcode;
                objOrderItemModel.color = item.color;
                objOrderItemModel.CardColor = item.CardColor;
                objOrderItemModel.quantity = item.quantity;
                objOrderItemModel.paper = item.paper;
                objOrderItemModel.size = item.size;
                objOrderItemModel.turnaround = item.turnaround;
                objOrderItemModel.country = item.country;
                objOrderItemModel.folding = item.folding;
                objOrderItemModel.agree = item.agree;
                objOrderItemModel.uenc = item.uenc;
                objOrderItemModel.JobType = item.JobType;
                objOrderItemModel.AdminCommentforContent = item.AdminCommentForContent;
                objOrderItemModel.Comments = item.Comments;
                objOrderItemModel.PkStageId = item.OrderStage.ToString();
                objOrderItemModel.IsDesignerJobClosed = item.IsDesignerJobClosed;
                objOrderItemModel.IsContentRequset = item.IsContentRequset;
                objOrderItemModel.StageName = GetStageNamebyId(item.OrderStage.ToString());
                objOrderItemModel.TemplateFile = item.TemplateFile;
                objOrderItemModel.ItemBackComment = item.ItemBackComment;
                objOrderItemModel.ItemFrontComment = item.ItemFrontComment;
                objOrderItemModel.CustomerUploadedFiles = item.CustomerUploadedFiles;


                objOrderItemModel.fileupload1 = item.fileupload1;
                objOrderItemModel.fileupload2 = item.fileupload2;
                objOrderItemModel.fileupload3 = item.fileupload3;
                objOrderItemModel.front = item.front;
                objOrderItemModel.Back = item.Back;
                objOrderItemModel.PrintingPrice = item.PrintingPrice;
                objOrderItemModel.TotalPrice = item.TotalPrice;
                objOrderItemModel.generalInstruction = item.generalInstruction;
                objOrderItemModel.TemplateFile = item.TemplateFile;
                objOrderItemModel.DesignInstructionsBack = item.DesignInstructionsBack;
                objOrderItemModel.DesignInstructionsFront = item.DesignInstructionsFront;
                objOrderItemModel.AdminAssignFile = item.AdminAssignFile;
                objOrderItemModel.DownloadUrl = item.DownloadUrl;
                objOrderItemModel.DownloadUrl2 = item.DownloadUrl2;
                objOrderItemModel.DownloadUrl3 = item.DownloadUrl3;
                objOrderItemModel.DownloadUrl4 = item.DownloadUrl4;


                ObjList.Add(objOrderItemModel);
            }

            return ObjList;
        }

        public ActionResult JobAssignedTemplate()
        {
            JobAssignEmailConfirmationModel objModel = new JobAssignEmailConfirmationModel();

            return View(objModel);
        }

        public List<OrderItemModel> GetOrderItemByOrderIdbyCurrentUserId(int OrderId)
        {
            List<OrderItemModel> ObjList = new List<OrderItemModel>();
            List<tblOrderItem> OrderItem = new List<tblOrderItem>();
            int CurrentUserId= WebSecurity.CurrentUserId;
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                OrderItem = objdb.tblOrderItems.Where(rec => rec.OrderID == OrderId && rec.fkContentWritterId == CurrentUserId).ToList();
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
                objOrderItemModel.DesignerName = GetDesignernamebyId(Convert.ToInt32(item.DesignerId));
                objOrderItemModel.ContentWriterName = GetContentWriterbyId(Convert.ToInt32(item.fkContentWritterId));
                objOrderItemModel.fkContentWritterId = item.fkContentWritterId;
                objOrderItemModel.ProjectName = item.ProjectName;
                objOrderItemModel.ClientName = item.ClientName;
                objOrderItemModel.ProjectType = item.ProjectType;
                objOrderItemModel.Phone = item.Phone;
                objOrderItemModel.Email = item.Email;
                objOrderItemModel.IsItemStatus = item.IsItemStatus;
                objOrderItemModel.IsItemRejected = item.IsItemRejected;
                objOrderItemModel.IsItemAccepted = item.IsItemAccepted;
                objOrderItemModel.IsContentWriterAccepted = item.IsContentWriterAccepted;
                objOrderItemModel.IsContentWriterJobClosed = item.IsContentWriterJobClosed;
                objOrderItemModel.IsContentAssign = (bool)item.IsContentAssign;
                objOrderItemModel.product = item.product;
                objOrderItemModel.qty = item.qty;
                objOrderItemModel.zipcode = item.zipcode;
                objOrderItemModel.color = item.color;
                objOrderItemModel.CardColor = item.CardColor;
                objOrderItemModel.quantity = item.quantity;
                objOrderItemModel.paper = item.paper;
                objOrderItemModel.size = item.size;
                objOrderItemModel.turnaround = item.turnaround;
                objOrderItemModel.country = item.country;
                objOrderItemModel.folding = item.folding;
                objOrderItemModel.agree = item.agree;
                objOrderItemModel.uenc = item.uenc;
                objOrderItemModel.JobType = item.JobType;
                objOrderItemModel.AdminCommentforContent = item.AdminCommentForContent;
                objOrderItemModel.PkStageId = item.OrderStage.ToString();
                objOrderItemModel.IsDesignerJobClosed = item.IsDesignerJobClosed;
                objOrderItemModel.IsContentRequset = item.IsContentRequset;
                objOrderItemModel.StageName = GetStageNamebyId(item.OrderStage.ToString());

                objOrderItemModel.ItemBackComment = item.ItemBackComment;
                objOrderItemModel.ItemFrontComment = item.ItemFrontComment;
                objOrderItemModel.CustomerUploadedFiles = item.CustomerUploadedFiles;




                ObjList.Add(objOrderItemModel);
            }

            return ObjList;
        }

        public string GetProjectNamebyID(int OrderID)
        {
            string ProjectName = string.Empty;
            using (pEasyPrintEntities Obj = new pEasyPrintEntities())
            {
                var record = Obj.tblOrderItems.Where(rec => rec.OrderID == OrderID).FirstOrDefault();
                if (record != null)
                {
                    ProjectName = record.ProjectName;
                }
            }
            return ProjectName;
        }


        public List<OrderItemModel> GetOrderItemByOrderId(int OrderId)
        {
            List<OrderItemModel> ObjList = new List<OrderItemModel>();
            List<tblOrderItem> OrderItem = new List<tblOrderItem>();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                OrderItem = objdb.tblOrderItems.Where(rec => rec.OrderID == OrderId).ToList();
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
                objOrderItemModel.DesignerName = GetDesignernamebyId(Convert.ToInt32(item.DesignerId));
                objOrderItemModel.ContentWriterName = GetContentWriterbyId(Convert.ToInt32(item.fkContentWritterId));
                objOrderItemModel.fkContentWritterId = item.fkContentWritterId;
                objOrderItemModel.ProjectName = item.ProjectName;
                objOrderItemModel.ClientName = item.ClientName;
                objOrderItemModel.ProjectType = item.ProjectType;
                objOrderItemModel.Phone = item.Phone;
                objOrderItemModel.Email = item.Email;
                objOrderItemModel.IsItemStatus = item.IsItemStatus;
                objOrderItemModel.IsItemRejected = item.IsItemRejected;
                objOrderItemModel.IsItemAccepted = item.IsItemAccepted;
                objOrderItemModel.IsContentWriterAccepted = item.IsContentWriterAccepted;
                objOrderItemModel.IsContentWriterJobClosed = item.IsContentWriterJobClosed;
                objOrderItemModel.IsContentAssign = (bool)item.IsContentAssign;
                objOrderItemModel.product = item.product;
                objOrderItemModel.qty = item.qty;
                objOrderItemModel.zipcode = item.zipcode;
                objOrderItemModel.color = item.color;
                objOrderItemModel.CardColor= item.CardColor;
                objOrderItemModel.quantity = item.quantity;
                objOrderItemModel.paper = item.paper;
                objOrderItemModel.size = item.size;
                objOrderItemModel.turnaround = item.turnaround;
                objOrderItemModel.country = item.country;
                objOrderItemModel.folding = item.folding;
                objOrderItemModel.agree = item.agree;
                objOrderItemModel.uenc = item.uenc;
                objOrderItemModel.JobType = item.JobType;
                objOrderItemModel.AdminCommentforContent = item.AdminCommentForContent;
                objOrderItemModel.PkStageId = item.OrderStage.ToString();
                objOrderItemModel.IsDesignerJobClosed = item.IsDesignerJobClosed;
                objOrderItemModel.IsContentRequset = item.IsContentRequset;
                objOrderItemModel.StageName = GetStageNamebyId(item.OrderStage.ToString());

                objOrderItemModel.ItemBackComment = item.ItemBackComment;
                objOrderItemModel.ItemFrontComment = item.ItemFrontComment;
                objOrderItemModel.CustomerUploadedFiles = item.CustomerUploadedFiles;
                objOrderItemModel.CustEmail = item.CustEmail;


                

                
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

        public string GetStageNamebyId(string  Id)
        {
            string StageName = string.Empty;
            int PkStageId = Convert.ToInt32(Id);
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
                var Designer = objdb.tblOrderStages.Where(rec => rec.PkStageId == PkStageId).FirstOrDefault();
                if (Designer != null)
                {
                    StageName = Designer.StageName;
                }
            }

            return StageName;
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



        public string GetStateNameById(int ? StateId)

        {
            string Result = string.Empty;
            try
            {
                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    var state = objdb.tblStates.Where(rec => rec.PkStateId == StateId).FirstOrDefault();
                    if (state != null)
                    {
                        Result = state.StateName;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return Result;
        }


        public string GetCityNameById(int ? CityId)
        {

            string Result = string.Empty;
            try
            {
                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    var state = objdb.tblCities.Where(rec => rec.PkCityId == CityId).FirstOrDefault();
                    if (state != null)
                    {
                        Result = state.CityName;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        public string GetCountryNameById(int ? CountryId)
        {
            string Result = string.Empty;
            try
            {
                using (pEasyPrintEntities objdb = new pEasyPrintEntities())
                {
                    var state = objdb.tblCountries.Where(rec => rec.PkCountryId == CountryId).FirstOrDefault();
                    if (state != null)
                    {
                        Result = state.CountryName;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

    }
}
