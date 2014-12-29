using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using pEasyPrint.Models;


namespace pEasyPrint.Areas.Admin.Models
{
    public class DesignerModel
    {
        public int DesignerID { get; set; }
        [DisplayName("Designer FirstName :")]
        [Required]
        public string DesignerFirstName { get; set; }
        [DisplayName("Desigener LastName :")]
        [Required]
        public string DesigenerLastName { get; set; }
        [Required(ErrorMessage = "Select Sex")]
        public string Sex { get; set; }
        [DisplayName("Designer Experience :")]
        [Required]
        public int? DesignerExperience { get; set; }
        [DisplayName("Designer Address :")]
        [Required]
        public string DesignerAddress { get; set; }
        //  [DisplayName("City :")]
        // [Required]
        [Required(ErrorMessage = "Select City")]
        public int? City { get; set; }
        //[DisplayName("State :")]
        // [Required]
        [Required(ErrorMessage = "Select State")]
        public int? State { get; set; }
        //   [DisplayName("Country :")]
        // [Required]
        [Required(ErrorMessage = "Select Country")]
        public int? Country { get; set; }
        [DisplayName("Zip :")]
        [Required]
        public int? Zip { get; set; }
        [DisplayName("Mobile :")]
        [Required]
        public string Mobile { get; set; }
        public int Landline { get; set; }
        [DisplayName("EmailId :")]
        [Required]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Select Date")]
        public DateTime? AddedOn { get; set; }
        public string AddedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Photo { get; set; }
        public List<tblDesigner> ListOfDesigner { get; set; }
        public string CountryName { get; set; }
        public string SkypeId { get; set; }

        public int PkStateId { get; set; }
        public string StateName { get; set; }
        public int PkCityId { get; set; }
        public string CityName { get; set; }
        public List<tblCountry> ListCountry { get; set; }
        public List<timeddl> timeValue { get; set; }
        public List<tblState> ListState { get; set; }
        public List<tblCity> ListCity { get; set; }
        public OrderModel objOrderModel { get; set; }

        public string HoursAvailable { get; set; }
        public string HoursAvailableFrom { get; set; }
        public string HoursAvailableTo { get; set; }




    }
    public class timeddl
    {
        public string Timetext { get; set; }
        public string TimeValue { get; set; }
    }

    public class OrderModel
    {

        public int pkOrderId { get; set; }
        public int parentId { get; set; }
        public int OrderStage { get; set; }
        public string OrderNumber { get; set; }
        public int OrderNumber1 { get; set; }

        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string StageStatus { get; set; }
        public string ProjectType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string OrderStatus { get; set; }

        public bool? IsStatus { get; set; }
        public bool? IsRejected { get; set; }
        public bool? IsAccepted { get; set; }
        public DateTime? OrderDate { get; set; }
        public bool? IsContentWriterJobClosed { get; set; }
        public bool? IsAcceptFullOrder { get; set; }


        public string product { get; set; }
        public string qty { get; set; }
        public string zipcode { get; set; }
        public string color { get; set; }
        public string quantity { get; set; }
        public string paper { get; set; }
        public string size { get; set; }
        public string turnaround { get; set; }
        public string country { get; set; }
        public string folding { get; set; }
        public string agree { get; set; }
        public string uenc { get; set; }



        public int? DesignerId { get; set; }
        public int? PkContentWritterId { get; set; }
        public string ContentFile { get; set; }
        public string DesignerName { get; set; }
        public bool IsContentAssign { get; set; }
        public bool IsContentWriterAccepted { get; set; }


        public int AssignOrder { get; set; }
        public int UnAssignOrder { get; set; }

        public int? TotalEarnings { get; set; }
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
        public int PrintJobsCount { get; set; }

        public string CategoryType { get; set; }

        public List<tblOrderProof> objtblOrderProof { get; set; }
        public List<DesingerList> DesingerList { get; set; }
        public List<ConentWritterList> ContentWriterList { get; set; }
        public List<tblOrderStage> StageList { get; set; }


        public List<ddlOrderList> ddlOrderList { get; set; }
        public List<tblOrder> OrderList { get; set; }

        public int DesignerID { get; set; }
        public string DesignerFirstName { get; set; }
        public string DesigenerLastName { get; set; }
        public string Sex { get; set; }
        public int? DesignerExperience { get; set; }
        public string DesignerAddress { get; set; }
        public int? City { get; set; }
        public int? State { get; set; }
        public int? Country { get; set; }
        public int? Zip { get; set; }
        public string Mobile { get; set; }
        public int Landline { get; set; }
        public string EmailId { get; set; }
        public DateTime? AddedOn { get; set; }
        public string AddedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Photo { get; set; }

        public List<OrderItemModel> GetOrderItems { get; set; }
        public string CountryName { get; set; }
        public int PkStateId { get; set; }
        public string StateName { get; set; }
        public int PkCityId { get; set; }
        public string CityName { get; set; }
        public string CustomEmail { get; set; }

        public List<tblCountry> ListCountry { get; set; }
        public List<tblState> ListState { get; set; }
        public List<tblCity> ListCity { get; set; }
        public List<tblDesigner> ListOfDesigner { get; set; }

        public List<tblOrderProof> ListOrderProof { get; set; }



        public List<OrderModel> ListOrderModel { get; set; }

        public OrderItemModel OrderItem { get; set; }



    }
    

    public class DesingerList
    {
        public string DesignerName { get; set; }
        public int DesignerId { get; set; }

    }
    public class ConentWritterList
    {
        public string WriterName { get; set; }
        public int pkWriterId { get; set; }

    }
    public class ddlOrderList
    {
        public int pkOrderId { get; set; }
        public string OrderNumber { get; set; }

    }
}