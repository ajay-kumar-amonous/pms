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
    
    public partial class tblOrderProof
    {
        public int PkOrderProofId { get; set; }
        public string FrontComments { get; set; }
        public string BackComments { get; set; }
        public string DesignerBackFiles { get; set; }
        public string DesignerFiles { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string CustomerFrontComment { get; set; }
        public string CustomerBackComment { get; set; }
        public string AdminBackComment { get; set; }
        public string AdminFrontComment { get; set; }
        public string CustomerBackFiles { get; set; }
        public string CustomerFiles { get; set; }
        public Nullable<System.DateTime> CustomerCommentTime { get; set; }
        public Nullable<bool> IsProofSelected { get; set; }
        public Nullable<bool> IsCustomerApproved { get; set; }
        public Nullable<bool> IsAdminApproved { get; set; }
        public Nullable<int> ParentProofId { get; set; }
        public Nullable<System.DateTime> RejectedDate { get; set; }
    }
}