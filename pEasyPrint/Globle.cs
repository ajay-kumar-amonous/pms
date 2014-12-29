using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pEasyPrint
{
    public class Globle
    {
    }

    partial class OrderAssignment
    {
        public int MonthEarnings { get; set; }
        public int NewOrderCount { get; set; }
        public int DesignStagerCount { get; set; }
        public int ProofStageCount { get; set; }
        public int ApprovedCount { get; set; }
        public int PrintingCount { get; set; }
        public int CompletedCount { get; set; }
    }
}