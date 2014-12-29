//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace pEasyPrint
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOrderItem
    {
        public int ID { get; set; }
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
        public string AdminAssignFile { get; set; }
        public string Email { get; set; }
        public Nullable<bool> IsItemStatus { get; set; }
        public Nullable<bool> IsItemRejected { get; set; }
        public Nullable<bool> IsItemAccepted { get; set; }
        public Nullable<bool> IsContentAssign { get; set; }
        public Nullable<bool> IsContentWriterAccepted { get; set; }
        public Nullable<bool> IsProofApproved { get; set; }
        public Nullable<bool> IsContentWriterJobClosed { get; set; }
        public string ItemFrontComment { get; set; }
        public Nullable<System.DateTime> AssignDate { get; set; }
        public string CustomerUploadedFiles { get; set; }
        public string ItemBackComment { get; set; }
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
        public string JobType { get; set; }
        public string AdminCommentForContent { get; set; }
        public Nullable<int> OrderStage { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsDesignerJobClosed { get; set; }
        public Nullable<bool> IsContentRequset { get; set; }
        public string fileupload1 { get; set; }
        public string fileupload2 { get; set; }
        public string fileupload3 { get; set; }
        public string fileupload4 { get; set; }
        public string fileupload5 { get; set; }
        public string front { get; set; }
        public string Back { get; set; }
        public string PrintingPrice { get; set; }
        public string DesignService { get; set; }
        public string TotalPrice { get; set; }
        public string generalInstruction { get; set; }
        public string TemplateFile { get; set; }
        public string DesignInstructionsBack { get; set; }
        public string DesignInstructionsFront { get; set; }
        public string CustEmail { get; set; }
        public string CategoryName { get; set; }
        public string Binding { get; set; }
        public string CardColor { get; set; }
        public string Cover { get; set; }
        public string FileDeliveryMethod { get; set; }
        public string Finish { get; set; }
        public string GeneralInstructions { get; set; }
        public string GlossUVCoating { get; set; }
        public string Grommet { get; set; }
        public string Hem { get; set; }
        public string HoleDrilling { get; set; }
        public string HolePosition { get; set; }
        public string H_Stakes { get; set; }
        public string Material { get; set; }
        public string NumberingColor { get; set; }
        public string NumberingPosition { get; set; }
        public string PageOrientation { get; set; }
        public string Pages { get; set; }
        public string Panel { get; set; }
        public string Perforation_Numbering { get; set; }
        public string Pocket { get; set; }
        public string PolePocket { get; set; }
        public string PrintingTurnaroundTime { get; set; }
        public string RoundCorners { get; set; }
        public string Shape { get; set; }
        public string Sheets { get; set; }
        public string Subtotal { get; set; }
        public string Tray { get; set; }
        public string UVCoating { get; set; }
        public string WindPosition { get; set; }
        public string Windslit { get; set; }
        public string WithWindow { get; set; }
        public string DownloadUrl { get; set; }
        public string DownloadUrl2 { get; set; }
        public string DownloadUrl3 { get; set; }
        public string DownloadUrl4 { get; set; }
        public string CustomFile1 { get; set; }
        public string CustomFile2 { get; set; }
        public string CustomFile3 { get; set; }
    }
}