using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CrystalSystemAPI.Models;
using System.Data.SqlClient;

namespace CrystalSystemAPI.Controllers
{
    public class OrderController : ApiController
    {
        //DT 2020-03-09 call our database entities
        private CerinimbusDevEntities db = new CerinimbusDevEntities();
        
        //DT 2020-03-09 create a function controller that will get all the product data
        [HttpGet, Route("api/Orders/")]
        [ResponseType(typeof(cvAssemblyOrdersUnlinked))]
        public IHttpActionResult  getAll(string orderNumber = "", string date="", string status="", string prodcategory = "", string orderType = "")
        {

            //JM: 2020-06-23 added try catch and generate a proper api response
            try
            {

                orderNumber = orderNumber ?? "";
                date = date ?? "";
                status = status ?? "";
                prodcategory = prodcategory ?? "";
                orderType = orderType ?? ""; 

                List<cvAssemblyOrdersUnlinked> result = new List<cvAssemblyOrdersUnlinked>();

                //NB add product category filter
                if (prodcategory != "")
                {
                    //split the productcategory string separated by comma and store in an array
                    string[] prodcategories = prodcategory.Split(',');
                    if (date == "")
                    {
                        result = db.cvAssemblyOrdersUnlinkeds.AsNoTracking().Where(
                        e =>
                            e.Session.ToString().Contains(orderNumber) &&
                            e.TransactionDate.ToString().Contains(date) &&
                            e.DocumentType == orderType
                            //NB: 2020-24-include null value of ProductionWorkflowStatus to be displayed if status=""
                            && (e.ProductionWorkflowStatus.Contains(status) || (status == "" && e.ProductionWorkflowStatus == null))
                            && prodcategories.Contains(e.FProductCategory)
                         ).OrderBy(e => e.Session).ThenBy(e => e.Document).ToList();
                    }
                    else
                    {
                        var tempDate = DateTime.Parse(date);
                        result = db.cvAssemblyOrdersUnlinkeds.AsNoTracking().Where(
                        e =>
                        e.Session.ToString().Contains(orderNumber) &&
                        e.TransactionDate == tempDate &&
                        e.DocumentType == orderType
                        //NB: 2020-24-include null value of ProductionWorkflowStatus to be displayed if status=""
                        && (e.ProductionWorkflowStatus.Contains(status) || (status=="" && e.ProductionWorkflowStatus == null))
                        && prodcategories.Contains(e.FProductCategory)
                    ).OrderBy(e => e.Session).ThenBy(e => e.Document).ToList();

                    }
                }
                else {
                    if (date == "")
                    {
                        result = db.cvAssemblyOrdersUnlinkeds.AsNoTracking().Where(
                        e =>
                            e.Session.ToString().Contains(orderNumber) &&
                            e.TransactionDate.ToString().Contains(date) &&
                            e.DocumentType == orderType
                            //NB: 2020-24-include null value of ProductionWorkflowStatus to be displayed if status=""
                            && (e.ProductionWorkflowStatus.Contains(status) || (status == "" && e.ProductionWorkflowStatus == null))

                        ).OrderBy(e => e.Session).ThenBy(e => e.Document).ToList();
                    }
                    else
                    {
                        var tempDate = DateTime.Parse(date);
                        result = db.cvAssemblyOrdersUnlinkeds.AsNoTracking().Where(
                        e =>
                        e.Session.ToString().Contains(orderNumber) &&
                        e.TransactionDate == tempDate &&
                        e.DocumentType == orderType
                        //NB: 2020-24-include null value of ProductionWorkflowStatus to be displayed if status=""
                        && (e.ProductionWorkflowStatus.Contains(status) || (status == "" && e.ProductionWorkflowStatus == null))
                    ).OrderBy(e => e.Session).ThenBy(e => e.Document).ToList();

                    }
                }

                
                return Ok(result);

            }
            catch (Exception e)
            {
                var message = e.InnerException.Message.ToString();
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
            }

        }

        [HttpGet, Route("api/Order/")]
        [ResponseType(typeof(cvAssemblyOrdersUnlinked))]
        public IHttpActionResult GetOrder(string orderNumber, string transacNumber) {
            var res = db.cvAssemblyOrdersUnlinkeds.Where(
                e => e.Session.ToString() == orderNumber &&
                     e.TransactionNumber.ToString() == transacNumber
            ).FirstOrDefault();
            return Ok(res);
        }

        // GET: api/Order/5
        //DT 2020-03-09 create a function controller that will get all the product data with the product id filter
        [ResponseType(typeof(cvAssemblyOrdersUnlinked))]
        public IHttpActionResult Getorder(string filter)
        {
            //DT 2020-03-11 let us apply the filter for the transaction number
            cvAssemblyOrdersUnlinked orders = db.cvAssemblyOrdersUnlinkeds.FirstOrDefault(e => e.TransactionNumber.ToString() == filter);
            if (orders == null)
            {
                //DT 2020-03-09 return NotFound() function if there is no product found
                return NotFound();
            }

            //DT 2020-03-09 return the product data
            return Ok(orders);
        }

        ////NB: 2020-08-02 create a function controller that will get all the product category
        //[HttpGet, Route("api/ProductCategory/")]
        //[ResponseType(typeof(cvAssemblyOrdersUnlinked))]
        //public IHttpActionResult getAllProdCategory(string orderNumber = "", string date = "", string status = "")
        //{

        //    try
        //    {

        //        orderNumber = orderNumber ?? "";
        //        date = date ?? "";
        //        status = status ?? "";

        //        List<cvAssemblyOrdersUnlinked> result = new List<cvAssemblyOrdersUnlinked>();
        //        if (date == "")
        //        {
        //            result = db.cvAssemblyOrdersUnlinkeds.Select(p => p.FProductType).Distinct().Where(
        //            e =>
        //                e.Session.ToString().Contains(orderNumber) &&
        //                e.TransactionDate.ToString().Contains(date)
        //            //NB: 2020-24-commented out ProductionWorkflowStatus filter to show rows with null value
        //            //&& e.ProductionWorkflowStatus.Contains(status)
        //            ).OrderBy(e => e.Session).ThenBy(e => e.Document).ToList();
        //        }
        //        else
        //        {
        //            var tempDate = DateTime.Parse(date);
        //            result = db.cvAssemblyOrdersUnlinkeds.AsNoTracking().Where(
        //            e =>
        //            e.Session.ToString().Contains(orderNumber) &&
        //            e.TransactionDate == tempDate &&
        //            e.ProductionWorkflowStatus.Contains(status)
        //        ).OrderBy(e => e.Session).ThenBy(e => e.Document).ToList();

        //        }
        //        return Ok(result);

        //    }
        //    catch (Exception e)
        //    {
        //        var message = e.InnerException.Message.ToString();
        //        throw new HttpResponseException(
        //            Request.CreateErrorResponse(HttpStatusCode.NotFound, message));
        //    }

        //}

    }
}
