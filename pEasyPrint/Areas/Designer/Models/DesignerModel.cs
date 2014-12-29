using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pEasyPrint.Models;


namespace pEasyPrint.Areas.Designer.Models
{
    public class DesignerModel
    {
        public int DesignerID { get; set; }
        public string DesignerFirstName { get; set; }
        public string DesigenerLastName { get; set; }
        public string Sex { get; set; }
        public int DesignerExperience { get; set; }
        public string DesignerAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int Zip { get; set; }
        public string Mobile { get; set; }
        public int Landline { get; set; }
        public string EmailId { get; set; }
        public DateTime AddedOn { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string Photo { get; set; }
        public List<tblDesigner> ListOfDesigner { get; set; }
    }

    public class OrderModel
    {

        public int pkOrderId { get; set; }
        public string OrderStage { get; set; }

        public int? OrderNumber { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ContentFile { get; set; }
        public bool? IsStatus { get; set; }
        public bool? IsRejected { get; set; }
        public bool? IsAccepted { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? DesignerId { get; set; }
        public bool IsAcceptFullOrder { get; set; }
        public string DesignerName { get; set; }
        public DateTime ? AssignDate { get; set; }


       


        
        public int?  TotalEarnings { get; set; }
        public int MonthEarnings { get; set; }
        public int NewOrdres { get; set; }
        public int InDesignStage { get; set; }
        public int Proof { get; set; }
        public int Approved { get; set; }
        public int InPrining { get; set; }
        public int Completed { get; set; }
        public int NewOrderCount { get; set; }
        public int DesignStagerCount { get; set; }
        public int ProofStageCount { get; set; }
        public int ApprovedCount { get; set; }
        public int PrintingCount { get; set; }
        public int CompletedCount { get; set; }
        public string Comment { get; set; }
        public List<tblOrderStage> StageList { get; set; }
        public int ? PkStageId { get; set; }
        public List<OrderItemModel> GetOrderItems { get; set; }
        public List<ddlOrderList> ddlOrderList { get; set; }

        public List<tblOrderProof> OrderPrrof { get; set; }
       // public List<tblCustomerOrderReview> CusomerReviewList { get; set; }

        public List<DesingerList> DesingerList { get; set; }
        public List<tblOrder> OrderList { get; set; }

        public OrderItemModel OrderItem { get; set; }
    }
   
    public class DesingerList
    {
        public string DesignerName { get; set; }
        public int DesignerId { get; set; }

    }
    public class ddlOrderList
    {
        public int pkOrderId { get; set; }
        public int? OrderNumber { get; set; }

    }
}