using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace pEasyPrint.Models
{
    public class OrderItemModel
    {
        public int ID { get; set; }
        public int OrderNumber { get; set; }
        public int OrderID { get; set; }
        public string ItemName { get; set; }
        public string Sku { get; set; }
        public string CreatedOn { get; set; }
        public Nullable<int> DesignerId { get; set; }
        public Nullable<int> fkContentWritterId { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string ProjectType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<bool> IsItemStatus { get; set; }
        public Nullable<bool> IsItemRejected { get; set; }
        public Nullable<bool> IsItemAccepted { get; set; }
        public Nullable<bool> IsContentWriterAccepted { get; set; }
        public Nullable<bool> IsContentWriterJobClosed { get; set; }


        public string DesignerName { get; set; }
        public string ContentWriterName { get; set; }
        public Nullable<bool> IsContentAssign { get; set; }

        public string product { get; set; }
        public string qty { get; set; }
        public string zipcode { get; set; }
        public string color { get; set; }
        public string CardColor { get; set; }
        public string quantity { get; set; }
        public string paper { get; set; }

        [Required]
        public string size { get; set; }
        public string turnaround { get; set; }
        public string country { get; set; }
        public string folding { get; set; }
        public string agree { get; set; }
        public string uenc { get; set; }
        public string JobType { get; set; }
        public List<OrderItemModel> ListOrderItemModel { get; set; }
        public string AdminCommentforContent { get; set; }
        public string Comments { get; set; }
        public string PkStageId { get; set; }
        public string ItemBackComment { get; set; }
        public string ItemFrontComment { get; set; }
        public string CustomerUploadedFiles { get; set; }
        public bool UVCoating { get; set; }
        public string Panel { get; set; }

        public string StageName { get; set; }
        public List<tblOrderProof> ItemProofs { get; set; }
        public Nullable<bool> IsDesignerJobClosed { get; set; }
        public Nullable<bool> IsContentRequset { get; set; }

        public string fileupload1                       { get; set; }
        public string fileupload2                    { get; set; }
        public string fileupload3                  { get; set; }
        public string front                        { get; set; }
        public string Back                           { get; set; }
        public string PrintingPrice              { get; set; }
        public string TotalPrice                { get; set; }
        public string generalInstruction           { get; set; }
        public string Binding { get; set; }
        public string Cover { get; set; }
        public string TemplateFile              { get; set; }
        public string DesignInstructionsBack   { get; set; }
        public string DesignInstructionsFront  { get; set; }
        public string AdminAssignFile  { get; set; }

        public string CustEmail { get; set; }

        public string DownloadUrl { get; set; }
        public string DownloadUrl2 { get; set; }
        public string DownloadUrl3 { get; set; }
        public string DownloadUrl4 { get; set; }
        public string BusinessName { get; set; }
        public string CustomFile1 { get; set; }
        public string CustomFile2 { get; set; }
        public string CustomFile3 { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingZip { get; set; }










    }

    public class JobAssignEmailConfirmationModel
    {
        public string UserName { get; set; }
        public string eMailAddress { get; set; }
        public string type { get; set; }
        public string ItemName { get; set; }


    }

}