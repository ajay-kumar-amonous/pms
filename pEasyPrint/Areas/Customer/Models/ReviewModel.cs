using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pEasyPrint.Areas.Designer.Models;

namespace pEasyPrint.Areas.Customer.Models
{
    public class ReviewModel
    {
        public List<tblOrderProof> ListOrderProofs { get; set; }
        public OrderModel OrderInfo { get; set; }
     }
}