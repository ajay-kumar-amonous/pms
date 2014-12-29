using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pEasyPrint.Areas.Admin.Models;

namespace pEasyPrint.Models
{
    public class OrderProofViewModel
    {
        public string FrontComment { get; set; }
        public string BackComment { get; set; }
        public string DesignerFiles { get; set; }        
        public DateTime? CreatedDate { get; set; }
        public int? OrderNumber { get; set; }
        public string CustomerFrontComment { get; set; }
        public string CustomerBackComment { get; set; }
        public string CustomerFiles { get; set; }
        public int? ItemID { get; set; }
        public int? OrderID { get; set; }
        public string  ProjectName { get; set; }
        public bool? IsProofApproved { get; set; }
        public string ItemName { get; set; }


        public List<tblOrderProof> ItemProofs { get; set; }

    }
}