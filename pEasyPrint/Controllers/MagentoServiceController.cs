using System.Linq;
using System.Web.Mvc;
using System.Net;
using pEasyPrint.MagentoService;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using pEasyPrint;
using System.Text.RegularExpressions;
using System.Collections;
using Conversive.PHPSerializationLibrary;
using WebMatrix.WebData;
using System.Web.Security;
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace pEasyPrint.Controllers
{


    public class MagentoServiceController : Controller
    {
        //
        // GET: /MagentoService/
        pEasyPrintEntities objdb = new pEasyPrintEntities();

        public ActionResult Index()
        {

            ServicePointManager.Expect100Continue = false;
            Mage_Api_Model_Server_V2_HandlerPortTypeClient ms = new Mage_Api_Model_Server_V2_HandlerPortTypeClient();
            string sessionId = ms.login("Andynguen", "peasy78377print");

            filters fil = new filters();
            tblOrder _orders = objdb.tblOrders.OrderByDescending(i => i.OrderNumber).FirstOrDefault();
            // List<salesOrderListEntity> orderlist = ms.salesOrderList(sessionId, fil).Where(i => Convert.ToDateTime(i.created_at) > (DateTime.Now.AddDays(-2))).ToList();
            List<salesOrderListEntity> orderlist = new List<salesOrderListEntity>();
            if (_orders != null)
            {
                orderlist = ms.salesOrderList(sessionId, fil).AsEnumerable().Where(i => Convert.ToInt64(i.increment_id) > Convert.ToInt64(_orders.OrderNumber)).ToList();
            }
            else
            {
                orderlist = ms.salesOrderList(sessionId, fil).ToList();
            }
            foreach (salesOrderListEntity oList in orderlist)
            {
                var increment_id = Convert.ToInt64(oList.increment_id);

                salesOrderEntity _salesOrderEntity = ms.salesOrderInfo(sessionId, oList.increment_id);
                salesOrderAddressEntity _salesOrderAddressEntityShipping = ms.salesOrderInfo(sessionId, oList.increment_id).shipping_address;
                salesOrderAddressEntity _salesOrderAddressEntityBilling = ms.salesOrderInfo(sessionId, oList.increment_id).billing_address;
                salesOrderItemEntity[] _salesOrderItemEntity = ms.salesOrderInfo(sessionId, oList.increment_id).items;

                Serializer serializer = new Serializer();

                //salesOrderShipmentEntity _salesOrderShipmentEntity = ms.salesOrderShipmentInfo(sessionId, _salesOrderAddressEntityShipping.increment_id);
                salesOrderPaymentEntity _salesOrderPaymentEntity = ms.salesOrderInfo(sessionId, oList.increment_id).payment;
                tblOrder _order = objdb.tblOrders.AsEnumerable().Where(i => i.OrderNumber == oList.increment_id).FirstOrDefault();
                if (_order == null)
                {
                    _order = new tblOrder();
                    _order.OrderNumber = oList.increment_id;
                    _order.ClientName = oList.customer_firstname;
                    //_order.ProjectName = oList.pro;
                    _order.Email = oList.customer_email;

                    // WebSecurity.CreateUserAndAccount(oList.customer_email, "123456");
                    // Roles.AddUserToRole(oList.customer_email, "Customer");

                    _order.Phone = oList.telephone.Replace(" ", "");
                    _order.OrderDate = oList.created_at;
                    objdb.tblOrders.Add(_order);
                    objdb.SaveChanges();

                    foreach (salesOrderItemEntity oItem in _salesOrderItemEntity)
                    {
                        tblOrderItem _OrderItem = new tblOrderItem();


                        Hashtable ht = (Hashtable)serializer.Deserialize(oItem.product_options);//Deserialize the Hashtable

                        Hashtable keys = (Hashtable)ht["info_buyRequest"];
                        ArrayList Itemoptions = (ArrayList)ht["options"];
                        if (Itemoptions != null)
                        {
                            foreach (Hashtable item in Itemoptions)
                            {
                                var value = item["value"].ToString();
                                var label = item["label"].ToString().ToLower().Trim();
                                _OrderItem.CustEmail = _order.Email;
                                _OrderItem.Email = _order.Email;
                                if (label == "size")
                                {
                                    _OrderItem.size = value.ToString();
                                }
                                if (label == "paper")
                                {
                                    _OrderItem.paper = value.ToString();
                                }
                                if (label.ToLower() == "perforation/numbering")
                                {
                                    _OrderItem.Perforation_Numbering = value.ToString();
                                }
                                if (label.ToLower() == "pocket")
                                {
                                    _OrderItem.Pocket = value.ToString();
                                }
                                if (label.ToLower() == "pole pocket")
                                {
                                    _OrderItem.PolePocket = value.ToString();
                                }
                                if (label == "card color")
                                {
                                    _OrderItem.CardColor = value.ToString();
                                }
                                if (label.ToLower() == "color")
                                {
                                    _OrderItem.color = value.ToString();
                                }
                                if (label.ToLower() == "cover")
                                {
                                    _OrderItem.Cover = value.ToString();
                                }
                                if (label.ToLower() == "round corners")
                                {
                                    _OrderItem.RoundCorners = value.ToString();
                                }
                                if (label.ToLower() == "shape")
                                {
                                    _OrderItem.Shape = value.ToString();
                                }
                                if (label.ToLower() == "sheets")
                                {
                                    _OrderItem.Sheets = value.ToString();
                                }
                                if (label.ToLower() == "category name")
                                {
                                    _OrderItem.CategoryName = value.ToString();

                                }
                                if (label == "quantity")
                                {
                                    _OrderItem.quantity = value.ToString();
                                    _OrderItem.qty = value.ToString();

                                }
                                if (label == "printing turnaround time")
                                {
                                    _OrderItem.turnaround = value.ToString();
                                }
                                if (label == "add files 1")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();

                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload1 = path1.ToString();
                                }

                                if (label == "add files 2")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();

                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload2 = path1.ToString();
                                }
                                if (label == "add logo")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();

                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload2 = path1.ToString();
                                }
                                // new cases

                                if (label == "file upload 1")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();

                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload1 = path1.ToString();
                                }
                                if (label == "file upload 2")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();

                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload2 = path1.ToString();

                                }
                                if (label == "file upload 3")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();
                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload3 = path1;

                                }
                                if (label == "file upload 4")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();
                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload4 = path1;

                                }
                                if (label == "file upload 5")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();
                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.fileupload5 = path1;

                                }
                                if (label == "front")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();
                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.front = path1.ToString();

                                }
                                if (label == "back")
                                {
                                    var path = value.Replace(";", "/");
                                    var value1 = item["option_value"].ToString();
                                    int start1 = value1.IndexOf("/media/custom_options/order") + 1;
                                    int end1 = value1.IndexOf(";s", start1);
                                    string result1 = value1.Substring(start1, end1 - start1);
                                    var path1 = result1.Replace("/", ";");
                                    _OrderItem.Back = path1.ToString();

                                }
                                if (label.ToLower() == "binding")
                                {

                                    _OrderItem.Binding = value.ToString();

                                }
                                if (label == "printing price")
                                {
                                    _OrderItem.PrintingPrice = value.ToString();

                                }
                                if (label == "design service")
                                {
                                    _OrderItem.DesignService = value.ToString();

                                }
                                if (label == "File Delivery Method")
                                {
                                    _OrderItem.FileDeliveryMethod = value.ToString();

                                }
                                if (label.ToLower() == "finish")
                                {
                                    _OrderItem.Finish = value.ToString();

                                }
                                if (label.ToLower() == "folding")
                                {
                                    _OrderItem.folding = value.ToString();

                                }
                                if (label == "subtotal")
                                {
                                    _OrderItem.TotalPrice = value.ToString();

                                }
                                if (label.ToLower() == "tray")
                                {
                                    _OrderItem.Tray = value.ToString();
                                }
                                if (label == "general instructions")
                                {
                                    _OrderItem.generalInstruction = value.ToString();

                                }
                                if (label.ToLower() == "gloss uv coating")
                                {
                                    _OrderItem.GlossUVCoating = value.ToString();

                                }
                                if (label.ToLower() == "uv coating")
                                {
                                    _OrderItem.UVCoating = value.ToString();

                                }
                                if (label.ToLower() == "wind position")
                                {
                                    _OrderItem.WindPosition = value.ToString();

                                }
                                if (label.ToLower() == "windslit")
                                {
                                    _OrderItem.Windslit = value.ToString();

                                }
                                if (label.ToLower() == "with window")
                                {
                                    _OrderItem.WithWindow = value.ToString();

                                }
                                if (label.ToLower() == "grommet")
                                {
                                    _OrderItem.Grommet = value.ToString();

                                }
                                if (label.ToLower() == "hem")
                                {
                                    _OrderItem.Hem = value.ToString();

                                }
                                if (label.ToLower() == "hole drilling")
                                {
                                    _OrderItem.HoleDrilling = value.ToString();

                                }
                                if (label.ToLower() == "hole position")
                                {
                                    _OrderItem.HolePosition = value.ToString();

                                }
                                if (label.ToLower() == "h-stakes")
                                {
                                    _OrderItem.H_Stakes = value.ToString();

                                }
                                if (label.ToLower() == "material")
                                {
                                    _OrderItem.Material = value.ToString();

                                }
                                if (label.ToLower() == "numbering color")
                                {
                                    _OrderItem.NumberingColor = value.ToString();

                                }
                                if (label.ToLower() == "numbering position")
                                {
                                    _OrderItem.NumberingPosition = value.ToString();

                                }
                                if (label.ToLower() == "page orientation")
                                {
                                    _OrderItem.PageOrientation = value.ToString();

                                }
                                if (label.ToLower() == "pages")
                                {
                                    _OrderItem.Pages = value.ToString();

                                }
                                if (label.ToLower() == "panel")
                                {
                                    _OrderItem.Panel = value.ToString();

                                }
                                if (label == "design instructions")
                                {
                                    _OrderItem.DesignInstructionsFront = value.ToString();
                                }
                                if (label == "design instructions back")
                                {
                                    _OrderItem.DesignInstructionsBack = value.ToString();
                                }
                                if (label == "project name")
                                {
                                    tblOrder _ordr = objdb.tblOrders.Where(i => i.OrderNumber == _order.OrderNumber).FirstOrDefault();
                                    _ordr.ProjectName = value.ToString();
                                    _OrderItem.ProjectName = value.ToString();

                                }

                                if (oItem.name.ToLower().StartsWith("upload"))
                                {
                                    _OrderItem.ProjectType = "Printing Job";
                                    _order.ProjectType = "Printing Job";
                                }
                                else
                                {
                                    _OrderItem.ProjectType = "Design Job";
                                    _order.ProjectType = "Design Job";
                                }



                            }
                        }

                        foreach (DictionaryEntry entry in keys)
                        {
                            if (entry.Key.ToString() == "product")
                            {
                                _OrderItem.product = entry.Value.ToString();
                            }
                            if (entry.Key.ToString() == "qty")
                            {
                                //  _OrderItem.qty = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "zipcode")
                            {
                                _OrderItem.zipcode = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "color_ddl")
                            {
                                // _OrderItem.color = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "quantity_ddl")
                            {
                                // _OrderItem.quantity = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "paper_ddl")
                            {
                                // _OrderItem.paper = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "size_ddl")
                            {
                                // _OrderItem.size = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "turnaround_ddl")
                            {
                                // _OrderItem.turnaround = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "country")
                            {
                                _OrderItem.country = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "options")
                            {
                                Hashtable options = (Hashtable)keys["options"];
                                if (options != null)
                                {
                                    foreach (DictionaryEntry oItems in options)
                                    {
                                        if (oItems.Value.ToString() == "10047")
                                        {

                                        }
                                        if (oItems.Value.ToString() == "10046")
                                        {

                                        }
                                        if (oItems.Value.ToString() == "10045")
                                        {

                                        }
                                    }
                                }

                            }
                            if (entry.Key.ToString() == "agree")
                            {
                                _OrderItem.agree = entry.Value.ToString();

                            }
                            if (entry.Key.ToString() == "uenc")
                            {
                                //   _OrderItem.uenc = entry.Value.ToString();


                            }
                            if (entry.Key.ToString() == "folding_ddl")
                            {
                                _OrderItem.folding = entry.Value.ToString();

                            }
                        }

                        string TemplateFile = oItem.name.ToLower().Replace(" ", "") + ".zip";
                        _OrderItem.OrderID = _order.pkOrderId;
                        _OrderItem.ItemName = oItem.name;


                        _OrderItem.Sku = oItem.sku;
                        _OrderItem.CreatedOn = oItem.created_at;
                        _OrderItem.IsItemAccepted = null;
                        _OrderItem.IsItemRejected = false;
                        _OrderItem.IsItemStatus = false;
                        _OrderItem.IsContentWriterAccepted = null;
                        _OrderItem.IsContentAssign = false;
                        _OrderItem.IsContentWriterJobClosed = false;
                        _OrderItem.IsDesignerJobClosed = false;
                        _OrderItem.IsContentRequset = false;
                        _OrderItem.OrderStage = 1;
                        _OrderItem.TemplateFile = TemplateFile;
                        if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "booklet")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.Binding + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "bookmark")
                        {
                            string corners = string.Empty;
                            if (string.IsNullOrEmpty(_OrderItem.RoundCorners))
                            {
                                corners = "square";
                            }
                            else
                            {
                                corners = _OrderItem.RoundCorners;
                            }
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + corners);
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "brochure")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.folding + "-inside");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.folding + "-outside");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "businesscard")
                        {
                            string corners = string.Empty;
                            if (string.IsNullOrEmpty(_OrderItem.RoundCorners))
                            {
                                corners = "square";
                            }
                            else
                            {
                                corners = _OrderItem.RoundCorners;
                            }
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + corners);
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "calendar")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "catalog")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.folding + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "cdpackage")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.Panel + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "clubflyer")
                        {
                            string corners = string.Empty;
                            if (string.IsNullOrEmpty(_OrderItem.RoundCorners))
                            {
                                corners = "square";
                            }
                            else
                            {
                                corners = _OrderItem.RoundCorners;
                            }
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + corners);
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "collectorcard")
                        {
                            string corners = string.Empty;
                            if (string.IsNullOrEmpty(_OrderItem.RoundCorners))
                            {
                                corners = "square";
                            }
                            else
                            {
                                corners = _OrderItem.RoundCorners;
                            }
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + corners);
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "doorhanger")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.folding + "-front");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.folding + "-back");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "dvdpackages")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "envelope")
                        {
                            string window = string.Empty;
                            if (!string.IsNullOrEmpty(_OrderItem.WithWindow))
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-window-front");
                                _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-window-bothsides");
                            }
                            else
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                                _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-bothsides");
                            }

                        }

                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "eventticket")
                        {
                           
                            if (_OrderItem.Perforation_Numbering.Replace(" ", "").ToLower() == "numberingfrontonly" || _OrderItem.Perforation_Numbering.Replace(" ", "").ToLower() == "numberingbackonly")
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-horizontal-" + _OrderItem.NumberingPosition + "-front");
                                _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-horizontal-" + _OrderItem.NumberingPosition + "-back");
                                _OrderItem.DownloadUrl3 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-vertical-" + _OrderItem.NumberingPosition + "-front");
                                _OrderItem.DownloadUrl4 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-vertical-" + _OrderItem.NumberingPosition + "-back");
                            }
                            else
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-horizontal-" + _OrderItem.NumberingPosition + "-perforated-front");
                                _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-horizontal-" + _OrderItem.NumberingPosition + "-perforated-back");
                                _OrderItem.DownloadUrl3 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-vertical-" + _OrderItem.NumberingPosition + "-perforated-front");
                                _OrderItem.DownloadUrl4 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-vertical-" + _OrderItem.NumberingPosition + "-perforated-back");
                            }

                        }

                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "flyer")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "hangtag")
                        {
                            string corners = string.Empty;
                            if (!string.IsNullOrEmpty(_OrderItem.RoundCorners))
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.HolePosition + "-" + _OrderItem.RoundCorners + "-front");
                                _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.HolePosition + "-" + _OrderItem.RoundCorners + "-back");
                            }
                            else
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.HolePosition + "-front");
                                _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.HolePosition + "-back");
                            }
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "letterhead")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "minimenu")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-inside");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-outside");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "notepad")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "postcard")
                        {
                            string corners = string.Empty;
                            if (!string.IsNullOrEmpty(_OrderItem.RoundCorners))
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                            }
                            else
                            {
                                _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.RoundCorners);
                            }
                           
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "poster")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "presentationfolder")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.Pocket + "-inside");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.Pocket + "-outside");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "rackcard")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "ripbusinessscard")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-back");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "rolllabel")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.Shape + "-front");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "specialshapes")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.Shape + "-front");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-" + _OrderItem.Shape + "-back");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "staggeredcutflyer")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");                          
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "sticker")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size);
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "tabletent")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size);
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "tentcard")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                            _OrderItem.DownloadUrl2 = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-back");
                        }
                        else if (_OrderItem.CategoryName != null && _OrderItem.CategoryName.ToLower().Replace(" ", "").Trim() == "yardsign")
                        {
                            _OrderItem.DownloadUrl = ProcessedURL("http://peasyprint.com/media/downloadtemplates/" + _OrderItem.CategoryName + "-" + _OrderItem.size + "-front");
                        }
                        objdb.tblOrderItems.Add(_OrderItem);
                        objdb.SaveChanges();

                        EmailList = new List<string>();

                        EmailList = objdb.tblDesigners.Where(i => i.IsActive == true).Select(rec => rec.EmailId).ToList();

                        SendNotificationToDesigner(oItem.name, EmailList);
                    }
                }
            }
            return View();

        }

        public List<string> EmailList { get; set; }


        private void SendNotificationToDesigner(string ProjectName, List<string> EmailsList)
        {

            try
            {

                SmtpClient mySmtpClient1 = new SmtpClient("smtp.gmail.com");
                int portNumber = 587;
                SmtpClient mySmtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpClient"].ToString());


                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = true;
                mySmtpClient.EnableSsl = true;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(ConfigurationSettings.AppSettings["fromAdd"].ToString(), ConfigurationSettings.AppSettings["password"].ToString());
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(ConfigurationSettings.AppSettings["fromAdd"].ToString());
                MailAddress to = new MailAddress(EmailsList.ElementAt(0).ToString());// new MailAddress(eMailAddress); 
                // MailAddress from = new MailAddress("lakhvinder.happy@gmail.com");
                // MailAddress to = new MailAddress("lakhvinder.kumar@team.amonous.com");// new MailAddress(eMailAddress); 

                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // set subject and encoding
                for (int i = 0; i <= EmailList.Count() - 1; i++)
                {
                    MailAddress SendCC = new MailAddress(EmailList.ElementAt(i).ToString());
                    myMail.CC.Add(SendCC);
                }

                myMail.Subject = "New Design Job.";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;




                var emailBody = string.Empty;

                emailBody = new StreamReader(Server.MapPath("~/Content/EmailTemplate/NewDesignJob.html")).ReadToEnd().ToString();

                var emailTemplate = emailBody.Replace("##ProjectName##", ProjectName);

                myMail.Body = emailTemplate;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;
                mySmtpClient.Send(myMail);


            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private string ProcessedURL(string url)
        {
            url = url.Replace(" ", "").Replace(",", "").ToLower();
            return url;
        }



    }

}
