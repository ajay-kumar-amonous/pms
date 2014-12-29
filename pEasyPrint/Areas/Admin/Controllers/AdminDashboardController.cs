using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pEasyPrint.Areas.Admin.Models;
using MvcContrib.Pagination;
using pEasyPrint.Controllers;




namespace pEasyPrint.Areas.Admin.Controllers
{
     [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        CommonController objCommonController = new CommonController();
        // GET: /Admin/AdminDashboard/
        [HttpGet]
        [Authorize(Roles="Admin")]
        public ActionResult Index(int? page)
        {
            List<OrderModel> orders = GetOrderList();
            IPagination pagedModel = orders.AsPagination(page ?? 1, 10);
            return View(pagedModel);
            
        }

        public List<OrderModel> GetOrderList()
        {
            List<OrderModel> ObjtblOrderList = new List<OrderModel>();
            OrderModel ordOrderModelLeftMenu = new OrderModel();
            using (pEasyPrintEntities objdb = new pEasyPrintEntities())
            {
               
              objCommonController.GetLeftMenuContent(ordOrderModelLeftMenu);

                ObjtblOrderList.Add(ordOrderModelLeftMenu);
            }
            return ObjtblOrderList;
        }
     

    }
}
